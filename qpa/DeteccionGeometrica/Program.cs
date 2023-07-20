using Comun;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DeteccionGeometrica
{
    public class Program
    {
        static void Main(string[] args)
        {
            // ¿por qué es naif?
            // es naif porque no toma en cuenta la linealidad de los puntos...
            // esto es, su crecimiento en un camino...
            //
            // 0    1    1    1    1    1    0    0    1    1    1    0         
            // **** **** **** **** **** **** **** **** **** **** **** ***** 100%
            //       --- ---- -  - ---- -              ---- ---    --        66%

            // ¿cómo se si una ristra de puntos pertenece a una ristra de recorridos?
            // ----------------------------------------------------------------------
            //
            // EJ: ABCRWDF
            //
            //     ABC
            //       CRW
            //         WDF

            // tengo que arreglar el reconocedor... :S
        }

        static int DetectarGeometriaNaif(
            IEnumerable<Punto> puntosContenedor, 
            IEnumerable<Punto> puntosComp, 
            int granularidad = 20
        )
        {
            if (puntosComp == null || !puntosComp.Any())
            {
                return 0;
            }

            var topes2DContenedor = Topes2D.CreateFromPuntos (puntosContenedor);
            var topes2DPuntos     = Topes2D.CreateFromPuntos (puntosComp);
            var topes2D           = Topes2D.CreateFromTopes  (topes2DContenedor, topes2DPuntos);

            // contenedor
            var casillerosContenedor = new HashSet<Casillero>();

            foreach (var ptx in puntosContenedor)
            {
                var casillero = Casillero.Create(topes2D, ptx, granularidad);
                casillerosContenedor.Add(casillero);
            }

            // comp
            var casillerosComp = new HashSet<Casillero>();

            foreach (var ptx in puntosComp)
            {
                var casillero = Casillero.Create(topes2D, ptx, granularidad);
                casillerosComp.Add(casillero);
            }

            // intersección
            // puede hacerse con el método Intersect<X> de LINQ pero no...

            int contadorComp = 0;
            int contadorOk   = 0;

            foreach (var elem in puntosComp)
            {
                contadorComp++;

                if (puntosContenedor.Contains(elem))
                {
                    contadorOk++;
                }
            }

            // la cuenta...
            return (contadorOk * 100) / contadorComp;
        }

    }
}
