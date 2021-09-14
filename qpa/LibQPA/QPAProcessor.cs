using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comun;

namespace LibQPA
{
    public class QPAProcessor
    {
        public int GranularidadMts { get; set; } = 20;
        public int RadioPuntasMts  { get; set; } = 800;

        public QPAResult Procesar(
            List<RecorridoLinBan>   recorridosTeoricos,
            List<PuntoHistorico>    puntosHistoricos,
            Topes2D                 topes2D,
            IEnumerable<PuntaLinea> puntasNombradas,
            Dictionary<string, List<KeyValuePair<int, int>>> recoPatterns
        )
        {
            // camino histórico
            var caminoHistorico = Camino<PuntoHistorico>.CreateFromPuntos(puntasNombradas, puntosHistoricos);
            Console.WriteLine(caminoHistorico.DescriptionRawSinRuido);



            return new QPAResult 
            { 
                Camino = caminoHistorico
            };
        }
    }
}
