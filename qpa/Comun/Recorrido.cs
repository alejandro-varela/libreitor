using System.Linq;
using System.Collections.Generic;

namespace Comun
{
    public class Recorrido
    {
        public IEnumerable<PuntoRecorrido> Puntos { get; set; }

        public PuntoRecorrido PuntoSalida
        {
            get
            {
                if (Puntos != null)
                {
                    return Puntos.First();
                }

                return null;
            }
        }

        public PuntoRecorrido PuntoLlegada
        {
            get
            {
                if (Puntos != null)
                {
                    return Puntos.Last();
                }

                return null;
            }
        }

        // inst 
        public int DameCuentaMasCercanaA(Punto punto)
        {
            return DameCuentaMasCercanaA(punto, Puntos);
        }

        // static
        public static int DameCuentaMasCercanaA(Punto punto, IEnumerable<PuntoRecorrido> puntos)
        {
            var minDist = double.MaxValue;
            var cuenta  = -1;

            foreach (var prec in puntos)
            {
                var dist = Haversine.GetDist(prec, punto);

                if (dist < minDist)
                {
                    cuenta = prec.Cuenta;
                    minDist = dist;
                }
            }

            return cuenta;
        }
    }
}
