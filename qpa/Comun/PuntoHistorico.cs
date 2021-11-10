using System;
using System.Collections.Generic;

namespace Comun
{
    public class PuntoHistorico : Punto, IEquatable<PuntoHistorico>
    {
        public DateTime Fecha { get; set; }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Alt, Lat, Lng, Fecha);
        }

        public override string ToString()
        {
            return $"{base.ToString()} Fecha={Fecha}";
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
