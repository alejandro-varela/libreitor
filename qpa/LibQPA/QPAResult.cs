using Comun;
using System.Collections.Generic;

namespace LibQPA
{
    public class QPAResult
    {
        public Camino<PuntoHistorico> Camino { get; set; }
        public List<QPASubCamino> SubCaminos { get; set; }

        public override string ToString()
        {
            return Camino?.Description ?? "";
        }
    }
}
