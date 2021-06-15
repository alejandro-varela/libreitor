using System.Linq;
using System.Collections.Generic;

namespace Recorridos
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

        public int DameCuentaMasCercanaA(Punto punto)
        {
            var minDist = double.MaxValue;
            var cuenta  = -1;

            foreach (var prec in Puntos)
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
