using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Comun;
using LibQPA;
using LibQPA.ProveedoresTecnobus;

namespace LibQPA_Testing
{
    [TestClass]
    public class TestsDeUsoNormal
    {
        [TestMethod]
        public void TestMethod1()
        {
            var proveedor = new ProveedorVersionesTecnobus(dirRepos: new[] { 
                "sacar de la configuración ", 
                "otra dirección por si las moscas.." 
            });

            QPAProcessor qpaProcessor = new QPAConfiguration()
                .SetProveedorRecorridosTeoricos (proveedor)
                .SetProveedorPuntosHistoricos   (null)
                .BuildProcessor                 ()
            ;

            qpaProcessor.Procesar(new int[] { 1, 2, 3 }, new DateTime());
        }
    }
}
