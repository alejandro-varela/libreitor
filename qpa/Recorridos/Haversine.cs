using Recorridos;
using System;

namespace Recorridos
{
    public class Haversine
    {
        // TODO: hacer algo con las alturas...

        public static double GetDist(double lat1, double long1, double lat2, double long2)
        {
            //  Planeta tierra:
            //      -> Radio ecuatorial (a): 6378 km
            //      -> Radio polar      (b): 6357 km
            //
            //  Atención:
            //      Esta función no funciona si estamos en otro planeta o
            //      si el planeta deja de ser una bola gigante.

            const double TO_RADIAN = (Math.PI / 180);

            double radioTierra = 6371000;      // Radio de la tierra en metros.

            double medio_dLat = 0.5 * TO_RADIAN * (lat2 - lat1);
            double medio_dLon = 0.5 * TO_RADIAN * (long2 - long1);

            double a = Math.Sin(medio_dLat) * Math.Sin(medio_dLat) +
                       Math.Cos(lat1 * TO_RADIAN) * Math.Cos(lat2 * TO_RADIAN) *
                       Math.Sin(medio_dLon) * Math.Sin(medio_dLon);

            return radioTierra * 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
        }

        public static double GetDist(Punto p1, Punto p2)
        {
            return GetDist(
                p1.Lat, p1.Lng, 
                p2.Lat, p2.Lng
            );
        }
    }
}
