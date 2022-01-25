using System;
using System.Collections.Generic;

namespace ComunSUBE
{
    public class ParEmpresaInterno : IEquatable<ParEmpresaInterno>
    { 
		public int Empresa { get; set; }
		public int Interno { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ParEmpresaInterno);
        }

        public bool Equals(ParEmpresaInterno other)
        {
            return 
                other != null &&
                Empresa == other.Empresa &&
                Interno == other.Interno
            ;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Empresa, Interno);
        }

        public static bool operator ==(ParEmpresaInterno left, ParEmpresaInterno right)
        {
            return EqualityComparer<ParEmpresaInterno>.Default.Equals(left, right);
        }

        public static bool operator !=(ParEmpresaInterno left, ParEmpresaInterno right)
        {
            return !(left==right);
        }

        public override string ToString()
        {
            return $"Empresa={Empresa} Interno={Interno}";
        }
    }
}
