using Comun;
using ComunSUBE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
			var consulta = GetConsulta(fechaDesde, fechaHasta);

			using var conn = new SqlConnection(connString);
			conn.Open();

			using var cmd = new SqlCommand(consulta, conn);
			cmd.CommandTimeout = Config.CommandTimeout;

			using var reader = cmd.ExecuteReader();
			
			while (reader.Read())
			{
				int			empresaSUBE	= Convert.ToInt32	(reader["C_EMPRESA"] ?? 0);
				int			interno		= Convert.ToInt32	(reader["C_INTERNO"] ?? 0);
				DateTime	fecha		= Convert.ToDateTime(reader["C_FECHA_EVENTO"] ?? DateTime.MinValue);
				double		lat			= Convert.ToDouble	(reader["LATITUD" ] ?? 0.0);
				double		lng			= Convert.ToDouble	(reader["LONGITUD"] ?? 0.0);

				var identificador = new ParEmpresaInterno { Empresa = empresaSUBE, Interno = interno };

				if (!ret.ContainsKey(identificador))
				{
					ret.Add(identificador, new List<PuntoHistorico>());
				}

				var puntoHistorico = new PuntoHistorico
				{
					Alt = 0,
					Fecha = fecha,
					Lat = lat,
					Lng = lng,
				};

				ret[identificador].Add(puntoHistorico);
			}

			return ret;
		}

        private string GetConsulta(DateTime fechaDesde, DateTime fechaHasta)
        {
			//var consulta =
			//	@"
			//		select
			//			C_EMPRESA,
			//			C_INTERNO,
			//			C_FECHA_EVENTO,
			//			-1 * ABS(C_LATITUD  / 100000) as LATITUD,
			//			-1 * ABS(C_LONGITUD / 100000) as LONGITUD
			//		from SUBE_VENTAS_CONCENTRADOR_KM
			//		where 
			//			C_FECHA_EVENTO >= '{{FECHA_DESDE}}'
			//			and
			//			C_FECHA_EVENTO <  '{{FECHA_HASTA}}'
			//		order by C_FECHA_EVENTO
			//	"
			//	.Trim()
			//	.Replace("{{FECHA_DESDE}}", fechaDesde.ToString("dd/MM/yyyy HH:mm:ss"))
			//	.Replace("{{FECHA_HASTA}}", fechaHasta.ToString("dd/MM/yyyy HH:mm:ss"))
			//;

			// - Se evitan duplicados (select distinct)
			// - Solo se toman los eventos de tipo 3 y 9 (por ahora) 
			// +---------------+-----------------------+
			// | C_TIPO_EVENTO | Des_Evento            |
			// +---------------+-----------------------+
			// | 3             | DATO DE GEOREFERENCIA |
			// | 5             | INICIO DE TURNO       |
			// | 6             | FIN DE TURNO          |
			// | 7             | INICIO DE SERVICIO    |
			// | 8             | FIN DE SERVICIO       |
			// | 9             | SIN CLASIFICAR        |
			// +---------------+-----------------------+
			
			var consulta =
				@"
					select distinct
						C_EMPRESA,
						C_INTERNO,
						C_FECHA_EVENTO,
						C_TIPO_EVENTO,
						-1 * ABS(C_LATITUD  / 100000) as LATITUD,
						-1 * ABS(C_LONGITUD / 100000) as LONGITUD
					from SUBE_VENTAS_CONCENTRADOR_KM
					where 
						C_FECHA_EVENTO >= '{{FECHA_DESDE}}'
						and
						C_FECHA_EVENTO <  '{{FECHA_HASTA}}'
						and
						C_TIPO_EVENTO in (3,9)
						and
						C_LATITUD != 0
						and
						C_LONGITUD != 0
					order by C_FECHA_EVENTO
				"
				.Trim()
				.Replace("{{FECHA_DESDE}}", fechaDesde.ToString("dd/MM/yyyy HH:mm:ss"))
				.Replace("{{FECHA_HASTA}}", fechaHasta.ToString("dd/MM/yyyy HH:mm:ss"))
			;

			return consulta;
        }
	}
}
