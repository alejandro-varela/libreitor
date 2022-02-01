using Comun;
using ComunSUBE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LibQPA.ProveedoresHistoricos.DbSUBE
{
    public class ProveedorHistoricoDbSUBE : IQPAProveedorPuntosHistoricos<ParEmpresaInterno>
    {
		DateTime _fechaCachePuntosHistoricos = DateTime.MinValue;
        Dictionary<ParEmpresaInterno, List<PuntoHistorico>>_cachePuntosHistoricos  = new Dictionary<ParEmpresaInterno, List<PuntoHistorico>>();

        public Configuracion Config { get; set; }
        public bool UsarCache { get; set; }

        public class Configuracion
        {
            public int				CommandTimeout			{ get; set; } = 600;
            public string			ConnectionStringPuntos	{ get; set; }
			public int				MaxCacheSeconds			{ get; set; } = 15 * 60; // 15 mins
			public DateTime			FechaDesde				{ get; set; }
			public DateTime			FechaHasta				{ get; set; }
			public Func<ParEmpresaInterno, List<PuntoHistorico>, List<PuntoHistorico>> Transformador { get; set; }
        }

        public ProveedorHistoricoDbSUBE(Configuracion config, bool usarCache = true)
        {
            Config = config;
            UsarCache = usarCache;
        }

		public Dictionary<ParEmpresaInterno, List<PuntoHistorico>> Get()
        {
            if (DateTime.Now.Subtract(_fechaCachePuntosHistoricos) > TimeSpan.FromMinutes(5))
            {
                _cachePuntosHistoricos = LeerDBPuntosHistoricos(Config.ConnectionStringPuntos, Config.FechaDesde, Config.FechaHasta);
                _fechaCachePuntosHistoricos = DateTime.Now;
            }

			if (Config.Transformador == null)
			{
				return _cachePuntosHistoricos;
			}
			else
			{
				var ret = new Dictionary<ParEmpresaInterno, List<PuntoHistorico>>();
				foreach (var (ident, pts) in _cachePuntosHistoricos)
				{
					var puntosTrafo = Config.Transformador(ident, pts).ToList();
					ret.Add(ident, puntosTrafo);
				}
				return ret;
			}
        }

        private Dictionary<ParEmpresaInterno, List<PuntoHistorico>> LeerDBPuntosHistoricos(
			string connString, 
			DateTime fechaDesde, 
			DateTime fechaHasta
		)
        {
			var ret = new Dictionary<ParEmpresaInterno, List<PuntoHistorico>>();

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
			
			while (reader.Read())
			{
				int empresaSUBE = Convert.ToInt32(reader["C_EMPRESA"] ?? 0);
				int interno = Convert.ToInt32(reader["C_INTERNO"] ?? 0);

				ParEmpresaInterno identificador = new ParEmpresaInterno { Empresa = empresaSUBE, Interno = interno };

				DateTime fecha = Convert.ToDateTime(reader["C_FECHA_EVENTO"] ?? DateTime.MinValue);
				double lat = Convert.ToDouble(reader["LATITUD" ] ?? 0.0);
				double lng = Convert.ToDouble(reader["LONGITUD"] ?? 0.0);

				if (!ret.ContainsKey(identificador))
				{
					ret.Add(identificador, new List<PuntoHistorico>());
				}

				var puntoHistorico = new PuntoHistorico
				{
					Alt = 0,
					//Fecha = fecha.AddHours(-3),
					Fecha = fecha,
					Lat = lat,
					Lng = lng,
				};

				ret[identificador].Add(puntoHistorico);
			}

			return ret;
		}

        private string ConsultaPuntosBuilder(DateTime fechaDesde, DateTime fechaHasta)
        {
			//var consulta =
			//	@"
			//		select		*
			//		from		log_posicionamientoSUBE
			//		where		fechaHoraUTC >= '{{FECHA_DESDE}}'
			//					and
			//					fechaHoraUTC <  '{{FECHA_HASTA}}'
			//		order by	fechaHoraUTC asc
			//	"
			//	.Trim()
			//	.Replace("{{FECHA_DESDE}}", fechaDesde.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss"))
			//	.Replace("{{FECHA_HASTA}}", fechaHasta.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss"))
			//;

			var consulta =
				@"
					select
						C_EMPRESA,
						C_INTERNO,
						C_FECHA_EVENTO,
						-1 * ABS(C_LATITUD  / 100000) as LATITUD,
						-1 * ABS(C_LONGITUD / 100000) as LONGITUD
					from SUBE_VENTAS_CONCENTRADOR_KM
					where 
						C_FECHA_EVENTO >= '{{FECHA_DESDE}}'
						and
						C_FECHA_EVENTO <  '{{FECHA_HASTA}}'
					order by C_FECHA_EVENTO
			"
			.Trim()
			.Replace("{{FECHA_DESDE}}", fechaDesde.ToString("dd/MM/yyyy HH:mm:ss"))
			.Replace("{{FECHA_HASTA}}", fechaHasta.ToString("dd/MM/yyyy HH:mm:ss"));

			return consulta;
        }
	}
}
