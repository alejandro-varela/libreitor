using System;
using System.Collections.Generic;

namespace Comun
{
    public class PuntoHistorico : Punto, IEquatable<PuntoHistorico>
    {
        public DateTime Fecha { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} Fecha={Fecha}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PuntoHistorico);
        }

        public bool Equals(PuntoHistorico other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Alt == other.Alt &&
                   Lat == other.Lat &&
                   Lng == other.Lng &&
                   Fecha == other.Fecha;
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), Alt, Lat, Lng, Fecha);
        //}

        public override int GetHashCode()
        {
            int hashCode = 621656858;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Alt.GetHashCode();
            hashCode = hashCode * -1521134295 + Lat.GetHashCode();
            hashCode = hashCode * -1521134295 + Lng.GetHashCode();
            hashCode = hashCode * -1521134295 + Fecha.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(PuntoHistorico left, PuntoHistorico right)
        {
            return EqualityComparer<PuntoHistorico>.Default.Equals(left, right);
        }

        public static bool operator !=(PuntoHistorico left, PuntoHistorico right)
        {
            return !(left == right);
        }
    }
}
