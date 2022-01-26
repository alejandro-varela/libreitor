using System;

namespace Comun
{
    public class BoletoComun
    {
        public string   Id              { get; set; }
        public double   ValorInicial    { get; set; }
        public double   ValorDescuento  { get; set; }
        public double   ValorFinal      { get; set; }
        public DateTime FechaCancelacion{ get; set; }
        public double   Latitud         { get; set; }
        public double   Longitud        { get; set; }

        public override string ToString()
        {
            return $"Id={Id} {FechaCancelacion} Lat={Latitud } Lng={Longitud} Val={ValorFinal}";
        }
    }
}
