using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comun
{
    public class Geom
    {
        public static HashSet<Casillero> PuntosAHashSetCasilleros(
            IEnumerable<Punto>  puntos,
            int                 granularidad,
            Topes2D             topes2D
        )
        { 
            return puntos
                .Select     (puntoReco => Casillero.Create(topes2D, puntoReco, granularidad))
                .ToHashSet  ()
            ;
        }

        public static bool PuntoEnRecorrido(
            Punto               punto,
            HashSet<Casillero>  casillerosDelRecorrido, 
            int                 granularidad, 
            Topes2D             topes2D
        )
        {
            var casReal = Casillero.Create(topes2D, punto, granularidad);
            return (casReal.PresenteFlexEn(casillerosDelRecorrido));
        }

        public static bool PuntoEnRecorrido(
            Punto               punto,
            IEnumerable<Punto>  recorrido, 
            int                 granularidad, 
            Topes2D             topes2D
        )
        {
            var casillerosDelRecorrido = PuntosAHashSetCasilleros(
                recorrido, granularidad, topes2D
            );

            return PuntoEnRecorrido(punto, casillerosDelRecorrido, granularidad, topes2D);
        }

        public static int Puntuacion(
            IEnumerable<Punto>  puntosReales,
            IEnumerable<Punto>  puntosTeoricos,
            int                 granularidad,
            Topes2D             topes2D
        )
        {
            var casillerosDelRecorrido = PuntosAHashSetCasilleros(
                puntosTeoricos, granularidad, topes2D
            );

            int puntaje = puntosReales
                .Select (puntoReal => Casillero.Create(topes2D, puntoReal, granularidad))
                .Where  (casillero => casillero.PresenteFlexEn(casillerosDelRecorrido))
                .Count  ()
            ;

            return puntaje;
        }
    }
}
