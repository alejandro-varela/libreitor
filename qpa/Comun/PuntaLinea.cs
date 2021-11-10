using System;
using System.Collections.Generic;

namespace Comun
{
    public class PuntaLinea : IPuntaLinea
    {
        public Punto    Punto       { get; set; }
        public Punto    Centroide   { get { return Punto; } }
        public string   Nombre      { get; set; }
        public double   Radio       { get; set; }
        public List<Tuple<int, int>> Varios { get; set; }

        public double AnchoIdenterminacion { get; set; } = 150.0;

        public IPuntaLinea.InfoPunto GetInformacionPunto(Punto px)
        {
            var dist = Haversine.GetDist(px, Centroide);

            if (dist <= Radio)
            {
                return new IPuntaLinea.InfoPunto { DistanciaAlCentroide = dist, Estado = EstadoPuntoEnPunta.Punta };
            }
            else if (dist <= Radio + AnchoIdenterminacion)
            {
                return new IPuntaLinea.InfoPunto { DistanciaAlCentroide = dist, Estado = EstadoPuntoEnPunta.Indet };
            }
            else
            {
                return new IPuntaLinea.InfoPunto { DistanciaAlCentroide = dist, Estado = EstadoPuntoEnPunta.Normal };
            }
        }

        public bool PuntoAdentro(Punto px, int? unRadio = null)
        {
            var radio = unRadio ?? Radio;
            var dist  = Haversine.GetDist(px, Centroide);
            return dist <= radio;
        }

        public override string ToString()
        {
            return $"{ Nombre } { Punto.Lat.ToString().Replace(',', '.') }, { Punto.Lng.ToString().Replace(',', '.') }";
        }
    }
}
