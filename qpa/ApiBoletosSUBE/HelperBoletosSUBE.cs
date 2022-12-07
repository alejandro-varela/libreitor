using Comun;
using ComunSUBE;
using LibQPA.ProveedoresVentas.DbSUBE;
using System;
using System.Collections.Generic;

namespace ApiBoletosSUBE
{
    public class HelperBoletosSUBE
    {
        public static Dictionary<ParEmpresaInterno, List<BoletoComun>> GetBoletosFromDbSUBE(
            string connectionString, 
            int commandTimeout,
            DateTime desde, 
            DateTime hasta
        )
        {
            var proveedorBoletosSUBE = new ProveedorBoletosSUBE(new ProveedorBoletosSUBE.Configuracion
            {
                CommandTimeout   = commandTimeout,
                ConnectionString = connectionString,
                FechaDesde = desde,
                FechaHasta = hasta,
            });

            var dic = proveedorBoletosSUBE.BoletosXIdentificador;

            return dic;
        }
    }
}
