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
        public List<PuntaLinea> Nodos = new();

        public string Description
        {
            get
            {
                var arr = Nodos
                    .Select(pl => pl.Nombre)
                    .Simplificar((nombre1, nombre2) => nombre1 == nombre2)
                    .Where(s => s != "." && s != "?")
                    .ToArray()
                ;

                return string.Join(string.Empty, arr);
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

            //Camino camino = new();

            //foreach (var prec in recorridoLinBan.Puntos)
            //{
            //    var puntaTristate = EnPuntaTristate(prec, puntas);

            //    switch (puntaTristate.Type)
            //    {
            //        case PuntoTristate.PuntoTristateType.Punta:
            //            camino.Nodos.Add(puntaTristate.Punta);
            //            break;
            //        case PuntoTristate.PuntoTristateType.Normal:
            //            camino.Nodos.Add(new PuntaLinea { Nombre = "." });
            //            break;
            //        case PuntoTristate.PuntoTristateType.Indet:
            //            camino.Nodos.Add(new PuntaLinea { Nombre = "?" });
            //            break;
            //    }
            //}

            //return camino;
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
                        Type  = PuntoTristate.PuntoTristateType.Punta,
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
                Type  = PuntoTristate.PuntoTristateType.Normal
            };
        }

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
    }
}
