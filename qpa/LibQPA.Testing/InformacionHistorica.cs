using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibQPA.Testing
{
    public class InformacionHistorica
    {
        public int GranularidadMetrosDistancia { get; set; } = 20;
        public bool GranularidadTiempoHabilitada { get; set; } = false;

        public List<PuntoHistorico> PuntosCrudos { get; set; }
        public CreadorPartesHistoricas CreadorPartes { get; set; }

        public IEnumerable<ParteHistorica> GetPartesHistoricas(bool habilitarSanitizacionBasica = true)
        {
            var puntos = habilitarSanitizacionBasica ? Sanitizar(PuntosCrudos) : PuntosCrudos;

            foreach (var parteHistorica in CreadorPartes.Crear(puntos))
            {
                yield return parteHistorica;
            }
        }

        List<PuntoHistorico> Sanitizar(List<PuntoHistorico> puntosCrudos)
        {
            return puntosCrudos
                .Where          (p => p.Lat != 0 && p.Lng != 0)
                .OrderBy        (p => p.Fecha)
                .HacerGranular  (GranularidadMetrosDistancia, GranularidadTiempoHabilitada)
                .ToList         ()
            ;
        }
    }
}
