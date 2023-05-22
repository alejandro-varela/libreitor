using System;
using System.Linq;

namespace Comun
{
    public class RecorridoLinBan : Recorrido
    {
        public int      Linea           { get; set; }
        public int      Bandera         { get; set; }
        public int      Version         { get; set; }
        public DateTime FechaActivacion { get; set; }

        public override string ToString()
        {
            return $"Reco Lin={Linea} Ban={Bandera} Ver={Version} CantPts={this.Puntos.Count()}";
        }
    }
}
