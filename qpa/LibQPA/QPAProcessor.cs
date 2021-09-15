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

            // reconocimiento del camino histórico
            var reconocimiento = ReconocedorDeCamino.Reconocer(
                caminoHistorico, 
                recoPatterns.Keys.ToList()
            );

            //////////////////////////////////////////////////////////////////////////////////////////////

            foreach (var unidadDeReconX in reconocimiento.Unidades)
            {
                if (unidadDeReconX is ReconocimientoUnidadError)
                { 
                    //
                }
                else if (unidadDeReconX is ReconocimientoUnidadMatch)
                {
                    var uni = unidadDeReconX as ReconocimientoUnidadMatch;

                    // Hora de salida
                    var grupoideSale = caminoHistorico.Grupoides[uni.IndexInicialGrupoide];
                    var horaSalida = (grupoideSale.GetDescansos().Count > 0) ?
                        grupoideSale.GetDescansos()[^1].FinDescanso :
                        grupoideSale.GetPuntoMasCentral().PuntoAsociado.Fecha
                    ;

                    // Hora de llegada
                    var grupoideLlega = caminoHistorico.Grupoides[uni.IndexFinalGrupoide];
                    var horaLlegada = (grupoideLlega.GetDescansos().Count > 0) ?
                        grupoideLlega.GetDescansos()[0].InicioDescanso :
                        grupoideLlega.GetPuntoMasCentral().PuntoAsociado.Fecha
                    ;

                    // Duración
                    var duracion = horaLlegada - horaSalida;
                    var duracionHoras = Convert.ToInt32(Math.Floor(duracion.TotalSeconds / 3600));
                    var duracionMinutos = Convert.ToInt32(Math.Floor(duracion.TotalMinutes - (duracionHoras * 60)));
                    var duracionSegundos = Convert.ToInt32(Math.Floor(duracion.TotalSeconds - (duracionHoras * 3600) - (duracionMinutos * 60)));

                    if (recoPatterns[uni.Pattern].Count > 1) // si hay varias banderas en un patrón debo desempatar...
                    {
                        var stats = Desempatar(
                            caminoHistorico,
                            recorridosTeoricos,
                            recoPatterns[uni.Pattern],
                            uni.IndexNombres,
                            uni.Pattern,
                            GranularidadMts,
                            topes2D
                        );

                        var ganadores = DameEstadisticaGanadora(stats);
                        //MostrarStats(ganadores, ConsoleColor.Cyan, "Ganadora  : ");
                    }
                    else
                    { 
                        // un solo ganador...
                    }
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////

            return new QPAResult 
            { 
                Camino = caminoHistorico
            };
        }

        static List<KeyValuePair<KeyValuePair<int, int>, int>> Desempatar<TPunto>(Camino<TPunto> caminoHistorico, List<RecorridoLinBan> recorridosRBus, List<KeyValuePair<int, int>> recorridosADesempatar, int index, string pattern, int granularidad, Topes2D topes2D)
            where TPunto : Punto
        {
            int myIndex = -1;
            int myLen = 0;
            int lenRealEnGrupoides = 0;
            int indexRealEnGrupoides = -1;
            bool yaEmpezo = false;
            bool yaTermino = false;

            foreach (var ggg in caminoHistorico.Grupoides)
            {
                if (ggg.Nombre != "." && ggg.Nombre != "?")
                {
                    // Index
                    myIndex += 1;
                    if (myIndex == index)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        yaEmpezo = true;
                        indexRealEnGrupoides = indexRealEnGrupoides + 1; // una sola vez
                    }

                    // Len
                    if (yaEmpezo)
                        myLen += 1;
                }

                if (!yaEmpezo)
                {
                    indexRealEnGrupoides += 1;
                }

                if (yaEmpezo && !yaTermino)
                {
                    lenRealEnGrupoides += 1;
                }

                //muestro mucha info sobre lo reconocido
                //Console.WriteLine($"\t\t\t{ggg.Nombre} {myIndex} {myLen} {yaEmpezo} {lenRealEnGrupoides} {indexRealEnGrupoides}");

                if (myLen == pattern.Length)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    yaTermino = true;
                }
            }

            var grupoidesElegidos = caminoHistorico.Grupoides
                .Skip(indexRealEnGrupoides)
                .Take(lenRealEnGrupoides)
                .ToList()
            ;

            grupoidesElegidos = grupoidesElegidos
                .Skip(1)
                .SkipLast(1)
                .ToList()
            ;

            var puntosAplanados = grupoidesElegidos
                .SelectMany
                (
                    (grupx) => grupx.Nodos,
                    (grupx, nodox) => nodox.PuntoAsociado
                )
                .ToList()
            ;

            // para cada recorrido a desempatar...
            var stats = new List<KeyValuePair<KeyValuePair<int, int>, int>>();
            foreach (var linban in recorridosADesempatar)
            {
                var linea = linban.Key;
                var bandera = linban.Value;
                //Console.WriteLine($"Desempatando: {linea} {bandera}");
                var recorridoTeorico = recorridosRBus
                    .Where(recox => recox.Linea == linea && recox.Bandera == bandera)
                    .First()
                ;

                var cuantos = ComparacionGeometrica(puntosAplanados, recorridoTeorico, granularidad, topes2D);
                stats.Add(new KeyValuePair<KeyValuePair<int, int>, int>(linban, cuantos));
            }

            return stats;
        }

        static int ComparacionGeometrica(
            IEnumerable<Punto> puntosReales,
            RecorridoLinBan recorridoTeorico,
            int granularidad,
            Topes2D topes2D
        )
        {
            int positivos = 0;

            HashSet<Casillero> casillerosTeoricos = new HashSet<Casillero>();

            foreach (var preco in recorridoTeorico.Puntos)
            {
                var casTeorico = Casillero.Create(topes2D, preco, granularidad);
                casillerosTeoricos.Add(casTeorico);
            }

            // averiguo que cantidad de puntos entran en ese recorrido...
            foreach (var puntoX in puntosReales)
            {
                var casReal = Casillero.Create(topes2D, puntoX, granularidad);
                var presente = casReal.PresenteFlexEn(casillerosTeoricos, granularidad);
                //var presente = casillerosTeoricos.Contains(casReal);

                if (presente)
                {
                    positivos++;
                }
            }

            return positivos;
        }

        private static List<KeyValuePair<KeyValuePair<int, int>, int>> DameEstadisticaGanadora(List<KeyValuePair<KeyValuePair<int, int>, int>> stats)
        {
            return stats
                .GroupBy(stat => stat.Value)
                .OrderByDescending(g => g.Key)
                .First()
                .ToList()
            ;
        }
    }
}
