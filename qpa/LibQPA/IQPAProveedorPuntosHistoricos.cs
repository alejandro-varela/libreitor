using System;
using System.Collections.Generic;
using Comun;

namespace LibQPA
{
    public interface IQPAProveedorPuntosHistoricos<IdType>
    {
        Dictionary<IdType, List<PuntoHistorico>> Get();
    }
}
