using Comun;
using ComunSUBE;
using LibQPA.ProveedoresHistoricos.DbXBus;
using LibQPA.ProveedoresTecnobus;
using LibQPA.ProveedoresVentas.DbSUBE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        [TestMethod]
        public void HayRepos()
        {
            foreach (var kvp in Configu.Repos)
            {
                var fullPath1 = Path.GetFullPath(Configu.Repos[kvp.Key]);
                Assert.IsTrue(Directory.Exists(fullPath1));
            }
        }

        string[] DameMockRepos()
        {
            return new[] { Configu.Repos["MockRepo1"], Configu.Repos["MockRepo2"] };
        }

        [TestMethod]
        public void FuncionaProveedorVersionesHoy()
        {
            var proveedor  = new ProveedorVersionesTecnobus(dirRepos: DameMockRepos());
            var recorridos = proveedor.Get(new QPAProvRecoParams()
            {
                LineasPosibles = new int[] { 159 },
                FechaVigencia  = DateTime.Today,
            });

            Assert.IsTrue(recorridos.Any(), "No hay recorridos");
        }

        [TestMethod]
        public void FuncionaProveedorHistorico()
        {
            var hoy = DateTime.Today;
            var ayer = hoy.AddDays(-1);

            var config = new ProveedorHistoricoDbXBus.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionStringPuntosXBus,
                Tipo = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS,
                FechaDesde = ayer,
                FechaHasta = hoy,
            };

            var proveedor = new ProveedorHistoricoDbXBus(config);

            var fichas = proveedor.Get().Keys.ToList();

            int foo = 0;
        }

        [TestMethod]
        public void TestQPA1()
        {
            ///////////////////////////////////////////////////////////////////
            // variables de entrada
            ///////////////////////////////////////////////////////////////////

            // granularidad del cálculo
            int granularidadMts = 20;

            // radio de puntas de línea
            int radioPuntasDeLineaMts = 800;

            // fechas desde hasta
            var desde = new DateTime(2021, 09, 7, 00, 00, 00);
            var hasta = new DateTime(2021, 09, 8, 00, 00, 00);

            // códigos de las líneas para este cálculo
            var lineasPosibles = new int[] { 159, 163 };

            // tipos de coches
            var tipoCoches = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS;

            ///////////////////////////////////////////////////////////////////
            // recorridos teóricos / topes / puntas nombradas / recopatterns
            ///////////////////////////////////////////////////////////////////
            
            var proveedorRecorridosTeoricos = new ProveedorVersionesTecnobus(dirRepos: DameMockRepos());
            var recorridosTeoricos = proveedorRecorridosTeoricos.Get(new QPAProvRecoParams() { 
                LineasPosibles = lineasPosibles, 
                FechaVigencia  = desde
            })
                .Select(reco => SanitizarRecorrido(reco, granularidadMts))
                .ToList()
            ;

            // puntos aplanados (todos)
            var todosLosPuntosDeLosRecorridos = recorridosTeoricos.SelectMany(
                (reco) => reco.Puntos,
                (reco, puntoReco) => puntoReco
            );

            // topes
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

            // puntas de línea
            var puntasNombradas = PuntasDeLinea.GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts);

            // caminos de los recos, es un diccionario:
            // ----------------------------------------
            //  -de clave tiene el patrón de recorrido
            //  -de valor tiene una lista de pares (linea, bandera) que son las banderas que encajan con ese patrón
            var recoPatterns = new Dictionary<string, List<KeyValuePair<int, int>>>();
            foreach (var recox in recorridosTeoricos)
            {
                // creo un camino (su descripción es la clave)
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntasNombradas, recox.Puntos);

                // si la clave no está en el diccionario la agrego...
                if (!recoPatterns.ContainsKey(camino.Description))
                {
                    recoPatterns.Add(camino.Description, new List<KeyValuePair<int, int>>());
                }

                // agrego un par (linea, bandera) a la entrada actual...
                recoPatterns[camino.Description].Add(new KeyValuePair<int, int>(recox.Linea, recox.Bandera));
            }


            ///////////////////////////////////////////////////////////////////
            // Datos Empresa-Interno / Ficha
            //  Sirve para:
            //      Puntos SUBE
            //      Ventas de boleto SUBE
            ///////////////////////////////////////////////////////////////////
            var datosEmpIntFicha = new ComunSUBE.DatosEmpIntFicha(new ComunSUBE.DatosEmpIntFicha.Configuration()
            {
                CommandTimeout  = 600,
                ConnectionString= Configu.ConnectionStringFichasXEmprIntSUBE,
                MaxCacheSeconds = 15 * 60,
            });

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos
            ///////////////////////////////////////////////////////////////////

            //var proveedorPuntosHistoricos = new ProveedorHistoricoDbXBus(
            //    new ProveedorHistoricoDbXBus.Configuracion
            //    {
            //        CommandTimeout = 600,
            //        ConnectionString = Configu.ConnectionStringPuntosXBus,
            //        Tipo = tipoCoches,
            //        FechaDesde = desde,
            //        FechaHasta = hasta,
            //    });

            var proveedorPuntosHistoricos = new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE(
                new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE.Configuracion
                {
                    CommandTimeout = 600,
                    ConnectionStringPuntos = Configu.ConnectionStringPuntosSUBE,
                    DatosEmpIntFicha = datosEmpIntFicha,
                    FechaDesde = desde,
                    FechaHasta = hasta,
                });

            var todosLosPuntosHistoricos = proveedorPuntosHistoricos.Get();
            var fichas = todosLosPuntosHistoricos.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////

            var proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(
                new ProveedorVentaBoletosDbSUBE.Configuracion
                {
                    CommandTimeout  = 600,
                    ConnectionString= Configu.ConnectionStringVentasSUBE,
                    DatosEmpIntFicha= datosEmpIntFicha,
                    FechaDesde      = desde, 
                    FechaHasta      = hasta,
                });

            proveedorVentaBoletos.TieneBoletosEnIntervalo(0, DateTime.Now, DateTime.Now);

            ///////////////////////////////////////////////////////////////////
            // Procesamiento de los datos (para todas las fichas)
            ///////////////////////////////////////////////////////////////////
            var qpaProcessor= new QPAProcessor();
            var resultados  = new List<QPAResult>();
            var resulfichas = new List<int>();

            foreach (var ficha in fichas)
            {
                var res = qpaProcessor.Procesar(
                    recorridosTeoricos  : recorridosTeoricos, 
                    puntosHistoricos    : todosLosPuntosHistoricos[ficha],
                    topes2D             : topes2D,
                    puntasNombradas     : puntasNombradas,
                    recoPatterns        : recoPatterns
                );

                resultados.Add(res);
                resulfichas.Add(ficha);
            }

            var resultadosPosibles = resultados
                .Where  (r => !string.IsNullOrEmpty(r.Camino.Description))
                .Where  (r => NCaracteresDiferentes(r.Camino.Description) > 1)
                .ToList ()
            ;

            var resultadosPasables = resultados
                .Where  (r => (r.PorcentajeReconocido >= 60))
                .ToList ()
            ;

            var resultadosMuyBuenos = resultados
                .Where(r => (r.PorcentajeReconocido >= 80))
                .ToList()
            ;

            var resultadosPerfectos = resultados
                .Where  (r => (r.PorcentajeReconocido >= 100))
                .ToList ()
            ;

            /*
                posibles    248 100% hay 240 fichas con datos que parecen ser un recorrido, los demas son galpon o no hay datos
                malos        32  13% de los resultados son malos      (menos del 60% de reconocimiento)
                pasables    216  87% de los resultados son pasables   (se pudo averiguar el 60% o mas)
                muybuenos   200  80% de los resultados son muy buenos (se pudo averiguar el 80% o mas)
                perfectos   146  59% de los resultados son perfectos  (se pudo averiguar el 100% de lo que hizo)
            */

            // TODO:
            //  RECONOCER SI LA PARTE NO RECONOCIDA ESTÁ EN LOS BORDES

            PonerResultadosEnUnArchivo(resultados, proveedorVentaBoletos, resulfichas);

            int foo = 0;
        }

        private void PonerResultadosEnUnArchivo(List<QPAResult> resultados, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, List<int> fichas)
        {
            var textosResultados = new List<string>();
            for (int i = 0; i < resultados.Count; i++)
            {
                var resultado = resultados[i];
                if (resultado.PorcentajeReconocido < 80) 
                { 
                    continue;
                }
                int ficha = fichas[i];
                var textoResultado = CrearTexto(resultado, proveedorVentaBoletos, ficha);
                textosResultados.Add(textoResultado);
            }
            File.WriteAllLines("dump.txt", textosResultados.ToArray());
        }

        private string CrearTexto(QPAResult resultado, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, int ficha)
        {
            var sbResult = new StringBuilder();
            sbResult.AppendLine(new string('-', 80));
            sbResult.AppendLine($"Ficha: {ficha}");
            sbResult.AppendLine($"");
            foreach (var subCamino in resultado.SubCaminos)
            {
                foreach (var linBanPun in subCamino.LineasBanderasPuntuaciones)
                {
                    sbResult.AppendLine($"Lin Ban: {linBanPun.Linea} {linBanPun.Bandera}");
                }
                sbResult.AppendLine($"\tInicio  : {subCamino.HoraComienzo}");
                sbResult.AppendLine($"\tFin     : {subCamino.HoraFin}");

                if (proveedorVentaBoletos.TieneBoletosEnIntervalo(ficha, subCamino.HoraComienzo, subCamino.HoraFin))
                {
                    var sbVentas= new StringBuilder();
                    var boletos = proveedorVentaBoletos
                        .GetBoletosEnIntervalo(ficha, subCamino.HoraComienzo, subCamino.HoraFin)
                        .ToList();
                    var fechas  = boletos.Select(bol => bol.FechaCancelacion);
                    var sFechas = string.Join(',', fechas);
                    sbVentas.Append($"Tiene {boletos.Count} boletos vendidos en: {sFechas}");
                    sbResult.AppendLine($"\tVenta   : {sbVentas}");
                }
                else
                {
                    sbResult.AppendLine($"\tVenta   : Sin datos por parte de archivos SUBE");
                }
                sbResult.AppendLine($"\tTotMins~: {Convert.ToInt32(subCamino.Duracion.TotalMinutes) }");
                sbResult.AppendLine($"");
            }
            return sbResult.ToString();
        }

        // TODO: pasar esta función a RecorridoLinBan
        // MMMMMMMMMMMMM no se si hay que hacer eso...
        // !!!!!!!!!!!!! revisar que tengo para la granularizacion en RULOS... puede ser mejor...
        static RecorridoLinBan SanitizarRecorrido(RecorridoLinBan reco, int granularidad)
        {
            return new RecorridoLinBan
            {
                Bandera = reco.Bandera,
                Linea = reco.Linea,
                Puntos = reco.Puntos.HacerGranular(granularidad),
            };
        }

        static int NCaracteresDiferentes(string s)
        {
            var conjunto = new HashSet<char>();

            foreach (var c in s)
            {
                conjunto.Add(c);
            }

            return conjunto.Count;
        }
    }
}
