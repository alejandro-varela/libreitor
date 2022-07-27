using Comun;
using System.Collections.Generic;

namespace LibQPA
{
    public abstract class CreadorPartesHistoricas
    {
        public abstract IEnumerable<ParteHistorica> Crear(List<PuntoHistorico> puntoHistoricos);
    }
}
