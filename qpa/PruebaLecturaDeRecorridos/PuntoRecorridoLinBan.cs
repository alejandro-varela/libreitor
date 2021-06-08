using Recorridos;
using System;

namespace PruebaLecturaDeRecorridos
{
    public class PuntoRecorridoLinBan : PuntoRecorrido
    {
        public int Linea    { get; set; }
        public int Bandera  { get; set; }

        public PuntoRecorridoLinBan()
        {
            //
        }

        public PuntoRecorridoLinBan(PuntoRecorrido prec, int lin, int ban) : this()
        {
            Cuenta  = prec.Cuenta;
            Index   = prec.Index;
            Alt     = prec.Alt;
            Lat     = prec.Lat;
            Lng     = prec.Lng;
            Linea   = lin;
            Bandera = ban;
        }

        public override string ToString()
        {
            return $"Lin={Linea:0000} Ban={Bandera:0000} Cta={Cuenta:0000}.{Index} Lat={Lat:00.00000} Lng={Lng:00.00000}";
        }
    }
}
