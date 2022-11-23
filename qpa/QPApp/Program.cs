using Comun;
using LibQPA;
using QPACreator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using LibQPA.ProveedoresTecnobus;
using System.Net.Http;
using System.Security.Authentication;
using System.Net;
using System.Data.SqlClient;

namespace QPApp
{
    public class Program
    {
        //static AutoResetEvent _areArchivoBajado = new AutoResetEvent(false); // es para señalizar que ya se bajó el archivo
        //static string _archivoBajando = string.Empty; // es para pasar la información del archivo que se está bajando

        static async Task<int> Main(string[] args)
        {
            // 1) ojin con el repositorio de los recorridos: pueden estar un poco viejitos
            //      - por ahora corregido...
            // 2) se debe hacer un árbol QPA para
            //      - saber que parte depende de que parte
            //      - saber que podemos usar para cada necesidad

            // * password                       el password de la DB
            // * modo (JsonSUBE|DriveUp|PicoBus|TecnobusSmGps) (de donde sacaré los datos y en que forma los procesaré)
            // * identificadorReporte           (una cadena para identificar univocamente a los reportes)
            // * desdeISO8601                   (la fecha desde en formato ISO8601)
            //   lineasPosiblesSeparadasPorComa default = "159,163"
            //   tipoPuntaLinea                 default = typeof(PuntaLinea)
            //   tipoCreadorPartesHistoricas    default = typeof(CreadorPartesHistoricasIdentidad)
            //   granularidadMts                default = 20
            //   radioPuntasDeLineaMts          default = 800

            Dictionary<string, string> misArgs = ArgsHelper.CreateDictionary(args);
            DateTime ahora = DateTime.Now;

            // password
            string password = ArgsHelper.SafeGetArgVal(misArgs, "password", "");

            // modo
            string modo = ArgsHelper.SafeGetArgVal(misArgs, "modo", "DriveUp");

            // idReporte
            string idReporte = modo.ToLower(); //ArgsHelper.SafeGetArgVal(misArgs, "idReporte", modo);

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
            string sLineasOrdenadas = string.Join('_', lineasOrdenadas.ToArray());

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

            // creo el qpaCreator
            var qpaCreator = new QPACreator.Creator(new CreatorConfiguration
            {
                ConnectionStringFichasXEmprIntSUBE = "Data Source=192.168.201.21;Initial Catalog=general;User ID=sa;Password=" + password,
                ConnectionStringVentasSUBE = "Data Source=192.168.201.42;Initial Catalog=sube;User ID=sa;Password=" + password,
                ConnectionStringPuntosSUBE = "Data Source=192.168.201.10;Initial Catalog=logsTecnobus;User ID=sa;Password=" + password,
                ConnectionStringPuntosXBus = "Data Source=192.168.201.10;Initial Catalog=logsTecnobus;User ID=sa;Password=" + password,
            });
            qpaCreator.Aviso += QpaCreator_Aviso;

            // recorridos teóricos
            IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos = new ProveedorVersionesTecnobus(
                new string[] { "./Datos" }
            );

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
                switch (modo.ToLower())
                {
                    case "driveup":
                        remoteUri = new Uri("https://vm-coches:5001/HistoriaCochesDriveUpAnteriores?formato=csv&diasMenos=" + diasMenos.ToString());
                        break;
                    case "picobus":
                        remoteUri = new Uri("https://vm-coches:5003/HistoriaCochesPicoBusAnteriores?formato=csv&diasMenos=" + diasMenos.ToString());
                        break;
                    case "tecnobussmgps":
                        remoteUri = new Uri("https://vm-coches:5005/HistoriaCochesTecnobusSmGpsAnteriores?formato=csv&diasMenos=" + diasMenos.ToString());
                        break;
                    default:
                        Console.WriteLine($"El modo '{modo}' no es válido");
                        return -1;
                }

                // bujar archivo...
                var handler = new HttpClientHandler()
                {
                    Proxy = new WebProxy("192.168.201.2:8080", true),
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
                };
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                var httpClient = new HttpClient(handler);
                var pepe = await httpClient.GetAsync(remoteUri);
                var sres = await pepe.Content.ReadAsStringAsync();
                File.WriteAllText(pathArchivoLocal, sres);

                //// bajar archivo...
                //Console.WriteLine($"Tratando de bajar archivo en modo '{modo}'");
                //var bajadorDeArchivo = new BajadorDeArchivo();
                //if (!bajadorDeArchivo.BajarArchivo(remoteUri, pathArchivoLocal))
                //{
                //    Console.WriteLine($"No pude bajar el archivo {bajadorDeArchivo.Error}");
                //    return -1;
                //}
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
                    IEnumerable<int> fichasParaUsar = DameFichasDeLineas(password, lineasOrdenadas, desde, hasta);

                    // filtrar el diccionario "puntosXIdentificador" con ese conjunto
                    puntosXIdentificador = FiltrarFichas(puntosXIdentificador, fichasParaUsar);
                }

                Console.WriteLine("\tInformación histórica ok");
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx);
                return 1;
            }

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
                granularidadMts: granularidad,
                radioPuntasDeLineaMts: radioPuntas
            );

            /////////////////////////////////////////////////////////
            // Creación del reporte 

            var dirReportes = "./Reportes";
            if (!Directory.Exists(dirReportes))
            {
                Directory.CreateDirectory(dirReportes);
            }

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

            var nombreArchivoReporte = $"Reporte_{idReporte}_Desde_{desde:yyyyMMdd}_Hasta_{hasta:yyyyMMdd}_Lineas_{sLineasOrdenadas}";
            var pathArchivoReporte = Path.Combine(dirReportes, nombreArchivoReporte);

            File.WriteAllText($"{pathArchivoReporte}.csv", string.Join(string.Empty, contenido));

            return 0;
        }

        static List<int> DameFichasDeLineas(string password, List<int> lineasOrdenadas, DateTime desde, DateTime hasta)
        {
            // conectarme a db-tecnobus...
            string connectionString = 
                "Data Source=192.168.201.10;Initial Catalog=logsTecnobus;User ID=sa;Password=" + 
                password
            ;
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // tomar las fichas que necesito
            var lineasSeparadasPorComas = string.Join(",", lineasOrdenadas.ToArray());
            var cmdText = @$"
                SELECT DISTINCT cod_linea, nro_ficha
                FROM log_gpsConCalculoAtraso
                WHERE
	                fechaHora >= '{desde.Day:00}/{desde.Month:00}/{desde.Year:0000}'
	                AND
	                fechaHora <  '{hasta.Day:00}/{hasta.Month:00}/{hasta.Year:0000}'
	                AND
	                cod_linea in ({lineasSeparadasPorComas})
                ORDER BY cod_linea, nro_ficha
            ";
            using SqlCommand command = new SqlCommand(cmdText.Trim(), connection);
            using SqlDataReader reader = command.ExecuteReader();
            var fichas = new List<int>();
            while (reader.Read())
            {
                var objFicha = reader["nro_ficha"];
                if (objFicha != null)
                {
                    try {
                        var ficha = Convert.ToInt32(objFicha);
                        fichas.Add(ficha);
                    } catch (Exception exx) {
                        Console.WriteLine(exx);
                    }
                }
            }

            // devolverlas por aca...
            return fichas;
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
