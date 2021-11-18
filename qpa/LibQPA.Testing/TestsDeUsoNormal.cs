using Comun;
using ComunSUBE;
using LibQPA.ProveedoresHistoricos.DbXBus;
using LibQPA.ProveedoresHistoricos.JsonSUBE;
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

        List<PuntoHistorico> Desfasar(int segundosDesfase, List<PuntoHistorico> puntoHistoricos)
        {
            return puntoHistoricos
                .Select(ph => new PuntoHistorico { 
                    Lat   = ph.Lat, 
                    Lng   = ph.Lng, 
                    Alt   = ph.Alt, 
                    Fecha = ph.Fecha.AddSeconds(segundosDesfase) 
                })
                .ToList()
            ;
        }

        private double MedirDistancia(List<PuntoHistorico> puntoHistoricos)
        {
            if (!puntoHistoricos.Any())
            {
                return 0.0;
            }
            var dist = 0.0;
            var anterior = puntoHistoricos.First();
            foreach (var pth in puntoHistoricos.Skip(1))
            {
                dist += Haversine.GetDist(anterior, pth);
                anterior = pth;
            }
            return dist;
        }

        public enum ProveedorKey
        { 
            DbSUBE,
            DbXBus,
            JsonSUBE,
        }

        [TestMethod]
        public void TestArch()
        {
            // fechas desde hasta
            var desde = new DateTime(2021, 09, 11, 23, 55, 00);
            var hasta = new DateTime(2021, 09, 12, 00, 05, 00);

            var proveedorPtsHistoJsonSUBE = new ProveedoresHistoricos.JsonSUBE.ProveedorHistoricoJsonSUBE
            {
                InputDir = @"D:\EstadosCoches\Agency49\",
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            var pepe = proveedorPtsHistoJsonSUBE.Get();
            var foo = 0;
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
            //var desde = new DateTime(2021, 09, 7, 00, 00, 00);
            //var hasta = new DateTime(2021, 09, 8, 00, 00, 00);
            var desde = new DateTime(2021, 10, 14, 00, 00, 00);
            var hasta = new DateTime(2021, 10, 15, 00, 00, 00);

            // códigos de las líneas para este cálculo
            var lineasPosibles = new int[] { 159, 163 };

            // tipos de coches
            var tipoCoches = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS;

            // proveedor key
            var proveedorKey = ProveedorKey.JsonSUBE;

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
            var llala = puntasNombradas.ToList();

            // caminos de los recos, es un diccionario:
            // ----------------------------------------
            //  -de clave tiene el patrón de recorrido
            //  -de valor tiene una lista de pares (linea, bandera) que son las banderas que encajan con ese patrón
            var recoPatterns = new Dictionary<string, List<KeyValuePair<int, int>>>();
            var llamadas = 0;
            foreach (var recox in recorridosTeoricos)
            {
                llamadas = Haversine.GetLlamadas();
                var fr1 = 0;

                // creo un camino (su descripción es la clave)
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntasNombradas, recox.Puntos);

                llamadas = Haversine.GetLlamadas();
                var fr2 = 0;

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

            /////////////////////////////////////////////////////////////////////
            //// Puntos históricos XBus
            /////////////////////////////////////////////////////////////////////
            //Dictionary<string, List<PuntoHistorico>> ptsHistoXBusPorFicha;
            //const string ARCHIVO_XBUS = "PtsHistoXBus.json";

            //if (File.Exists(ARCHIVO_XBUS))
            //{
            //    var json = File.ReadAllText(ARCHIVO_XBUS);
            //    ptsHistoXBusPorFicha = JsonConvert.DeserializeObject<Dictionary<string, List<PuntoHistorico>>>(json);
            //}
            //else
            //{
            //    var proveedorPtsHistoXBus = new ProveedorHistoricoDbXBus(
            //        new ProveedorHistoricoDbXBus.Configuracion
            //        {
            //            CommandTimeout = 600,
            //            ConnectionString = Configu.ConnectionStringPuntosXBus,
            //            Tipo = tipoCoches,
            //            FechaDesde = desde,
            //            FechaHasta = hasta,
            //        });
            //    ptsHistoXBusPorFicha = proveedorPtsHistoXBus.Get();
            //    File.WriteAllText(
            //        ARCHIVO_XBUS,
            //        JsonConvert.SerializeObject(ptsHistoXBusPorFicha)
            //    );
            //}
            //var fichasXBus = ptsHistoXBusPorFicha.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos SUBE
            ///////////////////////////////////////////////////////////////////
            
            Dictionary<string, List<PuntoHistorico>> ptsHistoSUBEPorIdent = null;
            string ARCHIVO_PUNTOS_SUBE = $"__pts{proveedorKey}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json";

            if (File.Exists(ARCHIVO_PUNTOS_SUBE))
            {
                var json = File.ReadAllText(ARCHIVO_PUNTOS_SUBE);
                ptsHistoSUBEPorIdent = JsonConvert.DeserializeObject<Dictionary<string, List<PuntoHistorico>>>(json);
            }
            else if (proveedorKey == ProveedorKey.DbSUBE)
            {
                var proveedorPtsHistoDbSUBE = new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE(
                    new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE.Configuracion
                    {
                        CommandTimeout = 600,
                        ConnectionStringPuntos = Configu.ConnectionStringPuntosSUBE,
                        //DatosEmpIntFicha = datosEmpIntFicha,
                        FechaDesde = desde,
                        FechaHasta = hasta,
                    });
                
                ptsHistoSUBEPorIdent = proveedorPtsHistoDbSUBE.Get();

                File.WriteAllText(
                    ARCHIVO_PUNTOS_SUBE,
                    JsonConvert.SerializeObject(ptsHistoSUBEPorIdent)
                );
            }
            else if (proveedorKey == ProveedorKey.JsonSUBE)
            { 
                var proveedorPtsHistoJsonSUBE = new ProveedoresHistoricos.JsonSUBE.ProveedorHistoricoJsonSUBE
                { 
                    InputDir    = @"D:\EstadosCoches\Agency49\",
                    FechaDesde  = desde,
                    FechaHasta  = hasta,
                };

                ptsHistoSUBEPorIdent = proveedorPtsHistoJsonSUBE.Get();

                File.WriteAllText(
                    ARCHIVO_PUNTOS_SUBE,
                    JsonConvert.SerializeObject(ptsHistoSUBEPorIdent)
                );
            }

            //if (proveedorKey == ProveedorKey.DbSUBE)
            //{
            //    if (File.Exists(ARCHIVO_PUNTOS_SUBE))
            //    {
            //        var json = File.ReadAllText(ARCHIVO_PUNTOS_SUBE);
            //        ptsHistoSUBEPorIdent = JsonConvert.DeserializeObject<Dictionary<string, List<PuntoHistorico>>>(json);
            //    }
            //    else
            //    {
            //        var proveedorPtsHistoDbSUBE = new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE(
            //        new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE.Configuracion
            //        {
            //            CommandTimeout = 600,
            //            ConnectionStringPuntos = Configu.ConnectionStringPuntosSUBE,
            //            //DatosEmpIntFicha = datosEmpIntFicha,
            //            FechaDesde = desde,
            //            FechaHasta = hasta,
            //        });
            //        ptsHistoSUBEPorIdent = proveedorPtsHistoDbSUBE.Get();
            //        File.WriteAllText(
            //            ARCHIVO_PUNTOS_SUBE,
            //            JsonConvert.SerializeObject(ptsHistoSUBEPorIdent)
            //        );
            //    }
            //}
            //else if (proveedorKey == ProveedorKey.JsonSUBE)
            //{
            //    if (File.Exists(ARCHIVO_PUNTOS_SUBE))
            //    {
            //        var json = File.ReadAllText(ARCHIVO_PUNTOS_SUBE);
            //        ptsHistoSUBEPorIdent = JsonConvert.DeserializeObject<Dictionary<string, List<PuntoHistorico>>>(json);
            //    }
            //    else
            //    { 
                    
            //    }
            //}

            var fichasSUBE = ptsHistoSUBEPorIdent
                .Keys
                .Select(ident => datosEmpIntFicha.GetFicha(ident))
                .ToList()
            ;

            /////////////////////////////////////////////////////////////////////
            //// Puntos históricos Suma (XBus + SUBE)
            /////////////////////////////////////////////////////////////////////
            //var ptsHistoSumaPorFicha = FusionarDiccionarios(
            //    ptsHistoXBusPorFicha, 
            //    ptsHistoSUBEPorFicha
            //);
            //var fichasSuma = ptsHistoSumaPorFicha.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////
            Dictionary<int, List<BoletoComun>> boletosXFicha;
            string ARCHIVO_BOLETOS = $"__boletosSUBE__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json";
            var proveedorVentaBoletosConfig = new ProveedorVentaBoletosDbSUBE.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionStringVentasSUBE,
                DatosEmpIntFicha = datosEmpIntFicha,
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            ProveedorVentaBoletosDbSUBE proveedorVentaBoletos;

            if (File.Exists(ARCHIVO_BOLETOS))
            {
                var json = File.ReadAllText(ARCHIVO_BOLETOS);
                boletosXFicha = JsonConvert.DeserializeObject<Dictionary<int, List<BoletoComun>>>(json);
                proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(
                    proveedorVentaBoletosConfig,
                    boletosXFicha
                );
            }
            else
            {
                proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(proveedorVentaBoletosConfig);
                proveedorVentaBoletos.TieneBoletosEnIntervalo(0, DateTime.Now, DateTime.Now);
                boletosXFicha = proveedorVentaBoletos.BoletosXIdentificador;
                File.WriteAllText(
                    ARCHIVO_BOLETOS,
                    JsonConvert.SerializeObject(boletosXFicha)
                );
            }

            ///////////////////////////////////////////////////////////////////
            // Procesamiento de los datos (para todas las fichas)
            ///////////////////////////////////////////////////////////////////

            //var (resultadosSUBE, resulFichasSUBE) = ProcesarTodo(
            var resultadosSUBE = ProcesarTodo(
                recorridosTeoricos, topes2D, puntasNombradas.Select(pu => (IPuntaLinea)pu).ToList(), recoPatterns, 
                ptsHistoSUBEPorIdent /*,fichasSUBE*/
            );

            var resulIdents = resultadosSUBE
                .Select(result => result.Identificador)
            ;
            
            var resulFichasSUBE = resulIdents
                .Select(ident => datosEmpIntFicha.GetFicha(ident, '-', -1))
                .ToList()
            ;

            //PonerResultadosEnUnArchivo(
            //    "SUBE_NUEVO_80", 
            //    resultadosSUBE, resulFichasSUBE, proveedorVentaBoletos, 
            //    (ficha, resultado) => resultado.PorcentajeReconocido >= 80
            //);

            //PonerResultadosEnUnArchivo(
            //    "SUBE_TODO_00",
            //    resultadosSUBE, resulFichasSUBE, proveedorVentaBoletos,
            //    (ficha, resultado) => resultado.PorcentajeReconocido >= 0
            //);

            Dictionary<int, (int, int)> fichasXEmpIntSUBE = datosEmpIntFicha
                .Get()
                .ToDictionary(x => x.Value, x => x.Key)
            ;

            var reporte = new CSVReport()
            {
                UsesHeader      = true,
                Separator       = ';',
                HeaderBuilder   = (sep) => string.Join(sep, new[] { "empresaSUBE", "internoSUBE", "ficha", "linea", "bandera", "inicio", "fin", "cantbol", "cantbolopt" }),
                ItemsBuilder    = (sep) => CrearItemsCSV(sep, resultadosSUBE, resulFichasSUBE, fichasXEmpIntSUBE, proveedorVentaBoletos)
            };

            File.WriteAllText($"ABCZ__CSV_{proveedorKey}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.txt", reporte.ToString());
            llamadas = Haversine.GetLlamadas();
            int foo = 0;
        }

        [TestMethod]
        public void TestQPA2()
        {
            ///////////////////////////////////////////////////////////////////
            // variables de entrada
            ///////////////////////////////////////////////////////////////////

            // granularidad del cálculo
            int granularidadMts = 20;

            // radio de puntas de línea
            int radioPuntasDeLineaMts = 200;

            // fechas desde hasta
            //var desde = new DateTime(2021, 09, 7, 00, 00, 00);
            //var hasta = new DateTime(2021, 09, 8, 00, 00, 00);
            var desde = new DateTime(2021, 10, 31, 00, 00, 00);
            var hasta = new DateTime(2021, 11, 01, 00, 00, 00);
            // 3087 everywhere

            // códigos de las líneas para este cálculo
            var lineasPosibles = new int[] { 165, 166, 167 };

            // tipos de coches
            // var tipoCoches = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS;

            // proveedor key
            var proveedorKey = ProveedorKey.JsonSUBE;

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
            var puntasNombradas = PuntasDeLinea2
                .GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts)
                .ToList()
            ;

            // caminos de los recos, es un diccionario:
            // ----------------------------------------
            //  -de clave tiene el patrón de recorrido
            //  -de valor tiene una lista de pares (linea, bandera) que son las banderas que encajan con ese patrón
            var recoPatterns = new Dictionary<string, List<KeyValuePair<int, int>>>();
            foreach (var recox in recorridosTeoricos)
            {
                // creo un camino (su descripción es la clave)
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntasNombradas, recox.Puntos);

                //if (camino.Description.Length == 1)
                //    continue;

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

            /////////////////////////////////////////////////////////////////////
            //// Puntos históricos XBus
            /////////////////////////////////////////////////////////////////////
            //Dictionary<string, List<PuntoHistorico>> ptsHistoXBusPorFicha;
            //const string ARCHIVO_XBUS = "PtsHistoXBus.json";

            //if (File.Exists(ARCHIVO_XBUS))
            //{
            //    var json = File.ReadAllText(ARCHIVO_XBUS);
            //    ptsHistoXBusPorFicha = JsonConvert.DeserializeObject<Dictionary<string, List<PuntoHistorico>>>(json);
            //}
            //else
            //{
            //    var proveedorPtsHistoXBus = new ProveedorHistoricoDbXBus(
            //        new ProveedorHistoricoDbXBus.Configuracion
            //        {
            //            CommandTimeout = 600,
            //            ConnectionString = Configu.ConnectionStringPuntosXBus,
            //            Tipo = tipoCoches,
            //            FechaDesde = desde,
            //            FechaHasta = hasta,
            //        });
            //    ptsHistoXBusPorFicha = proveedorPtsHistoXBus.Get();
            //    File.WriteAllText(
            //        ARCHIVO_XBUS,
            //        JsonConvert.SerializeObject(ptsHistoXBusPorFicha)
            //    );
            //}
            //var fichasXBus = ptsHistoXBusPorFicha.Keys.ToList();

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos SUBE
            ///////////////////////////////////////////////////////////////////

            Dictionary<string, List<PuntoHistorico>> ptsHistoSUBEPorIdent = null;
            string ARCHIVO_PUNTOS_SUBE = $"__pts{proveedorKey}__agn132__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json";

            if (File.Exists(ARCHIVO_PUNTOS_SUBE))
            {
                var json = File.ReadAllText(ARCHIVO_PUNTOS_SUBE);
                ptsHistoSUBEPorIdent = JsonConvert.DeserializeObject<Dictionary<string, List<PuntoHistorico>>>(json);
            }
            else if (proveedorKey == ProveedorKey.DbSUBE)
            {
                var proveedorPtsHistoDbSUBE = new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE(
                    new ProveedoresHistoricos.DbSUBE.ProveedorHistoricoDbSUBE.Configuracion
                    {
                        CommandTimeout = 600,
                        ConnectionStringPuntos = Configu.ConnectionStringPuntosSUBE,
                        //DatosEmpIntFicha = datosEmpIntFicha,
                        FechaDesde = desde,
                        FechaHasta = hasta,
                    });

                ptsHistoSUBEPorIdent = proveedorPtsHistoDbSUBE.Get();

                File.WriteAllText(
                    ARCHIVO_PUNTOS_SUBE,
                    JsonConvert.SerializeObject(ptsHistoSUBEPorIdent)
                );
            }
            else if (proveedorKey == ProveedorKey.JsonSUBE)
            {
                var proveedorPtsHistoJsonSUBE = new ProveedoresHistoricos.JsonSUBE.ProveedorHistoricoJsonSUBE
                {
                    InputDir = @"D:\EstadosCoches\Agency132\",
                    FechaDesde = desde,
                    FechaHasta = hasta,
                };

                ptsHistoSUBEPorIdent = proveedorPtsHistoJsonSUBE.Get();

                File.WriteAllText(
                    ARCHIVO_PUNTOS_SUBE,
                    JsonConvert.SerializeObject(ptsHistoSUBEPorIdent)
                );
            }

            var fichasSUBE = ptsHistoSUBEPorIdent
                .Keys
                .Select(ident => datosEmpIntFicha.GetFicha(ident))
                .ToList()
            ;

            ///////////////////////////////////////////////////////////////////
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////
            Dictionary<int, List<BoletoComun>> boletosXFicha;
            string ARCHIVO_BOLETOS = $"__boletosSUBE__agn132__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json";
            var proveedorVentaBoletosConfig = new ProveedorVentaBoletosDbSUBE.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionStringVentasSUBE,
                DatosEmpIntFicha = datosEmpIntFicha,
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            ProveedorVentaBoletosDbSUBE proveedorVentaBoletos;

            if (File.Exists(ARCHIVO_BOLETOS))
            {
                var json = File.ReadAllText(ARCHIVO_BOLETOS);
                boletosXFicha = JsonConvert.DeserializeObject<Dictionary<int, List<BoletoComun>>>(json);
                proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(
                    proveedorVentaBoletosConfig,
                    boletosXFicha
                );
            }
            else
            {
                proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(proveedorVentaBoletosConfig);
                proveedorVentaBoletos.TieneBoletosEnIntervalo(0, DateTime.Now, DateTime.Now); // esto solo es para inicializar
                boletosXFicha = proveedorVentaBoletos.BoletosXIdentificador;
                File.WriteAllText(
                    ARCHIVO_BOLETOS,
                    JsonConvert.SerializeObject(boletosXFicha)
                );
            }

            ///////////////////////////////////////////////////////////////////
            // Procesamiento de los datos (para todas las fichas)
            ///////////////////////////////////////////////////////////////////

            //var (resultadosSUBE, resulFichasSUBE) = ProcesarTodo(
            var resultadosSUBE = ProcesarTodo(
                recorridosTeoricos, topes2D, puntasNombradas.Select(pu => (IPuntaLinea)pu).ToList(), recoPatterns,
                ptsHistoSUBEPorIdent /*,fichasSUBE*/
            );

            var resulIdents = resultadosSUBE
                .Select(result => result.Identificador)
            ;

            var resulFichasSUBE = resulIdents
                .Select(ident => datosEmpIntFicha.GetFicha(ident, '-', -1))
                .ToList()
            ;

            //PonerResultadosEnUnArchivo(
            //    "SUBE_NUEVO_80", 
            //    resultadosSUBE, resulFichasSUBE, proveedorVentaBoletos, 
            //    (ficha, resultado) => resultado.PorcentajeReconocido >= 80
            //);

            //PonerResultadosEnUnArchivo(
            //    "SUBE_TODO_00",
            //    resultadosSUBE, resulFichasSUBE, proveedorVentaBoletos,
            //    (ficha, resultado) => resultado.PorcentajeReconocido >= 0
            //);

            Dictionary<int, (int, int)> fichasXEmpIntSUBE = datosEmpIntFicha
                .Get()
                .ToDictionary(x => x.Value, x => x.Key)
            ;

            var reporte = new CSVReport()
            {
                UsesHeader = true,
                Separator = ';',
                HeaderBuilder = (sep) => string.Join(sep, new[] { "empresaSUBE", "internoSUBE", "ficha", "linea", "bandera", "inicio", "fin", "cantbol", "cantbolopt" }),
                ItemsBuilder = (sep) => CrearItemsCSV(sep, resultadosSUBE, resulFichasSUBE, fichasXEmpIntSUBE, proveedorVentaBoletos)
            };

            File.WriteAllText($"GBOURG__CSV_{proveedorKey}__agn132__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.txt", reporte.ToString());

            int foo = 0;
        }

        IEnumerable<string> CrearItemsCSV(char sep, List<QPAResult> resultadosSUBE, List<int> fichasSUBE, Dictionary<int, (int, int)> fichasXEmpIntSUBE, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos)
        {
            int count = 0;
            for (int i = 0; i < resultadosSUBE.Count; i++)
            {
                if (resultadosSUBE[i].PorcentajeReconocido >= 80)
                {
                    count++;
                    var itemCSV = CrearItemCSV(sep, resultadosSUBE[i], fichasSUBE[i], fichasXEmpIntSUBE, proveedorVentaBoletos);

                    if (!string.IsNullOrEmpty(itemCSV))
                    {
                        yield return itemCSV.Trim();
                    }
                }
            }
            int foo = 0;
        }

        string CrearItemCSV(char sep, QPAResult qPAResult, int ficha, Dictionary<int, (int, int)> fichasXEmpIntSUBE, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos)
        {
            if (ficha == -1)
            {
                return "";
            }

            // OJO! esta func crea varios renglones CSV... uno para cada SubCamino (bandera) reconocido

            var sbRenglones = new StringBuilder();
            QPASubCamino subCaminoAnterior = null;

            foreach (var subCamino in qPAResult.SubCaminos)
            {
                // empresa e ident sube...
                var empresaSUBE = fichasXEmpIntSUBE[ficha].Item1;
                var internoSUBE = fichasXEmpIntSUBE[ficha].Item2;

                // linea y bandera
                var linBanPuns = subCamino
                    .LineasBanderasPuntuaciones
                    .OrderByDescending(lbp => lbp.Puntuacion)
                    .ToList()
                ;
                var linea   = linBanPuns[0].Linea;
                var bandera = linBanPuns[0].Bandera;

                // inicio y fin
                var inicio  = subCamino.HoraComienzo.ToString("dd/MM/yyyy HH:mm:ss");
                var fin     = subCamino.HoraFin.ToString("dd/MM/yyyy HH:mm:ss");

                // cant de boletos (naive)
                var cantBoletosNaive = proveedorVentaBoletos
                    .GetBoletosEnIntervalo(ficha, subCamino.HoraComienzo, subCamino.HoraFin)
                    .Count()
                ;

                // cant de boletos (optimizada)
                var horaComienzoBoletos = subCaminoAnterior == null ?
                    subCamino.HoraComienzo :
                    subCaminoAnterior.HoraFin
                ;
                var horaFinBoletos = subCamino.HoraFin;
                var cantBoletosOpt = proveedorVentaBoletos
                    .GetBoletosEnIntervalo(ficha, horaComienzoBoletos, horaFinBoletos)
                    .Count()
                ;

                // construcción del renglón
                var valores = new object[] { empresaSUBE, internoSUBE, ficha, linea, bandera, inicio, fin, cantBoletosNaive, cantBoletosOpt };
                var renglon = string.Join(sep, valores);

                // acum renglón
                sbRenglones.AppendLine(renglon);

                // guardo el subCamino actual en la variable "anterior"
                // esto sirve para procesar mas boletos...
                subCaminoAnterior = subCamino;
            }

            var ret = sbRenglones.ToString();
            return ret;
        }

        List<PuntoHistorico> FusionarPuntos(params List<PuntoHistorico>[] ptss)
        {
            var ret = new List<PuntoHistorico>();

            foreach (var pts in ptss)
            {
                ret.AddRange(pts);
            }

            return ret.OrderBy(ph => ph.Fecha).ToList();
        }

        Dictionary<int, List<PuntoHistorico>> FusionarDiccionarios(params Dictionary<int, List<PuntoHistorico>> [] ptss)
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

        private List<QPAResult> ProcesarTodo(
            List<RecorridoLinBan>                               recorridosTeoricos,
            Topes2D                                             topes2D,
            List<IPuntaLinea>                                   puntasNombradas,
            Dictionary<string, List<KeyValuePair<int, int>>>    recoPatterns,
            Dictionary<string, List<PuntoHistorico>>            puntosHistoricosPorIdent //,
            //List<int>                                           fichas
        )
        {
            var qpaProcessor= new QPAProcessor();
            var resultados  = new List<QPAResult>();

            //foreach (var ficha in fichas.OrderBy(f => f))
            foreach (var ident in puntosHistoricosPorIdent.Keys)
            {
                try
                {
                    var res = qpaProcessor.Procesar(
                        identificador       : ident,
                        recorridosTeoricos  : recorridosTeoricos,
                        puntosHistoricos    : puntosHistoricosPorIdent[ident],
                        topes2D             : topes2D,
                        puntasNombradas     : puntasNombradas,
                        recoPatterns        : recoPatterns
                    );

                    resultados.Add(res);

                }
                catch (Exception exx)
                {
                    int foofoo = 0;
                }
            }

            //return (resultados, resulfichas);
            return resultados;
        }

        private void PonerResultadosEnUnArchivo(string nombreSinExtension, List<QPAResult> resultados, List<int> fichas, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, Func<int, QPAResult, bool> filtroFichaResultado)
        {
            var lstTextosPlanos = new List<string>();
            var lstTextosCSV = new List<string>();
            //3199;163;2769;2021/09/07 05:49:38;2021/09/07 07:33:10;50;
            lstTextosCSV.Add("ficha;linea;bandera;inicio;fin;cantbol");

            for (int i = 0; i < resultados.Count; i++)
            {
                var resultado = resultados[i];
                int ficha = fichas[i];

                //if (resultado.PorcentajeReconocido < 80) 
                if (!filtroFichaResultado(ficha, resultado))
                { 
                    continue;
                }
                
                var textoPlano  = CrearTextoPlano   (resultado, proveedorVentaBoletos, ficha);
                var textoCSV    = CrearTextoCSV     (resultado, proveedorVentaBoletos, ficha).TrimEnd();

                lstTextosPlanos.Add(textoPlano);
                lstTextosCSV.Add(textoCSV);
            }

            File.WriteAllLines(nombreSinExtension + ".txt", lstTextosPlanos .ToArray());
            File.WriteAllLines(nombreSinExtension + ".csv", lstTextosCSV    .ToArray());
        }

        private string CrearTextoCSV(QPAResult resultado, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, int ficha, char sep = ';')
        {
            var sbRenglones = new StringBuilder();

            foreach (var subCamino in resultado.SubCaminos)
            {
                var linBanPuns = subCamino
                    .LineasBanderasPuntuaciones
                    .OrderBy(lbp => lbp.Puntuacion)
                    .ToList ()
                ;
                var linea   = linBanPuns[0].Linea;
                var bandera = linBanPuns[0].Bandera;
                var inicio  = subCamino.HoraComienzo.ToString("dd/MM/yyyy HH:mm:ss");
                var fin     = subCamino.HoraFin.ToString("dd/MM/yyyy HH:mm:ss");
                var cantBoletos = proveedorVentaBoletos
                    .GetBoletosEnIntervalo(ficha, subCamino.HoraComienzo, subCamino.HoraFin)
                    .Count  ()
                ;
                var renglon = $"{ficha}{sep}{linea}{sep}{bandera}{sep}{inicio}{sep}{fin}{sep}{cantBoletos}";
                sbRenglones.AppendLine(renglon);
            }

            var ret = sbRenglones.ToString();
            return ret;
        }

        private string CrearTextoPlano(QPAResult resultado, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, int ficha)
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
