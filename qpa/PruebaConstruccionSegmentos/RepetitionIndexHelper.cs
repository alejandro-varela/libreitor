using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaConstruccionSegmentos
{    
    public class RepetitionIndexHelper
    {
        // la ventaja de escribir las combinaciones de FIRMAS en vez de usar argumentos opcionales es que
        // podemos explotar muchisimo el uso de generics...
        // si tengo firmas fijas puedo retornar tipos dependiendo de la presencia o no de los argumentos
        // si tengo argumentos opcionales no podria hacer tal cosa...
        // *en estos casos estoy retornando IndexInfo<TElem> si no me pasan un selector
        // *si me pasan un parametro selector puedo retornar IndexInfo<TSel>, siendo TSel posiblemente o no igual a TElem
        // *si no me pasan un parametro elemsAreEqual puedo forzar a que TElem sea IEquatable<TElem>
        public static IEnumerable<IndexInfo<TSel>> GetIndexes<TElem, TSel>(
            IEnumerable<TElem> elems,
            Func<TElem, TSel> selector,
            Func<TElem, TElem, bool> elemsAreEqual
        )
        {
            if (elems == null)
            {
                throw new ArgumentNullException(nameof(elems));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (elemsAreEqual == null)
            {
                throw new ArgumentNullException(nameof(elemsAreEqual));
            }

            if (!elems.Any())
            {
                yield break;
            }

            TElem actual = default;
            int index = 0;
            IndexInfo<TSel> indexInfo = null;

            foreach (var elem in elems)
            {
                // si hay un cambio o es el primer elemento
                if (!elemsAreEqual(elem, actual) || index == 0)
                {
                    // si hay un cambio pero no es el primer cambio
                    if (index > 0)
                    {
                        // termino de construir la información previa con el index anterior
                        // y la yield retorno
                        indexInfo.EndIndex = index-1;
                        yield return indexInfo;
                    }

                    // creo un IndexInfo con el elemento actual y el index actual
                    indexInfo = new IndexInfo<TSel> { Value = selector(elem), StartIndex = index };
                    actual = elem;
                }

                index++;
            }

            // esto es porque siempre queda el ultimo elemento incompleto...
            // termino de construir la información previa con el index anterior
            // y la yield retorno
            indexInfo.EndIndex = index-1;
            yield return indexInfo;
        }

        public static IEnumerable<IndexInfo<TElem>> GetIndexes<TElem>(
            IEnumerable<TElem> elems,
            Func<TElem, TElem, bool> elemsAreEqual
        )
        {
            var indexes = GetIndexes<TElem, TElem>(
                elems,
                e => e,
                elemsAreEqual
            );

            foreach (IndexInfo<TElem> elemx in indexes)
            {
                yield return elemx;
            }
        }

        public static IEnumerable<IndexInfo<TSel>> GetIndexes<TElem, TSel>(
            IEnumerable<TElem> elems,
            Func<TElem, TSel> selector
        ) where TElem : IEquatable<TElem>
        {
            var indexes = GetIndexes<TElem, TSel>(
                elems,
                selector,
                (e1, e2) => e1.Equals(e2)
            );

            foreach (IndexInfo<TSel> elemx in indexes)
            {
                yield return elemx;
            }
        }

        public static IEnumerable<IndexInfo<TElem>> GetIndexes<TElem>(
            IEnumerable<TElem> elems
        ) where TElem : IEquatable<TElem>
        {
            var indexes = GetIndexes<TElem, TElem>(
                elems,
                e => e,
                (e1, e2) => e1.Equals(e2)
            );

            foreach (IndexInfo<TElem> elemx in indexes)
            {
                yield return elemx;
            }
        }

        public static void ShowIndexes(string s, Func<IndexInfo<char>, bool> filter = null)
        {
            IEnumerable<IndexInfo<char>> indexes = null;

            if (filter == null)
            {
                indexes = GetIndexes(s);
            }
            else
            {
                indexes = GetIndexes(s).Where(filter);
            }

            foreach (var index in indexes)
            {
                Console.WriteLine(index);
            }
        }
    }
}
