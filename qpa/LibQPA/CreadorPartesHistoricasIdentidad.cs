using Comun;
using System.Collections.Generic;

namespace LibQPA
{
    public class CreadorPartesHistoricasIdentidad : CreadorPartesHistoricas
    {
        public override IEnumerable<ParteHistorica> Crear(List<PuntoHistorico> puntosHistoricos)
        {
            yield return new ParteHistorica
            {
                Puntos = puntosHistoricos
            };
        }
    }
}
