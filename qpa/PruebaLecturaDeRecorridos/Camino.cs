using Recorridos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaLecturaDeRecorridos
{
    public class Camino
    {
        private class PuntoTristate
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

        public List<PuntaLinea> Nodos = new();

        public string Description
        {
            get
            {
                //var arr = Nodos
                //    .Select(pl => pl.Nombre)
                //    .Simplificar((nombre1, nombre2) => nombre1 == nombre2)
                //    .Where(s => s != "." && s != "?")
                //    .ToArray()
                //;

                var arr = DescriptionRawSinRuido
                    .Where(s => s != '.' && s != '?')
                    .ToArray()
                ;

                return string.Join(string.Empty, arr); 
            }
        }

        public string DescriptionRawSinRuido
        {
            get
            {
                return QuitarRuido(DescriptionRaw);
            }
        }

        public string DescriptionRaw
        {
            get
            {
                var arr = Nodos
                    .Select(pl => pl.Nombre)
                    .Simplificar((nombre1, nombre2) => nombre1 == nombre2)
                    .ToArray()
                ;

                return string.Join(string.Empty, arr);
            }
        }

        public string DescriptionRawSinSimplificar
        {
            get
            {
                var arr = Nodos
                    .Select(pl => pl.Nombre)
                    .ToArray()
                ;

                return string.Join(string.Empty, arr);
            }
        }

        static string QuitarRuido(string sRuidosa)
        {
            var simbols = sRuidosa
                .ToCharArray()
                .Distinct()
                .Where(c => c != '?')
                .Where(c => c != '.')
                .ToArray()
            ;

            var newString = string.Empty;

            foreach (var simbol in simbols)
            {
                string deforme = $"{simbol}?{simbol}";
                sRuidosa = sRuidosa.Replace($"{simbol}?{simbol}", simbol.ToString());
            }

            return sRuidosa;
        }

        // debe retornar tres cosas:
        //    enpunta...
        //    no punta...
        //    indet (por estar muy cerca del borde de lo que es una punta o no)
        static PuntoTristate EnPuntaTristate(Punto punto, IEnumerable<PuntaLinea> puntas)
        {
            foreach (var puntaX in puntas)
            {
                var dist = Haversine.GetDist(punto, puntaX.Punto);

                if (dist < puntaX.Radio)
                {
                    return new PuntoTristate
                    {
                        Type = PuntoTristate.PuntoTristateType.Punta,
                        Punta = puntaX
                    };
                }

                if (dist < (puntaX.Radio + 150))
                {
                    return new PuntoTristate
                    {
                        Type = PuntoTristate.PuntoTristateType.Indet
                    };
                }
            }

            return new PuntoTristate
            {
                Type = PuntoTristate.PuntoTristateType.Normal
            };
        }

        public static Camino CreateFromPuntos(IEnumerable<PuntaLinea> puntas, IEnumerable<Punto> puntos)
        {
            Camino camino = new();

            foreach (var puntoX in puntos)
            {
                var puntaTristate = EnPuntaTristate(puntoX, puntas);

                switch (puntaTristate.Type)
                {
                    case PuntoTristate.PuntoTristateType.Punta:
                        camino.Nodos.Add(puntaTristate.Punta);
                        break;
                    case PuntoTristate.PuntoTristateType.Normal:
                        camino.Nodos.Add(new PuntaLinea { Nombre = "." });
                        break;
                    case PuntoTristate.PuntoTristateType.Indet:
                        camino.Nodos.Add(new PuntaLinea { Nombre = "?" });
                        break;
                }
            }

            return camino;
        }

        public static Camino CreateFromRecorrido(IEnumerable<PuntaLinea> puntas, RecorridoLinBan recorridoLinBan)
        {
            return CreateFromPuntos(puntas, recorridoLinBan.Puntos);
        }
    }
}
