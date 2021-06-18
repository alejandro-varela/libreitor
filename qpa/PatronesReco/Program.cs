using System;
using System.Collections.Generic;
using System.Linq;

namespace PatronesReco
{
    class Program
    {
        static readonly string[] _patronesRaw = new [] {
            "AB",
            "ABC",
            "BA",
            "CBA",
        };

        //static List<string> _patronHisto1 = new("ABABCBA".Select(c => c.ToString()));
        static string _patronHisto1 = "ABABCBABCBA";
        //                             AB  .  .  .  <-- recorrido de A a B
        //                              BA .  .  .  <-- recorrido de B a A
        //                               ABC  .  .  <-- recorrido de A que pasa por B hasta llegar a C
        //                                 CBA.  .  <-- recorrido de C que pasa por B hasta llegar a A
        //                                   ABC .  <-- recorrido de A que pasa por B hasta llegar a C
        //                                     CBA  <-- recorrido de C que pasa por B hasta llegar a A

        static void Main(string[] args)
        {
            var patronesOrdenados = _patronesRaw
                .OrderByDescending  (p => p.Length) // ordeno por tamaño
                .ThenBy             (p => p)        // entonces lexicográficamente
                .ToList             ()              // convierto todo en una lista
            ;

            foreach (var patronX in patronesOrdenados)
            {
                Console.WriteLine(patronX);
            }

            Reconocer(patronesOrdenados, _patronHisto1);
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
                        Console.WriteLine($"El patron {patronPosible} es bueno para {patronHistorico} en index:{ptr}");
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
