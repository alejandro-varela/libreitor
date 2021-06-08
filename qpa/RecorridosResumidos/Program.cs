using System;
using System.Collections.Generic;
using System.Linq;

namespace RecorridosResumidos
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var n in Resumir(Enumerable.Range(0, 1000), 16))
            {
                Console.Write($"{n:0000}");
                Console.Write(' ');
            }
        }

        static IEnumerable<T> Resumir<T>(IEnumerable<T> cosas, Func<T,int,bool> reducer)
        {
            int index = 0;

            foreach (var cosa in cosas)
            {
                if (reducer(cosa, index))
                {
                    yield return cosa;
                }

                index++;
            }
        }

        static IEnumerable<T> Resumir<T>(IEnumerable<T> cosas, int divisor)
        {
            bool fnReductorMod(T cosa, int index)
            {
                return index % divisor == 0;
            }

            foreach (var cosa in Resumir(cosas, fnReductorMod))
            {
                yield return cosa;
            }
        }
    }
}
