using Comun;
using ComunSUBE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace LibQPA.ProveedoresVentas.DbSUBE
{
    public class ProveedorBoletosSUBE : QPAProveedorVentaBoletos<ParEmpresaInterno>
    {
        public class Configuracion
        {
            public int      CommandTimeout  { get; set; } = 600;
            public string   ConnectionString{ get; set; }
            public DateTime FechaDesde      { get; set; }
            public DateTime FechaHasta      { get; set; }
        }

        public Configuracion Config { get; set; }

        public ProveedorBoletosSUBE(Configuracion configuracion) : 
            this(configuracion, null)
        { 
            //
        }

        public ProveedorBoletosSUBE(
            Configuracion configuracion,
            Dictionary<ParEmpresaInterno, List<BoletoComun>> boletosXIdentificador)
        {
            Config = configuracion;
            BoletosXIdentificador = boletosXIdentificador;
        }

        public override Dictionary<ParEmpresaInterno, List<BoletoComun>> LeerDB()
        {
            var ret = new Dictionary<ParEmpresaInterno, List<BoletoComun>>();

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

            using var conn = new SqlConnection(Config.ConnectionString);
            conn.Open();
            using var cmd = new SqlCommand(consulta, conn);
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
                var latitud         = Convert.ToDouble(reader["C_LONGITUD"]) / 100000.0;
                var longitud        = Convert.ToDouble(reader["C_LATITUD"]) / 100000.0;
                // ********************************************************************

                var identificador = new ParEmpresaInterno { Empresa=empresa, Interno=interno };

                if (!ret.ContainsKey(identificador))
                {
                    ret.Add(identificador, new List<BoletoComun>());
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

                ret[identificador].Add(boleto);                
            }

            return ret;
        }
    }
}
