using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Comun;

namespace LibQPA.ProveedoresHistoricos.DbXBus
{
	public partial class ProveedorHistoricoDbXBus : IQPAProveedorPuntosHistoricos
	{
		DateTime _fechaCache = DateTime.MinValue;
		Dictionary<string, List<PuntoHistorico>> _cache = new Dictionary<string, List<PuntoHistorico>>();

		[Flags]
		public enum TipoEquipo
		{
			TECNOBUS = 1,
			PICOBUS = 2,
		}

		public class Configuracion
		{
			public TipoEquipo Tipo { get; set; } = TipoEquipo.TECNOBUS | TipoEquipo.PICOBUS;
			public int CommandTimeout { get; set; } = 600;
			public string ConnectionString { get; set; }
            public DateTime FechaDesde { get; set; }
            public DateTime FechaHasta { get; set; }
        }

		public Configuracion Config { get; set; }
		public bool UsarCache { get; set; }

		public ProveedorHistoricoDbXBus(Configuracion config, bool usarCache = true)
		{
			Config = config;
			UsarCache = usarCache;
		}

		public Dictionary<string, List<PuntoHistorico>> LeerDB(string connString, int equipoDesde, int equipoHasta, DateTime fechaDesde, DateTime fechaHasta)
		{
			var ret = new Dictionary<string, List<PuntoHistorico>>();

			// hacer consulta
			var consulta = ConsultaSQLBasica(
				equipoDesde,
				equipoHasta,
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
				//int equipo = Convert.ToInt32(reader["equipo"] ?? 0);
				int ficha = Convert.ToInt32(reader["ficha"] ?? 0);
				DateTime fecha = Convert.ToDateTime(reader["fecha"] ?? DateTime.MinValue);
				double lat = Convert.ToDouble(reader["lat"] ?? 0.0);
				double lng = Convert.ToDouble(reader["lng"] ?? 0.0);

				// corrijo la latitud / longitud rosariobusense
				var latitudCorregida = -Math.Abs(lat);
				var longitudCorregida = -Math.Abs(lng);

				if (!ret.ContainsKey(ficha.ToString()))
				{
					ret.Add(ficha.ToString(), new List<PuntoHistorico>());
				}

				var puntoHistorico = new PuntoHistorico
				{
					Alt = 0,
					Fecha = fecha,
					Lat = latitudCorregida,
					Lng = longitudCorregida,
				};

				ret[ficha.ToString()].Add(puntoHistorico);
			}

			return ret;
		}

		public Dictionary<string, List<PuntoHistorico>> Get()
        {
			int equipoDesde = 0;
			int equipoHasta = 0;

			if (Config.Tipo.HasFlag(TipoEquipo.TECNOBUS) && 
				Config.Tipo.HasFlag(TipoEquipo.PICOBUS))
			{
				equipoDesde = 2_000_000;
				equipoHasta = 4_000_000;
			}
			else if (Config.Tipo.HasFlag(TipoEquipo.TECNOBUS))
			{
				equipoDesde = 2_000_000;
				equipoHasta = 3_000_000;
			}
			else if (Config.Tipo.HasFlag(TipoEquipo.PICOBUS))
			{
				equipoDesde = 3_000_000;
				equipoHasta = 4_000_000;
			}
			else
			{
				return new Dictionary<string, List<PuntoHistorico>>();
			}

			if (DateTime.Now.Subtract(_fechaCache) > TimeSpan.FromMinutes(1))
			{
				_cache = LeerDB(Config.ConnectionString, equipoDesde, equipoHasta, Config.FechaDesde, Config.FechaHasta);
				_fechaCache = DateTime.Now;
			}

			return _cache;
        }

		public string ConsultaSQLBasica(int equipoDesde, int equipoHasta, DateTime fechaDesde, DateTime fechaHasta)
        {
			// 2021-09-02T00:00:00
			var fechaFmt = "yyyy-MM-ddTHH:mm:ss";

			return @"
				select * from
				(
					select	'CCA' as tabla, 
							nro_equipo as equipo,
							nro_ficha as ficha,
							fechaHora as fecha,
							latitud as lat,
							longitud as lng
					from	log_gpsConCalculoAtraso

					union

					select	'SCA' as tabla,
							nro_equipo as equipo,
							nro_ficha as ficha,
							fechaHora as fecha,
							latitud as lat,
							longitud as lng
					from	log_gpsSinCalculoAtraso

					union

					select	'CCX' as tabla,
							nro_equipo as equipo,
							nro_ficha as ficha,
							fechaHora as fecha,
							latitud as lat,
							longitud as lng
					from	log_gpsConCalculoAtrasoYExcesoVelocidad

					union

					select	'SCX' as tabla,
							nro_equipo as equipo,
							nro_ficha as ficha,
							fechaHora as fecha,
							latitud as lat,
							longitud as lng
					from	log_gpsSinCalculoAtrasoYExcesoVelocidad

				) as todo

				where
					ficha > 0
					and
					fecha >= 'FECHA_DESDE'
					and
					fecha <  'FECHA_HASTA'
					and
					equipo >= EQUIPO_DESDE
					and
					equipo < EQUIPO_HASTA
					and
					lat > 0
					and
					lng > 0
				order by ficha, fecha
            "
			.Replace("FECHA_DESDE", fechaDesde.ToString(fechaFmt))
			.Replace("FECHA_HASTA", fechaHasta.ToString(fechaFmt))
			.Replace("EQUIPO_DESDE", equipoDesde.ToString())
			.Replace("EQUIPO_HASTA", equipoHasta.ToString())
			;
        }
    }
}
