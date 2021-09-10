using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 select * from
(
	select	'CCA' as tabla, 
			nro_equipo as equipo,
			nro_ficha as ficha,
			fechaHora as fecha,
			latitud as lat,
			longitud as lng
	from	log_gpsConCalculoAtraso

	union

	select	'SCA' as tabla,
			nro_equipo as equipo,
			nro_ficha as ficha,
			fechaHora as fecha,
			latitud as lat,
			longitud as lng
	from	log_gpsSinCalculoAtraso

	union

	select	'CCX' as tabla,
			nro_equipo as equipo,
			nro_ficha as ficha,
			fechaHora as fecha,
			latitud as lat,
			longitud as lng
	from	log_gpsConCalculoAtrasoYExcesoVelocidad

	union

	select	'SCX' as tabla,
			nro_equipo as equipo,
			nro_ficha as ficha,
			fechaHora as fecha,
			latitud as lat,
			longitud as lng
	from	log_gpsSinCalculoAtrasoYExcesoVelocidad

) as todo

where
	ficha > 0
	and
	fecha >= '2021-09-02T00:00:00'
	and
	fecha <  '2021-09-03T00:00:00'
	and
	equipo >= 3000000
	and
	equipo < 4000000
	and
	lat > 0
	and
	lng > 0
order by ficha, fecha

 */

namespace PruebaDesempate
{
    class Program
    {
        static void Main(string[] args)
        {
            // granularidad de trabajo: 20 mts
            const int GRANULARIDAD = 20;
            const int RADIO_PUNTAS = 800;

            // 2 Junio 2021
            var fechaConsulta = new DateTime(2021, 9, 2, 0, 0, 0);

            // recorridos de '159' (203-PILAR) y '163' (203-MORENO)
            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos("../../../../Datos/ZipRepo/", new int[] { 159, 163 }, fechaConsulta)
                .Select(reco => SanitizarRecorrido(reco, granularidad: GRANULARIDAD))
                .ToList()
            ;

            // puntos aplanados (todos)
            var todosLosPuntosDeLosRecorridos = recorridosRBus.SelectMany(
                (reco) => reco.Puntos,
                (reco, puntoReco) => puntoReco
            );

            // topes
            var topes2d = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

            // puntas de línea
            var puntas = PuntasDeLinea.GetPuntasNombradas(recorridosRBus, RADIO_PUNTAS);

            // caminos de los recos
            var recoPatterns = new Dictionary<string, List<KeyValuePair<int,int>>>();
            foreach (var recox in recorridosRBus)
            {
                //var camino = Camino<PuntoRecorrido>.CreateFromRecorrido(puntas, recox);
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntas, recox.Puntos);

                if (! recoPatterns.ContainsKey(camino.Description))
                {
                    recoPatterns.Add(camino.Description, new List<KeyValuePair<int, int>>());
                }

                recoPatterns[camino.Description].Add(new KeyValuePair<int, int>(recox.Linea, recox.Bandera));

                Console.WriteLine($"{recox.Linea,-3} {recox.Bandera, -4} {camino.Description}");
            }

            // historia real
            var puntosHistoricos = Historia.GetRaw(
                3330, //4380, //4334 h!!!, // 4377, // 3850
                fechaConsulta, 
                fechaConsulta.AddDays(1), 
                new PuntosHistoricosGetFromCSVConfig
                {
                    ExcluirCeros = true,
                    InvertLat = true,
                    InvertLng = true,
                }
            );

            // camino histórico
            var caminoHistorico = Camino<PuntoHistorico>.CreateFromPuntos(puntas, puntosHistoricos);
            Console.WriteLine(caminoHistorico.Description);
            caminoHistorico.Grupoides[0].GetDescansos();
            // ¿reconocer?
            // Reconocer(recoPatterns.Keys.ToList(), caminoHistorico.Description);

            var reconocimiento = ReconocedorDeCamino.Reconocer(caminoHistorico, recoPatterns.Keys.ToList());
            var unidadesDeRecon = Reconocer2(recoPatterns.Keys.ToList(), caminoHistorico.Description);

            // ok, ahora que ya se tienen los patrones, debo ver a que recorrido pertenece cada patron...
            // algunos patrones tienen múltiples recorridos asociados por lo cual debe haber un "desempate"
            // esto se puede hacer por pertenencia de esos puntos a la pizza...
            
            Console.WriteLine("Análisis real");
            
            //foreach (var unidadDeReconX in unidadesDeRecon)
            foreach (var unidadDeReconX in reconocimiento.Unidades)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (unidadDeReconX is ReconocimientoUnidadError)
                {

                }
                else if (unidadDeReconX is ReconocimientoUnidadMatch)
                {
                    var uni = unidadDeReconX as ReconocimientoUnidadMatch;

                    Console.WriteLine($"\t -> {uni.Pattern} (Index: {uni.IndexNombres} Largo: {uni.Pattern.Length})");
                    Console.ForegroundColor = recoPatterns[uni.Pattern].Count == 1 ?
                        ConsoleColor.Green : ConsoleColor.DarkGray;

                    // SALE
                    var grupoideSale = caminoHistorico.Grupoides[uni.IndexInicialGrupoide];
                    var horaSalida = (grupoideSale.GetDescansos().Count > 0) ?
                        grupoideSale.GetDescansos()[^1].FinDescanso :
                        grupoideSale.GetPuntoMasCentral().PuntoAsociado.Fecha
                    ;
                    Console.WriteLine($"SALE : {horaSalida}");

                    // Llega
                    var grupoideLlega = caminoHistorico.Grupoides[uni.IndexFinalGrupoide];
                    var horaLlegada = (grupoideLlega.GetDescansos().Count > 0) ?
                        grupoideLlega.GetDescansos()[0].InicioDescanso :
                        grupoideLlega.GetPuntoMasCentral().PuntoAsociado.Fecha
                    ;
                    Console.WriteLine($"LLEGA: {horaLlegada}");

                    // Duración
                    var duracion = horaLlegada - horaSalida;
                    var duracionHoras = Convert.ToInt32(Math.Floor(duracion.TotalSeconds / 3600));
                    var duracionMinutos = Convert.ToInt32(Math.Floor(duracion.TotalMinutes - (duracionHoras * 60)));
                    var duracionSegundos = Convert.ToInt32( Math.Floor(duracion.TotalSeconds - (duracionHoras * 3600) - (duracionMinutos * 60)));

                    Console.WriteLine($"DURAC: {duracionHoras:00}:{duracionMinutos:00}:{duracionSegundos:00} ({duracion.TotalSeconds} segs)");

                    //muestra las banderas del patron reconocido
                    //foreach (var kvp in recoPatterns[uni.Pattern])
                    //{
                    //    Console.WriteLine($"\t\tLínea: {kvp.Key} Bandera:{kvp.Value}");
                    //}

                    //if (recoPatterns[uni.Pattern].Count > 1) // si hay varias banderas en un patrón debo desempatar...
                    {
                        var stats = Desempatar(
                            caminoHistorico,
                            recorridosRBus,
                            recoPatterns[uni.Pattern],
                            uni.IndexNombres,
                            uni.Pattern,
                            GRANULARIDAD,
                            topes2d
                        );

                        //MostrarStats(stats, ConsoleColor.DarkCyan, "Desempatar: ");

                        var ganadores = DameEstadisticaGanadora(stats);
                        MostrarStats(ganadores, ConsoleColor.Cyan, "Ganadora  : ");
                    }
                }
            }

            // TODO: hacer un "reconocedor de galpón"
            // TODO: hacer que se informe el porcentaje reconocido y no reconocido...
            // TODO: si no se empieza con un galpon, se puede ampliar los bordes (unas horas) hasta encontrar un galpón...

            ////MostrarHorasDeLosGrupoides(caminoHistorico);
            //foreach (var gx in caminoHistorico.Grupoides)
            //{
            //    //if (gx.Nombre == "." || gx.Nombre == "?") { continue; }
            //    MostrarTiemposDelGrupoide(gx);
            //}

            int foo = 0;
        }

        //static int GetIndexGrupoide(int offset, string pattern, List<Grupoide<PuntoHistorico>> grupoides)
        //{
        //    int index = 0;
        //    string acum = "";

        //    foreach (var g in grupoides)
        //    {
        //        // esto ignorará los grupoides que tenga nombres que no esten en el patron...
        //        if (!pattern.Contains(g.Nombre, StringComparison.Ordinal))
        //        {
        //            index++;
        //            continue;
        //        }

        //        if (index >= offset)
        //        {
        //            acum += g.Nombre;

        //            if (acum == pattern)
        //            {
        //                break;
        //            }
        //        }

        //        index++;
        //    }

        //    return index;
        //}

        static void MostrarStats(List<KeyValuePair<KeyValuePair<int, int>, int>> ganadores, ConsoleColor consoleColor, string prefix)
        {
            foreach (var stat in ganadores)
            {
                var linea = stat.Key.Key;
                var bandera = stat.Key.Value;
                var puntaje = stat.Value;
                var ccanterior = Console.ForegroundColor;
                Console.ForegroundColor = consoleColor;
                var msg = $"{prefix}Lin={linea} Ban={bandera} : {puntaje}";
                //Console.WriteLine(new string('-', msg.Length));
                Console.WriteLine(msg);
                Console.ForegroundColor = ccanterior;
            }
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

        // TODO: pasar esta función a RecorridoLinBan
        static RecorridoLinBan SanitizarRecorrido(RecorridoLinBan reco, int granularidad)
        {
            return new RecorridoLinBan
            {
                Bandera = reco.Bandera,
                Linea   = reco.Linea,
                Puntos  = reco.Puntos.HacerGranular(granularidad),
            };
        }
       
        // TODO: pasar esto a una librería

        static List<ReconocimientoUnidad> Reconocer2(List<string> patrones, string patronHistorico)
        {
            if (patronHistorico == null)
            {
                //Console.WriteLine("Patron Nulo");
                //Console.WriteLine("FINE");
                return new List<ReconocimientoUnidad>();
            }

            if (patronHistorico == string.Empty)
            {
                //Console.WriteLine("Patron Vacío");
                //Console.WriteLine("FINE");
                return new List<ReconocimientoUnidad>();
            }

            var patronesOrdenados = patrones
                .OrderByDescending(p => p.Length)   // ordeno por tamaño
                .ThenBy(p => p)                     // entonces lexicográficamente
                .ToList()                           // convierto todo en una lista
            ;

            List<ReconocimientoUnidad> ret = new();
            int ptr = 0;

            for (; ; )
            {
                var puntaInicial = patronHistorico[ptr].ToString();
                //Console.Write($"para la punta {puntaInicial} ");

                var patronesPosibles = patronesOrdenados
                    .Where(p => p.StartsWith(puntaInicial))
                    .Distinct()
                    .ToList()
                ;

                if (patronesPosibles.Count == 0)
                {
                    //Console.WriteLine($"No hay patrones para '{puntaInicial}'");
                    ret.Add(new ReconocimientoUnidadError { IndexNombres = ptr, ErrDescription = $"No hay patrones para '{puntaInicial}'" });

                    ptr++;

                    if (ptr >= patronHistorico.Length - 1)
                    {
                        break;
                    }

                    continue;
                }

                // Console.WriteLine("existen los patrones: ");
                // patronesPosibles
                //    .ToList()
                //    .ForEach((pattern) => Console.WriteLine($"\t{pattern}"));

                // de mayor a menor me fijo si encaja...
                string patronElegido = null;
                int patternIndex = 0;
                foreach (var patronPosibleX in patronesPosibles)
                {
                    if (patronHistorico.Substring(ptr).StartsWith(patronPosibleX)) // también se puede hacer patronHistorico[ptr..]
                    {
                        //Console.ForegroundColor = ConsoleColor.Green;
                        //Console.WriteLine($"El patron {patronPosible} es bueno para {patronHistorico} en index:{ptr}");
                        //Console.ForegroundColor = ConsoleColor.Gray;
                        ret.Add(new ReconocimientoUnidadMatch { IndexNombres = ptr, Pattern = patronPosibleX });
                        patronElegido = patronPosibleX;
                        break;
                    }

                    patternIndex++;
                }

                if (patronElegido == null)
                {
                    //Console.WriteLine("ERROR!!!!!!!");
                    ptr++;
                    if (ptr >= patronHistorico.Length - 1)
                    {
                        //Console.WriteLine("FINE");
                        break;
                    }
                    continue;
                }
                else
                {
                    ptr += patronElegido.Length - 1;
                }

                if (ptr >= patronHistorico.Length - 1)
                {
                    //Console.WriteLine("FINE");
                    break;
                }
            }

            return ret;
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

            HashSet<Casillero> casillerosTeoricos = new();

            foreach (var preco in recorridoTeorico.Puntos)
            {
                var casTeorico = Casillero.Create(topes2D, preco, granularidad);
                casillerosTeoricos.Add(casTeorico);
            }

            // averiguo que cantidad de puntos entran en ese recorrido...
            foreach (var puntoX in puntosReales)
            {
                var casReal  = Casillero.Create(topes2D, puntoX, granularidad);
                var presente = casReal.PresenteFlexEn(casillerosTeoricos, granularidad);
                //var presente = casillerosTeoricos.Contains(casReal);

                if (presente)
                {
                    positivos++;
                }
            }

            return positivos;
        }
    }
}
