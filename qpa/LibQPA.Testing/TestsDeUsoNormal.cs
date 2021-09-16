using Comun;
using LibQPA.ProveedoresHistoricos.DbXBus;
using LibQPA.ProveedoresTecnobus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var config = new ProveedorHistoricoDbXBus.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionString,
                Tipo = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS,
            };

            var proveedor = new ProveedorHistoricoDbXBus(config);

            var hoy = DateTime.Today;
            var ayer= hoy.AddDays(-1);
            var fichas = proveedor.Get(ayer, hoy).Keys.ToList();

            int foo = 0;
        }

        [TestMethod]
        public void TestQPA1()
        {
            // granularidad del cálculo
            int granularidadMts = 20;

            // radio de puntas de línea
            int radioPuntasDeLineaMts = 800;

            // fechas desde hasta
            //var desde = new DateTime(2021, 09, 10, 00, 00, 00);
            //var hasta = new DateTime(2021, 09, 11, 00, 00, 00);
            var desde = new DateTime(2021, 09, 14, 00, 00, 00);
            var hasta = new DateTime(2021, 09, 15, 00, 00, 00);

            // códigos de las líneas para este cálculo
            var lineasPosibles = new int[] { 159, 163 };

            // tipos de coches
            var tipoCoches = ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS;

            // recorridos teóricos
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

            // puntos históricos
            var proveedorPuntosHistoricos = new ProveedorHistoricoDbXBus(
                new ProveedorHistoricoDbXBus.Configuracion {
                    CommandTimeout  = 600,
                    ConnectionString= Configu.ConnectionString,
                    Tipo            = tipoCoches
                });

            var todosLosPuntosHistoricos = proveedorPuntosHistoricos.Get(desde, hasta);
            var fichas = todosLosPuntosHistoricos.Keys.ToList();

            // procesamiento de los datos...
            var qpaProcessor = new QPAProcessor();
            var resultados = new List<QPAResult>();
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

            int foo = 0;
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
