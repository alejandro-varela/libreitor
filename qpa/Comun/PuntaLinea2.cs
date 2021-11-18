using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Comun
{
    public class PuntaLinea2 : IPuntaLinea
    {
        public string Nombre { get; set; }
        public List<PuntoRecorrido> Puntos { get; set; }
        public List<Tuple<int, int>> Varios { get; set; }
        public PuntoRecorrido Punto { get; internal set; }

        public double Radio { get; internal set; }

        public double AnchoIndeterminacion { get; set; } = 150.0;
        public object RadioAgrupacion { get; internal set; }

        public IPuntaLinea.InfoPunto GetInformacionPunto(Punto px)
        {
            /*             
              38_070_230 ¡38M veces!
             225_524 para empezar...
                :|
            333_159_399
            673_312_247

            222_275 para empezar...
            249_249 27000 llamadas mas o menos...
             
             */

            // busco la distancia minima a cualquier punto de la punta de línea
            // para eso creo una secuencia de distancias
            // y luego busco el mínimo elemento de la secuencia

            var llamadas1 = Haversine.GetLlamadas();
            var distancias = Puntos
                .Select(punto => Haversine.GetDist(punto, px))
                .ToList()
            ;

            var llamadas2 = Haversine.GetLlamadas();

            var minDist = distancias
                .Min()
            ;

            if (minDist <= Radio)
            {
                return new IPuntaLinea.InfoPunto { DistanciaAlCentroide = minDist, Estado = EstadoPuntoEnPunta.Punta };
            }
            else if (minDist <= Radio + AnchoIndeterminacion)
            {
                return new IPuntaLinea.InfoPunto { DistanciaAlCentroide = minDist, Estado = EstadoPuntoEnPunta.Indet };
            }
            else
            {
                return new IPuntaLinea.InfoPunto { DistanciaAlCentroide = minDist, Estado = EstadoPuntoEnPunta.Normal };
            }
        }

        public override string ToString()
        {
            return $"{ Nombre } { Punto.Lat.ToString().Replace(',', '.') }, { Punto.Lng.ToString().Replace(',', '.') }";
        }
    }
}
