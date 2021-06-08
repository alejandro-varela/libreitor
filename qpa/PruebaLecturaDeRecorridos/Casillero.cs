using Recorridos;
using System;

namespace PruebaLecturaDeRecorridos
{
    public class Casillero
    {
        public int IndexHorizontal { get; set; }
        public int IndexVertical   { get; set; }

        public string FixedToString(string format)
        { 
            return $"h{IndexHorizontal.ToString(format)}v{IndexVertical.ToString(format)}";
        }

        public override string ToString()
        {
            //return $"c[{IndexHorizontal},{IndexVertical}]";
            return $"h{IndexHorizontal}v{IndexVertical}";
        }

        public static Casillero DameCasillero(Topes2D topes2D, Punto puntoide, int granularidad)
        {
            // se puede precalc

            var maxIndexCasilleroHorizontal = Convert.ToInt32(Math.Ceiling(topes2D.AnchoMaximoMts / granularidad));
            var maxIndexCasilleroVertical = Convert.ToInt32(Math.Ceiling(topes2D.AlturaMts / granularidad));

            var deltaLng = topes2D.Right - topes2D.Left;
            var deltaLat = topes2D.Top - topes2D.Bottom;

            // no se puede precalc

            var deltaPuntoideLng = puntoide.Lng - topes2D.Left;
            var deltaPuntoideLat = puntoide.Lat - topes2D.Bottom;

            System.Diagnostics.Debug.Assert(deltaPuntoideLng <= deltaLng);
            System.Diagnostics.Debug.Assert(deltaPuntoideLat <= deltaLat);

            var indexCasilleroHorizontal = Convert.ToInt32((deltaPuntoideLng * maxIndexCasilleroHorizontal) / deltaLng);
            var indexCasilleroVertical = Convert.ToInt32((deltaPuntoideLat * maxIndexCasilleroVertical) / deltaLat);

            System.Diagnostics.Debug.Assert(indexCasilleroVertical <= maxIndexCasilleroVertical);
            System.Diagnostics.Debug.Assert(indexCasilleroHorizontal <= maxIndexCasilleroHorizontal);

            System.Diagnostics.Debug.Assert(indexCasilleroVertical >= 0);
            System.Diagnostics.Debug.Assert(indexCasilleroHorizontal >= 0);

            // deltaLng         = 1000 casilleros
            // deltaPuntoideLng = ?

            // 5000        1000
            //  500          ?= 100 ((500 * 1000) / 5000)

            return new Casillero
            {
                IndexHorizontal = indexCasilleroHorizontal,
                IndexVertical = indexCasilleroVertical,
            };
        }
    }
}