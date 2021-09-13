using System;
using System.Collections.Generic;
using Comun;

namespace LibQPA
{
    public interface IQPAProveedorPuntosHistoricos
    {
        IEnumerable<PuntoHistorico> Get(DateTime desde, DateTime hasta);
    }
}
