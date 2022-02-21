using Comun;
using System.Collections.Generic;

namespace LibQPA.Testing
{
    public abstract class CreadorPartesHistoricas
    {
        public abstract IEnumerable<ParteHistorica> Crear(List<PuntoHistorico> puntoHistoricos);
    }
}
