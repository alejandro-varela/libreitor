using Recorridos;
using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;

namespace PruebaLecturaDeRecorridos
{
    partial class Program
    {
        //static readonly ConnectionMultiplexer _muxer = ConnectionMultiplexer.Connect("localhost:6379");
        const string GEO_PUNTAS_LINEA = "geo_puntas_de_linea"; // poner una guid y una fecha para ser borrado mas tarde...
        const string GEOHASH = "geoda";

        const int GRANULARIDAD = 300;

        static void Main(string[] args)
        {
            //var tete = "AAABBBCCCAAABBBCCC"
            //    .Simplificar((c1, c2) => c1 == c2)
            //    .Stringificar("___")
            //;

            var start = Environment.TickCount;

            // Leo una colección de recorridos a partir de las líneas dadas (contienen linea y banderas), puede filtrarse
            var recorridosRBus = LeerRecorridosPorArchivos("../../../REC203/", new int[] { 159, 163 }, DateTime.Now);

            var puntasDeLinea = PuntasDeLinea
                .Get    (recorridosRBus)
                .ToList()
            ;

            // Creo una lista PLANA de puntos con lina y bandera
            var puntosLinBan = recorridosRBus.SelectMany(
                collectionSelector: reco => reco.Puntos.HacerGranular(15),
                resultSelector: (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
            );

            // Averiguo los TOPES para el cálculo de este mapa
            var topes2D = Topes2D.CreateFromPuntos(puntosLinBan.Select(plinban => (Punto)plinban));

            Console.Clear();
            //DibujarPuntosLinBan(puntosLinBan, topes2D);
            DibujarPuntos(puntosLinBan.Select(plb=> (Punto)plb), topes2D, 500, '.', ConsoleColor.DarkGray);
            DibujarPuntos(puntasDeLinea, topes2D, 500, 'X', ConsoleColor.Blue);


            // Chusmeo algunas cosas aca para ver que los puntos vienen como quiero
            var cantidadTotalDePuntos   = puntosLinBan.Count();
            var paraMirarMientrasDepuro = puntosLinBan.ToList();

            // Ahora tenemos que crear una colección de casilleros por recorrido...
            // Si, una colección de nombres que esté relacionada con casilleros "virtuales" y los puntos dados...
            // Se podria generar una lista nueva con los puntos y sus casilleros correspondientes...
            // Asi cada vez que se haga referencia a un casillero se puede relacionar con los puntos que se encuentren en él
            // La información debe transformarse, pero debe seguir un hilo no destructivo, para poder fluir en su génesis

            //Dictionary<Casillero, List<PuntoRecorridoLinBan>> dicPuntosLinBanXCasillero = new();
            List<(int, int, string)> patrones = new();
            List<(int, int, Regex)> regexes = new();

            foreach (var recorridoRBusX in recorridosRBus)
            {
                var casillerosParaEsteRecorrido = new List<Casillero>();

                foreach (var puntoRecorridoX in recorridoRBusX.Puntos)
                {
                    var casillero = Casillero.Create(topes2D, puntoRecorridoX, GRANULARIDAD);

                    //// meto info en un diccionario, a lo mejor es util en el futuro...
                    //var key = casillero;
                    //if (!dicPuntosLinBanXCasillero.ContainsKey(key))
                    //{
                    //    dicPuntosLinBanXCasillero.Add(key, new List<PuntoRecorridoLinBan>());
                    //}
                    //var puntoAGuardar = new PuntoRecorridoLinBan(puntoRecorridoX, recorridoRBusX.Linea, recorridoRBusX.Bandera);
                    //dicPuntosLinBanXCasillero[key].Add(puntoAGuardar);

                    // creo la lista de casilleros
                    casillerosParaEsteRecorrido.Add(casillero);
                }

                var pattern = casillerosParaEsteRecorrido
                    //.Simplificar((c1, c2) =>
                    //    c1.IndexHorizontal == c2.IndexHorizontal &&
                    //    c1.IndexVertical == c2.IndexVertical
                    //)
                    .Select(s => $"({s})*")
                    .Stringificar()
                ;

                patrones.Add((recorridoRBusX.Linea, recorridoRBusX.Bandera, pattern));
                regexes.Add((recorridoRBusX.Linea, recorridoRBusX.Bandera, new Regex(pattern, RegexOptions.Compiled)));

                //Console.WriteLine($"{recorridoRBusX.Linea} {recorridoRBusX.Bandera} :: {pattern}");
            }

            //Console.WriteLine($"{Environment.TickCount - start} milis");

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // TODO: estudiar 2495 en 2 de junio (no tiene nada que ver, es una ficha de rosario)
            // TODO: estudiar la firma de 3943 en 2 de junio
            // 4102 2 jun galpon
            // 4071 2 jun (pico 159) tiene puntos tanto en 159 como en 163 (pero en 163 pueden ser puntas línea, ademas son menos)
            // 4314 2 jun (pico 163) 

            var desde = new DateTime(2021, 06, 02);
            var hasta = desde.AddDays(1);
            var historia = Historia.GetFromCSV(4314, desde, hasta, puntasDeLinea, 150, new HistoriaGetFromCSVConfig { InvertLat = true, InvertLng = true });

            foreach (var subHistoriaX in historia.SubHistorias)
            {
                DibujarPuntos(puntosLinBan.Select(plb => (Punto)plb), topes2D, 500, '.', ConsoleColor.DarkGray);
                DibujarPuntos(subHistoriaX.Select(ph => ph.Punto), topes2D, 500, '#', ConsoleColor.Gray);
                DibujarPuntos(puntasDeLinea, topes2D, 500, 'X', ConsoleColor.Blue);
                Console.ReadKey();
                Console.Clear();
            }

            var firma = historia.Puntos
                .Select(ph => ph.Punto)
                .Select(p => Casillero.Create(topes2D, p, GRANULARIDAD))
                .Simplificar((c1, c2) => c1.IndexHorizontal == c2.IndexHorizontal && c1.IndexVertical == c2.IndexVertical)
                .Stringificar()
            ;

            var flen = firma.Length;

            int foo2 = 0;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            foreach ((int, int, string) patronX in patrones)
            //foreach ((int, int, Regex) patronX in regexes)
            {
                int linea = patronX.Item1;
                int bandera = patronX.Item2;
                string pattern = patronX.Item3;
                //Regex r = patronX.Item3;

                Regex r = new(pattern);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{linea} {bandera}");

                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (Match matchX in r.Matches(firma))
                {
                    if (matchX.Value.Trim().Length == 0)
                    {
                        continue;
                    }

                    if (matchX.Value.Length == 11)
                        continue;

                    Console.WriteLine($"{matchX.Index} {matchX.Value}");
                }
            }

            int foo3 = 0;
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////
            /// CREACION DE LA REGEX
            var sb = new StringBuilder();
            foreach (var plb in puntosLinBan.Where(p => p.Linea == 159 && p.Bandera == 2744))
            {
                var casillero = Casillero.Create(topes2D, plb, GRANULARIDAD);
                //sb.Append('<');
                //sb.Append(plb.Cuenta);
                //sb.Append('>');
                sb.Append('(');
                sb.Append(casillero.FixedToString("0000"));
                sb.Append(')');
                sb.Append('*');
            }

            string Pattern_0159_2744 = sb.ToString();
            Console.WriteLine(Pattern_0159_2744);
            Console.WriteLine($"La regex tiene {Pattern_0159_2744.Length} caracteres");

            //////////////////////////////////////////////////////////////////////
            /// COMPARACION CON UN PATRON REAL
            var realreal = "h0647v0718h0646v0718h0645v0718PAPASAVAh0645v0718h0644v0718h0643v0718";
            var regex = new Regex(Pattern_0159_2744);
            foreach (Match match in regex.Matches(realreal))
            {
                Console.WriteLine($"{match.Index} {match.Value}");
            }

            //////////////////////////////////////////////////////////////////////
            /// TODO: JUSTIFICAR LOS MATCHES GRAFICAMENTE

            ///////////////////////////////////////////////////////////////////////
            /// DIBUJO DEL MAPA
            /// 

            //Console.WriteLine($"{Environment.TickCount - start} milis");
            //Console.ReadLine();

            //Console.Clear();

            //foreach (var xxx in puntosLinBan.Where(p => p.Linea == 159))
            //{
            //    char output = xxx.Linea == 159 ? 'x' : 's';

            //    Console.ForegroundColor = (ConsoleColor)((xxx.Bandera % 14) + 1);

            //    if (xxx.Linea == 163)
            //    {
            //        //Console.ForegroundColor = ConsoleColor.Blue;
            //    }
            //    else if (xxx.Linea == 159)
            //    {
            //        //
            //    }

            //    var casillero = Casillero.Create(topes2D, xxx, 500);
            //    Console.CursorLeft = casillero.IndexHorizontal;
            //    Console.CursorTop = 61 - casillero.IndexVertical;
            //    Console.Write(output);
            //    //Console.WriteLine(casillero);
            //}

            //int foo = 0;

            // p21830981:3291839 = linea:24 recorrido:200 cuenta 123 subindex=0 | bla bla bla...

            /*

            var start = Environment.TickCount;
            var recorridoDeArchivo = RecorridosParser
                .ReadFile("../../../REC203/PILAR159/r01592744.txt")
                .ToList()
            ;

            Console.WriteLine($"Tardó {Environment.TickCount - start} millis en leer el archivo");

            PuntoRecorrido puntoAnterior = null;

            var redis = _muxer.GetDatabase();

            bool deleted = redis.KeyDelete(GEOHASH);

            var sumatoriaDistancia = 0.0;
            var contadorRegistros = 0;
            var contadorNombre = 0;

            var recorridoSuavizado = recorridoDeArchivo
                .HacerGranular(1)
                .AchicarPorAcumulacionMetros(10)
            ;

            foreach (var puntoActual in recorridoSuavizado)
            {
                var dist = 0.0;
                
                if (puntoAnterior != null)
                {
                    dist = Haversine.GetDist(puntoAnterior.Lat, puntoAnterior.Lng, puntoActual.Lat, puntoActual.Lng);
                    sumatoriaDistancia += dist;
                }

                AgregarPuntoAlGeoHash(redis, GEOHASH, puntoActual, $"P{contadorNombre:000000}");
                contadorNombre += 1;

                //Console.WriteLine($"{puntoActual.Cuenta,-6} {puntoActual.Lat,-20} {puntoActual.Lng,-20} Dist mts anterior {dist}");
                //Console.WriteLine($"{puntoActual} Dist.mts.anterior {dist:##00.0000}");

                puntoAnterior = puntoActual;
                contadorRegistros += 1;
            }

            var starten = Environment.TickCount;
            var punten = recorridoSuavizado.ToList()[100];
            for (int i = 0; i < 1440; i++)
            {
                var resultadosCercania =
                    DameNombresCercanosA(redis, GEOHASH, punten, radioEnMetros: 100, limiteCantResultados: 5)
                        .ToList();

                //foreach (var resulX in resultadosCercania)
                //{
                //    Console.WriteLine(resulX);
                //}
            }
            var enden = Environment.TickCount;
            Console.WriteLine($"lalala {enden - starten}");

            Console.WriteLine($"Tardó {Environment.TickCount - start} millis");
            Console.WriteLine($"Distancia total: {sumatoriaDistancia}");
            Console.WriteLine($"Cantidad de registros: {contadorRegistros}");
            */
        }

        private static void DibujarPuntos(IEnumerable<Punto> puntos, Topes2D topes2D, int granularidad, char pen, ConsoleColor color)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            HashSet<string> nombresCasilleros = new();

            foreach (Punto px in puntos)
            {
                var casillero = Casillero.Create(topes2D, px, 500);
                var casilleroKey = casillero.ToString();
                if (nombresCasilleros.Contains(casilleroKey))
                {
                    continue;
                }

                Console.CursorLeft = casillero.IndexHorizontal;
                Console.CursorTop = 65 - casillero.IndexVertical;
                Console.Write(pen);

                nombresCasilleros.Add(casilleroKey);
            }
            Console.ForegroundColor = old;
        }

        private static void DibujarPuntosLinBan(IEnumerable<PuntoRecorridoLinBan> puntosLinBan, Topes2D topes2D)
        {
            foreach (var xxx in puntosLinBan.Where(p => true /*p.Linea == 159 || p.Linea == 163*/))
            {
                char output = xxx.Linea == 159 ? '.' : '_';

                //Console.ForegroundColor = (ConsoleColor)((xxx.Bandera % 14) + 1);
                Console.ForegroundColor = ConsoleColor.DarkGray;

                if (xxx.Linea == 163)
                {
                    //Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (xxx.Linea == 159)
                {
                    //
                }

                var casillero = Casillero.Create(topes2D, xxx, 500);
                Console.CursorLeft = casillero.IndexHorizontal;
                Console.CursorTop = 61 - casillero.IndexVertical;
                Console.Write(output);
                //Console.WriteLine(casillero);
            }
        }

        static DateTime GetVerFecha(string fileName)
        {
            // 000000 000011 1111 11 11 22 22 22
            // 012345 678901 2345 67 89 01 23 45
            // verrec 000034 2015 12 24 00 00 00

            return new DateTime(
                year: int.Parse(fileName.Substring(12, 4)),
                month: int.Parse(fileName.Substring(16, 2)),
                day: int.Parse(fileName.Substring(18, 2)),
                hour: int.Parse(fileName.Substring(20, 2)),
                minute: int.Parse(fileName.Substring(22, 2)),
                second: int.Parse(fileName.Substring(24, 2))
            );
        }

        static IEnumerable<RecorridoLinBan> LeerRecorridosLinBanFromZip(string pathArchivoRec)
        {
            using FileStream zipStream = File.OpenRead(pathArchivoRec);
            using ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in zipArchive.Entries)
            {
                if (entry.Name.StartsWith("r"))
                {
                    // 0123456789012
                    // rLLLLBBBB.txt
                    int linea = int.Parse(entry.Name.Substring(1, 4));
                    int bandera = int.Parse(entry.Name.Substring(5, 4));

                    using Stream entryStream = entry.Open();
                    var puntosRecorrido = RecorridosParser.ReadFile(entryStream);

                    yield return new RecorridoLinBan
                    {
                        Linea = linea,
                        Bandera = bandera,
                        Puntos = puntosRecorrido,
                    };
                }
            }
        }

        public static IEnumerable<RecorridoLinBan> LeerRecorridosPorArchivos(string dir, int[] codLineas, DateTime fechaInicioCalculo)
        {
            // los archivos estan guardados con el formato verrec
            // nombre vvvvvv yyyy MM dd hh mm ss
            // verrec 000034 2015 12 24 00 00 00
            // primero listo los directorios que tengan que ver con las líneas en cuestion...

            var dirsLineas = codLineas.Select(codLinea => Path.Combine(dir, codLinea.ToString("0000")));

            foreach (var dirLinea in dirsLineas)
            {
                var pathVersionRecorridos = Directory
                    .GetFiles(dirLinea)
                    .Where(path => Path.GetFileName(path).StartsWith("verrec"))
                    .Where(path => path.EndsWith(".zip"))
                    .Where(path => GetVerFecha(Path.GetFileName(path)) <= fechaInicioCalculo)
                    .OrderByDescending(path => path)
                    .FirstOrDefault()
                ;

                foreach (var recorridoLinBan in LeerRecorridosLinBanFromZip(pathVersionRecorridos))
                {
                    yield return recorridoLinBan;
                }
            }
        }

        public static void AgregarPuntoAlGeoHash(IDatabase redis, string nombreGeoHash, Punto punto, string nombrePunto)
        {
            redis.GeoAdd(nombreGeoHash, punto.Lng, punto.Lat, nombrePunto);
        }

        public static IEnumerable<string> DameNombresCercanosA(IDatabase redis, string nombreGeoHash, Punto punto, int radioEnMetros, int limiteCantResultados = -1)
        {
            var result = redis.GeoRadius(
                nombreGeoHash, 
                punto.Lng, 
                punto.Lat,
                radioEnMetros, 
                GeoUnit.Meters, 
                limiteCantResultados, 
                Order.Ascending, 
                GeoRadiusOptions.Default
            );

            foreach (var rx in result)
            {
                yield return rx.Member;
            }
        }
    }
}
