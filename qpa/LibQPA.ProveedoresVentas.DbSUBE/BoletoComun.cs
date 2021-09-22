using System;

namespace LibQPA.ProveedoresVentas.DbSUBE
{
    public class BoletoComun
    {
        public double   ValorInicial     { get; set; }
        public double   ValorDescuento   { get; set; }
        public double   ValorFinal       { get; set; }
        public DateTime FechaCancelacion { get; set; }
        public double   Latitud          { get; set; }
        public double   Longitud         { get; set; }
    }
}
