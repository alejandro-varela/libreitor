using System;
using System.Collections.Generic;
using Comun;

namespace LibQPA
{
    public interface IQPAProveedorPuntosHistoricos
    {
        Dictionary<int, List<PuntoHistorico>> Get();
    }
}
