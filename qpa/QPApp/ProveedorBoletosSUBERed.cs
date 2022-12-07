using Comun;
using ComunSUBE;
using LibQPA;
using System.Collections.Generic;

namespace QPApp
{
    public class ProveedorBoletosSUBERed : QPAProveedorVentaBoletos<ParEmpresaInterno>
    {
        public override Dictionary<ParEmpresaInterno, List<BoletoComun>> LeerOrigenDeDatos()
        {
            return null;
        }
    }
}
