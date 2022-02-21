using Comun;
using System;
using System.Collections.Generic;

namespace LibQPA.Testing
{
    public class InformacionHistorica
    {
        public List<PuntoHistorico> PuntosCrudos { get; set; }
        public CreadorPartesHistoricas CreadorPartes { get; set; }

        public IEnumerable<ParteHistorica> GetPartesHistoricas(bool sanitizacionBasica = true)
        {
            var puntos = sanitizacionBasica ? Sanitizar(PuntosCrudos) : PuntosCrudos;

            foreach (var parteHistorica in CreadorPartes.Crear(puntos))
            {
                yield return parteHistorica;
            }
        }

        List<PuntoHistorico> Sanitizar(List<PuntoHistorico> puntosCrudos)
        {
            return puntosCrudos;
        }
    }
}
