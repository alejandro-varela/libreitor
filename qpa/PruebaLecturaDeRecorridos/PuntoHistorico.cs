using Recorridos;
using System;

namespace PruebaLecturaDeRecorridos
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
