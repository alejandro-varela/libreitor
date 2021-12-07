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
            var desde = new DateTime(2021, 10, 01, 00, 00, 00);
            var hasta = new DateTime(2021, 10, 02, 00, 00, 00);

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

                // TODO: hay que ver el "porqué"
                // elimino los patrones de recorrido que tengan un solo char, ej. "A"
                // ya que estos patrones producen duraciones negativas
                if (camino.Description.Length == 1)
                {
                    continue;
                }

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

        [DataTestMethod]
        [DataRow("AGN132", "2021-10-01", "2021-10-02", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-02", "2021-10-03", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-03", "2021-10-04", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-04", "2021-10-05", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-05", "2021-10-06", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-06", "2021-10-07", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-07", "2021-10-08", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-08", "2021-10-09", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-09", "2021-10-10", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-10", "2021-10-11", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-11", "2021-10-12", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-12", "2021-10-13", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-13", "2021-10-14", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-14", "2021-10-15", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-15", "2021-10-16", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-16", "2021-10-17", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-17", "2021-10-18", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-18", "2021-10-19", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-19", "2021-10-20", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-20", "2021-10-21", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-21", "2021-10-22", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-22", "2021-10-23", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-23", "2021-10-24", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-24", "2021-10-25", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-25", "2021-10-26", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-26", "2021-10-27", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-27", "2021-10-28", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-28", "2021-10-29", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-29", "2021-10-30", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-30", "2021-10-31", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        [DataRow("AGN132", "2021-10-31", "2021-11-01", "165,166,167", @"D:\EstadosCoches\Agency132\", typeof(PuntaLinea2), 20, 200)]
        /*
        [DataRow("AGN49" , "2021-10-01", "2021-10-02", "159,163"    , @"D:\EstadosCoches\Agency49\" , typeof(PuntaLinea) , 20, 800)]
        */
        public void TestQPA3(
            string identificador,
            string desdeISO8601,
            string hastaISO8601,
            string lineasPosiblesSeparadasPorComa,
            string jsonInputDir,
            Type tipoPuntaLinea,
            int granularidadMts = 20,
            int radioPuntasDeLineaMts = 200
        )
        {
            var desde = DateTime.Parse(desdeISO8601);
            var hasta = DateTime.Parse(hastaISO8601);
            var lineasPosibles = lineasPosiblesSeparadasPorComa
                .Split(',')
                .Select(s => int.Parse(s))
                .ToArray()
            ;

            Assert.IsTrue(hasta.Subtract(desde).TotalDays == 1);
            Assert.IsTrue(hasta > desde);

            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas = null;

            if (tipoPuntaLinea == typeof(PuntaLinea))
            {
                creadorPuntasNombradas = recorridosTeoricos => 
                    PuntasDeLinea
                        .GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts)
                        .Select(pulin => (IPuntaLinea)pulin)
                        .ToList()
                    ;
            }
            else if (tipoPuntaLinea == typeof(PuntaLinea2))
            {
                creadorPuntasNombradas = recorridosTeoricos => 
                    PuntasDeLinea2
                        .GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts, radioPuntasDeLineaMts * 2)
                        .Select(pulin => (IPuntaLinea)pulin)
                        .ToList()
                    ;
            }
            
            CalcularQPA_GenerarReporteQPA_SUBE(
                identificador,
                desde,
                hasta,
                lineasPosibles,
                jsonInputDir,
                creadorPuntasNombradas,
                granularidadMts
            );
        }

        public void CalcularQPA_GenerarReporteQPA_SUBE(
            string      identificador,
            DateTime    desde,
            DateTime    hasta,
            int[]       lineasPosibles,
            string      jsonInputDir, // @"D:\EstadosCoches\Agency132\"
            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas,
            int         granularidadMts = 20
        )
        {
            var resultadosSUBE = CalcularQPA_SUBE(
                identificador,
                desde,
                hasta,
                lineasPosibles,
                jsonInputDir,
                creadorPuntasNombradas,
                granularidadMts
            );

            GenerarReporteQPA(
                identificador, 
                desde, 
                hasta, 
                resultadosSUBE
            );
        }

        public void GenerarReporteQPA(
            string          identificador,
            DateTime        desde,
            DateTime        hasta,
            List<QPAResult> resultadosSUBE
        )
        {
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
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////
            Dictionary<int, List<BoletoComun>> boletosXFicha;
            
            string ARCHIVO_BOLETOS = $"Boletos__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json";

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
                    JsonConvert.SerializeObject(boletosXFicha, Formatting.Indented)
                );
            }

            var resulFichasSUBE = resultadosSUBE
                .Select(resul => resul.Identificador)
                .Select(ident => datosEmpIntFicha.GetFicha(ident, '-', -1))
                .ToList()
            ;

            Dictionary<int, (int, int)> fichasXEmpIntSUBE = datosEmpIntFicha
                .Get()
                .ToDictionary(x => x.Value, x => x.Key)
            ;

            var reporte = new CSVReport()
            {
                UsesHeader = true,
                Separator = ';',
                HeaderBuilder = (sep) => string.Join(sep, new[] { "empresaSUBE", "internoSUBE", "ficha", "linea", "bandera", "inicio", "fin", "cantbol", "cantbolopt" }),
                ItemsBuilder = (sep) => CrearItemsCSV(
                    sep, 
                    resultadosSUBE, 
                    resulFichasSUBE, 
                    fichasXEmpIntSUBE, 
                    proveedorVentaBoletos
                )
            };

            string nombreReporte = $"Reporte__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.txt";
            File.WriteAllText(nombreReporte, reporte.ToString());
        }

        public List<QPAResult> CalcularQPA_SUBE(
            string      identificador,
            DateTime    desde,
            DateTime    hasta,
            int[]       lineasPosibles,
            string      jsonInputDir, // @"D:\EstadosCoches\Agency132\"
            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas,
            int         granularidadMts = 20
        )
        {
            var proveedorRecorridosTeoricos = new ProveedorVersionesTecnobus(dirRepos: DameMockRepos());

            var proveedorPuntosHistoricos = new ProveedoresHistoricos.JsonSUBE.ProveedorHistoricoJsonSUBE
            {
                InputDir = jsonInputDir,
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            var resultadosSUBE = CalcularQPA(
                identificador,
                lineasPosibles,
                desde,
                hasta,
                proveedorRecorridosTeoricos,
                proveedorPuntosHistoricos,
                creadorPuntasNombradas,
                granularidadMts
            );

            return resultadosSUBE;
        }

        public List<QPAResult> CalcularQPA(
            string                          identificador,
            int[]                           lineasPosibles,
            DateTime                        desde,
            DateTime                        hasta,
            IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos,
            IQPAProveedorPuntosHistoricos   proveedorPuntosHistoricos,
            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas,
            int granularidadMts             = 20
        )
        {
            identificador ??= string.Empty;

            #region Recos

            ///////////////////////////////////////////////////////////////////
            // recorridos teóricos / topes / puntas nombradas / recopatterns
            ///////////////////////////////////////////////////////////////////
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
            List<IPuntaLinea> puntasNombradas = creadorPuntasNombradas(recorridosTeoricos);

            // caminos de los recos, es un diccionario:
            // ----------------------------------------
            //  -de clave tiene el patrón de recorrido
            //  -de valor tiene una lista de pares (linea, bandera) que son las banderas que encajan con ese patrón
            var recoPatterns = new Dictionary<string, List<KeyValuePair<int, int>>>();
            foreach (var recox in recorridosTeoricos)
            {
                // creo un camino (su descripción es la clave)
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntasNombradas, recox.Puntos);

                // TODO: hay que ver el "porqué"
                // elimino los patrones de recorrido que tengan un solo char, ej. "A"
                // ya que estos patrones producen duraciones negativas
                if (camino.Description.Length == 1)
                {
                    continue;
                }

                // si la clave no está en el diccionario la agrego...
                if (!recoPatterns.ContainsKey(camino.Description))
                {
                    recoPatterns.Add(camino.Description, new List<KeyValuePair<int, int>>());
                }

                // agrego un par (linea, bandera) a la entrada actual...
                recoPatterns[camino.Description].Add(new KeyValuePair<int, int>(recox.Linea, recox.Bandera));
            }

            #endregion

            #region Puntos Históricos

            ///////////////////////////////////////////////////////////////////
            // Puntos históricos
            ///////////////////////////////////////////////////////////////////

            Dictionary<string, List<PuntoHistorico>> ptsHistoSUBEPorIdent = null;
            string ARCHIVO_PUNTOS_HISTORICOS = $"PtsHist__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json";

            if (File.Exists(ARCHIVO_PUNTOS_HISTORICOS))
            {
                var json = File.ReadAllText(ARCHIVO_PUNTOS_HISTORICOS);
                ptsHistoSUBEPorIdent = JsonConvert.DeserializeObject<Dictionary<string, List<PuntoHistorico>>>(json);
            }
            else
            {
                ptsHistoSUBEPorIdent = proveedorPuntosHistoricos.Get();

                File.WriteAllText(
                    ARCHIVO_PUNTOS_HISTORICOS,
                    JsonConvert.SerializeObject(ptsHistoSUBEPorIdent)
                );
            }

            #endregion

            ///////////////////////////////////////////////////////////////////
            // Procesamiento de los datos (para todas las fichas)
            ///////////////////////////////////////////////////////////////////
            var resultadosSUBE = ProcesarTodo(
                recorridosTeoricos, 
                topes2D, 
                puntasNombradas.Select(pu => (IPuntaLinea)pu).ToList(), 
                recoPatterns,
                ptsHistoSUBEPorIdent
            );

            return resultadosSUBE;
        }

        IEnumerable<string> CrearItemsCSV(
            char                        sep, 
            List<QPAResult>             resultadosSUBE, 
            List<int>                   fichasSUBE, 
            Dictionary<int, (int, int)> fichasXEmpIntSUBE, 
            ProveedorVentaBoletosDbSUBE proveedorVentaBoletos
        )
        {
            var count = 0;
            var dicCasillerosXLinBan = new Dictionary<(int, int), HashSet<Casillero>>();

            for (int i = 0; i < resultadosSUBE.Count; i++)
            {
                var dameCasillerosXLinBan = new FuncCasillerosXLinBan((int linea, int bandera, int granuCustom) =>
                {
                    var key = (linea, bandera);
                    if (!dicCasillerosXLinBan.ContainsKey(key))
                    {
                        var recorrido = resultadosSUBE[i].RecorridosTeoricos
                            .Where(r => r.Linea == linea && r.Bandera == bandera)
                            .First()
                        ;
                        var casilleros = Geom.PuntosAHashSetCasilleros(
                            recorrido.Puntos,
                            granuCustom,
                            resultadosSUBE[i].Topes2D
                        );
                        dicCasillerosXLinBan[key] = casilleros;
                    }
                    return dicCasillerosXLinBan[key];
                });

                if (resultadosSUBE[i].PorcentajeReconocido >= 80)
                {
                    count++;
                    var itemCSV = CrearItemCSV(
                        sep, 
                        resultadosSUBE[i], 
                        fichasSUBE[i], 
                        fichasXEmpIntSUBE, 
                        proveedorVentaBoletos,
                        dameCasillerosXLinBan
                    );

                    if (!string.IsNullOrEmpty(itemCSV))
                    {
                        yield return itemCSV.Trim();
                    }
                }
            }
        }

        delegate HashSet<Casillero> FuncCasillerosXLinBan(int linea, int bandera, int granularidad);

        HashSet<string> _boletosReconocidos = new HashSet<string>();

        string CrearItemCSV(
            char                        sep, 
            QPAResult                   qpaResult, 
            int                         ficha, 
            Dictionary<int, (int, int)> fichasXEmpIntSUBE, 
            ProveedorVentaBoletosDbSUBE proveedorVentaBoletos,
            FuncCasillerosXLinBan       dameCasillerosXLinBan
        )
        {
            // ATENCION esta func crea varios renglones CSV
            // uno para cada SubCamino (bandera) reconocido

            if (ficha == -1)
            {
                return string.Empty;
            }

            var sbRenglones = new StringBuilder();

            foreach (var subCamino in qpaResult.SubCaminos.ToArray().Reverse())
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
                var inicio  = subCamino.HoraSalida .ToString("dd/MM/yyyy HH:mm:ss");
                var fin     = subCamino.HoraLlegada.ToString("dd/MM/yyyy HH:mm:ss");

                // cant de boletos (naive)
                var cantBoletosNaive = proveedorVentaBoletos
                    .GetBoletosEnIntervalo(ficha, subCamino.HoraSalida, subCamino.HoraLlegada)
                    .Count()
                ;

                // cant de boletos (optimizada)
                var horaComienzoBoletos = subCamino.SubCaminoAnterior == null ?
                    subCamino.HoraSalida :
                    subCamino.SubCaminoAnterior.HoraLlegada
                ;
                var horaFinBoletos = subCamino.HoraLlegada;
                
                // BASE DE DATOS DE BOLETOS = DESASTRE
                //  - LA LAT Y LNG INVERTIDA Y ENTERAAAAA    - OK
                //  - BOLETOS QUE TIENEN LAT LNG EN 0        - OK
                //  - ELIMINAR BOLETOS IGUALES               - TODO
                var boletos = proveedorVentaBoletos
                    .GetBoletosEnIntervalo(ficha, horaComienzoBoletos, horaFinBoletos)
                    .ToList()
                ;
                var deltaGran = 2;
                var boletosOpt = new List<BoletoComun>();

                // 1) los boletos que entran en el recorrido
                foreach (var bolx in boletos)
                {
                    bool enReco = Geom.PuntoEnRecorrido(
                        new Punto { Lat = bolx.Latitud, Lng = bolx.Longitud },
                        dameCasillerosXLinBan(linea, bandera, qpaResult.Granularidad * deltaGran),
                        qpaResult.Granularidad * deltaGran,
                        qpaResult.Topes2D
                    );

                    if (enReco && !_boletosReconocidos.Contains(bolx.Id))
                    {
                        boletosOpt.Add(bolx);
                        _boletosReconocidos.Add(bolx.Id);
                    }
                }

                // 2) los boletos que están a un radio "grande" de la salida
                //    esto es, unos 250 mts por lo menos
                var puntoSalida = qpaResult.RecorridosTeoricos
                    .Where(recot => recot.Linea == linea && recot.Bandera == bandera)
                    .First()
                    .Puntos
                    .First()
                ;
                foreach (var bolx in boletos)
                {
                    var puntoBoleto = new Punto { Lat=bolx.Latitud, Lng=bolx.Longitud };

                    if ((Haversine.GetDist(puntoSalida, puntoBoleto) <= 250) &&
                        (!_boletosReconocidos.Contains(bolx.Id)))
                    {
                        boletosOpt.Add(bolx);
                        _boletosReconocidos.Add(bolx.Id);
                    }
                }

                // 3) los boletos lat=0 lng=0 que estén en el intervalo de tiempo
                //    mayor o igual al boleto mas viejo que acabo de calcular
                if (boletosOpt.Any())
                {
                    var fechaMasVieja = boletosOpt
                        .OrderBy(b => b.FechaCancelacion)
                        .First()
                        .FechaCancelacion
                    ;

                    foreach (var bolx in boletos)
                    {
                        if ((bolx.Latitud == 0 || bolx.Longitud == 0) &&
                            bolx.FechaCancelacion >= fechaMasVieja  &&
                            !_boletosReconocidos.Contains(bolx.Id))
                        {
                            boletosOpt.Add(bolx);
                            _boletosReconocidos.Add(bolx.Id);
                        }
                    }
                }

                // 4) ahora, con la fecha original de inicio (subCamino.HoraSalida)
                //    veremos si entran los boletos con lat=0 lng=0 que quedaron afuera
                foreach (var bolx in boletos)
                {
                    if (/*(bolx.Latitud == 0 || bolx.Longitud == 0) &&*/
                        bolx.FechaCancelacion >= subCamino.HoraSalida  &&
                        !_boletosReconocidos.Contains(bolx.Id))
                    {
                        var pepe = linea;
                        var pipo = bandera;
                        boletosOpt.Add(bolx);
                        _boletosReconocidos.Add(bolx.Id);
                    }
                }

                var cantBoletosOpt = boletosOpt.Count;

                //////////////// CALCULO ANTERIOR DE BOLETOS OPTIMIZADOS //////////////
                //var cantBoletosOpt = boletos
                //    .Where(boleto => 
                //        boleto.Latitud  == 0 || 
                //        boleto.Longitud == 0 || 
                //        Geom.PuntoEnRecorrido(
                //            new Punto { Lat = boleto.Latitud, Lng = boleto.Longitud }, 
                //            dameCasillerosXLinBan(linea, bandera, qpaResult.Granularidad * deltaGran), 
                //            qpaResult.Granularidad * deltaGran,
                //            qpaResult.Topes2D
                //        ))
                //    .Count()
                //;
                ///////////////////////////////////////////////////////////////////////

                // construcción del renglón
                var valores = new object[] { empresaSUBE, internoSUBE, ficha, linea, bandera, inicio, fin, cantBoletosNaive, cantBoletosOpt };
                var renglon = string.Join(sep, valores);

                // acum renglón
                // sbRenglones.AppendLine(renglon);
                sbRenglones.Insert(0, renglon + "\r\n");
            }

            var ret = sbRenglones.ToString();
            return ret;
        }

        private List<QPAResult> ProcesarTodo(
            List<RecorridoLinBan>                               recorridosTeoricos,
            Topes2D                                             topes2D,
            List<IPuntaLinea>                                   puntasNombradas,
            Dictionary<string, List<KeyValuePair<int, int>>>    recoPatterns,
            Dictionary<string, List<PuntoHistorico>>            puntosHistoricosPorIdent
        )
        {
            var qpaProcessor= new QPAProcessor();
            var resultados  = new List<QPAResult>();

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

                    // PASARLE AL PROCESAR LOS CRITERIOS...
                    // ¿POR QUÉ ACA Y NO ANTES?
                    // PORQUE ACA TENEMOS CRITERIOS DE USUARIO.

                    // 1) la duración de cada subcamino debe ser positiva
                    // 2) los subcaminos no deben estar superpuestos en el tiempo
                    // 3) la velocidad del subcamino que representa debe ser una velocidad real
                    //      (no demasiado rápida) ej: no puede ser mas de 120-kmh
                    // 4) los puntos deben completar (por lo menos en un 65%) realmente el camino que dicen ser

                    res = VelocidadNormal   (res);
                    res = DuracionPositiva  (res);

                    resultados.Add(res);
                }
                catch (Exception exx)
                {
                    int foofoo = 0;
                }
            }

            return resultados;
        }

        int _veloMal = 0; // generalmente por no sanitizar las lng que vienen mal
        List<double> _velosMal = new List<double>();
        List<double> _velosBien = new List<double>();

        int _duraMal = 0; // evitado por eliminar los patrones con una sola pta de línea
        List<double> _durasMal = new List<double>();
        List<double> _durasBien = new List<double>();

        private QPAResult VelocidadNormal(QPAResult res)
        {
            var newSubCaminos = res.SubCaminos
                .Where(subCamino => 
                    subCamino.VelocidadKmhPromedio <= 120 && 
                    subCamino.VelocidadKmhPromedio >= 5
                )
                .ToList()
            ;

            //if (res.SubCaminos.Count != newSubCaminos.Count)
            //{
            //    var anormales = res.SubCaminos
            //        .Where(subCamino => subCamino.VelocidadKmhPromedio > 120 || subCamino.VelocidadKmhPromedio < 5)
            //        .ToList()
            //    ;
            //    _veloMal += 1;
            //    _velosMal.AddRange(anormales.Select(sc => sc.VelocidadKmhPromedio));
            //    int foo = 0;
            //}
            //else
            //{
            //    _velosBien.AddRange(newSubCaminos.Select(sc => sc.VelocidadKmhPromedio));
            //}

            return new QPAResult
            {
                Granularidad        = res.Granularidad,
                RecorridosTeoricos  = res.RecorridosTeoricos,
                Topes2D             = res.Topes2D,
                Camino              = res.Camino,
                Identificador       = res.Identificador,
                SubCaminos          = newSubCaminos,
            };
        }

        private QPAResult DuracionPositiva(QPAResult res)
        {
            var newSubCaminos = res.SubCaminos
                .Where  (subCamino => subCamino.HoraLlegada > subCamino.HoraSalida)
                .ToList ()
            ;

            //if (res.SubCaminos.Count > newSubCaminos.Count)
            //{
            //    var anormales = res.SubCaminos
            //        .Where(subCamino => subCamino.DuracionHoras < 0)
            //        .ToList()
            //    ;
            //    _duraMal += 1;
            //    _durasMal.AddRange(anormales.Select(sc => sc.DuracionHoras));
            //    int foo = 0;
            //}
            //else
            //{
            //    _durasBien.AddRange(newSubCaminos.Select(sc => sc.DuracionHoras));
            //}

            return new QPAResult
            {
                Granularidad        = res.Granularidad,
                RecorridosTeoricos  = res.RecorridosTeoricos,
                Topes2D             = res.Topes2D,
                Camino              = res.Camino,
                Identificador       = res.Identificador,
                SubCaminos          = newSubCaminos,
            };
        }

        //private void PonerResultadosEnUnArchivo(string nombreSinExtension, List<QPAResult> resultados, List<int> fichas, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, Func<int, QPAResult, bool> filtroFichaResultado)
        //{
        //    var lstTextosPlanos = new List<string>();
        //    var lstTextosCSV = new List<string>();
        //    //3199;163;2769;2021/09/07 05:49:38;2021/09/07 07:33:10;50;
        //    lstTextosCSV.Add("ficha;linea;bandera;inicio;fin;cantbol");

        //    for (int i = 0; i < resultados.Count; i++)
        //    {
        //        var resultado = resultados[i];
        //        int ficha = fichas[i];

        //        //if (resultado.PorcentajeReconocido < 80) 
        //        if (!filtroFichaResultado(ficha, resultado))
        //        { 
        //            continue;
        //        }
                
        //        var textoPlano  = CrearTextoPlano   (resultado, proveedorVentaBoletos, ficha);
        //        var textoCSV    = CrearTextoCSV     (resultado, proveedorVentaBoletos, ficha).TrimEnd();

        //        lstTextosPlanos.Add(textoPlano);
        //        lstTextosCSV.Add(textoCSV);
        //    }

        //    File.WriteAllLines(nombreSinExtension + ".txt", lstTextosPlanos .ToArray());
        //    File.WriteAllLines(nombreSinExtension + ".csv", lstTextosCSV    .ToArray());
        //}

        //private string CrearTextoCSV(QPAResult resultado, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, int ficha, char sep = ';')
        //{
        //    var sbRenglones = new StringBuilder();

        //    foreach (var subCamino in resultado.SubCaminos)
        //    {
        //        var linBanPuns = subCamino
        //            .LineasBanderasPuntuaciones
        //            .OrderBy(lbp => lbp.Puntuacion)
        //            .ToList ()
        //        ;
        //        var linea   = linBanPuns[0].Linea;
        //        var bandera = linBanPuns[0].Bandera;
        //        var inicio  = subCamino.HoraSalida.ToString("dd/MM/yyyy HH:mm:ss");
        //        var fin     = subCamino.HoraLlegada.ToString("dd/MM/yyyy HH:mm:ss");
        //        var cantBoletos = proveedorVentaBoletos
        //            .GetBoletosEnIntervalo(ficha, subCamino.HoraSalida, subCamino.HoraLlegada)
        //            .Count  ()
        //        ;
        //        var renglon = $"{ficha}{sep}{linea}{sep}{bandera}{sep}{inicio}{sep}{fin}{sep}{cantBoletos}";
        //        sbRenglones.AppendLine(renglon);
        //    }

        //    var ret = sbRenglones.ToString();
        //    return ret;
        //}

        //private string CrearTextoPlano(QPAResult resultado, ProveedorVentaBoletosDbSUBE proveedorVentaBoletos, int ficha)
        //{
        //    var sbResult = new StringBuilder();
        //    sbResult.AppendLine(new string('-', 80));
        //    sbResult.AppendLine($"Ficha: {ficha}");
        //    sbResult.AppendLine($"");
        //    foreach (var subCamino in resultado.SubCaminos)
        //    {
        //        foreach (var linBanPun in subCamino.LineasBanderasPuntuaciones)
        //        {
        //            sbResult.AppendLine($"Lin Ban: {linBanPun.Linea} {linBanPun.Bandera}");
        //        }
        //        sbResult.AppendLine($"\tInicio  : {subCamino.HoraSalida}");
        //        sbResult.AppendLine($"\tFin     : {subCamino.HoraLlegada}");

        //        if (proveedorVentaBoletos.TieneBoletosEnIntervalo(ficha, subCamino.HoraSalida, subCamino.HoraLlegada))
        //        {
        //            var sbVentas= new StringBuilder();
        //            var boletos = proveedorVentaBoletos
        //                .GetBoletosEnIntervalo(ficha, subCamino.HoraSalida, subCamino.HoraLlegada)
        //                .ToList();
        //            var fechas  = boletos.Select(bol => bol.FechaCancelacion);
        //            var sFechas = string.Join(',', fechas);
        //            sbVentas.Append($"Tiene {boletos.Count} boletos vendidos en: {sFechas}");
        //            sbResult.AppendLine($"\tVenta   : {sbVentas}");
        //        }
        //        else
        //        {
        //            sbResult.AppendLine($"\tVenta   : Sin datos por parte de archivos SUBE");
        //        }
        //        sbResult.AppendLine($"\tTotMins~: {Convert.ToInt32(subCamino.Duracion.TotalMinutes) }");
        //        sbResult.AppendLine($"");
        //    }
        //    return sbResult.ToString();
        //}

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
