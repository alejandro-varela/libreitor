using System;
using System.Collections.Generic;
using System.Linq;

namespace Comun
{
    public class DetectorIslas
    {
        public static IEnumerable<List<T>> GetIslas<T>(IEnumerable<T> items, Func<T, T, bool> sonDeLaMismaIsla)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (!items.Any())
            {
                yield break;
            }

            List<T> listaAux = new List<T>();
            T anterior = items.First();

            listaAux.Add(anterior);
            //Console.Write(datoAnterior + " ");

            foreach (T actual in items.Skip(1))
            {
                if (!sonDeLaMismaIsla(anterior, actual))
                {
                    yield return listaAux;
                    listaAux = new List<T>();
                    //Console.WriteLine();
                }

                listaAux.Add(actual);
                //Console.Write(datoActual  + " ");

                anterior = actual;
            }

            yield return listaAux;
        }

        public static List<List<T>> GetIslasList<T>(IEnumerable<T> items, Func<T, T, bool> sonDeLaMismaIsla)
        {
            return GetIslas(items, sonDeLaMismaIsla).ToList();
        }

        //public static List<List<T>> GetIslasList<T>(IEnumerable<T> datos, Func<T, T, bool> sonDeLaMismaIsla)
        //{
        //    if (datos == null)
        //    {
        //        throw new ArgumentNullException(nameof(datos));
        //    }
        //    List<List<T>> listaRet = new List<List<T>>();
        //    if (!datos.Any())
        //    {
        //        return listaRet;
        //    }
        //    T datoAnterior = datos.First();
        //    List<T> listaAux = new List<T>();
        //    listaAux.Add(datoAnterior);
        //    foreach (T datoActual in datos.Skip(1))
        //    {
        //        if (!sonDeLaMismaIsla(datoAnterior, datoActual))
        //        {
        //            listaRet.Add(listaAux);
        //            listaAux = new List<T>();
        //            //Console.WriteLine();
        //        }
        //        listaAux.Add(datoActual);
        //        //Console.Write(datoActual  + " ");
        //        datoAnterior = datoActual;
        //    }
        //    listaRet.Add(listaAux);
        //    return listaRet;
        //}
    }
}
