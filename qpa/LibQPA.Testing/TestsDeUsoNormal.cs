using Comun;
using ComunSUBE;
using LibQPA.ProveedoresHistoricos.DbXBus;
using LibQPA.ProveedoresTecnobus;
using LibQPA.ProveedoresVentas.DbSUBE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
        public void TestFusion()
        {
            // fechas desde hasta
            var desde = new DateTime(2021, 09, 7, 00, 00, 00);
            var hasta = new DateTime(2021, 09, 8, 00, 00, 00);

            // tipos de coches
            var tipoCoches = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS;

            ///////////////////////////////////////////////////////////////////
            // Datos Empresa-Interno / Ficha
            //  Sirve para:
            //      Puntos SUBE
            //      Ventas de boleto SUBE
            ///////////////////////////////////////////////////////////////////
            var datosEmpIntFicha = new ComunSUBE.DatosEmpIntFicha(new ComunSUBE.DatosEmpIntFicha.Configuration()
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionStringFichasXEmprIntSUBE,
                MaxCacheSeconds = 15 * 60,
            });
            
            ///////////////////////////////////////////////////////////////////
            // Puntos históricos XBus
            ///////////////////////////////////////////////////////////////////
            Dictionary<int, List<PuntoHistorico>> ptsHistoXBusPorFicha;
            const string ARCHIVO_XBUS = "PtsHistoXBus.json";

            if (File.Exists(ARCHIVO_XBUS))
            {
                var json = File.ReadAllText(ARCHIVO_XBUS);
                ptsHistoXBusPorFicha = JsonConvert.DeserializeObject<Dictionary<int, List<PuntoHistorico>>>(json);
            }
            else
            {
                var proveedorPtsHistoXBus = new ProveedorHistoricoDbXBus(
                    new ProveedorHistoricoDbXBus.Configuracion
                    {
                        CommandTimeout = 600,
                        ConnectionString = Configu.ConnectionStringPuntosXBus,
                        Tipo = tipoCoches,
                        FechaDesde = desde,
                        FechaHasta = hasta,
                    });
                ptsHistoXBusPorFicha = proveedorPtsHistoXBus.Get();
                File.WriteAllText(
                    ARCHIVO_XBUS,
                    JsonConvert.SerializeObject(ptsHistoXBusPorFicha)
                );
            }
            var fichasXBus = ptsHistoXBusPorFicha.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos SUBE
            ///////////////////////////////////////////////////////////////////
            Dictionary<int, List<PuntoHistorico>> ptsHistoSUBEPorFicha;
            const string ARCHIVO_SUBE = "PtsHistoSUBE.json";

            if (File.Exists(ARCHIVO_SUBE))
            {
                var json = File.ReadAllText(ARCHIVO_SUBE);
                ptsHistoSUBEPorFicha = JsonConvert.DeserializeObject<Dictionary<int, List<PuntoHistorico>>>(json);
            }
            else
            {
                var proveedorPtsHistoSUBE = new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE(
                new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE.Configuracion
                {
                    CommandTimeout = 600,
                    ConnectionStringPuntos = Configu.ConnectionStringPuntosSUBE,
                    DatosEmpIntFicha = datosEmpIntFicha,
                    FechaDesde = desde,
                    FechaHasta = hasta,
                });
                ptsHistoSUBEPorFicha = proveedorPtsHistoSUBE.Get();
                File.WriteAllText(
                    ARCHIVO_SUBE,
                    JsonConvert.SerializeObject(ptsHistoSUBEPorFicha)
                );
            }

            var fichasSUBE = ptsHistoSUBEPorFicha.Keys.ToList();

            var sumaaa = FusionarPuntos(
                ptsHistoXBusPorFicha, 
                ptsHistoSUBEPorFicha
            );

            var fichasSuma = sumaaa.Keys.ToList();

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
            var recorridosTeoricos = proveedorRecorridosTeoricos.Get(new QPAProvRecoParams()
            {
                LineasPosibles = lineasPosibles,
                FechaVigencia = desde
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
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionStringFichasXEmprIntSUBE,
                MaxCacheSeconds = 15 * 60,
            });

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos XBus
            ///////////////////////////////////////////////////////////////////

            var proveedorPtsHistoXBus = new ProveedorHistoricoDbXBus(
                new ProveedorHistoricoDbXBus.Configuracion
                {
                    CommandTimeout = 600,
                    ConnectionString = Configu.ConnectionStringPuntosXBus,
                    Tipo = tipoCoches,
                    FechaDesde = desde,
                    FechaHasta = hasta,
                });
            var ptsHistoXBusPorFicha = proveedorPtsHistoXBus.Get();
            var fichasXBus = ptsHistoXBusPorFicha.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos SUBE
            ///////////////////////////////////////////////////////////////////
            
            var proveedorPtsHistoSUBE = new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE(
                new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE.Configuracion
                {
                    CommandTimeout = 600,
                    ConnectionStringPuntos = Configu.ConnectionStringPuntosSUBE,
                    DatosEmpIntFicha = datosEmpIntFicha,
                    FechaDesde = desde,
                    FechaHasta = hasta,
                });

            var ptsHistoSUBEPorFicha = proveedorPtsHistoSUBE.Get();
            var fichasSUBE = ptsHistoSUBEPorFicha.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos Suma (XBus + SUBE)
            ///////////////////////////////////////////////////////////////////
            var ptsHistoSumaPorFicha = FusionarPuntos(
                ptsHistoXBusPorFicha, 
                ptsHistoSUBEPorFicha
            );

            var fichasSuma = ptsHistoSumaPorFicha.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////

            var proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(
                new ProveedorVentaBoletosDbSUBE.Configuracion
                {
                    CommandTimeout = 600,
                    ConnectionString = Configu.ConnectionStringVentasSUBE,
                    DatosEmpIntFicha = datosEmpIntFicha,
                    FechaDesde = desde,
                    FechaHasta = hasta,
                });

            proveedorVentaBoletos.TieneBoletosEnIntervalo(0, DateTime.Now, DateTime.Now);

            ///////////////////////////////////////////////////////////////////
            // Procesamiento de los datos (para todas las fichas)
            ///////////////////////////////////////////////////////////////////

            var (resultadosSUBE, resulFichasSUBE) = ProcesarTodo(recorridosTeoricos, topes2D, puntasNombradas, recoPatterns, ptsHistoSUBEPorFicha, fichasSUBE);
            PonerResultadosEnUnArchivo("SUBE.txt", resultadosSUBE, resulFichasSUBE, proveedorVentaBoletos);

            var (resultadosXBus, resulFichasXBus) = ProcesarTodo(recorridosTeoricos, topes2D, puntasNombradas, recoPatterns, ptsHistoXBusPorFicha, fichasXBus);
            PonerResultadosEnUnArchivo("XBus.txt", resultadosXBus, resulFichasXBus, proveedorVentaBoletos);

            var (resultadosSuma, resulFichasSuma) = ProcesarTodo(recorridosTeoricos, topes2D, puntasNombradas, recoPatterns, ptsHistoSumaPorFicha, fichasSuma);
            PonerResultadosEnUnArchivo("Suma.txt", resultadosSuma, resulFichasSuma, proveedorVentaBoletos);

            int foo = 0;
        }

        Dictionary<int, List<PuntoHistorico>> FusionarPuntos(params Dictionary<int, List<PuntoHistorico>> [] ptss)
        {
            var ret = new Dictionary<int, List<PuntoHistorico>>();

            // para cada diccionario que me pasaron
            foreach (Dictionary<int, List<PuntoHistorico>> pts in ptss)
            {
                // para cada clave del diccionario
                foreach (var key in pts.Keys)
                {
                    // creo la entrada si no existe
                    if (!ret.ContainsKey(key))
                    {
                        ret.Add(key, new List<PuntoHistorico>());
                    }

                    // agrego los puntos del diccionario a esa entrada
                    ret[key].AddRange(pts[key]);
                }
            }

            // ordeno los puntos por fecha
            foreach (var key in ret.Keys)
            {
                ret[key].Sort(new Comparison<PuntoHistorico>((p1, p2) =>
                {
                    if (p1.Fecha > p2.Fecha) return 1;
                    if (p1.Fecha < p2.Fecha) return -1;
                    return 0;
                }));
            }

            return ret;
        }

        private (List<QPAResult>, List<int>) ProcesarTodo(
            List<RecorridoLinBan>                               recorridosTeoricos,
            Topes2D                                             topes2D,
            IEnumerable<PuntaLinea>                             puntasNombradas,
            Dictionary<string, List<KeyValuePair<int, int>>>    recoPatterns,
            Dictionary<int, List<PuntoHistorico>>               puntosHistoricosPorFicha,
            List<int>                                           fichas
        )
        {
            var qpaProcessor= new QPAProcessor();
            var resultados  = new List<QPAResult>();
            var resulfichas = new List<int>();

            foreach (var ficha in fichas)
            {
                try
                {
                    var res = qpaProcessor.Procesar(
                        recorridosTeoricos  : recorridosTeoricos,
                        puntosHistoricos    : puntosHistoricosPorFicha[ficha],
                        topes2D             : topes2D,
                        puntasNombradas     : puntasNombradas,
                        recoPatterns        : recoPatterns
                    );

                    resultados.Add(res);
                    resulfichas.Add(ficha);
                }
                catch (Exception exx)
                {
                    int foofoo = 0;
                }
            }

            //var resultadosPosibles = resultados
            //    .Where(r => !string.IsNullOrEmpty(r.Camino.Description))
            //    .Where(r => NCaracteresDiferentes(r.Camino.Description) > 1)
            //    .ToList()
            //;

            //var resultadosPasables = resultados
            //    .Where(r => (r.PorcentajeReconocido >= 60))
            //    .ToList()
            //;

            //var resultadosMuyBuenos = resultados
            //    .Where(r => (r.PorcentajeReconocido >= 80))
            //    .ToList()
            //;

            //var resultadosPerfectos = resultados
            //    .Where(r => (r.PorcentajeReconocido >= 100))
            //    .ToList()
            //;

            return (resultados, resulfichas);
        }

        private void PonerResultadosEnUnArchivo(string nombreArchivo, List<QPAResult> resultados, List<int> fichas, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos)
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
            File.WriteAllLines(nombreArchivo, textosResultados.ToArray());
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
