using System.Collections.Generic;

namespace Comun
{
    public class Grupoide<TPunto> where TPunto : Punto
    { 
        public string Nombre { get; set; }
        public PuntaLinea PuntaLinea { get; set; }
        public List<PuntoCamino<TPunto>> Nodos { get; set; } = new();

        public override string ToString()
        {
            return $"{Nombre} {Nodos.Count}";
        }
    }
}
