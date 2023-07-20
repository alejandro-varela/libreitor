using System;
using System.Collections.Generic;
using System.Linq;

namespace Comun
{
    // es un trapezio
    public class Topes2D
    {
        public double Left     { get; set; }
        public double Right    { get; set; }
        public double Top      { get; set; }
        public double Bottom   { get; set; }

        public static Topes2D CreateFromPuntos(IEnumerable<Punto> puntos)
        {
            // tomo el primer punto de la colección
            var primerPunto = puntos.FirstOrDefault();

            // si no hay chau, retorno null
            if (primerPunto == null)
            {
                return null;
            }

            // inicializo top  y bottom con el primer valor de Lat que encuentro en la colección
            // inicializo left y right  con el primer valor de Lng que encuentro en la colección
            var top = primerPunto.Lat;
            var bottom = primerPunto.Lat;
            var left = primerPunto.Lng;
            var right = primerPunto.Lng;

            foreach (var punto in puntos)
            {
                /////////////////////////////////////////////////////
                // la latitud (sentido vertical) 
                //            (-32) norte
                //              |
                //              |
                //            (-33) sur

                // el valor mayor de Lat (norte) corresponde al Top
                if (punto.Lat > top)
                {
                    top = punto.Lat;
                }

                // el valor menor de Lat (sur) corresponde al Bottom
                if (punto.Lat < bottom)
                {
                    bottom = punto.Lat;
                }

                /////////////////////////////////////////////////////
                // la longitud (sentido horizontal) 
                //
                // oeste (-60)---------(-59) este

                // el valor menor de Lng (oeste) corresponde al Left
                if (punto.Lng < left)
                {
                    left = punto.Lng;
                }

                // el valor mayor de Lng (este) corresponde al Right
                if (punto.Lng > right)
                {
                    right = punto.Lng;
                }
            }

            return new Topes2D
            {
                Left    = left,
                Right   = right,
                Top     = top,
                Bottom  = bottom,
            };
        }

        public Punto PuntoNorOeste
        {
            get
            {
                return new Punto
                {
                    Lat = Top,
                    Lng = Left,
                };
            }
        }

        public Punto PuntoNorEste
        {
            get
            {
                return new Punto
                {
                    Lat = Top,
                    Lng = Right,
                };
            }
        }

        public Punto PuntoSurOeste
        {
            get
            {
                return new Punto
                {
                    Lat = Bottom,
                    Lng = Left,
                };
            }
        }

        public Punto PuntoSurEste
        {
            get
            {
                return new Punto
                {
                    Lat = Bottom,
                    Lng = Right,
                };
            }
        }

        public double AnchoArribaMts
        {
            get
            {
                return Haversine.GetDist(PuntoNorOeste, PuntoNorEste);
            }
        }

        public double AnchoAbajoMts
        {
            get
            {
                return Haversine.GetDist(PuntoSurOeste, PuntoSurEste);
            }
        }

        public double AnchoMaximoMts
        {
            get
            {
                return Math.Max(AnchoArribaMts, AnchoAbajoMts);
            }
        }

        public double AlturaMts
        {
            get
            {
                return Haversine.GetDist(PuntoNorOeste, PuntoSurOeste);
            }
        }

        public int GetAnchoGranular(double granularidad)
        {
            // 0 1 2 3 4 5 6 ... N
            var maxIndexCasilleroHorizontal = Convert.ToInt32(Math.Ceiling(this.AnchoMaximoMts / granularidad));
            return maxIndexCasilleroHorizontal + 1; // N + 1
        }

        public int GetAlturaGranular(double granularidad)
        {
            // 0 1 2 3 4 5 6 ... N
            var maxIndexCasilleroVertical = Convert.ToInt32(Math.Ceiling(this.AlturaMts / granularidad));
            return maxIndexCasilleroVertical + 1; // N + 1
        }

        public static Topes2D CreateFromTopes(params Topes2D[] grupoTopes2D)
        {
            if (grupoTopes2D == null || grupoTopes2D.Length == 0)
            {
                return new Topes2D();
            }

            if (grupoTopes2D.Length == 1)
            {
                return grupoTopes2D[0];
            }

            // Ok si pasaron bien los argumentos...

            Topes2D ret = grupoTopes2D[0];

            foreach (var topx in grupoTopes2D.Skip(1))
            {
                // Left
                if (topx.Left < ret.Left)
                { 
                    ret.Left = topx.Left;
                }

                // Right
                if (topx.Right > ret.Right)
                {
                    ret.Right = topx.Right;
                }

                // Bottom
                if (topx.Bottom < ret.Bottom)
                { 
                    ret.Bottom = topx.Bottom;
                }

                // Top
                if (topx.Top > ret.Top)
                { 
                    ret.Top = topx.Top;
                }
            }

            return ret;
        }
    }
}
