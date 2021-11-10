namespace Comun
{
    public interface IPuntaLinea
    {
        public class InfoPunto
        { 
            public EstadoPuntoEnPunta Estado { get; set; }

            public double DistanciaAlCentroide { get; set; }
        }

        string Nombre { get; set; }

        InfoPunto GetInformacionPunto(Punto px);
    }
}
