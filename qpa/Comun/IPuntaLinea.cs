namespace Comun
{
    public class InfoPunto
    {
        public EstadoPuntoEnPunta Estado { get; set; }

        public double DistanciaAlCentroide { get; set; }
    }

    public interface IPuntaLinea
    {    
        string Nombre { get; set; }

        InfoPunto GetInformacionPunto(Punto px);
    }
}
