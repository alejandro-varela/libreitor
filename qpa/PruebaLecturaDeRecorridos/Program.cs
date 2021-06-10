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
    // tener en cuenta los cambios de recorrido por fechas

    /*
     
    consulta: traigo los puntos
	tengo que generar la "cadena de nombres" esto es, convertir puntos geográficos a una cadena de nombres
		>>> necesito generar esos nombres (para cada punto de linea-bandera-recorrido crear un index en un diccionario)
			-primero pongo TODOS los puntos de recorrido en una lista
			-el index de esa lista será de 0 a cantidadPuntos-1
			-entonces ese index será la "clave" de cada punto, rellenando con 0 para que todas tengan el mismo tamaño
				p.linea=159  p.ban=256  p.cuenta=233  p.esPunta=true   p.lat=-32.4343  p.lng=-60.4213
				p.linea=163  p.ban=156  p.cuenta=345  p.esPunta=false  p.lat=-32.6624  .
 				p.linea=121  p.ban=999  p.cuenta=123  .                .               .
				p.linea=110  p.ban=777  .             .                .               .
				p.linea=112  .          .             .                .               .
			-OK... YA TENGO EL NOMBRE... PARA CADA PUNTO DE TODOS LOS RECORRIDOS REALES...
			-ahora tengo que ponerlos en un repositorio por el cual yo pueda:
				
		>>> para cada recorrido, necesito generar un PATRON de REGEX (ya tengo los nombres y los puntos asi que será fácil)
			con la forma "(nombre1)*(nombre2)*(nombre3)*(nombre4)*(nombre5)*...(nombreN)*"
			esto es el QUID de la cuestión

		>>> necesito una forma eficiente de vincular puntos con esos nombres
			esto es, PEDIR los nombres de los puntos mas cercanos a LAT LNG dada

			tengo estas dos opciones: 
				redis		(https://www.nuget.org/packages/StackExchange.Redis/2.2.4)
				geohash-dotnet	(https://www.nuget.org/packages/geohash-dotnet/)
		
		>>> entonces, para cada punto en la consulta obtendré la siguiente cadena, 
		    en donde se ignorarán los puntos que no correspondan remotamente a un recorrido real:

			Esta cadena puede ser larga, esto es, tener hasta 1440 items...
			X0024 X0024 X0025 X0025 X0026 X0045 X0043 X0029 X0029 X0029 X0030 X0024 X0031 ... X0012

		>>> averiguar los MATCH
     
     */

    partial class Program
    {
        //static readonly ConnectionMultiplexer _muxer = ConnectionMultiplexer.Connect("localhost:6379");
        //const string GEOHASH = "geoda";

        static void Main(string[] args)
        {
            //var tete = "AAABBBCCCAAABBBCCC"
            //    .Simplificar((c1, c2) => c1 == c2)
            //    .Stringificar("___")
            //;

            var start = Environment.TickCount;

            // Leo una colección de recorridos a partir de las líneas dadas (contienen linea y banderas), puede filtrarse
            var recorridosRBus = LeerRecorridosPorArchivos("../../../REC203/", new int[] { 159, 163 }, DateTime.Now);

            // Creo una lista PLANA de puntos con lina y bandera
            var puntosLinBan    = recorridosRBus.SelectMany(
                collectionSelector  : reco => reco.Puntos.HacerGranular(15),
                resultSelector      : (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
            );

            // Averiguo los TOPES para el cálculo de este mapa
            var topes2D = Topes2D.CreateFromPuntos(puntosLinBan.Select(plinban => (Punto)plinban));

            // Chusmeo algunas cosas aca para ver que los puntos vienen como quiero
            var cantidadTotalDePuntos   = puntosLinBan.Count ();
            var paraMirarMientrasDepuro = puntosLinBan.ToList();

            // Ahora tenemos que crear una colección de casilleros por recorrido...
            // Si, una colección de nombres que esté relacionada con casilleros "virtuales" y los puntos dados...
            // Se podria generar una lista nueva con los puntos y sus casilleros correspondientes...
            // Asi cada vez que se haga referencia a un casillero se puede relacionar con los puntos que se encuentren en él
            // La información debe transformarse, pero debe seguir un hilo no destructivo, para poder fluir en su génesis

            Dictionary<Casillero, List<PuntoRecorridoLinBan>> dicPuntosLinBanXCasillero = new();
            List<(int, int, string)> patrones = new();
            List<(int, int, Regex)>  regexes = new();

            foreach (var recorridoRBusX in recorridosRBus)
            {
                var casillerosParaEsteRecorrido = new List<Casillero>();

                foreach (var puntoRecorridoX in recorridoRBusX.Puntos)
                {
                    var casillero = Casillero.Create(topes2D, puntoRecorridoX, 30);

                    // meto info en un diccionario, a lo mejor es util en el futuro...
                    var key = casillero;
                    if (!dicPuntosLinBanXCasillero.ContainsKey(key))
                    {
                        dicPuntosLinBanXCasillero.Add(key, new List<PuntoRecorridoLinBan>());
                    }
                    var puntoAGuardar = new PuntoRecorridoLinBan(puntoRecorridoX, recorridoRBusX.Linea, recorridoRBusX.Bandera);
                    dicPuntosLinBanXCasillero[key].Add(puntoAGuardar);

                    // creo la lista de casilleros
                    casillerosParaEsteRecorrido.Add(casillero);    
                }

                var casSinRepe = casillerosParaEsteRecorrido
                    .Simplificar((c1, c2) => 
                        c1.IndexHorizontal == c2.IndexHorizontal && 
                        c1.IndexVertical == c2.IndexVertical
                    )
                ;

                var pattern = casSinRepe
                    //.Select(c => c.ToString())
                    //.Select(c => c.FixedToString("0000"))
                    //.Select(c => string.Format("{0:0000}h{1:0000}v", c.IndexHorizontal, c.IndexVertical))
                    .Select(s => $"({s})*")
                    .Stringificar()
                ;

                patrones.Add((recorridoRBusX.Linea, recorridoRBusX.Bandera, pattern));
                regexes.Add((recorridoRBusX.Linea, recorridoRBusX.Bandera, new Regex(pattern, RegexOptions.Compiled)));

                //Console.WriteLine($"{recorridoRBusX.Linea} {recorridoRBusX.Bandera} :: {pattern}");
            }

            Console.WriteLine($"{Environment.TickCount - start} milis");

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var desde = new DateTime(2021, 06, 02);
            var hasta = desde.AddDays(1);
            var pepino = Historia.GetFromCSV(3646, desde, hasta, new HistoriaGetFromCSVConfig { InvertLat = true, InvertLng = true });

            var firma = pepino.PuntosHistoricos
                .Select(ph => ph.Punto)
                .Select(p => Casillero.Create(topes2D, p, 30))
                .Simplificar((c1, c2) => c1.IndexHorizontal == c2.IndexHorizontal && c1.IndexVertical == c2.IndexVertical)
                .Stringificar()
            ;

            int foofoo = 0;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            int startfaafaa = Environment.TickCount;
            foreach ((int, int, string) patronX in patrones)
            //foreach ((int, int, Regex) patronX in regexes)
            {
                int linea = patronX.Item1;
                int bandera = patronX.Item2;
                string pattern = patronX.Item3;
                //Regex r = patronX.Item3;

                Regex r = new(pattern);

                Console.WriteLine($"{linea} {bandera}");

                foreach (Match matchX in r.Matches(firma))
                {
                    if (matchX.Value.Trim().Length == 0)
                    {
                        continue;
                    }

                    Console.WriteLine($"{matchX.Index} {matchX.Value}");
                }
            }
            Console.WriteLine($"MILIS: {Environment.TickCount - startfaafaa}");

            int faafaa = 0;
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////
            /// CREACION DE LA REGEX
            var sb = new StringBuilder();
            foreach (var plb in puntosLinBan.Where(p => p.Linea == 159 && p.Bandera == 2744))
            {
                var casillero = Casillero.Create(topes2D, plb, 30);
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

            Console.WriteLine($"{Environment.TickCount - start} milis");
            Console.ReadLine();

            Console.Clear();

            foreach (var xxx in puntosLinBan.Where(p => p.Linea == 159))
            {
                char output = xxx.Linea == 159 ? 'x' : 's';

                Console.ForegroundColor = (ConsoleColor) ((xxx.Bandera % 14) + 1);

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
                Console.CursorTop  = 61-casillero.IndexVertical;
                Console.Write(output);
                //Console.WriteLine(casillero);
            }

            int foo = 0;

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
