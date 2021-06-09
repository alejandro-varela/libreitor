using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaLecturaDeRecorridos
{
    public static class ExtensionesRepeticion
    {
        public static IEnumerable<T> Simplificar<T>(this IEnumerable<T> source, Func<T,T,bool> compareFunction)
        {
            // si source es null tiro error
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // si source no tiene elementos corto 
            if (!source.Any())
            {
                yield break;
            }

            // tengo elemento anterior y si fue inicializado...
            T    anterior = default;
            bool init     = false;

            // para cada elemento en source...
            foreach (T t in source)
            {
                // si no no hubo inicialización, esto es si estamos en el primer elemento, retorno el actual
                if (!init)
                {
                    yield return t;
                }
                // en caso contrario y si el anterior es distinto que el actual, retorno el actual
                else if (!compareFunction(anterior, t))
                {
                    yield return t;
                }

                // ya pasamos por el primer elemento (init <- true)
                // guardo el elemento actual en "anterior"
                init = true;
                anterior = t;
            }
        }
    }
}
