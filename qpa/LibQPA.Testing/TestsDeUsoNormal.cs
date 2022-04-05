// tirar con los dos algoritmos
// 	-algoritmo lineal
// 	-algoritmo de islas
//
// comparar los resultados
//
// si los resultados son menos por parte del algoritmo de islas
//	-miro que banderas faltan
//	-tomo los intervalos de tiempo de las banderas que faltan
//	-examinar las islas, ver si alguna tiene cubierto el recorrido de esa bandera al menos un 90%
//
// (si el resultado es positivo, agregar esa bandera a los resultados "bundle")

using Comun;
using ComunSUBE;
using LibQPA.ProveedoresHistoricos.DbSUBE;
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

        public enum ProveedorKey
        { 
            DbSUBE,
            DbXBus,
            JsonSUBE,
        }

        T MemoizarPorArchivo<T>(string nombreArchivo, Func<T> productor)
        {
            if (File.Exists(nombreArchivo))
            {
                var json = File.ReadAllText(nombreArchivo);
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                var producto = productor();
                var productoSerializado = JsonConvert.SerializeObject(producto, Formatting.Indented);
                File.WriteAllText(nombreArchivo, productoSerializado);
                return producto;
            }
        }

        Dictionary<TIdent, InformacionHistorica> ConvertirPuntosAInformacion<TIdent>(
            Dictionary<TIdent, List<PuntoHistorico>> puntosXIdentificador, 
            CreadorPartesHistoricas creadorPartes
        )
        {
            var ret = new Dictionary<TIdent, InformacionHistorica>();

            foreach (var kvp in puntosXIdentificador)
            {
                var infoHistorica = new InformacionHistorica
                {
                    PuntosCrudos  = kvp.Value,
                    CreadorPartes = creadorPartes,
                };

                ret.Add(kvp.Key, infoHistorica);
            }

            return ret;
        }

        [DataTestMethod]
        //[DataRow("AGN49", "2022-03-01", "2022-03-02", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-02", "2022-03-03", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-03", "2022-03-04", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-04", "2022-03-05", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-05", "2022-03-06", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-06", "2022-03-07", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-07", "2022-03-08", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-08", "2022-03-09", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-09", "2022-03-10", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-10", "2022-03-11", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-11", "2022-03-12", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-12", "2022-03-13", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-13", "2022-03-14", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-14", "2022-03-15", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]
        //[DataRow("AGN49", "2022-03-15", "2022-03-16", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), 20, 800)]

        [DataRow("AGN49", "2022-03-16", "2022-03-17", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-17", "2022-03-18", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-18", "2022-03-19", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-19", "2022-03-20", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-20", "2022-03-21", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-21", "2022-03-22", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-22", "2022-03-23", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-23", "2022-03-24", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-24", "2022-03-25", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-25", "2022-03-26", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-26", "2022-03-27", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-27", "2022-03-28", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-28", "2022-03-29", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-29", "2022-03-30", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-30", "2022-03-31", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]
        [DataRow("AGN49", "2022-03-31", "2022-04-01", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIdentidad), 20, 800)]

        [DataRow("AGN49TMP", "2022-03-16", "2022-03-17", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-17", "2022-03-18", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-18", "2022-03-19", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-19", "2022-03-20", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-20", "2022-03-21", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-21", "2022-03-22", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-22", "2022-03-23", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-23", "2022-03-24", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-24", "2022-03-25", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-25", "2022-03-26", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-26", "2022-03-27", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-27", "2022-03-28", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-28", "2022-03-29", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-29", "2022-03-30", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-30", "2022-03-31", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]
        [DataRow("AGN49TMP", "2022-03-31", "2022-04-01", "159,163", @"D:\ApiSUBE\2022\03\", "49_*", typeof(PuntaLinea), typeof(CreadorPartesHistoricasIslasTemp), 20, 800)]

        //[DataRow("AGN132", "2022-03-01", "2022-03-02", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-02", "2022-03-03", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-03", "2022-03-04", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-04", "2022-03-05", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-05", "2022-03-06", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-06", "2022-03-07", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-07", "2022-03-08", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-08", "2022-03-09", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-09", "2022-03-10", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-10", "2022-03-11", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-11", "2022-03-12", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-12", "2022-03-13", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-13", "2022-03-14", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-14", "2022-03-15", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("AGN132", "2022-03-15", "2022-03-16", "165,166,167", @"D:\ApiSUBE\2022\03\", "132_*", typeof(PuntaLinea2), 20, 150)]

        public void TestJsonSUBEQPA_ConReporte(
            string identificador,
            string desdeISO8601,
            string hastaISO8601,
            string lineasPosiblesSeparadasPorComa,
            string jsonInputDir,
            string zipPrefix, // prefijo del zip
            Type tipoPuntaLinea,
            Type tipoCreadorPartesHistoricas,
            int granularidadMts = 20,
            int radioPuntasDeLineaMts = 200
        )
        {
            var desde = DateTime.Parse(desdeISO8601);
            var hasta = DateTime.Parse(hastaISO8601);

            // Puntos históricos
            var puntosXIdentificador = MemoizarPorArchivo(
                $"PtsHist__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json",
                () => new ProveedorHistoricoJsonSUBE2 { 
                    InputDir = jsonInputDir, 
                    FechaDesde = desde,
                    FechaHasta = hasta,
                    ZipPrefix = zipPrefix,
                    DeleteExtractDirectory = true
                }.Get()
            );

            //// Puntos históricos
            //var puntosXIdentificador = MemoizarPorArchivo(
            //    $"PtsHist__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json",
            //    () => new ProveedorHistoricoJsonSUBE { InputDir = jsonInputDir, FechaDesde = desde, FechaHasta = hasta }.Get()
            //);

            // Ahora convierto puntosXIdentificador en infohXIdentificador
            var infohXIdentificador = ConvertirPuntosAInformacion(
                puntosXIdentificador, 
                Activator.CreateInstance(tipoCreadorPartesHistoricas) as CreadorPartesHistoricas
            );

            // Función local que toma una lista de resultados QPA y los convierte en una lista de Fichas
            List<int> constructorFichas(DatosEmpIntFicha datosEmpIntFicha, List<QPAResult<string>> resultadosQPA) =>
                resultadosQPA
                    .Select(resul => resul.Identificador)
                    .Select(ident => datosEmpIntFicha.GetFicha(ident, '-', -1))
                    .ToList()
                ;

            TestGenericoQPA_ConReporte<string>(
                identificador,
                desdeISO8601,
                hastaISO8601,
                lineasPosiblesSeparadasPorComa,
                infohXIdentificador,
                constructorFichas,
                tipoPuntaLinea, 
                granularidadMts, 
                radioPuntasDeLineaMts
            );
        }

        [DataTestMethod]
        ///////////////////////////////////////////////////////////////////////////////////////
        // enero 2022 grandbourg
        //[DataRow("KMS132", "2022-01-01", "2022-01-02", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-02", "2022-01-03", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-03", "2022-01-04", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-04", "2022-01-05", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-05", "2022-01-06", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-06", "2022-01-07", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-07", "2022-01-08", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-08", "2022-01-09", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-09", "2022-01-10", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-10", "2022-01-11", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-11", "2022-01-12", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-12", "2022-01-13", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-13", "2022-01-14", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-14", "2022-01-15", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-15", "2022-01-16", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-16", "2022-01-17", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-17", "2022-01-18", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-18", "2022-01-19", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-19", "2022-01-20", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-20", "2022-01-21", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-21", "2022-01-22", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-22", "2022-01-23", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-23", "2022-01-24", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-24", "2022-01-25", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-25", "2022-01-26", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-26", "2022-01-27", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-27", "2022-01-28", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-28", "2022-01-29", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-29", "2022-01-30", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-30", "2022-01-31", "165,166,167", typeof(PuntaLinea2), 20, 150)]
        //[DataRow("KMS132", "2022-01-31", "2022-02-01", "165,166,167", typeof(PuntaLinea2), 20, 150)]

        ///////////////////////////////////////////////////////////////////////////////////////
        // enero 2022 línea 203
        //[DataRow("KMS49", "2022-01-01", "2022-01-02", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-02", "2022-01-03", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-03", "2022-01-04", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-04", "2022-01-05", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-05", "2022-01-06", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-06", "2022-01-07", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-07", "2022-01-08", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-08", "2022-01-09", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-09", "2022-01-10", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-10", "2022-01-11", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-11", "2022-01-12", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-12", "2022-01-13", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-13", "2022-01-14", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-14", "2022-01-15", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-15", "2022-01-16", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-16", "2022-01-17", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-17", "2022-01-18", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-18", "2022-01-19", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-19", "2022-01-20", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-20", "2022-01-21", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-21", "2022-01-22", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-22", "2022-01-23", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-23", "2022-01-24", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-24", "2022-01-25", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-25", "2022-01-26", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-26", "2022-01-27", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-27", "2022-01-28", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-28", "2022-01-29", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-29", "2022-01-30", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-30", "2022-01-31", "159,163", typeof(PuntaLinea2), 20, 800)]
        //[DataRow("KMS49", "2022-01-31", "2022-02-01", "159,163", typeof(PuntaLinea2), 20, 800)]

        public void TestKmsSUBEQPA_ConReporte(
            string identificador,
            string desdeISO8601,
            string hastaISO8601,
            string lineasPosiblesSeparadasPorComa,
            Type tipoPuntaLinea,
            int granularidadMts = 20,
            int radioPuntasDeLineaMts = 200)
        {
            var desde = DateTime.Parse(desdeISO8601);
            var hasta = DateTime.Parse(hastaISO8601);

            // Puntos históricos
            // .. Newtonsoft.Json no serializa diccionarios con clave compuesta asi que lo guardo como un arreglo de KeyValuePair
            var arrayKVP = MemoizarPorArchivo(
                $"PtsHist__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json",
                () =>
                {
                    var configProveedorPuntosHistorico = new ProveedorHistoricoDbSUBE.Configuracion
                    {
                        ConnectionStringPuntos = Configu.ConnectionStringVentasSUBE,
                        FechaDesde = desde,
                        FechaHasta = hasta,
                        Transformador = (ident, pts) => pts.Where(p => p.Lat != 0 && p.Lng != 0).OrderBy(p => p.Fecha).ToList()
                    };
                    return new ProveedorHistoricoDbSUBE(configProveedorPuntosHistorico).Get().ToArray();
                }
            );

            // .. Convierto ahora el arreglo de KeyValuePair a un diccionario:
            // .. de tipo [ParEmpresaInterno, List<PuntoHistorico>]
            var puntosXIdentificador = arrayKVP.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Ahora convierto puntosXIdentificador en infohXIdentificador
            var infohXIdentificador = ConvertirPuntosAInformacion(puntosXIdentificador, new CreadorPartesHistoricasIdentidad());

            // Función local que toma una lista de resultados QPA y los convierte en una lista de Fichas
            static List<int> constructorFichas(DatosEmpIntFicha datosEmpIntFicha, List<QPAResult<ParEmpresaInterno>> resultadosQPA) =>
                resultadosQPA
                    .Select(resul => datosEmpIntFicha.GetFicha(resul.Identificador.Empresa, resul.Identificador.Interno))
                    .ToList()
                ;

            TestGenericoQPA_ConReporte(
                identificador,
                desdeISO8601,
                hastaISO8601,
                lineasPosiblesSeparadasPorComa,
                infohXIdentificador,
                constructorFichas,
                tipoPuntaLinea,
                granularidadMts,
                radioPuntasDeLineaMts
            );
        }

        [DataTestMethod]
        //[DataRow("XBUS203", "2021-12-01", "2021-12-02", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-02", "2021-12-03", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-03", "2021-12-04", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-04", "2021-12-05", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-05", "2021-12-06", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-06", "2021-12-07", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-07", "2021-12-08", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-08", "2021-12-09", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-09", "2021-12-10", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-10", "2021-12-11", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-11", "2021-12-12", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-12", "2021-12-13", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-13", "2021-12-14", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-14", "2021-12-15", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-15", "2021-12-16", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-16", "2021-12-17", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-17", "2021-12-18", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-18", "2021-12-19", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-19", "2021-12-20", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-20", "2021-12-21", "159,163", typeof(PuntaLinea), 20, 800)]
        //[DataRow("XBUS203", "2021-12-21", "2021-12-22", "159,163", typeof(PuntaLinea), 20, 800)]
        public void TestDbXBusQPA_ConReporte(
            string identificador,
            string desdeISO8601,
            string hastaISO8601,
            string lineasPosiblesSeparadasPorComa,
            Type tipoPuntaLinea,
            int granularidadMts = 20,
            int radioPuntasDeLineaMts = 200
        )
        {
            var desde = DateTime.Parse(desdeISO8601);
            var hasta = DateTime.Parse(hastaISO8601);

            // Puntos históricos
            var puntosXIdentificador = MemoizarPorArchivo(
                $"PtsHist__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json",
                () =>
                {
                    var config = new ProveedorHistoricoDbXBus.Configuracion
                    { 
                        ConnectionString = Configu.ConnectionStringPuntosXBus, 
                        FechaDesde = desde, 
                        FechaHasta = hasta, 
                        Tipo = ProveedorHistoricoDbXBus.TipoEquipo.TECNOBUS | ProveedorHistoricoDbXBus.TipoEquipo.PICOBUS 
                    };
                    return new ProveedorHistoricoDbXBus(config).Get();
                }
            );

            // Ahora convierto puntosXIdentificador en infohXIdentificador
            var infohXIdentificador = ConvertirPuntosAInformacion(puntosXIdentificador, new CreadorPartesHistoricasIdentidad());

            // Función local que toma una lista de resultados QPA y los convierte en una lista de Fichas
            static List<int> constructorFichas(DatosEmpIntFicha datosEmpIntFicha, List<QPAResult<string>> resultadosQPA) =>
                resultadosQPA
                    .Select(resul => int.Parse(resul.Identificador ?? "-1"))
                    .ToList()
                ;

            TestGenericoQPA_ConReporte<string>(
                identificador,
                desdeISO8601,
                hastaISO8601,
                lineasPosiblesSeparadasPorComa,
                infohXIdentificador,
                constructorFichas,
                tipoPuntaLinea,
                granularidadMts,
                radioPuntasDeLineaMts
            );
        }

        public void TestGenericoQPA_ConReporte<TIdent>(
            string identificador,
            string desdeISO8601,
            string hastaISO8601,
            string lineasPosiblesSeparadasPorComa,
            Dictionary<TIdent, InformacionHistorica> infohXIdentificador,
            ConstructorFichasDesdeResultados<TIdent> constructorFichas,
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

            // los recorridos teóricos...
            var proveedorRecorridosTeoricos = new ProveedorVersionesTecnobus(dirRepos: DameMockRepos());

            // trabajo... :S
            CalcularQPA_GenerarReporteQPA(
                identificador,
                desde,
                hasta,
                lineasPosibles,
                proveedorRecorridosTeoricos,
                infohXIdentificador,
                constructorFichas,
                creadorPuntasNombradas,
                granularidadMts
            );
        }

        public void CalcularQPA_GenerarReporteQPA<TIdent>(
            string      identificador,
            DateTime    desde,
            DateTime    hasta,
            int[] lineasPosibles,
            IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos,
            Dictionary<TIdent, InformacionHistorica> infohXIdentificador,
            ConstructorFichasDesdeResultados<TIdent> constructorFichas,
            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas,
            int         granularidadMts = 20
        )
        {
            var resultadosQPA = CalcularQPA<TIdent>(
                identificador,
                desde,
                hasta,
                lineasPosibles,
                proveedorRecorridosTeoricos,
                infohXIdentificador,
                creadorPuntasNombradas,
                granularidadMts
            );

            GenerarReporteQPA<TIdent>(
                identificador, 
                desde, 
                hasta, 
                resultadosQPA,
                constructorFichas
            );
        }

        public delegate List<int> ConstructorFichasDesdeResultados<TIdent>(DatosEmpIntFicha datosEmpIntFicha, List<QPAResult<TIdent>> resultados);

        public void GenerarReporteQPA<TIdent>(
            string          identificador,
            DateTime        desde,
            DateTime        hasta,
            List<QPAResult<TIdent>> resultadosQPA,
            ConstructorFichasDesdeResultados<TIdent> constructorFichas
        )
        {
            ///////////////////////////////////////////////////////////////////
            // Datos Empresa-Interno / Ficha
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

            var resulFichas = constructorFichas(datosEmpIntFicha, resultadosQPA);

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
                    resultadosQPA, 
                    resulFichas, 
                    fichasXEmpIntSUBE, 
                    proveedorVentaBoletos
                )
            };

            string nombreReporte = $"Reporte__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.txt";
            File.WriteAllText(nombreReporte, reporte.ToString());
        }

        static RecorridoLinBan SanitizarRecorrido(RecorridoLinBan reco, int granularidad)
        {
            return new RecorridoLinBan
            {
                Bandera = reco.Bandera,
                Linea   = reco.Linea,
                Puntos  = reco.Puntos.HacerGranular(granularidad),
            };
        }

        public List<QPAResult<TIdent>> CalcularQPA<TIdent>(
            string                          identificador,
            DateTime                        desde,
            DateTime                        hasta,
            int[]                           lineasPosibles,
            IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos,
            Dictionary<TIdent, InformacionHistorica> infohXIdentificador,
            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas,
            int granularidadMts             = 20
        )
        {
            identificador ??= string.Empty;

            #region Recorridos teóricos

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

            ///////////////////////////////////////////////////////////////////
            // Procesamiento de los datos (para todas las fichas)
            ///////////////////////////////////////////////////////////////////
            var resultadosQPA = ProcesarTodo<TIdent>(
                recorridosTeoricos, 
                topes2D, 
                puntasNombradas.Select(pu => (IPuntaLinea)pu).ToList(), 
                recoPatterns,
                infohXIdentificador // puntosXIdentificador
            );

            return resultadosQPA;
        }

        IEnumerable<string> CrearItemsCSV<TIdent>(
            char                        sep, 
            List<QPAResult<TIdent>>     resultadosSUBE, 
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

        int DBG_recosmal = 0;
        int DBG_recosbien = 0;

        string CrearItemCSV<TIdent>(
            char                        sep, 
            QPAResult<TIdent>           qpaResult, 
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

                var DBG_cantBoletosConLatLng = boletos
                    .Where(bolx => bolx.Latitud != 0 && bolx.Longitud != 0)
                    .Count()
                ;
                var DBG_cantBoletosQueEntranEnRecorrido = 0;

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
                        DBG_cantBoletosQueEntranEnRecorrido++;
                    }
                }

                if (DBG_cantBoletosConLatLng > 0 && DBG_cantBoletosQueEntranEnRecorrido == 0)
                {
                    int QUE_bandera = bandera;
                    int QUE_linea = linea;
                    int foo = 0;
                    DBG_recosmal++;
                }
                else
                {
                    DBG_recosbien++;
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
                    if ((bolx.Latitud == 0 || bolx.Longitud == 0) &&
                        bolx.FechaCancelacion >= subCamino.HoraSalida  &&
                        !_boletosReconocidos.Contains(bolx.Id))
                    {
                        var pepe = linea;
                        var pipo = bandera;
                        boletosOpt.Add(bolx);
                        _boletosReconocidos.Add(bolx.Id);
                    }
                }

                // cantidad de boletos "optimizados"
                var cantBoletosOpt = boletosOpt.Count;

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

        private List<QPAResult<TIdent>> ProcesarTodo<TIdent>(
            List<RecorridoLinBan>                               recorridosTeoricos,
            Topes2D                                             topes2D,
            List<IPuntaLinea>                                   puntasNombradas,
            Dictionary<string, List<KeyValuePair<int, int>>>    recoPatterns,
            Dictionary<TIdent, InformacionHistorica>                   infohXIdentificador
        )
        {
            var qpaProcessor= new QPAProcessor();
            var resultados  = new List<QPAResult<TIdent>>();

            foreach (var ident in infohXIdentificador.Keys)
            {
                try
                {
                    var partesHistoricas = infohXIdentificador[ident].GetPartesHistoricas().ToList();

                    foreach (ParteHistorica parteHistorica in partesHistoricas)
                    {
                        var resultado = qpaProcessor.Procesar(
                            identificador      : ident,
                            recorridosTeoricos : recorridosTeoricos,
                            puntosHistoricos   : parteHistorica.Puntos,
                            topes2D            : topes2D,
                            puntasNombradas    : puntasNombradas,
                            recoPatterns       : recoPatterns
                        );

                        resultado = VelocidadNormal (resultado);
                        resultado = DuracionPositiva(resultado);
                            
                        if (resultado.SubCaminos.Any())
                        {
                            resultados.Add(resultado);
                        }
                    }
                }
                catch (Exception exx)
                {
                    int foo = 0;
                }
            } // para cada identificador...

            return resultados;
        }

        private static bool HayMovimiento<PointType>(List<PointType> puntos, int radioMts)
            where PointType : Punto
        {
            if (puntos == null) { throw new ArgumentNullException(nameof(puntos)); }
            if (!puntos.Any())  { return false; }

            PointType p = puntos.First();

            foreach (PointType px in puntos.Skip(1))
            {
                var dist = Haversine.GetDist(p, px);
                if (dist > Math.Abs(radioMts))
                {
                    return true;
                }
            }

            return false;
        }

        private QPAResult<TIdent> VelocidadNormal<TIdent>(QPAResult<TIdent> res)
        {
            var newSubCaminos = res.SubCaminos
                .Where(subCamino => 
                    subCamino.VelocidadKmhPromedio <= 120 && 
                    subCamino.VelocidadKmhPromedio >= 5
                )
                .ToList()
            ;

            return new QPAResult<TIdent>
            {
                Granularidad        = res.Granularidad,
                RecorridosTeoricos  = res.RecorridosTeoricos,
                Topes2D             = res.Topes2D,
                Camino              = res.Camino,
                Identificador       = res.Identificador,
                SubCaminos          = newSubCaminos,
            };
        }

        private QPAResult<TIdent> DuracionPositiva<TIdent>(QPAResult<TIdent> res)
        {
            var newSubCaminos = res.SubCaminos
                .Where  (subCamino => subCamino.HoraLlegada > subCamino.HoraSalida)
                .ToList ()
            ;

            return new QPAResult<TIdent>
            {
                Granularidad        = res.Granularidad,
                RecorridosTeoricos  = res.RecorridosTeoricos,
                Topes2D             = res.Topes2D,
                Camino              = res.Camino,
                Identificador       = res.Identificador,
                SubCaminos          = newSubCaminos,
            };
        }
    }
}
