using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comun
{
    public static class ExtensionesParaElCaso
    {
        static IEnumerable<PuntoRecorrido> CrearPuntosIntermediosNaif(
            PuntoRecorrido puntoA,
            PuntoRecorrido puntoB,
            double cant
        )
        {
            var latDiff = puntoB.Lat - puntoA.Lat;
            var lngDiff = puntoB.Lng - puntoA.Lng;

            var latPart = latDiff / cant;
            var lngPart = lngDiff / cant;

            // para que no aparezca distancia cero poner (cantPuntosRelleno - 1)
            for (int i = 0; i < cant - 1; i++)
            {
                yield return new PuntoRecorrido
                {
                    Cuenta  = puntoA.Cuenta,
                    Index   = i + 1,
                    Alt     = puntoA.Alt,
                    Lat     = puntoA.Lat + (latPart * (i + 1)),
                    Lng     = puntoA.Lng + (lngPart * (i + 1)),
                };
            }
        }

        static IEnumerable<PuntoHistorico> CrearPuntosIntermediosNaif(
            PuntoHistorico puntoA,
            PuntoHistorico puntoB,
            double cant
        )
        {
            var latDiff = puntoB.Lat - puntoA.Lat;
            var lngDiff = puntoB.Lng - puntoA.Lng;

            var latPart = latDiff / cant;
            var lngPart = lngDiff / cant;

            // para que no aparezca distancia cero poner (cantPuntosRelleno - 1)
            for (int i = 0; i < cant - 1; i++)
            {
                yield return new PuntoHistorico
                {
                    Fecha   = puntoA.Fecha,
                    Alt     = puntoA.Alt,
                    Lat     = puntoA.Lat + (latPart * (i + 1)),
                    Lng     = puntoA.Lng + (lngPart * (i + 1)),
                };
            }
        }

        public static IEnumerable<PuntoRecorrido> HacerGranular(
            this IEnumerable<PuntoRecorrido> recorrido,
            double maxDistMetros
        )
        {
            PuntoRecorrido puntoAnterior = default;

            foreach (var puntoActual in recorrido)
            {
                if (puntoAnterior == null)
                {
                    // retorno el primer punto
                    yield return puntoActual;
                }
                else
                {
                    // sacar distacia entre anterior y actual
                    var distancia = Haversine.GetDist(
                        puntoAnterior.Lat, puntoAnterior.Lng,
                        puntoActual.Lat, puntoActual.Lng
                    );

                    // sacar "cantPuntosIntermedios" = Math.Ceiling(distancia / maxDistMetros)
                    // 0,0 <-- anterior
                    //    1,1 <-- intermedio
                    //       2,2 <-- intermedio
                    //          3,3 <-- actual
                    var cantPuntosIntermedios = Math.Ceiling(distancia / maxDistMetros);

                    // creamos los puntos intermedios y los retornamos
                    var puntosIntermediosNaif = CrearPuntosIntermediosNaif(
                        puntoAnterior,
                        puntoActual,
                        cantPuntosIntermedios
                    );

                    foreach (var puntoIntermedio in puntosIntermediosNaif)
                    {
                        yield return puntoIntermedio;
                    }

                    // luego yield retornaremos el punto actual
                    yield return puntoActual;
                }

                puntoAnterior = puntoActual;
            }
        }

        public static IEnumerable<PuntoHistorico> HacerGranular(
            this IEnumerable<PuntoHistorico> recorrido,
            double maxDistMetros
        )
        {
            PuntoHistorico puntoAnterior = default;

            foreach (var puntoActual in recorrido)
            {
                if (puntoAnterior == null)
                {
                    // retorno el primer punto
                    yield return puntoActual;
                }
                else
                {
                    // sacar distacia entre anterior y actual
                    var distancia = Haversine.GetDist(
                        puntoAnterior.Lat, puntoAnterior.Lng,
                        puntoActual.Lat, puntoActual.Lng
                    );

                    // sacar "cantPuntosIntermedios" = Math.Ceiling(distancia / maxDistMetros)
                    // 0,0 <-- anterior
                    //    1,1 <-- intermedio
                    //       2,2 <-- intermedio
                    //          3,3 <-- actual
                    var cantPuntosIntermedios = Math.Ceiling(distancia / maxDistMetros);

                    // creamos los puntos intermedios y los retornamos
                    var puntosIntermediosNaif = CrearPuntosIntermediosNaif(
                        puntoAnterior,
                        puntoActual,
                        cantPuntosIntermedios
                    );

                    foreach (var puntoIntermedio in puntosIntermediosNaif)
                    {
                        yield return puntoIntermedio;
                    }

                    // luego yield retornaremos el punto actual
                    yield return puntoActual;
                }

                puntoAnterior = puntoActual;
            }
        }

        public static IEnumerable<T> AchicarPorAcumulacionMetros<T>(
            this IEnumerable<T> recorrido,
            int maxMetros
        ) where T : Punto
        {
            double acumMetros = 0.0;
            T puntoAnterior = default;

            foreach (var puntoActual in recorrido)
            {
                var dist = 0.0;

                if (puntoAnterior == null)
                {
                    yield return puntoActual;
                }
                else
                {
                    dist = Haversine.GetDist(puntoAnterior.Lat, puntoAnterior.Lng, puntoActual.Lat, puntoActual.Lng);

                    if (dist + acumMetros >= maxMetros)
                    {
                        acumMetros = 0.0;
                        yield return puntoAnterior;
                    }

                    acumMetros += dist;
                }

                // guardo el punto
                puntoAnterior = puntoActual;
            }

            // devuelvo el último punto guardado, si acumMetros > 0
            if (acumMetros > 0.0)
            {
                yield return puntoAnterior;
            }
        }

        public static string Stringificar<T>(this IEnumerable<T> source, string separator = "")
        {
            var arr = source
                .Select (e => e.ToString())
                .ToArray()
            ;

            var str = string.Join(separator, arr);

            return str;
        }

        //public static string Stringify(this IEnumerable<string> source)
        //{
        //    var arr = source.ToArray();
        //    var str = string.Join(string.Empty, arr);
        //    return str;
        //}

        //public static string Stringificar(this IEnumerable<char> source)
        //{
        //    return source
        //        .Select(c => c.ToString())
        //        .Stringify()
        //    ;
        //}
    }
}
