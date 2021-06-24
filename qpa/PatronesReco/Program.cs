using System;
using System.Collections.Generic;
using System.Linq;

namespace PatronesReco
{
    class Program
    {
        // encaja todo entre Start y End (en este caso " y ")
        // Start([\s\S]*)End


        // PARA SACAR LAS PUNTAS DE LOS RECORRIDOS SE PUEDE USAR "EL ESTOY YENDO A ..."
        // ESTO ES:
        //
        //      A ---- B     F
        //       \   /  \   /
        //        \ /    \ /
        //         C      D-----Z
        //
        //   si en el medio de A y B hay dos puntos, puedo encontrar los estoy "yendo" que seria si voy a vuelvo a tal o cual punta
        //   o sea, al encontrar una direccionalidad entre los puntos pudo determinar que se fue a tal o cual punta sin llegar a tocarla...
        //   si la tocamos mejor...

        static readonly string[] _patronesRaw1 = new [] {
            "AB",
            "ABC",
            "BA",
            "CBA",
            "AD",
            "ADF",
            "DA",
            "FDA",
        };

        static readonly string[] _patronesRaw2 = new[] {
            "DA",
            "AA",
            "DE",
            "ED",
            "AIA",
            "DHF",
            "FHD",
            "DEGIA",
            "DEGIA",
            "DEGIA",
            "AIGED",
            "AIGED",
            "AIGED",
            "DEGIA",
            "FHEGIA",
            "AIGEHF",
            "DEGIA",
            "DEG",
            "GED",
            "DH",
            "HD",
            "IGED",
            "DEGI"
        };

        //static List<string> _patronHisto1 = new("ABABCBA".Select(c => c.ToString()));
        static string _patronHisto1 = "ABABCBABCBADFDADA";
        //                             AB  .  .  .  .  . <-- recorrido de A a B
        //                              BA .  .  .  .  . <-- recorrido de B a A
        //                               ABC  .  .  .  . <-- recorrido de A que pasa por B hasta llegar a C
        //                                 CBA.  .  .  . <-- recorrido de C que pasa por B hasta llegar a A
        //                                   ABC .  .  . <-- recorrido de A que pasa por B hasta llegar a C
        //                                     CBA  .  . <-- recorrido de C que pasa por B hasta llegar a A
        //                                       ADF.  . <-- recorrido de A que pasa por D hasta llegar a F
        //                                         FDA . <-- recorrido de F que pasa por D hasta llegar a A
        //                                           AD. <-- recorrido de A a D
        //                                            DA <-- recorrido de D a A

        static string _patronHisto_3856 = "DEGIAIGEEGIAIGEDEGIAIGEDEGIA"; // tiene problemas pero sigue bien
        static string _patronHisto_4323 = "GEDEGIAIGEHFHDEGIA"; // perfecto
        static string _patronHisto_4319 = "DEGIAIGEDEGIAIGED"; // perfecto
        static string _patronHisto_4309 = "JA";
        static string _patronHisto_4349 = "AIGEDEGIAIGEDEGIAIGEDHFHDEGIA";
        static string _patronHisto_4267 = "DCDEGIAIGEDHFHD";
        static string _patronHisto_3809 = "DDDHCDDDHDD";
        static string _patronHisto_4361 = "DEGIAIGEDEGIGEDD";
        static string _patronHisto_4325 = "IGEDEGIAIGEDHD";
        static string _patronHisto_4354 = "DEGIAIGEHFHDEGIAIGEDEGIAIGED";

        static void Main(string[] args)
        {
            var patronesOrdenados = _patronesRaw2
                .OrderByDescending  (p => p.Length) // ordeno por tamaño
                .ThenBy             (p => p)        // entonces lexicográficamente
                .ToList             ()              // convierto todo en una lista
            ;

            foreach (var patronX in patronesOrdenados)
            {
                Console.WriteLine(patronX);
            }

            Reconocer(patronesOrdenados, _patronHisto_4354);
        }

        static void Reconocer(List<string> patrones, string patronHistorico)
        {
            int ptr = 0;

            for (; ; )
            {
                var puntaInicial = patronHistorico[ptr].ToString();
                Console.Write($"para la punta {puntaInicial} ");

                var patronesPosibles = patrones
                    .Where(pattern => pattern.StartsWith(puntaInicial))
                    .ToList()
                ;

                if (patronesPosibles.Count == 0)
                {
                    Console.WriteLine($"No hay patrones para '{puntaInicial}'");
                    ptr++;
                    if (ptr == patronHistorico.Length - 1)
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
                    if (ptr == patronHistorico.Length - 1)
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

                if (ptr == patronHistorico.Length - 1)
                {
                    Console.WriteLine("FINE");
                    break;
                }

                int fafafa = 0;
            }
        }
    }
}
