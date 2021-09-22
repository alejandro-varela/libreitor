using Comun;
using ComunSUBE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LibQPA.ProveedoresHistoricos.DbSUBE
{
    public class ProveedorHistoricoDbSUBE : IQPAProveedorPuntosHistoricos
    {
		DateTime                                _fechaCachePuntosHistoricos = DateTime.MinValue;
        Dictionary<int, List<PuntoHistorico>>   _cachePuntosHistoricos      = new Dictionary<int, List<PuntoHistorico>>();
        public Configuracion                    Config      { get; set; }
        public bool                             UsarCache   { get; set; }

        public class Configuracion
        {
            public int				CommandTimeout			{ get; set; } = 600;
            public string			ConnectionStringPuntos	{ get; set; }
			public int				MaxCacheSeconds			{ get; set; } = 15 * 60; // 15 mins
			public DatosEmpIntFicha DatosEmpIntFicha		{ get; set; }
			public DateTime			FechaDesde				{ get; set; }
			public DateTime			FechaHasta				{ get; set; }
		}

        public ProveedorHistoricoDbSUBE(Configuracion config, bool usarCache = true)
        {
            Config = config;
            UsarCache = usarCache;
        }

        public Dictionary<int, List<PuntoHistorico>> Get()
        {
            if (DateTime.Now.Subtract(_fechaCachePuntosHistoricos) > TimeSpan.FromMinutes(5))
            {
                _cachePuntosHistoricos = LeerDBPuntosHistoricos(Config.ConnectionStringPuntos, Config.FechaDesde, Config.FechaHasta);
                _fechaCachePuntosHistoricos = DateTime.Now;
            }

            return _cachePuntosHistoricos;
        }

        private Dictionary<int, List<PuntoHistorico>> LeerDBPuntosHistoricos(
			string connString, 
			DateTime fechaDesde, 
			DateTime fechaHasta
		)
        {
			var ret = new Dictionary<int, List<PuntoHistorico>>();

			// hacer consulta
			var consulta = ConsultaPuntosBuilder(
				fechaDesde,
				fechaHasta
			);

			using var conn = new SqlConnection(connString);
			conn.Open();
			using var cmd = new SqlCommand(consulta, conn);
			cmd.CommandTimeout = Config.CommandTimeout;
			using var reader = cmd.ExecuteReader();
			
			var malfichos = new List<string>();

			while (reader.Read())
			{
				int empresaSUBE = Convert.ToInt32(reader["empresaSube"] ?? 0);
				int interno = Convert.ToInt32(reader["interno"] ?? 0);

				int identificador = Config.DatosEmpIntFicha.GetFicha(
					empresa: empresaSUBE, 
					interno: interno, 
					@default: -1
				);

				if (identificador == -1)
				{
					malfichos.Add($"Empresa: {empresaSUBE} Interno: {interno} No entcontrados en ficha...");
					continue;
				}

				DateTime fecha = Convert.ToDateTime(reader["fechaHoraUTC"] ?? DateTime.MinValue);
				double lat = Convert.ToDouble(reader["latitud" ] ?? 0.0);
				double lng = Convert.ToDouble(reader["longitud"] ?? 0.0);

				// corrijo la latitud / longitud rosariobusense
				var latitudCorregida  = -Math.Abs(lat);
				var longitudCorregida = -Math.Abs(lng);

				if (!ret.ContainsKey(identificador))
				{
					ret.Add(identificador, new List<PuntoHistorico>());
				}

				var puntoHistorico = new PuntoHistorico
				{
					Alt = 0,
					Fecha = fecha.AddHours(-3),
					Lat = latitudCorregida,
					Lng = longitudCorregida,
				};

				ret[identificador].Add(puntoHistorico);
			}

			return ret;
		}

        private string ConsultaPuntosBuilder(DateTime fechaDesde, DateTime fechaHasta)
        {
			var consulta =
				@"
					select		*
					from		log_posicionamientoSUBE
					where		fechaHoraUTC >= '{{FECHA_DESDE}}'
								and
								fechaHoraUTC <  '{{FECHA_HASTA}}'
					order by	fechaHoraUTC asc
				"
				.Trim()
				.Replace("{{FECHA_DESDE}}", fechaDesde.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss"))
				.Replace("{{FECHA_HASTA}}", fechaHasta.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss"))
			;

			return consulta;
        }
	}
}
