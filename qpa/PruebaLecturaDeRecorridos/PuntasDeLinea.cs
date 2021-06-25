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
        // las puntas se podrían agrupar usando el algoritmo "#" y mirando la superficie del cuadrado enmarcado
        public static IEnumerable<PuntaLinea> GetPuntasNombradas(IEnumerable<RecorridoLinBan> recorridos, int radio)
        {
            List<PuntaLinea> puntas = new();
            int n = 0;
            foreach (var recorrido in recorridos)
            {
                n = AgregarSiNoEstaPresente(recorrido.Linea, recorrido.Bandera, puntas, recorrido.PuntoSalida , n, radio);
                n = AgregarSiNoEstaPresente(recorrido.Linea, recorrido.Bandera, puntas, recorrido.PuntoLlegada, n, radio);
            }
            return puntas;
        }

        static int AgregarSiNoEstaPresente(int linea, int bandera, List<PuntaLinea> puntas, PuntoRecorrido punto, int n, int radio)
        {
            foreach (var puntaX in puntas)
            {
                var dist = Haversine.GetDist(puntaX.Punto, punto);
                if (dist < radio)
                {
                    puntaX.Varios.Add(new Tuple<int, int>(linea, bandera));
                    return n;
                }
            }

            var varios = new List<Tuple<int, int>>();
            varios.Add(new Tuple<int, int>(linea, bandera));
            var nombre = ((char)(n + 'A')).ToString();
            puntas.Add(new PuntaLinea { Punto = punto, Nombre = nombre, Varios = varios, Radio = radio });

            return n + 1;
        }

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
                var dist = Haversine.GetDist(punta, ph);
                if (dist <= radioEnMetros)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class PuntaLinea
    {
        public Punto Punto { get; set; }
        public string Nombre { get; set; }
        public int Radio { get; set; }
        public List<Tuple<int, int>> Varios { get; set; }
    }
}
