﻿using Recorridos;
using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace PruebaLecturaDeRecorridos
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    // TODO: reforzar los dibujos
    //          dibujador bmp
    //          dibujador ascii art
    //          dibujador de línea temporal (rectangulo con las rayitas)
    // TODO: reforzar las puntas de linea, cortar si es posible, no dar bola a falsos cortes
    // TODO: se pueden probar con distintas distancias a las puntas de linea para ver cual da mejor resultado con las subhistorias...
    ///////////////////////////////////////////////////////////////////////////////////////////
    
    // 4267
    // DCDEGIAIGEDHFHD
    // ??
    //   DEGIA
    //       AIGED
    //           DHF
    //             FHD

    partial class Program
    {
        //static readonly ConnectionMultiplexer _muxer = ConnectionMultiplexer.Connect("localhost:6379");
        //const string GEO_PUNTAS_LINEA = "geo_puntas_de_linea"; // poner una guid y una fecha para ser borrado mas tarde...
        //const string GEOHASH = "geoda";

        const int GRANULARIDAD = 500;

        static void Main(string[] args)
        {
            //var tete = "AAABBBCCCAAABBBCCC"
            //    .Simplificar((c1, c2) => c1 == c2)
            //    .Stringificar("___")
            //;

            var start = Environment.TickCount;

            // Leo una colección de recorridos a partir de las líneas dadas (contienen linea y banderas), puede filtrarse
            var recorridosRBus = LeerRecorridosPorArchivos("../../../REC203/", new int[] { 159, 163, 127 }, DateTime.Now);

            //foreach (var petex in PuntasDeLinea.Get(recorridosRBus))
            //    Console.WriteLine(petex);

            var puntasDeLinea = PuntasDeLinea
                .Get    (recorridosRBus)
                .ToList()
            ;

            // Creo una lista PLANA de puntos con lina y bandera
            var puntosLinBan = recorridosRBus.SelectMany(
                collectionSelector: reco => reco.Puntos.HacerGranular(20), // 15 anda perfecto
                resultSelector: (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
            );

            // Averiguo los TOPES para el cálculo de este mapa
            var topes2D = Topes2D.CreateFromPuntos(puntosLinBan.Select(plinban => (PuntoRecorrido)plinban));


            var RADIO_PUNTAS = 1000;
            //var puntasNombradas = PuntasDeLinea.GetPuntasNombradas(recorridosRBus.Where(reco => reco.Linea == 163), radio: RADIO_PUNTAS);
            var puntasNombradas = PuntasDeLinea.GetPuntasNombradas(recorridosRBus, radio: RADIO_PUNTAS);

            foreach (var recoX in recorridosRBus)
            {
                var caminoFromReco = Camino.CreateFromRecorrido(
                    puntasNombradas,
                    recoX
                );
                var caminoDescr = caminoFromReco.Description;
                Console.WriteLine($"{recoX.Linea:0000} {recoX.Bandera:0000} : {caminoFromReco.Description} : RAW_SR: {caminoFromReco.DescriptionRawSinRuido} RAW: {caminoFromReco.DescriptionRaw} ");
            }

            var desdi = new DateTime(2021, 6, 2);
            var hasti = desdi.AddDays(1);
            var caminoFromHisto = Camino.CreateFromPuntos(
                puntasNombradas,
                //              Historia.GetRaw(3850, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4267, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4334, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4380, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4366, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4323, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4307, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // EN GALPON    Historia.GetRaw(3851, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(3856, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4319, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // SIN DATOS    Historia.GetRaw(4372, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4368, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // MOV GALPON   Historia.GetRaw(4309, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // SIN DATOS    Historia.GetRaw(4338, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // SIN DATOS    Historia.GetRaw(4262, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // EN GALPON    Historia.GetRaw(3847, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4349, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // EN GALPON    Historia.GetRaw(4336, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4267, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4377, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // MASO RARO    Historia.GetRaw(3809, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4361, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4103, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                // SIN DATOS    Historia.GetRaw(4313, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4360, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                //              Historia.GetRaw(4325, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
                Historia.GetRaw(4354, desdi, hasti, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true }).Select(ph => ph.Punto)
            );

            // 4354

            Console.WriteLine(caminoFromHisto.Description);
            Console.WriteLine(caminoFromHisto.DescriptionRaw);
            Console.WriteLine(caminoFromHisto.DescriptionRawSinSimplificar);
            var pepepep = caminoFromHisto.Grupoides;

            int falifruli = 0;


            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //PRUEBAS
            //Pruebas.PruebaPuntosDesechados(recorridosRBus, topes2D);
            //Pruebas.EnQueRecoEstaEstePunto(recorridosRBus, topes2D);
            //foreach (var reco in recorridosRBus)
            //{
            //    var listaRecos = new List<RecorridoLinBan>() { reco };
            //    var nombreArchivo = $"reco_{reco.Linea:0000}_{reco.Bandera:0000}.png";
            //    Pruebas.PintarRecorridos(listaRecos, topes2D, 20, nombreArchivo);
            //}

            //{
            //    var nombreArchivo = $"reco_todos.png";
            //    Pruebas.PintarRecorridos(recorridosRBus, topes2D, 20, nombreArchivo);
            //}

            var desde1 = new DateTime(2021, 06, 02, 0, 0, 0); // new DateTime(2021, 06, 02, 13, 0, 0);
            var hasta1 = desde1.AddDays(1);
            var histor = Historia.GetFromCSV(3850, desde1, hasta1, puntasDeLinea, 400, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true });

            var starti = Environment.TickCount;
            foreach (var recoX in recorridosRBus.Where(reco => reco.Linea == 163))
            {
                new PintorDeRecorrido(topes2D: topes2D, granularidad: 20)
                    .SetColorFondo(Color.FromArgb(255, 50, 50, 50))
                    //.PintarRadios(puntasNombradas.Select(punta => punta.Punto), Color.LimeGreen, size: RADIO_PUNTAS / 10) // punta de línea
                    .PintarPuntos(recoX.Puntos.Select(prec => (Punto)prec), Color.GreenYellow, 11)
                    .PintarPuntos(puntosLinBan.Select(plb => (Punto)plb), Color.FromArgb(90, 90, 90), 3) // la gran máscara de recorridos
                    //.PintarPuntos(puntosLinBan.Where(plb => plb.Linea == 127).Select(plb => (Punto)plb), Color.Lime, size: 1)
                    .PintarPuntos(puntosLinBan.Where(plb => plb.Linea == 159).Select(plb => (Punto)plb), Color.Red, size: 1)
                    .PintarPuntos(puntosLinBan.Where(plb => plb.Linea == 163).Select(plb => (Punto)plb), Color.Cyan, size: 1)
                    .PintarRadiosNombrados(puntasNombradas.Select(puntaNombrada => (puntaNombrada.Punto, puntaNombrada.Nombre)), Color.Pink, size: RADIO_PUNTAS / 10)
                    .PintarPuntos(histor.Puntos.Select(ph => ph.Punto), Color.HotPink, 3)
                    .PintarPuntos(new[] { recoX.PuntoSalida }, Color.Fuchsia, size: 20)
                    .PintarPuntos(new[] { recoX.PuntoLlegada }, Color.Blue, size: 20)
                    .Render()
                    .Save($"reco_{recoX.Linea:0000}_{recoX.Bandera:0000}.png", ImageFormat.Png)
                ;
            }
            Console.WriteLine(Environment.TickCount - starti);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            Console.Clear();
            //DibujarPuntosLinBan(puntosLinBan, topes2D);
            DibujarPuntos(puntosLinBan.Select(plb=> (Punto)plb), topes2D, GRANULARIDAD, '.', ConsoleColor.Green);
            //DibujarPuntos(puntasDeLinea, topes2D, GRANULARIDAD, 'X', ConsoleColor.Blue);


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
            // 4314 2 jun (pico 163) <--
            // 4316 2 jun (pico 163) 
            // 4306 2 jun (pico 163) nada
            // 3751 2 jun (pico 163) nada
            // 4337 2 jun (pico 163) nada
            // 3850 2 jun (pico 163) 

            var desde = new DateTime(2021, 06, 02);
            var hasta = desde.AddDays(1);
            var historia = Historia.GetFromCSV(3850, desde, hasta, puntasDeLinea, 400, new PuntosHistoricosGetFromCSVConfig { InvertLat = true, InvertLng = true });

            foreach (var subHistoriaX in historia.SubHistorias)
            {
                if (subHistoriaX.Count < 3)
                {
                    continue;
                }

                DibujarPuntos(puntosLinBan.Select(plb => (Punto)plb), topes2D, GRANULARIDAD, '.', ConsoleColor.DarkGray);
                DibujarPuntos(subHistoriaX.Select(ph => ph.Punto), topes2D, GRANULARIDAD, '#', ConsoleColor.DarkCyan);
                DibujarPuntos(puntasDeLinea, topes2D, GRANULARIDAD, 'P', ConsoleColor.Red);

                IEnumerable<Punto> pts2777 = recorridosRBus
                    .First(recx => recx.Bandera == 2777 && recx.Linea == 163)
                    .Puntos
                    .Select(prec => (Punto)prec)
                ;

                Console.ReadKey();
                DibujarPuntos(pts2777, topes2D, GRANULARIDAD, '%', ConsoleColor.Yellow);
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
        }

        private static void DibujarPuntos(IEnumerable<Punto> puntos, Topes2D topes2D, int granularidad, char pen, ConsoleColor color)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            HashSet<string> nombresCasilleros = new();

            int n = 0;
            foreach (Punto px in puntos)
            {
                var casillero = Casillero.Create(topes2D, px, granularidad);
                var casilleroKey = casillero.ToString();
                if (nombresCasilleros.Contains(casilleroKey) && pen != '%')
                {
                    continue;
                }

                Console.CursorLeft = casillero.IndexHorizontal;
                Console.CursorTop = 65 - casillero.IndexVertical;
                Console.Write(pen);

                n++;
                if (pen == '%' && n % 5 == 0)
                    System.Threading.Thread.Sleep(1);

                nombresCasilleros.Add(casilleroKey);
            }

            ///////////// EN CASO DE % //////////////
            if (pen == '#')
            {
                var primero = puntos.First();
                var casillerop = Casillero.Create(topes2D, primero, granularidad);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.CursorLeft = casillerop.IndexHorizontal;
                Console.CursorTop = 65 - casillerop.IndexVertical;
                Console.Write('S');

                var ultimo = puntos.Last();
                var casillerou = Casillero.Create(topes2D, ultimo, granularidad);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.CursorLeft = casillerou.IndexHorizontal;
                Console.CursorTop = 65 - casillerou.IndexVertical;
                Console.Write('L');
            }
            ///////////// DIBUJO ULTIMO PUNTO //////////////
            



            Console.ForegroundColor = old;
        }

        //private static void DibujarPuntosLinBan(IEnumerable<PuntoRecorridoLinBan> puntosLinBan, Topes2D topes2D)
        //{
        //    foreach (var xxx in puntosLinBan.Where(p => true /*p.Linea == 159 || p.Linea == 163*/))
        //    {
        //        char output = xxx.Linea == 159 ? '.' : '_';

        //        //Console.ForegroundColor = (ConsoleColor)((xxx.Bandera % 14) + 1);
        //        Console.ForegroundColor = ConsoleColor.DarkGray;

        //        if (xxx.Linea == 163)
        //        {
        //            //Console.ForegroundColor = ConsoleColor.Blue;
        //        }
        //        else if (xxx.Linea == 159)
        //        {
        //            //
        //        }

        //        var casillero = Casillero.Create(topes2D, xxx, 500);
        //        Console.CursorLeft = casillero.IndexHorizontal;
        //        Console.CursorTop = 61 - casillero.IndexVertical;
        //        Console.Write(output);
        //        //Console.WriteLine(casillero);
        //    }
        //}

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

        //static IEnumerable<RecorridoLinBan> LeerRecorridosLinBanFromZip(string pathArchivoRec)
        static List<RecorridoLinBan> LeerRecorridosLinBanFromZip(string pathArchivoRec)
        {
            var ret = new List<RecorridoLinBan>();

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

                    var recoLinBan = new RecorridoLinBan
                    {
                        Linea   = linea,
                        Bandera = bandera,
                        Puntos  = puntosRecorrido,
                    };

                    //yield return recoLinBan;
                    ret.Add(recoLinBan);
                }
            }

            return ret;
        }

        //public static IEnumerable<RecorridoLinBan> LeerRecorridosPorArchivos(string dir, int[] codLineas, DateTime fechaInicioCalculo)
        public static List<RecorridoLinBan> LeerRecorridosPorArchivos(string dir, int[] codLineas, DateTime fechaInicioCalculo)
        {
            // los archivos estan guardados con el formato verrec
            // nombre vvvvvv yyyy MM dd hh mm ss
            // verrec 000034 2015 12 24 00 00 00
            // primero listo los directorios que tengan que ver con las líneas en cuestion...

            var dirsLineas = codLineas.Select(codLinea => Path.Combine(dir, codLinea.ToString("0000")));
            var ret = new List<RecorridoLinBan>();

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
                    //yield return recorridoLinBan;
                    ret.Add(recorridoLinBan);
                }
            }

            return ret;
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
