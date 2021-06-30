using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaDesempate
{
    class Program
    {
        static void Main(string[] args)
        {
            // 2 Junio 2021
            var fechaConsulta = new DateTime(2021, 6, 2, 0, 0, 0);

            // recorridos de '159' (203-PILAR) y '163' (203-MORENO)
            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos("../../../../Datos/ZipRepo/", new int[] { 159, 163 }, fechaConsulta)
                .Select(reco => SanitizarRecorrido(reco, granularidad: 20))
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
            var puntas = PuntasDeLinea.GetPuntasNombradas(recorridosRBus, 800);

            // caminos de los recos
            var patronesReco = new List<string>();
            foreach (var recox in recorridosRBus)
            {
                var camino = Camino.CreateFromRecorrido(puntas, recox);
                patronesReco.Add(camino.Description);
                Console.WriteLine($"{recox.Linea,-3} {recox.Bandera, -4} {camino.Description}");
            }

            //3850 ok
            //4267 ok
            //4334 ok
            //4380 ok
            //4366 ok
            //4323 ok
            //4307 ok
            //3851 ok en galpón (TODO: hacer un reconocedor de galpón)
            //3856 ok tiene problema en una parte del patrón porque falta la D en un AIGE?EGIA, TODO: ¿se podría adivinar?
            //4319 ok
            //4372 ok (no tiene datos)
            //4368 ok
            //4309 ok en galpón (TODO: hacer un reconocedor de galpón)
            //4338 ok (no tiene datos)
            //4262 ok (no tiene datos)
            //3847 ok en galpón (TODO: hacer un reconocedor de galpón)
            //4349 ok
            //4336 ok en galpón (TODO: hacer un reconocedor de galpón)
            //4267 ok
            //4377 ok
            //3809 reconoció parcial, pero es un desastre...
            //4361 ok
            //4103 ok
            //4313 ok (no tiene datos)
            //4360 ok
            //4325 ok

            // historia real
            var puntosHistoricos = Historia.GetRaw(
                4325,
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
            var caminoHistorico = Camino.CreateFromPuntos(puntas, puntosHistoricos);
            Console.WriteLine(caminoHistorico.Description);

            // ¿reconocer?
            Reconocer(patronesReco, caminoHistorico.Description);
            var unidadesDeRecon = Reconocer2(patronesReco, caminoHistorico.Description);

            // ok, ahora que ya se tienen los patrones, debo ver a que recorrido pertenece cada patron...
            // algunos patrones tienen múltiples recorridos asociados por lo cual debe haber un "desempate"
            // esto se puede hacer por pertenencia de esos puntos a la pizza...
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Análisis real");
            foreach (var unidadDeReconX in unidadesDeRecon)
            {
                if (unidadDeReconX is RecognitionUnitError)
                {

                }
                else if (unidadDeReconX is RecognitionUnitMatch)
                {
                    var uni = unidadDeReconX as RecognitionUnitMatch;
                    Console.WriteLine("\t -> " + uni.Pattern);
                }
            }


            // TODO: hacer un "reconocedor de galpón"
            // TODO: hacer que se informe el porcentaje reconocido y no reconocido...
            // TODO: si no se empieza con un galpon, se puede ampliar los bordes (unas horas) hasta encontrar un galpón...

            int foo = 0;
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
        static void Reconocer(List<string> patrones, string patronHistorico)
        {
            if (patronHistorico == null)
            {
                Console.WriteLine("Patron Nulo");
                Console.WriteLine("FINE");
                return;
            }

            if (patronHistorico == string.Empty)
            {
                Console.WriteLine("Patron Vacío");
                Console.WriteLine("FINE");
                return;
            }

            var patronesOrdenados = patrones
                .OrderByDescending(p => p.Length)   // ordeno por tamaño
                .ThenBy(p => p)                     // entonces lexicográficamente
                .ToList()                           // convierto todo en una lista
            ;

            int ptr = 0;

            for (; ; )
            {
                var puntaInicial = patronHistorico[ptr].ToString();
                Console.Write($"para la punta {puntaInicial} ");

                var patronesPosibles = patronesOrdenados
                    .Where(pattern => pattern.StartsWith(puntaInicial))
                    .Distinct()
                    .ToList()
                ;

                if (patronesPosibles.Count == 0)
                {
                    Console.WriteLine($"No hay patrones para '{puntaInicial}'");
                    ptr++;
                    if (ptr >= patronHistorico.Length - 1)
                    {
                        Console.WriteLine("FINE");
                        break;
                    }
                    continue;
                }

                Console.WriteLine("existen los patrones: ");
                patronesPosibles
                    .ToList()
                    .ForEach((pattern) => Console.WriteLine($"\t{pattern}"));

                // de mayor a menor me fijo si encaja...
                string patronElegido = null;
                foreach (var patronPosible in patronesPosibles)
                {
                    if (patronHistorico.Substring(ptr).StartsWith(patronPosible)) // también se puede hacer patronHistorico[ptr..]
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"El patron {patronPosible} es bueno para {patronHistorico} en index:{ptr}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        patronElegido = patronPosible;
                        break;
                    }
                }

                if (patronElegido == null)
                {
                    Console.WriteLine("ERROR!!!!!!!");
                    ptr++;
                    if (ptr >= patronHistorico.Length - 1)
                    {
                        Console.WriteLine("FINE");
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
                    Console.WriteLine("FINE");
                    break;
                }

                int fafafa = 0;
            }
        }

        static List<RecognitionUnit> Reconocer2(List<string> patrones, string patronHistorico)
        {
            if (patronHistorico == null)
            {
                //Console.WriteLine("Patron Nulo");
                //Console.WriteLine("FINE");
                return new List<RecognitionUnit>();
            }

            if (patronHistorico == string.Empty)
            {
                //Console.WriteLine("Patron Vacío");
                //Console.WriteLine("FINE");
                return new List<RecognitionUnit>();
            }

            var patronesOrdenados = patrones
                .OrderByDescending(p => p.Length)   // ordeno por tamaño
                .ThenBy(p => p)                     // entonces lexicográficamente
                .ToList()                           // convierto todo en una lista
            ;

            List<RecognitionUnit> ret = new();
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
                    ret.Add(new RecognitionUnitError { 
                        Index = ptr, 
                        ErrDescription = $"No hay patrones para '{puntaInicial}'" 
                    });

                    ptr++;

                    if (ptr >= patronHistorico.Length - 1)
                    {
                        //Console.WriteLine("FINE");
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
                        ret.Add(new RecognitionUnitMatch { Index = ptr, Pattern = patronPosibleX });
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
    }

    public abstract class RecognitionUnit
    {
        public int Index { get; set; }
    }

    public class RecognitionUnitMatch : RecognitionUnit
    { 
        public string Pattern { get; set; }
    }

    public class RecognitionUnitError : RecognitionUnit
    {
        public string ErrDescription { get; set; }
    }
}
