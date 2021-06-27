using System;

namespace Comun
{
    public class PuntoRecorrido : Punto
    {
        public int Cuenta   { get; set; }
        public int Index    { get; set; }

        public static PuntoRecorrido CreateFromBuffer(byte[] buffer)
        {
            var lat     = BitConverter.ToSingle(buffer, 4);
            var lng     = BitConverter.ToSingle(buffer, 8);
            var cuenta  = BitConverter.ToUInt16(buffer, 12);

            return new PuntoRecorrido
            {
                Alt     = 0,
                Lat     = -lat,
                Lng     = -lng,
                Cuenta  = cuenta,
                Index   = 0,
            };
        }

        public override string ToString()
        {
            return $"Cta={Cuenta:0000}.{Index} Lat={Lat:00.00000} Lng={Lng:00.00000}";
        }
    }
}
