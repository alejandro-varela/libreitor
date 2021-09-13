using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Comun;
using LibQPA;
using LibQPA.ProveedoresTecnobus;
using LibQPA.ProveedoresHistoricos.DbXBus;

namespace LibQPA_Testing
{
    [TestClass]
    public class TestsDeUsoNormal
    {
        public string DIR_REPO_1 = "..\\..\\..\\..\\Datos\\ZipRepo\\";
        public string DIR_REPO_2 = "..\\..\\..\\..\\Datos\\ZipRepo\\";

        public ProveedorVersionesTecnobus CrearProveedorVersionesTecnobus(int[] lineas)
        {
            return new ProveedorVersionesTecnobus(
                dirRepos: new[] {
                    DIR_REPO_1,
                    DIR_REPO_2,
                }
            );
        }

        [TestMethod]
        public void HayRepos()
        {
            var fullPath1 = Path.GetFullPath(DIR_REPO_1);
            Assert.IsTrue(Directory.Exists(fullPath1));
            var fullPath2 = Path.GetFullPath(DIR_REPO_2);
            Assert.IsTrue(Directory.Exists(fullPath2));
        }

        [TestMethod]
        public void FuncionaProveedorVersionesHoy()
        {
            var proveedor = CrearProveedorVersionesTecnobus(
                new int[] { 159 }
            );
            
            var foo = proveedor.Get(
                new int[] { 159 },
                DateTime.Today
            );
        }

        [TestMethod]
        public void FuncionaProveedorHistorico()
        {
            var config = new ProveedorHistoricoDbXBus.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = "lalalallala",
                Tipo = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS,
            };

            ProveedorHistoricoDbXBus prov = new ProveedorHistoricoDbXBus(config);

            var hoy = DateTime.Today;
            var ayer= hoy.AddDays(-1);
            var ienm= prov.Get(ayer, hoy).ToList();

            int foo = 0;
        }

        [TestMethod]
        public void Foo()
        {
            /*
            QPAProcessor qpaProcessor = new QPAConfiguration()
                .SetProveedorRecorridosTeoricos (proveedor)
                .SetProveedorPuntosHistoricos   (null)
                .BuildProcessor                 ()
            ;

            qpaProcessor.Procesar(new int[] { 1, 2, 3 }, new DateTime());
            */
        }
    }
}
