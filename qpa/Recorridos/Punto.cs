using System;

namespace Recorridos
{
    public class Punto
    {
        public double Alt { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public static Punto FromLatLng(
            double lat,
            double lng,
            double alt
        )
        {
            return new Punto()
            {
                Alt = alt,
                Lat = lat,
                Lng = lng,
            };
        }

        public static Punto FromDegrees(
            int latDegrees, int latMinutes, double latSeconds,
            int lngDegrees, int lngMinutes, double lngSeconds,
            double alt
        )
        {
            var lat = latDegrees + ((1.0 / 60.0) * latMinutes) + ((1.0 / 3600.0) * latSeconds);
            var lng = lngDegrees + ((1.0 / 60.0) * lngMinutes) + ((1.0 / 3600.0) * lngSeconds);

            return new Punto()
            {
                Alt = alt,
                Lat = lat,
                Lng = lng,
            };
        }

        public override string ToString()
        {
            return $"Lat={Lat} Lng={Lng} Alt={Alt}";
        }

        public bool EsCercanoA(Punto otro, int radioEnMetros)
        {
            var dist = Haversine.GetDist(this, otro);
            return dist <= radioEnMetros;
        }
    }
}
