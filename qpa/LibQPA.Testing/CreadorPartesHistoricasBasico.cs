using Comun;
using System.Collections.Generic;

namespace LibQPA.Testing
{
    public class CreadorPartesHistoricasBasico : CreadorPartesHistoricas
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
