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
    }
}
