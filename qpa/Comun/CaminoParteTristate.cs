namespace Comun
{
    public partial class Camino<TPunto> where TPunto : Punto
    {
        public class CaminoParteTristate
        {
            public enum PuntoTristateType
            {
                Indet,
                Punta,
                Normal,
            }

            public PuntoTristateType Type { get; set; }
            public PuntaLinea Punta { get; set; }
        }
    }
}
