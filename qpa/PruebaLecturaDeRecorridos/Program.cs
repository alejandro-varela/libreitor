using Mediciones;
using Recorridos;
using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

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
        static readonly ConnectionMultiplexer _muxer = ConnectionMultiplexer.Connect("localhost:6379");
        const string GEOHASH = "geoda";

        static void Main(string[] args)
        {
            Task.Delay(1000);

            var granularidad = 500;
            var puntosLinBan = LeerRecorridosPorArchivos("../../../REC203/", new int[] { 159, 163 }, DateTime.Now);

            // averiguar topes

            var puntos  = puntosLinBan.Select(plinban => (Punto) plinban);
            var cuantos = puntos.Count(); // hay unos 250 000 puntos para dos líneas solamente :S

            var topes2D = Topes2D.CreateFromPuntos(puntos);
            
            var casillerosHorizontales  = Math.Floor((topes2D.AnchoMaximoMts / granularidad) + 1); // 1981
            var casillerosVerticales    = Math.Floor((topes2D.AlturaMts      / granularidad) + 1); // 1001

            //c0-0
            //c0-999
            //c1230-567
            //c1981-1001

            // como se "que" casillero le toca a un punto arbitrario
            var puntoide  = puntos.FirstOrDefault();

            foreach (var xxx in puntosLinBan)
            {
                if (xxx.Linea == 163)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (xxx.Linea == 159)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                var casillero = DameCasillero(topes2D, xxx, granularidad);
                Console.CursorLeft = casillero.IndexHorizontal;
                Console.CursorTop  = 61-casillero.IndexVertical;
                Console.Write("x");
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

        static Casillero DameCasillero(Topes2D topes2D, Punto puntoide, int granularidad)
        {
            // se puede precalc

            var maxIndexCasilleroHorizontal = Convert.ToInt32( Math.Ceiling(topes2D.AnchoMaximoMts / granularidad));
            var maxIndexCasilleroVertical   = Convert.ToInt32( Math.Ceiling(topes2D.AlturaMts      / granularidad));

            var deltaLng = topes2D.Right - topes2D.Left;
            var deltaLat = topes2D.Top   - topes2D.Bottom;

            // no se puede precalc

            var deltaPuntoideLng = puntoide.Lng - topes2D.Left;
            var deltaPuntoideLat = puntoide.Lat - topes2D.Bottom;

            System.Diagnostics.Debug.Assert(deltaPuntoideLng <= deltaLng);
            System.Diagnostics.Debug.Assert(deltaPuntoideLat <= deltaLat);

            var indexCasilleroHorizontal = Convert.ToInt32((deltaPuntoideLng * maxIndexCasilleroHorizontal) / deltaLng);
            var indexCasilleroVertical   = Convert.ToInt32((deltaPuntoideLat * maxIndexCasilleroVertical  ) / deltaLat);

            System.Diagnostics.Debug.Assert(indexCasilleroVertical   <= maxIndexCasilleroVertical  );
            System.Diagnostics.Debug.Assert(indexCasilleroHorizontal <= maxIndexCasilleroHorizontal);

            System.Diagnostics.Debug.Assert(indexCasilleroVertical   >= 0);
            System.Diagnostics.Debug.Assert(indexCasilleroHorizontal >= 0);

            // deltaLng         = 1000 casilleros
            // deltaPuntoideLng = ?

            // 5000        1000
            //  500          ?= 100 ((500 * 1000) / 5000)

            return new Casillero
            {
                IndexHorizontal = indexCasilleroHorizontal,
                IndexVertical   = indexCasilleroVertical,
            };
        }

        public static IEnumerable<PuntoRecorridoLinBan> LeerRecorridosPorArchivos(string dir, int[] codLineas, DateTime fechaInicioCalculo)
        {
            // los archivos estan guardados con el formato verrec
            // nombre vvvvvv yyyy MM dd hh mm ss
            // verrec 000034 2015 12 24 00 00 00
            // primero listo los directorios que tengan que ver con las líneas en cuestion...

            int conttt = 0;
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

                foreach (var recorrido in LeerRecorridosLinBanFromZip(pathVersionRecorridos))
                {
                    var puntosLinBan = recorrido.Puntos
                        .HacerGranular(30)
                        //.AchicarPorAcumulacionMetros(10)
                        .Select(puntoRecorrido => new PuntoRecorridoLinBan(puntoRecorrido, recorrido.Linea, recorrido.Bandera));

                    foreach (var puntoLinBan in puntosLinBan)
                    {
                        conttt++;
                        //Console.WriteLine(puntoLinBan);
                        yield return puntoLinBan;
                    }
                }
                
                int faa = 0;
            }

            int foo = 0;
        }

        // TODO!! aca tambien estabamos bajando responsabilidades...
        // tenemos que subir las responsabilidades para devolver solo un IEnumerable<PuntoRecorrido>
        private static IEnumerable<RecorridoLinBan> LeerRecorridosLinBanFromZip(string pathArchivoRec)
        {
            using FileStream zipStream  = File.OpenRead(pathArchivoRec);
            using ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in zipArchive.Entries)
            {
                if (entry.Name.StartsWith("r"))
                {
                    // 0123456789012
                    // rLLLLBBBB.txt
                    int linea   = int.Parse(entry.Name.Substring(1, 4));
                    int bandera = int.Parse(entry.Name.Substring(5, 4));
                    
                    using Stream entryStream = entry.Open();
                    var puntosRecorrido = RecorridosParser.ReadFile(entryStream);

                    yield return new RecorridoLinBan
                    {
                        Linea   = linea,
                        Bandera = bandera,
                        Puntos  = puntosRecorrido,
                    };
                }
            }
        }

        static DateTime GetVerFecha(string fileName)
        {
            // 000000 000011 1111 11 11 22 22 22
            // 012345 678901 2345 67 89 01 23 45
            // verrec 000034 2015 12 24 00 00 00

            return new DateTime(
                year  : int.Parse(fileName.Substring(12, 4)),
                month : int.Parse(fileName.Substring(16, 2)),
                day   : int.Parse(fileName.Substring(18, 2)),
                hour  : int.Parse(fileName.Substring(20, 2)),
                minute: int.Parse(fileName.Substring(22, 2)),
                second: int.Parse(fileName.Substring(24, 2))
            );
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

    public static class ExtensionesParaElCaso
    {
        static IEnumerable<PuntoRecorrido> CrearPuntosIntermediosNaif(
            PuntoRecorrido puntoA,
            PuntoRecorrido puntoB,
            double cant
        )
        {
            var latDiff = puntoB.Lat - puntoA.Lat;
            var lngDiff = puntoB.Lng - puntoA.Lng;

            var latPart = latDiff / cant;
            var lngPart = lngDiff / cant;

            // para que no aparezca distancia cero poner (cantPuntosRelleno - 1)
            for (int i = 0; i < cant - 1; i++)
            {
                yield return new PuntoRecorrido
                {
                    Cuenta  = puntoA.Cuenta,
                    Index   = i + 1,
                    Alt     = puntoA.Alt,
                    Lat     = puntoA.Lat + (latPart * (i + 1)),
                    Lng     = puntoA.Lng + (lngPart * (i + 1)),
                };
            }
        }

        public static IEnumerable<PuntoRecorrido> HacerGranular(
            this IEnumerable<PuntoRecorrido> recorrido,
            double maxDistMetros
        )
        {
            PuntoRecorrido puntoAnterior = default;

            foreach (var puntoActual in recorrido)
            {
                if (puntoAnterior == null)
                {
                    // retorno el primer punto
                    yield return puntoActual;
                }
                else
                {
                    // sacar distacia entre anterior y actual
                    var distancia = Haversine.GetDist(
                        puntoAnterior.Lat, puntoAnterior.Lng,
                        puntoActual.Lat, puntoActual.Lng
                    );

                    // sacar "cantPuntosIntermedios" = Math.Ceiling(distancia / maxDistMetros)
                    // 0,0 <-- anterior
                    //    1,1 <-- intermedio
                    //       2,2 <-- intermedio
                    //          3,3 <-- actual
                    var cantPuntosIntermedios = Math.Ceiling(distancia / maxDistMetros);

                    // creamos los puntos intermedios y los retornamos
                    var puntosIntermediosNaif = CrearPuntosIntermediosNaif(
                        puntoAnterior,
                        puntoActual,
                        cantPuntosIntermedios
                    );

                    foreach (var puntoIntermedio in puntosIntermediosNaif)
                    {
                        yield return puntoIntermedio;
                    }

                    // luego yield retornaremos el punto actual
                    yield return puntoActual;
                }

                puntoAnterior = puntoActual;
            }
        }

        public static IEnumerable<T> AchicarPorAcumulacionMetros<T>(
            this IEnumerable<T> recorrido,
            int maxMetros
        ) where T : Punto
        {
            double acumMetros = 0.0;
            T puntoAnterior = default;

            foreach (var puntoActual in recorrido)
            {
                var dist = 0.0;

                if (puntoAnterior == null)
                {
                    yield return puntoActual;
                }
                else
                {
                    dist = Haversine.GetDist(puntoAnterior.Lat, puntoAnterior.Lng, puntoActual.Lat, puntoActual.Lng);

                    if (dist + acumMetros >= maxMetros)
                    {
                        acumMetros = 0.0;
                        yield return puntoAnterior;
                    }

                    acumMetros += dist;
                }

                // guardo el punto
                puntoAnterior = puntoActual;
            }

            // devuelvo el último punto guardado, si acumMetros > 0
            if (acumMetros > 0.0)
            {
                yield return puntoAnterior;
            }
        }
    }
}
