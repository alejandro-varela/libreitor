using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comun;

namespace Comun
{
    public class PuntasDeLinea
    {
        public static IEnumerable<PuntaLinea> GetPuntasNombradas(
            IEnumerable<RecorridoLinBan> recorridos, 
            int radio
        )
        {
            List<PuntaLinea> puntas = new List<PuntaLinea>();
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
            var nombre = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz"[n].ToString();
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

        public static bool EsPunta(Punto px, IEnumerable<PuntaLinea> puntas, int? unRadio)
        {
            return puntas.Any(punta => punta.PuntoAdentro(px, unRadio));
        }
    }

    public class PuntaLinea
    {
        public Punto    Punto       { get; set; }
        public Punto    Centroide   { get { return Punto; } }
        public string   Nombre      { get; set; }
        public int      Radio       { get; set; }
        public List<Tuple<int, int>> Varios { get; set; }

        public (bool, double, double) PuntoAdentroYDists(Punto px, int? unRadio = null)
        {
            var adentro     = PuntoAdentro      (px, unRadio);
            var distCentro  = DistanciaAlCentro (px);
            var distBorde   = DistanciaAlBorde  (px, unRadio);
            return (adentro, distCentro, distBorde);
        }

        public bool PuntoAdentro(Punto px, int? unRadio = null)
        {
            var radio = unRadio ?? Radio;
            var dist  = Haversine.GetDist(px, Centroide);
            return dist <= radio;
        }

        public double DistanciaAlCentro(Punto px)
        {
            return Haversine.GetDist(px, Centroide);
        }

        public double DistanciaAlBorde(Punto px, int? unRadio = null)
        {
            var radio = unRadio ?? Radio;
            return Haversine.GetDist(px, Centroide) - radio;
        }

        public override string ToString()
        {
            return $"{ Nombre } { Punto.Lat.ToString().Replace(',', '.') }, { Punto.Lng.ToString().Replace(',', '.') }";
        }
    }
}
