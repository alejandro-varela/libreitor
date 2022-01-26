using Comun;
using ComunSUBE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LibQPA.ProveedoresVentas.DbSUBE
{
    [Obsolete("Esta clase está deprecated, usar ProveedorBoletosSUBE en su lugar")]
    public class ProveedorVentaBoletosDbSUBE : QPAProveedorVentaBoletos<BoletoComun>
    {
        public class Configuracion
        {
            public int CommandTimeout { get; set; }
            public string ConnectionString { get; set; }
            public string Consulta { get; set; }
            public DateTime FechaDesde { get; set; }
            public DateTime FechaHasta { get; set; }
            public DatosEmpIntFicha DatosEmpIntFicha { get; set; }
        }

        public Configuracion Config { get; set; }

        public ProveedorVentaBoletosDbSUBE(Configuracion configuracion) : 
            this(configuracion, null)
        {
            Config = configuracion;
        }

        public ProveedorVentaBoletosDbSUBE(
            Configuracion configuracion, 
            Dictionary<int, List<BoletoComun>> boletosXIdentificador)
        {
            BoletosXIdentificador = boletosXIdentificador;
        }

        public IEnumerable<BoletoComun> GetBoletos(int ficha)
        {
            var todasLasKeys = BoletosXIdentificador.Keys.ToList();

            if (!BoletosXIdentificador.ContainsKey(ficha))
            {
                yield break;
            }

            var boletos = BoletosXIdentificador[ficha];

            foreach (var boletoX in boletos)
            {
                yield return boletoX;
            }
        }

        public IEnumerable<BoletoComun> GetBoletosEnIntervalo(int ficha, DateTime fechaComienzo, DateTime fechaFin)
        {
            var boletos = GetBoletos(ficha)
                .Where(boleto =>
                    boleto.FechaCancelacion >= fechaComienzo &&
                    boleto.FechaCancelacion <  fechaFin
                );

            foreach (var boletoX in boletos)
            {
                yield return boletoX;
            }
        }

        public bool TieneBoletosEnIntervalo(int ficha, DateTime horaComienzo, DateTime horaFin)
        {
            return GetBoletosEnIntervalo(ficha, horaComienzo, horaFin).Any();
        }

        public override Dictionary<int, List<BoletoComun>> LeerDB()
        {
            var ret = new Dictionary<int, List<BoletoComun>>();

            var consulta = @"
                select      C_INTERNO, C_EMPRESA, D_FECHA_EVENTO_ORIGINAL, D_IMPORTE_ORIGINAL, D_IMPORTE_DESCUENTO, D_IMPORTE_TARIFA, C_LATITUD, C_LONGITUD
                from	    SUBE_VENTAS_CONCENTRADOR_DETALLE_FULL
                where	    D_FECHA_EVENTO_ORIGINAL >= '{{FECHA_DESDE}}'
		                    and
		                    D_FECHA_EVENTO_ORIGINAL <  '{{FECHA_HASTA}}'
                order by    C_EMPRESA, C_INTERNO, D_FECHA_EVENTO_ORIGINAL
            "
            .Replace("{{FECHA_DESDE}}", Config.FechaDesde.ToString("dd/MM/yyyy HH:mm:ss"))
            .Replace("{{FECHA_HASTA}}", Config.FechaHasta.ToString("dd/MM/yyyy HH:mm:ss"))
            ;
            
            using var conn   = new SqlConnection(Config.ConnectionString);
            conn.Open();
            using var cmd    = new SqlCommand(consulta, conn);
            cmd.CommandTimeout = Config.CommandTimeout;
            using var reader = cmd.ExecuteReader();

            int boletoId = 0;
            while (reader.Read())
            {
                var interno         = Convert.ToInt32   (reader["C_INTERNO"] ?? 0);
                var empresa         = Convert.ToInt32   (reader["C_EMPRESA"] ?? 0);
                var fecha           = Convert.ToDateTime(reader["D_FECHA_EVENTO_ORIGINAL"]);
                var valorInicial    = Convert.ToDouble  (reader["D_IMPORTE_ORIGINAL"]);
                var valorDescuento  = Convert.ToDouble  (reader["D_IMPORTE_DESCUENTO"]);
                var valorFinal      = Convert.ToDouble  (reader["D_IMPORTE_TARIFA"]);

                // ************************* ATENCIÓN *********************************
                // Ripley’s Believe It or Not! Jack Palance nos cuenta que:
                //  1) En la base de datos la latitud y la longitud estan invertidas
                //     Asi que la latitud es C_LONGITUD y la longitud es C_LATITUD
                //  2) Los dos valores estan guardados en un valor entero (por lo que
                //     debemos dividirlos por 100)
                var latitud         = Convert.ToDouble  (reader["C_LONGITUD"]) / 100000.0;
                var longitud        = Convert.ToDouble  (reader["C_LATITUD" ]) / 100000.0;
                // ********************************************************************

                var ficha = Config.DatosEmpIntFicha.GetFicha(empresa, interno);

                if (ficha > 0)
                {
                    if (!ret.ContainsKey(ficha))
                    {
                        ret.Add(ficha, new List<BoletoComun>());
                    }
                    
                    boletoId++;
                    
                    var boleto = new BoletoComun
                    {
                        Id                  = $"id-{boletoId}",
                        FechaCancelacion    = fecha,
                        Latitud             = latitud,
                        Longitud            = longitud,
                        ValorInicial        = valorInicial,
                        ValorDescuento      = valorDescuento,
                        ValorFinal          = valorFinal,
                    };

                    ret[ficha].Add(boleto);
                }
            }

            return ret;
        }
    }
}
