using Comun;
using System;

namespace Comun
{
    public class PuntoHistorico : Punto
    { 
        public DateTime Fecha { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} {Fecha}";
        }
    }
}
