﻿using Comun;
using LibQPA;
using QPACreator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using LibQPA.ProveedoresTecnobus;
using System.Net.Http;
using System.Security.Authentication;
using System.Net;
using Newtonsoft.Json;
using ComunSUBE;
using System.Text;

namespace QPApp
{
    public partial class Program
    {
        static bool _usarHttps = false;

        static async Task<int> Main(string[] args)
        {
            // 1) ojin con el repositorio de los recorridos: pueden estar un poco viejitos
            // 2) se debe hacer un árbol QPA para
            //      - saber que parte depende de que parte
            //      - saber que podemos usar para cada necesidad

            // * modo (JsonSUBE|DriveUp|PicoBus|TecnobusSmGps) (de donde sacaré los datos y en que forma los procesaré)
            // * identificadorReporte           (una cadena para identificar univocamente a los reportes)
            // * desdeISO8601                   (la fecha desde en formato ISO8601)
            //   lineasPosiblesSeparadasPorComa default = "159,163"
            //   tipoPuntaLinea                 default = typeof(PuntaLinea)
            //   tipoCreadorPartesHistoricas    default = typeof(CreadorPartesHistoricasIdentidad)
            //   granularidadMts                default = 20
            //   radioPuntasDeLineaMts          default = 800

            #region Parsing de Argumentos
            Dictionary<string, string> misArgs = ArgsHelper.CreateDictionary(args);
            DateTime ahora = DateTime.Now;

            // modo
            string modo = ArgsHelper.SafeGetArgVal(misArgs, "modo", "DriveUp");

            // idReporte
            string idReporte = modo.ToLower();

            // desde hasta
            string sDesde = ArgsHelper.SafeGetArgVal(misArgs, "desde", ahora.ToString("yyyy-MM-dd"));
            DateTime desde = DateTime.Parse(sDesde);
            DateTime hasta = desde.AddDays(1);

            // líneas y sus representaciones
            string sLineas = ArgsHelper.SafeGetArgVal(misArgs, "lineas", string.Empty);
            List<int> lineasComoVienen = sLineas
                .Split(',')
                .Where(sLinX => EsNumeroEntero(sLinX))
                .Select(sLinX => int.Parse(sLinX.Trim()))
                .ToList()
            ;
            List<int> lineasOrdenadas = lineasComoVienen
                .OrderBy(n => n)
                .ToList()
            ;
            
            // ficha en especial
            string sFicha = ArgsHelper.SafeGetArgVal(misArgs, "ficha", "0");
            int ficha = int.Parse(sFicha);

            // tipo punta de línea
            string sTipoPuntaLinea = ArgsHelper.SafeGetArgVal(misArgs, "tipoPuntas", "PuntaLinea");
            Type tipoPuntaLinea = null;
            if (sTipoPuntaLinea == "PuntaLinea") { tipoPuntaLinea = typeof(PuntaLinea); }
            else if (sTipoPuntaLinea == "PuntaLinea2") { tipoPuntaLinea = typeof(PuntaLinea2); }

            // tipo creador partes históricas
            Type tipoCreadorPartesHistoricas = typeof(CreadorPartesHistoricasIdentidad);

            // granularidad en metros
            string sGranularidad = ArgsHelper.SafeGetArgVal(misArgs, "granularidad", "20");
            int granularidad = int.Parse(sGranularidad);

            // radio de las puntas de línea
            string sRadioPuntas = ArgsHelper.SafeGetArgVal(misArgs, "radioPuntas", "800");
            int radioPuntas = int.Parse(sRadioPuntas);
            #endregion

            #region Nombre del Reporte / Directorio de salida
            var dirReportes = "./Reportes";
            if (!Directory.Exists(dirReportes))
            {
                Directory.CreateDirectory(dirReportes);
            }

            var nombreArchivoReporteSinExtension = GetNombreArchivoReporteSinExtension(
                idReporte,
                desde,
                hasta,
                tipoPuntaLinea,
                radioPuntas,
                granularidad,
                lineasOrdenadas,
                ficha
            );

            var pathArchivoReporteSinExtension = Path.Combine(dirReportes, nombreArchivoReporteSinExtension);

            if (File.Exists($"{pathArchivoReporteSinExtension}.csv"))
            {
                Console.WriteLine("El reporte ya fue creado");
                return 0;
            }
            #endregion

            #region Recuperación de Recorridos Teóricos
            Console.WriteLine("Buscando Recorridos Teóricos");
            List<RecorridoLinBan> recorridosTeoricos = null;
            try
            {
                RetVal<List<RecorridoLinBan>> retvalRecorridosTeoricos =
                    await DameRecorridosTeoricos(desde, lineasOrdenadas, granularidad);

                if (retvalRecorridosTeoricos.IsOk)
                {
                    recorridosTeoricos = retvalRecorridosTeoricos.Value;
                }
                else
                {
                    Console.WriteLine(retvalRecorridosTeoricos.ErrorMessage);
                    return 1;
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.Message);
                return 1;
            }
            #endregion

            #region Recuperación de Puntos Históricos
            Console.WriteLine("Buscando Puntos Históricos");
            Dictionary<int, List<PuntoHistorico>> puntosXIdentificador = null;
            try
            {
                RetVal<Dictionary<int, List<PuntoHistorico>>> retvalPuntosHistoricos =
                    await DamePuntosHistoricosAsync(modo, desde, hasta, lineasOrdenadas, ficha);
                if (retvalPuntosHistoricos.IsOk)
                {
                    puntosXIdentificador = retvalPuntosHistoricos.Value;
                }
                else
                {
                    Console.WriteLine(retvalPuntosHistoricos.ErrorMessage);
                    return 1;
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx);
                return 1;
            }
            #endregion

            #region Recuperación de Empresa-Interno X Ficha
            Console.WriteLine("Buscando empresa-interno SUBE x fichas");
            Dictionary<int, (int, int)> empresaInternoSUBEXFichas = null;
            try
            {
                RetVal<Dictionary<int, (int, int)>> retEmpresaInternoSUBEXFichas =
                    await DameEmpresaInternoXFichasAsync();
                if (retEmpresaInternoSUBEXFichas.IsOk)
                {
                    empresaInternoSUBEXFichas = retEmpresaInternoSUBEXFichas.Value;
                }
                else
                {
                    Console.WriteLine(retEmpresaInternoSUBEXFichas.ErrorMessage);
                    return 1;
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx);
                return 1;
            }
            #endregion

            #region Recuperación de boletos SUBE y creación de Proveedor de venta de boletos
            Console.WriteLine("Buscando boletos SUBE");
            ProveedorBoletosSUBERed proveedorVentaBoletos2 = null;
            try
            {
                var retBoletosXIdentificador =
                    await DameBoletosPorIdentificador(fechaDesde: desde, fechaHasta: hasta);

                if (retBoletosXIdentificador.IsOk)
                {
                    proveedorVentaBoletos2 = new ProveedorBoletosSUBERed
                    {
                        BoletosXIdentificador = retBoletosXIdentificador.Value
                    };
                }
                else
                {
                    Console.WriteLine(retBoletosXIdentificador.ErrorMessage);
                    return 1;
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx);
                return 1;
            }
            #endregion

            // creo el qpaCreator
            var qpaCreator = new QPACreator.Creator(new CreatorConfiguration());
            qpaCreator.Aviso += QpaCreator_Aviso;
            var (resultadosQPA, reporteQPA) = qpaCreator.Calculate<int>(
                idReporte,
                desde,
                hasta,
                lineasOrdenadas,
                tipoPuntaLinea,
                tipoCreadorPartesHistoricas,
                recorridosTeoricos,
                puntosXIdentificador,
                ConstructorFichasInt,
                empresaInternoSUBEXFichas,
                proveedorVentaBoletos2,
                granularidadMts: granularidad,
                radioPuntasDeLineaMts: radioPuntas
            );

            /////////////////////////////////////////////////////////
            // Creación del reporte 

            Reporte rpx = new Reporte();
            var aux = new ReporteQPASubItem<string>();
            CSVFormatter formatter = new CSVFormatter
            {
                EnableTitle = true,
                NewLine = "\r\n",
                Separator = ';',
                CellBuilders = new List<CSVCellBuilder>
                {
                    new CSVCellBuilder{ PropertyName = nameof(aux.EmpresaSUBE        ), ResultName = "empresaSUBE" },
                    new CSVCellBuilder{ PropertyName = nameof(aux.InternoSUBE        ), ResultName = "internoSUBE" },
                    new CSVCellBuilder{ PropertyName = nameof(aux.Ficha              ), ResultName = "ficha"       },
                    new CSVCellBuilder{ PropertyName = nameof(aux.Linea              ), ResultName = "linea"       },
                    new CSVCellBuilder{ PropertyName = nameof(aux.Bandera            ), ResultName = "bandera"     },
                    new CSVCellBuilder{ PropertyName = nameof(aux.Inicio             ), ResultName = "inicio"      , FormatFunction = d => ((DateTime)d).ToString("dd/MM/yyyy HH:mm:ss") },
                    new CSVCellBuilder{ PropertyName = nameof(aux.Fin                ), ResultName = "fin"         , FormatFunction = d => ((DateTime)d).ToString("dd/MM/yyyy HH:mm:ss") },
                    new CSVCellBuilder{ PropertyName = nameof(aux.CantBoletosNaive   ), ResultName = "cantbol"     },
                    new CSVCellBuilder{ PropertyName = nameof(aux.CantBoletosOpt     ), ResultName = "cantbolopt"  },
                }
            };

            var lineasContenido = rpx.RenderAllLines(
                rpx.Create(reporteQPA.ListaAplanadaSubItems.Cast<object>(), formatter),
                formatter
            );

            IEnumerable<string> contenido = lineasContenido;

            if (modo == "picobus")
            {
                var lineasContenidoFiltrado = lineasContenido
                    .Where(s => !s.EndsWith(";0;0\n") && !s.EndsWith(";0;0\r\n"))
                ;

                contenido = lineasContenidoFiltrado;
            }

            File.WriteAllText(
                $"{pathArchivoReporteSinExtension}.csv", 
                string.Join(string.Empty, contenido)
            );

            return 0;
        }

        private static string GetNombreArchivoReporteSinExtension(
            string          idReporte, 
            DateTime        desde, 
            DateTime        hasta, 
            Type            tipoPuntaLinea,
            int             radioPuntas,
            int             granularidad,
            IEnumerable<int>lineas,
            int             ficha
        )
        {
            StringBuilder sbNombre = new StringBuilder();
            const string SEPARADOR_VARIABLES = "__";
            const string SEPARADOR_ARGUMENTO = "_";
            
            sbNombre.Append("DH");
            sbNombre.Append(SEPARADOR_ARGUMENTO);
            sbNombre.Append(desde.ToString("yyyy-MM-ddTHHmmss"));
            sbNombre.Append(SEPARADOR_ARGUMENTO);
            sbNombre.Append(hasta.ToString("yyyy-MM-ddTHHmmss"));

            sbNombre.Append(SEPARADOR_VARIABLES);

            sbNombre.Append("ALG");
            sbNombre.Append(SEPARADOR_ARGUMENTO);
            sbNombre.Append(tipoPuntaLinea.Name);
            sbNombre.Append(SEPARADOR_ARGUMENTO);
            sbNombre.Append($"R{radioPuntas}");
            sbNombre.Append(SEPARADOR_ARGUMENTO);
            sbNombre.Append($"G{granularidad}");

            sbNombre.Append(SEPARADOR_VARIABLES);

            sbNombre.Append("LIN");
            sbNombre.Append(SEPARADOR_ARGUMENTO);
            sbNombre.Append(string.Join(SEPARADOR_ARGUMENTO, lineas.OrderBy(l => l).ToArray()));

            sbNombre.Append(SEPARADOR_VARIABLES);

            sbNombre.Append("MODO");
            sbNombre.Append(SEPARADOR_ARGUMENTO);
            sbNombre.Append(idReporte);

            if (ficha > 0)
            {
                sbNombre.Append(SEPARADOR_VARIABLES);

                sbNombre.Append("FICHA");
                sbNombre.Append(SEPARADOR_ARGUMENTO);
                sbNombre.Append(ficha);
            }

            var nombre = sbNombre.ToString();

            return nombre;

            //return $"Reporte_{idReporte}_Desde_{desde:yyyyMMdd}_Hasta_{hasta:yyyyMMdd}_Lineas_{sLineasOrdenadas}";
        }

        private async static Task<RetVal<List<RecorridoLinBan>>> DameRecorridosTeoricos(
            DateTime    desde,
            List<int>   lineasOrdenadas,
            int         granularidad
        )
        {
            try
            {
                IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos =
                    new ProveedorVersionesTecnobus(new string[] { "./Datos" });

                Filtro filtro = Filtro.CreateFrom("./Filtros", lineasOrdenadas);

                var recorridosTeoricos = proveedorRecorridosTeoricos.Get(new QPAProvRecoParams()
                {
                    LineasPosibles = lineasOrdenadas.ToArray(),
                    FechaVigencia = desde
                })
                    .Where(reco => filtro.EsRecorridoPermitido(reco.Linea, reco.Bandera))
                    .Select(reco => SanitizarRecorrido(reco, granularidad))
                    .ToList()
                ;

                return new RetVal<List<RecorridoLinBan>>
                {
                    IsOk = true,
                    Value = recorridosTeoricos,
                };
            }
            catch (Exception exx)
            {
                return new RetVal<List<RecorridoLinBan>>
                {
                    IsOk = false,
                    ErrorMessage = exx.Message,
                    ErrorException = exx,
                };
            }
        }

        private static async Task<RetVal<Dictionary<int, List<PuntoHistorico>>>> DamePuntosHistoricosAsync(
            string modo, 
            DateTime    desde, 
            DateTime    hasta,
            List<int>   lineasOrdenadas,
            int         ficha
        )
        {
            // creo el directorio de bajada si no existe...
            var dirBajados = "./Bajados/";
            if (!Directory.Exists(dirBajados))
            {
                Directory.CreateDirectory(dirBajados);
            }

            var nombreArchivoLocal = $"{modo.ToLower()}_{desde.Year:0000}_{desde.Month:00}_{desde.Day:00}.csv";
            var pathArchivoLocal = Path.Combine(dirBajados, nombreArchivoLocal).Replace('\\', '/');

            if (File.Exists(pathArchivoLocal))
            {
                // ya existe el archivo
                Console.WriteLine($"El archivo {pathArchivoLocal} existe en modo '{modo}'");
            }
            else
            {
                // bajo el nuevo archivo
                int diasMenos = Convert.ToInt32((DateTime.Now.Date.Subtract(desde.Date)).TotalDays);
                Uri remoteUri = null;

                // dependiendo del "modo" bajo de un lugar u otro
                // vm-coches = 192.168.201.74
                switch (modo.ToLower())
                {
                    case "driveup":
                        remoteUri = _usarHttps ?
                            new Uri("https://192.168.201.74:5001/HistoriaCochesDriveUpAnteriores?diasMenos=" + diasMenos.ToString()) :
                            new Uri("http://192.168.201.74:5000/HistoriaCochesDriveupAnteriores?diasMenos=" + diasMenos.ToString());
                        break;
                    case "picobus":
                        remoteUri = _usarHttps?
                            new Uri("https://192.168.201.74:5003/HistoriaCochesPicoBusAnteriores?formato=csv&diasMenos=" + diasMenos.ToString()):
                            new Uri("http://192.168.201.74:5002/HistoriaCochesPicoBusAnteriores?formato=csv&diasMenos=" + diasMenos.ToString());                        
                        break;
                    case "tecnobussmgps":
                        remoteUri = _usarHttps ?
                            new Uri("https://192.168.201.74:5005/HistoriaCochesTecnobusSmGpsAnteriores?formato=csv&diasMenos=" + diasMenos.ToString()):
                            new Uri("http://192.168.201.74:5004/HistoriaCochesTecnobusSmGpsAnteriores?formato=csv&diasMenos=" + diasMenos.ToString());
                        break;
                    default:
                        var errMsg = $"El modo '{modo}' no es válido";
                        return new RetVal<Dictionary<int, List<PuntoHistorico>>>
                        {
                             ErrorMessage = errMsg,
                             IsOk = false,
                        };
                }

                // bajar archivo...
                HttpClient httpClient = null;

                if (_usarHttps)
                {
                    var handler = new HttpClientHandler()
                    {
                        Proxy = new WebProxy("192.168.201.2:8080", true),
                        SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
                    };
                    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    httpClient = new HttpClient(handler);
                }
                else
                {
                    httpClient = new HttpClient();
                }

                httpClient.Timeout = TimeSpan.FromMinutes(10);

                var pepe = await httpClient.GetAsync(remoteUri);
                var sres = await pepe.Content.ReadAsStringAsync();
                File.WriteAllText(pathArchivoLocal, sres);
            }

            Console.WriteLine("Tratando de construir información histórica");

            Dictionary<int, List<PuntoHistorico>> puntosXIdentificador = null;

            try
            {
                puntosXIdentificador = GetPuntosPorIdentificador(pathArchivoLocal, modo, soloEstaFicha: ficha);

                // acá intentaré dejar solo las fichas que sean de las líneas elegidas
                if (ficha == 0 && lineasOrdenadas.All(LineaHabilitadaPrecalcularFichas))
                {
                    // averiguar las fichas de las líneas elegidas y ponerlas en un conjunto
                    //List<int> fichasParaUsar = DameFichasDeLineas_DB(password, lineasOrdenadas, desde, hasta);
                    RetVal<List<int>> retValFichasXLinea = await DameFichasDeLineas_RedAsync(lineasOrdenadas, desde, hasta);

                    if (retValFichasXLinea.IsOk)
                    {
                        // filtro el diccionario "puntosXIdentificador" con esas fichas
                        puntosXIdentificador = FiltrarFichas(
                            puntosXIdentificador,
                            retValFichasXLinea.Value
                        );
                    }
                    else
                    {
                        // aca, por alguna razón, el programa falló
                        var errMsg = "\tNo se pudo filtrar información histórica";
                        return new RetVal<Dictionary<int, List<PuntoHistorico>>>
                        {
                            ErrorMessage = errMsg,
                            IsOk = false,
                        };
                    }

                }

                Console.WriteLine("\tInformación histórica ok");

                return new RetVal<Dictionary<int, List<PuntoHistorico>>>
                {
                    IsOk = true,
                    Value = puntosXIdentificador,
                };
            }
            catch (Exception exx)
            {
                return new RetVal<Dictionary<int, List<PuntoHistorico>>>
                {
                    ErrorMessage = exx.Message,
                    ErrorException = exx,
                    IsOk = false,
                };
            }

        }

        private static async Task<RetVal<Dictionary<ParEmpresaInterno, List<BoletoComun>>>> DameBoletosPorIdentificador(DateTime fechaDesde, DateTime fechaHasta)
        {
            // vm-coches = 192.168.201.74
            var url = $"http://192.168.201.74:5006/BoletosXIdentSUBE_KvpList?desde={fechaDesde.Year:0000}-{fechaDesde.Month:00}-{fechaDesde.Day:00}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<KeyValuePair<ParEmpresaInterno, List<BoletoComun>>>>(json);
                var val = obj.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                return new RetVal<Dictionary<ParEmpresaInterno, List<BoletoComun>>>
                {
                    IsOk = true,
                    Value= val,
                };
            }
            else
            {
                return new RetVal<Dictionary<ParEmpresaInterno, List<BoletoComun>>>
                {
                    IsOk = false,
                };
            }            
        }

        public class EmpresaInternoFicha
        {
            public int Empresa { get; set; }
            public int Interno { get; set; }
            public int Ficha   { get; set; }
        }

        private static async Task<RetVal<Dictionary<int, (int, int)>>> DameEmpresaInternoXFichasAsync()
        {
            try
            {
                var httpClient = new HttpClient();
                var url = "http://192.168.201.74:5008/EmpresaInternoFicha";
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var lst = JsonConvert.DeserializeObject<List<EmpresaInternoFicha>>(json);
                    var val = new Dictionary<int, (int, int)>();
                    foreach (var eif in lst)
                    {
                        if (!val.ContainsKey(eif.Ficha))
                        {
                            val.Add(eif.Ficha, (eif.Empresa, eif.Interno));
                        }
                    }

                    return new RetVal<Dictionary<int, (int, int)>>
                    {
                        Value = val
                    };
                }
                else
                {
                    return new RetVal<Dictionary<int, (int, int)>>
                    {
                        IsOk = false,
                        ErrorMessage = $"Error {response.StatusCode}"
                    };
                }

            }
            catch (Exception exx)
            {
                return new RetVal<Dictionary<int, (int, int)>>
                {
                    IsOk = false,
                    ErrorMessage = exx.Message,
                    ErrorException = exx
                };
            }
        }

        class ParLineaFicha
        {
            public int Linea { get; set; }
            public int Ficha { get; set; }
        }

        static async Task<RetVal<List<int>>> DameFichasDeLineas_RedAsync(
            List<int> lineasOrdenadas, 
            DateTime desde, 
            DateTime hasta
        )
        {
            try
            {
                var httpClient = new HttpClient();
                var url = "http://192.168.201.74:5100/FichasXLinea?lineas=100&desde=2022-11-29&hasta=2022-11-30";
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var listaParLineaFicha = JsonConvert.DeserializeObject<List<ParLineaFicha>>(json);
                    var listaFichas = listaParLineaFicha
                        .Select(par => par.Ficha)
                        .ToList()
                    ;
                    return new RetVal<List<int>> { 
                        Value = listaFichas 
                    };
                }
                else
                {
                    return new RetVal<List<int>> { 
                        IsOk = false, 
                        ErrorMessage = $"Error {response.StatusCode}" 
                    };
                }
            }
            catch (Exception exx)
            {
                return new RetVal<List<int>> { 
                    IsOk = false, 
                    ErrorMessage = exx.Message, 
                    ErrorException = exx 
                };
            }            
        }

        static Dictionary<int, List<PuntoHistorico>> FiltrarFichas(
            Dictionary<int, List<PuntoHistorico>> puntosXIdentificador, 
            IEnumerable<int> fichasParaUsar
        )
        {
            var ret = new Dictionary<int, List<PuntoHistorico>>();
            var conjuntoFichasParaUsar = fichasParaUsar.ToHashSet();

            foreach (var (k, v) in puntosXIdentificador)
            {
                if (conjuntoFichasParaUsar.Contains(k))
                {
                    ret.Add(k, v);
                }
            }

            return ret;
        }

        static bool LineaHabilitadaPrecalcularFichas(int linea)
        {
            return linea switch
            {
                1  => true ,
                21 => true ,
                100=> true ,
                101=> true ,
                103=> true ,
                _  => false,
            };
        }

        static void QpaCreator_Aviso(object sender, Creator.AvisoEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(e.Mensaje);
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

        static Dictionary<int, List<PuntoHistorico>> GetPuntosPorIdentificador(
            string  pathArchivoLocal, 
            string  modo, 
            int     soloEstaFicha = 0
        )
        {
            int posFic = 0;
            int posLat = 0;
            int posLng = 0;
            int posFec = 0;
            int lenMin = 5;

            // modo driveup
            //
            // 0      1          2            3                    4                    5
            // Ficha; Lat      ; Lng        ; FechaLocal         ; Recordedat         ; FechaLlegadaLocal
            // 3749 ; -34.47732; -58.5095933; 2022-07-06 00:00:01; 2022-07-06 03:00:01; 2022-07-06 00:00:00
            if (modo == "driveup")
            {
                posFic = 0;
                posLat = 1;
                posLng = 2;
                posFec = 3;
                lenMin = 6;
            }

            // modo picobus
            //
            // 0      1        2          3          4                    5
            // Ficha; Id     ; Lat      ; Lng      ; FechaLlegadaLocal  ; FechaDeProduccion
            // 4403 ; 3000696; -34.54017; -58.82160; 2022-08-15 00:00:00; 2022-08-15 00:00:00
            if (modo == "picobus")
            {
                posFic = 0;
                posLat = 2;
                posLng = 3;
                posFec = 4;
                lenMin = 6;
            }

            // modo tecnobussmgps
            //
            // Ficha; Lat     ; Lng     ; Fecha 
            // 2853 ; -32.8488; -60.7249; 2022-11-17 00:00:00
            //
            if (modo == "tecnobussmgps")
            {
                posFic = 0;
                posLat = 1;
                posLng = 2;
                posFec = 3;
                lenMin = 4;
            }

            var ret = new Dictionary<int, List<PuntoHistorico>>();

            foreach (var sLine in File.ReadLines(pathArchivoLocal).Skip(1))
            {
                var partes = sLine.Split(';');
                
                if (partes.Length < lenMin)
                {
                    continue;
                }

                var key = int       .Parse(partes[posFic]);
                var lat = double    .Parse(partes[posLat], CultureInfo.InvariantCulture);
                var lng = double    .Parse(partes[posLng], CultureInfo.InvariantCulture);
                var fec = DateTime  .Parse(partes[posFec]);

                if (key == 0)
                {
                    continue;
                }

                if (soloEstaFicha != 0 && soloEstaFicha != key)
                {
                    continue;
                }

                if (! ret.ContainsKey(key))
                { 
                    ret.Add(key, new List<PuntoHistorico>());
                }

                ret[key].Add(new PuntoHistorico
                {
                    Alt = 0,
                    Lat = lat,
                    Lng = lng,
                    Fecha = fec,
                });
            }

            return ret;
        }

        static List<int> ConstructorFichasInt(List<QPAResult<int>> resultadosQPA)
        {
            return resultadosQPA
                .Select(rqpa => rqpa.Identificador)
                .ToList()
            ;
        }

        static bool EsDigito(char c)
        {
            return c >= '0' && c <= '9';
        }

        static bool EsNumeroEntero(string sLinX)
        {
            if (string.IsNullOrEmpty(sLinX))
            {
                return false;
            }

            foreach (char c in sLinX)
            {
                if (!EsDigito(c))
                {
                    return false;
                }
            }

            return true;
        }

    }

    public class Filtro
    {
        public Dictionary<int, Dictionary<int, bool>> _dic = new Dictionary<int, Dictionary<int, bool>>();
        public Dictionary<int, bool> _overrides = new Dictionary<int, bool>();

        public Filtro(Dictionary<int, Dictionary<int, bool>> dic, Dictionary<int, bool> overrides)
        {
            _dic = dic;
            _overrides = overrides;
        }

        public static Filtro CreateFrom(string dirBaseFiltros, List<int> lineas)
        {
            var dic = new Dictionary<int, Dictionary<int, bool>>();
            var overrides = new Dictionary<int, bool>();

            foreach (int lineaX in lineas)
            {
                var pathFiltro = Path.GetFullPath(
                    Path.Combine(dirBaseFiltros, $"{lineaX:0000}\\filtros.csv"))
                .Replace('\\', '/');

                if (File.Exists(pathFiltro))
                {
                    overrides.Add(lineaX, false);
                    dic.Add(lineaX, new Dictionary<int, bool>());
                    // rellenar con archivo...
                    //CodLin;CodBan;Clasif
                    //167;2805;COM
                    //167;2804;COM
                    //167;3080;POS
                    //167;3081;POS
                    foreach (string renglon in File.ReadLines(pathFiltro).Skip(1))
                    {
                        var partes = renglon.Split(";");
                        var linea = int.Parse(partes[0]);
                        var bandera = int.Parse(partes[1]);
                        var ok = partes[2].ToUpper() == "COM";
                        dic[linea].Add(bandera, ok);
                    }
                }
                else
                {
                    // no hay archivo de filtro para esta linea, la guardo en la lista de overrides
                    // cuando se llame a EsRecorridoPermitido siempre dará true
                    overrides.Add(lineaX, true);
                }
            }

            return new Filtro(dic, overrides);
        }

        public bool EsRecorridoPermitido(int linea, int bandera)
        {
            if (!_overrides.ContainsKey(linea))
            {
                throw new Exception($"Este filtro no fue creado para la línea {linea}");
            }

            if (_overrides[linea])
            {
                return true;
            }

            if (!_dic.ContainsKey(linea))
            {
                throw new Exception($"Este filtro no fue creado para la línea {linea}");
            }

            if (!_dic[linea].ContainsKey(bandera))
            {
                //throw new Exception($"Este filtro no fue creado para la bandera {bandera}");
                return false;
            }

            return _dic[linea][bandera];
        }

    }

}
