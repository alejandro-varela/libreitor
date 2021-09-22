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
		Dictionary<string, int> _cacheDatos = null;

		public DatosEmpIntFicha (Configuration config)
		{
			Config = config;
		}

		private Dictionary<string, int> LeerFichasXEmpreInterSUBE()
		{
			var ret = new Dictionary<string, int>();

			var consulta = ConsultaFichasBuilder();

			using var conn = new SqlConnection(Config.ConnectionString);
			conn.Open();
			using var cmd = new SqlCommand(consulta, conn);
			cmd.CommandTimeout = Config.CommandTimeout;
			using var reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				int empresa = Convert.ToInt32(reader["empresa"] ?? 0);
				int interno = Convert.ToInt32(reader["interno"] ?? 0);
				int ficha   = Convert.ToInt32(reader["ficha"  ] ?? 0);
				ret.Add(KeyBuilder(empresa, interno), ficha);
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

        public Dictionary<string, int> Get(bool overrideCache = false)
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
			var key = KeyBuilder(empresa, interno);

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

        private static string KeyBuilder(int empresa, int interno)
        {
            return $"{empresa}-{interno}";
        }
    }
}
