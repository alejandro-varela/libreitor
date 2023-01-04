using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ComunSUBE
{
    public class DatosEmpIntFicha
    {
		public class Configuration
		{
			public string	ConnectionString	{ get; set; }
			public int		CommandTimeout		{ get; set; }
			public double	MaxCacheSeconds		{ get; set; } = 15 * 60; // 15 mins
        }

		public Configuration Config { get; private set; }

		DateTime _fechaCacheDatos = DateTime.MinValue;
		Dictionary<(int, int), int> _cacheDatos = null;

		public DatosEmpIntFicha (Configuration config)
		{
			Config = config;
		}

		public DatosEmpIntFicha(Configuration config, Dictionary<(int, int), int> cacheDatos, DateTime fechaCacheDatos)
		{
			Config			= config;
			_cacheDatos		= cacheDatos;
			_fechaCacheDatos= fechaCacheDatos;
		}

		private Dictionary<(int, int), int> LeerFichasXEmpreInterSUBE()
		{
			var ret = new Dictionary<(int, int), int>();

			var consulta = ConsultaFichasBuilder();

			using (var conn = new SqlConnection(Config.ConnectionString))
			{
				conn.Open();
				using (var cmd = new SqlCommand(consulta, conn))
				{
					cmd.CommandTimeout = Config.CommandTimeout;
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							int empresa = Convert.ToInt32(reader["empresa"] ?? 0);
							int interno = Convert.ToInt32(reader["interno"] ?? 0);
							int ficha = Convert.ToInt32(reader["ficha"] ?? 0);
							ret.Add((empresa, interno), ficha);
						}
					}
				}
			}

			return ret;
		}

		private string ConsultaFichasBuilder()
		{
			var consulta =
				@"
					select   cod_empresa as empresa,
							 i_interno   as interno,
					         ficha
					from     vw_internosSUBE
					order by cod_empresa, i_interno
				"
				.Trim()
			;

			return consulta;
		}

        public Dictionary<(int, int), int> Get(bool overrideCache = false)
		{
			if (_cacheDatos == null || overrideCache || DateTime.Now.Subtract(_fechaCacheDatos).TotalSeconds > Config.MaxCacheSeconds)
			{
				_cacheDatos = LeerFichasXEmpreInterSUBE();
				_fechaCacheDatos = DateTime.Now;
			}

			return _cacheDatos;
        }

		public int GetFicha(int empresa, int interno, int @default = -1, bool overrideCache = false)
        {
			var key = (empresa, interno);

			var cosas = Get(overrideCache);

			if (cosas.ContainsKey(key))
			{
				return cosas[key];
			}
			else
			{
				return @default;
			}
        }

		public int GetFicha(string identSUBE, char sep = '-', int @default = -1, bool overrideCache = false)
		{
			string sEmpresa = identSUBE.Split(sep)[0];
			string sInterno = identSUBE.Split(sep)[1];

			int empresa = int.Parse(sEmpresa);
			int interno = int.Parse(sInterno);

			return GetFicha(empresa, interno, @default, overrideCache);
		}

		private static string KeyBuilder(int empresa, int interno)
        {
            return $"{empresa}-{interno}";
        }
    }
}
