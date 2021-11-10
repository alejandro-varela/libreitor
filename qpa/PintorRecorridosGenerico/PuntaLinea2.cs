using Comun;
using System;
using System.Collections.Generic;

namespace PintorRecorridosGenerico
{
    public class PuntaLinea2
    {
        public string Nombre { get; set; }
        public List<PuntoRecorrido> Puntos { get; set; }
        public List<Tuple<int, int>> Varios { get; set; }
    }
}