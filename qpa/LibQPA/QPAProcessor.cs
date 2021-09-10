using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comun;

namespace LibQPA
{
    public class QPAProcessor
    {
        QPAConfiguration _conf = null;

        public QPAProcessor(QPAConfiguration conf)
        {
            _conf = conf;
        }

        public QPAResult Procesar(int [] lineas, DateTime fechaDesde)
        {
            /* Primera parte */

            //// EN CONF
            //// granularidad de trabajo: 20 mts
            //const int GRANULARIDAD = 20;
            //const int RADIO_PUNTAS = 800;

            //// EN CONF
            //// 2 Junio 2021
            //var fechaConsulta = new DateTime(2021, 9, 2, 0, 0, 0);

            //// CAMBIADO POR...
            //// recorridos de '159' (203-PILAR) y '163' (203-MORENO)
            //var recorridosTeoricos = Recorrido.LeerRecorridosPorArchivos("../../../../Datos/ZipRepo/", new int[] { 159, 163 }, fechaConsulta)
            //    .Select(reco => SanitizarRecorrido(reco, granularidad: GRANULARIDAD))
            //    .ToList()
            //;

            var recorridosTeoricos = _conf.ProveedorRecorridosTeoricos.Get(lineas, fechaDesde)
                .Select(reco => SanitizarRecorrido(reco, _conf.GranularidadMts))
                .ToList()
            ;

            // puntos aplanados (todos)
            var todosLosPuntosDeLosRecorridos = recorridosTeoricos.SelectMany(
                (reco) => reco.Puntos,
                (reco, puntoReco) => puntoReco
            );

            // topes
            var topes2d = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

            // puntas de línea
            var puntas = PuntasDeLinea.GetPuntasNombradas(recorridosTeoricos, _conf.RadioPuntasMts);



            return new QPAResult();
        }

        // TODO: pasar esta función a RecorridoLinBan
        // MMMMMMMMMMMMM no se si hay que hacer eso...
        // !!!!!!!!!!!!! revisar que tengo para la granularizacion en RULOS... puede ser mejor...
        static RecorridoLinBan SanitizarRecorrido(RecorridoLinBan reco, int granularidad)
        {
            return new RecorridoLinBan
            {
                Bandera = reco.Bandera,
                Linea = reco.Linea,
                Puntos = reco.Puntos.HacerGranular(granularidad),
            };
        }
    }
}
