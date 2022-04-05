using System;
using System.Collections.Generic;
using System.Linq;
using Comun;

namespace Correspondencias
{
    public class Correspondencias
    {
        public IEnumerable<List<T>> GetParticionesPorTamano<T>(IEnumerable<T> elems, int psizeMayorQueCero)
        {
            if (psizeMayorQueCero <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(psizeMayorQueCero), "psize debe ser mayor que 0");
            }

            List<T> aux = null;

            foreach (T t in elems)
            {
                if (aux == null)
                {
                    aux = new List<T>();
                }

                aux.Add(t);

                if (aux.Count == psizeMayorQueCero)
                {
                    yield return aux;
                    aux = null;
                }
            }

            // último pedazo puede no haber sido retornado
            // (ej si el tamaño es menor que psize)
            if (aux != null && aux.Count > 0)
            {
                yield return aux;
            }
        }

        public IEnumerable<List<T>> GetParticionesPorCantidad<T>(IEnumerable<T> elems, int cantidad)
        {
            int count = elems.Count();

            int psize = count / cantidad;
            int sobra = count % cantidad;

            int realsize = psize + (sobra > 0 ? 1 : 0);

            foreach (var particion in GetParticionesPorTamano(elems, realsize))
            {
                yield return particion;
            }
        }

        public double GetPorcentajeCorrespondencia(
            List<Punto> puntosBase, 
            List<Punto> puntosParaProbar, 
            int         cantidadPartes,
            int         granularidad = 100
        )
        {
            // *** los topes deben calcularse aca!!! ***
            Topes2D topes2D = Topes2D.CreateFromPuntos(puntosBase.Concat(puntosParaProbar));

            var particiones = GetParticionesPorCantidad(puntosBase, cantidadPartes).ToList();
            HashSet<int> indexes = new HashSet<int>();
            foreach (var punto in puntosParaProbar)
            {
                int indexParticion = DeterminarIndexParticionPertenecePunto(particiones, punto, granularidad, topes2D);

                if (indexParticion != -1)
                {
                    indexes.Add(indexParticion);
                }
            }

            return (indexes.Count * 100.0) / cantidadPartes;
        }

        private int DeterminarIndexParticionPertenecePunto(List<List<Punto>> particiones, Punto punto, int granularidad, Topes2D topes2D)
        {
            for (int i = 0; i < particiones.Count; i ++)
            {
                if (ParticionContienePunto(particiones[i], punto, granularidad, topes2D))
                {
                    return i;
                }
            }

            return -1;
        }

        private bool ParticionContienePunto(List<Punto> puntos, Punto punto, int granularidad, Topes2D topes2D)
        {
            Casillero casilleroPunto = Casillero.Create(topes2D, punto, granularidad);

            HashSet<Casillero> conjuntoCasilleros = Casillero.CreateHashSet(topes2D, puntos, granularidad);

            return Casillero.PresenteFlexEn(casilleroPunto, conjuntoCasilleros);
        }

        bool ListaContiene(List<Punto> particion, Punto punto, int granularidad)
        {
            return false;
        }

        public int GetCorrespCantPartes(
            List<Punto> teorico, 
            List<Punto> real, 
            int cantidadDePartes,
            int granularidad = 100
        )
        {
            int sizeSobra = teorico.Count % cantidadDePartes;
            int sizeParte = teorico.Count / cantidadDePartes;
            
            var partes = new List<(int, List<Punto>)>();
            foreach (Punto px in teorico)
            {
                // si "no hay partes" o "la última parte cargada está llena"
                if (partes.Count == 0 || partes[^1].Item2.Count == sizeParte)
                {
                    // creo un nuevo casillero en la lista
                    var valorInicial = (0, new List<Punto>());
                    partes.Add(valorInicial);
                }

                // agrego un punto al último casillero de la lista
                partes[^1].Item2.Add(px);
            }

            ////////////////////////////////////////////////////////////////////
            // ahora tenemos las "partes", ¿como creamos las correspondencias?

            // ¿podemos crear casilleros geométricos?
            // [....][....][....][....]

            var topes2D = Topes2D.CreateFromPuntos(Enumerable.Concat(teorico, real));

            var casillero1 = Casillero.Create(topes2D, teorico.Skip(5).First(), granularidad);

            casillero1.PresenteFlexEn(new List<Casillero>());

            return 0;
        }
    }
}
