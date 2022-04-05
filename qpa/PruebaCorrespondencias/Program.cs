using System;
using Correspondencias;
using Comun;
using System.Collections.Generic;
using System.Linq;

namespace PruebaCorrespondencias
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new Correspondencias.Correspondencias();

            var teorico = new List<Punto>
            {
                new Punto { Lat=-32.00, Lng=-60.14 },
                new Punto { Lat=-32.01, Lng=-60.13 },
                new Punto { Lat=-32.02, Lng=-60.12 },
                new Punto { Lat=-32.03, Lng=-60.11 },
                new Punto { Lat=-32.04, Lng=-60.10 },
                new Punto { Lat=-32.05, Lng=-60.09 },
                new Punto { Lat=-32.06, Lng=-60.08 },
                new Punto { Lat=-32.07, Lng=-60.07 },
                new Punto { Lat=-32.08, Lng=-60.06 },
                new Punto { Lat=-32.09, Lng=-60.05 },
                new Punto { Lat=-32.10, Lng=-60.04 },
                new Punto { Lat=-32.11, Lng=-60.03 },
                new Punto { Lat=-32.12, Lng=-60.02 },
                new Punto { Lat=-32.13, Lng=-60.01 },
                new Punto { Lat=-32.14, Lng=-60.00 },
            };

            var real = new List<Punto>
            {
                new Punto { Lat=-32.00, Lng=-60.14 },
                new Punto { Lat=-32.01, Lng=-60.13 },
                //new Punto { Lat=-32.02, Lng=-60.12 },
                //new Punto { Lat=-32.03, Lng=-60.11 },
                //new Punto { Lat=-32.04, Lng=-60.10 },
                //new Punto { Lat=-32.05, Lng=-60.09 },
                //new Punto { Lat=-32.06, Lng=-60.08 },
                //new Punto { Lat=-32.07, Lng=-60.07 },
                //new Punto { Lat=-32.08, Lng=-60.06 },
                //new Punto { Lat=-32.09, Lng=-60.05 },
                new Punto { Lat=-32.10, Lng=-60.04 },
                //new Punto { Lat=-32.11, Lng=-60.03 },
                //new Punto { Lat=-32.12, Lng=-60.02 },
                new Punto { Lat=-32.13, Lng=-60.01 },
                new Punto { Lat=-32.14, Lng=-60.00 },
            };

            var porcentaje = c.GetCorrespCantPartes(
                teorico, 
                real, 
                2
            );

            var pepe = c
                .GetParticionesPorTamano(teorico, 20)
                .ToList()
            ;

            var pipo = c
                .GetParticionesPorCantidad(teorico, 4)
                .ToList()
            ;

            var porcent = c.GetPorcentajeCorrespondencia(teorico, real, 15, 50);

            int fii = 0;
        }
    }
}
