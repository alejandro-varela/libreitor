using Comun;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LibQPA.ProveedoresHistoricos.DbSUBE
{
    public class ProveedorHistoricoDbSUBE : IQPAProveedorPuntosHistoricos
    {
        DateTime                                _fechaCache = DateTime.MinValue;
        Dictionary<int, List<PuntoHistorico>>   _cache      = new Dictionary<int, List<PuntoHistorico>>();
        public Configuracion                    Config      { get; set; }
        public bool                             UsarCache   { get; set; }

        public class Configuracion
        {
            public int    CommandTimeout   { get; set; } = 600;
            public string ConnectionString { get; set; }
        }

        public ProveedorHistoricoDbSUBE(Configuracion config, bool usarCache = true)
        {
            Config = config;
            UsarCache = usarCache;
        }

        public Dictionary<int, List<PuntoHistorico>> Get(DateTime desde, DateTime hasta)
        {
            if (DateTime.Now.Subtract(_fechaCache) > TimeSpan.FromMinutes(1))
            {
                _cache = LeerDB(Config.ConnectionString, desde, hasta);
                _fechaCache = DateTime.Now;
            }

            return _cache;
        }

        private Dictionary<int, List<PuntoHistorico>> LeerDB(string connString, DateTime fechaDesde, DateTime fechaHasta)
        {
			var ret = new Dictionary<int, List<PuntoHistorico>>();

			// hacer consulta
			var consulta = ConsultaSQLBasica(
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
				int identificador = 
					Convert.ToInt32(reader["empresaSube"] ?? 0) * 10000 +
					Convert.ToInt32(reader["interno"	] ?? 0)
				;

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
					Fecha = fecha,
					Lat = latitudCorregida,
					Lng = longitudCorregida,
				};

				ret[identificador].Add(puntoHistorico);
			}

			return ret;
		}

        private string ConsultaSQLBasica(DateTime fechaDesde, DateTime fechaHasta)
        {
			return 
				@"
					select		*
					from		log_posicionamientoSUBE
					where		fechaHoraUTC >= '07/09/2021 00:00'
								and
								fechaHoraUTC <  '08/09/2021 00:00'
					order by	fechaHoraUTC asc
				"
				.Trim()
			;
        }
    }
}
