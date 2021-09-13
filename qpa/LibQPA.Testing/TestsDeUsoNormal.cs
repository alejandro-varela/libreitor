using LibQPA.ProveedoresHistoricos.DbXBus;
using LibQPA.ProveedoresTecnobus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace LibQPA.Testing
{
    [TestClass]
    public class TestsDeUsoNormal
    {
        TestConfiguration Configu { get; set; }

        [TestInitialize]
        public void Init()
        {
            var configPath = Path.GetFullPath(".");
            Configu = TestConfiguration.Get(configPath);
            int foo = 0;
        }

        public ProveedorVersionesTecnobus CrearProveedorVersionesTecnobus(int[] lineas)
        {
            return new ProveedorVersionesTecnobus(
                dirRepos: new[] {
                    Configu.Repos["MockRepo1"],
                    Configu.Repos["MockRepo2"],
                }
            );
        }

        [TestMethod]
        public void HayRepos()
        {
            foreach (var kvp in Configu.Repos)
            {
                var fullPath1 = Path.GetFullPath(Configu.Repos[kvp.Key]);
                Assert.IsTrue(Directory.Exists(fullPath1));
            }
        }

        [TestMethod]
        public void FuncionaProveedorVersionesHoy()
        {
            var proveedor = CrearProveedorVersionesTecnobus(
                new int[] { 159 }
            );
            
            var recorridos = proveedor.Get(
                new int[] { 159 },
                DateTime.Today
            );

            Assert.IsTrue(recorridos.Any(), "No hay recorridos");
        }

        [TestMethod]
        public void FuncionaProveedorHistorico()
        {
            var config = new ProveedorHistoricoDbXBus.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionString,
                Tipo = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS,
            };

            ProveedorHistoricoDbXBus prov = new ProveedorHistoricoDbXBus(config);

            var hoy = DateTime.Today;
            var ayer= hoy.AddDays(-1);
            var ienm= prov.Get(ayer, hoy).ToList();

            int foo = 0;
        }
    }
}
