namespace Comun
{
    public class PuntoCamino<TPunto> where TPunto : Punto
    {
        public TPunto       PuntoAsociado   { get; set; }
        public IPuntaLinea  PuntaDeLinea    { get; set; }
    }
}
