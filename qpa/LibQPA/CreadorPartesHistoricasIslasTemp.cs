using Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibQPA
{
    public class CreadorPartesHistoricasIslasTemp : CreadorPartesHistoricas
    {
        public int SegundosMismaIsla { get; set; } = 10 * 60;

        bool SonDeLaMismaIsla(PuntoHistorico p1, PuntoHistorico p2)
        {
            var secsDiff = Math.Abs((p2.Fecha - p1.Fecha).TotalSeconds);
            return secsDiff < SegundosMismaIsla;
        }

        public override IEnumerable<ParteHistorica> Crear(List<PuntoHistorico> puntoHistoricos)
        {
            IEnumerable<PuntoHistorico> puntosOrdenados = 
                puntoHistoricos.OrderBy(ph => ph.Fecha);

            IEnumerable<List<PuntoHistorico>> islas = DetectorIslas.GetIslas(
                puntosOrdenados, 
                SonDeLaMismaIsla
            );

            foreach (List<PuntoHistorico> islaX in islas)
            {
                yield return new ParteHistorica { Puntos = islaX };
            }
        }
    }
}
