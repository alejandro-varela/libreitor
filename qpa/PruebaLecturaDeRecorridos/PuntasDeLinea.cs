using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recorridos;

namespace PruebaLecturaDeRecorridos
{
    public class PuntasDeLinea
    {
        public static IEnumerable<Punto> Get(IEnumerable<RecorridoLinBan> recorridos)
        {
            foreach (var recorrido in recorridos)
            {
                yield return recorrido.PuntoSalida;
                yield return recorrido.PuntoLlegada;
            }
        }

        public static bool EsPunta(PuntoHistorico ph, IEnumerable<Punto> puntas, int radioEnMetros)
        {
            foreach (var punta in puntas)
            {
                var dist = Haversine.GetDist(punta, ph.Punto);
                if (dist <= radioEnMetros)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
