using Comun;

namespace Comun
{
    public class PuntoCamino<TPunto> where TPunto : Punto
    {
        public TPunto       PuntoAsociado   { get; set; }
        public PuntaLinea   PuntaDeLinea    { get; set; }
    }
}
