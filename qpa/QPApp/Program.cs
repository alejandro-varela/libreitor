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

namespace QPApp
{
    public class Program
    {
        static AutoResetEvent _areArchivoBajado = new AutoResetEvent(false); // es para señalizar que ya se bajó el archivo
        static string _archivoBajando = string.Empty; // es para pasar la información del archivo que se está bajando

        static async Task<int> Main(string[] args)
        {
            // 1) ojin con el repositorio de los recorridos: pueden estar un poco viejitos
            //      - por ahora corregido...
            // 2) se debe hacer un árbol QPA para
            //      - saber que parte depende de que parte
            //      - saber que podemos usar para cada necesidad

            // * password                       el password de la DB
            // * modo (JsonSUBE|DriveUpApi)     (de donde sacaré los datos y en que forma los procesaré)
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
            string modo = ArgsHelper.SafeGetArgVal(misArgs, "modo", "JsonSUBE");
            
            // idReporte
            string idReporte = ArgsHelper.SafeGetArgVal(misArgs, "idReporte", Guid.NewGuid().ToString());
            
            // desde hasta
            string sDesde = ArgsHelper.SafeGetArgVal(misArgs, "desde", ahora.ToString("yyyy-dd-MM"));
            DateTime desde = DateTime.Parse(sDesde);
            DateTime hasta = desde.AddDays(1);
            
            // lineas
            string sLineas = ArgsHelper.SafeGetArgVal(misArgs, "lineas", string.Empty);
            List<int> lineas = sLineas
                .Split(',')
                .Where(sLinX => EsNumeroEntero(sLinX))
                .Select(sLinX => int.Parse(sLinX.Trim()))
                .ToList()
            ;

            // tipo punta de línea
            Type tipoPuntaLinea = typeof(PuntaLinea);

            // tipo creador partes históricas
            Type tipoCreadorPartesHistoricas = typeof(CreadorPartesHistoricasIdentidad);

            // granularidad en metros
            string sGranularidad = ArgsHelper.SafeGetArgVal(misArgs, "granularidad", "20");

            // radio de las puntas de línea
            string sRadioPuntas = ArgsHelper.SafeGetArgVal(misArgs, "radioPuntas", "800");

            // creo el qpaCreator
            var qpaCreator = new QPACreator.Creator(new CreatorConfiguration
            {
                ConnectionStringFichasXEmprIntSUBE = "Data Source=192.168.201.21;Initial Catalog=general;User ID=sa;Password=" + password,
                ConnectionStringVentasSUBE = "Data Source=192.168.201.42;Initial Catalog=sube;User ID=sa;Password=" + password,
                ConnectionStringPuntosSUBE = "Data Source=192.168.201.10;Initial Catalog=logsTecnobus;User ID=sa;Password=" + password,
                ConnectionStringPuntosXBus = "Data Source=192.168.201.10;Initial Catalog=logsTecnobus;User ID=sa;Password=" + password,
                Repos = new Dictionary<string, string> {
                    { "MockRepo1", "./Datos" },
                    { "MockRepo2", "./Datos" },
                }
            });

            qpaCreator.Aviso += QpaCreator_Aviso;

            // creo el directorio de bajada si no existe...
            var dirBajados = "./Bajados/";
            if (!Directory.Exists(dirBajados))
            {
                Directory.CreateDirectory(dirBajados);
            }
            
            var nombreArchivoLocal = $"driveup_{desde.Year:0000}_{desde.Month:00}_{desde.Day:00}.csv";
            var pathArchivoLocal = Path.Combine(dirBajados, nombreArchivoLocal).Replace('\\', '/');

            if (File.Exists(pathArchivoLocal))
            {
                // ya existe el archivo
                Console.WriteLine($"El archivo {pathArchivoLocal} existe");
            }
            else
            {
                // bajo el nuevo archivo
                int diasMenos = Convert.ToInt32((DateTime.Now.Date.Subtract(desde.Date)).TotalDays);
                var remoteUri = new Uri(
                    "https://vm-coches:5001/HistoriaCochesDriveupAnteriores?formato=csv&diasMenos=" +
                    diasMenos.ToString()
                );
                var bajadorDeArchivo = new BajadorDeArchivo();
                Console.WriteLine("Tratando de bajar archivo driveup ");
                if (!bajadorDeArchivo.BajarArchivo(remoteUri, pathArchivoLocal))
                {
                    Console.WriteLine("No pude bajar el archivo " + bajadorDeArchivo.Error);
                    return -1;
                }
            }

            Console.WriteLine("Tratando de construir información histórica ");
            Dictionary<int, List<PuntoHistorico>> puntosXIdentificador;
            try
            {
                puntosXIdentificador = GetPuntosPorIdentificador(pathArchivoLocal);
                Console.WriteLine("\tInformación historica ok");
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
                lineas,
                tipoPuntaLinea,
                tipoCreadorPartesHistoricas,
                puntosXIdentificador,
                ConstructorFichasInt
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

            var contenido = rpx.RenderAllText(
                rpx.Create(reporteQPA.ListaAplanadaSubItems.Cast<object>(), formatter),
                formatter
            );

            var nombreArchivoReporte = $"Reporte_{idReporte}_Desde_{desde:yyyyMMdd}_Hasta_{hasta:yyyyMMdd}";
            var pathArchivoReporte   = Path.Combine(dirReportes, nombreArchivoReporte);
            File.WriteAllText($"{pathArchivoReporte}.csv", contenido);

            return 0;
        }

        static void QpaCreator_Aviso(object sender, Creator.AvisoEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(e.Mensaje);
        }

        static void Mostrar(string s)
        {
            Console.WriteLine(s);
        }

        static Dictionary<int, List<PuntoHistorico>> GetPuntosPorIdentificador(string nombreArchivoLocal)
        {
            //0      1          2            3                    4                    5
            //Ficha; Lat;       Lng;         FechaLocal;          Recordedat;          FechaLlegadaLocal
            //3749 ; -34.47732; -58.5095933; 2022-07-06 00:00:01; 2022-07-06 03:00:01; 2022-07-06 00:00:00

            var ret = new Dictionary<int, List<PuntoHistorico>>();

            foreach (var sLine in File.ReadLines(nombreArchivoLocal).Skip(1))
            {
                var partes = sLine.Split(';');
                
                if (partes.Length < 5)
                {
                    continue;
                }

                var key = int.Parse(partes[0]);
                var lat = double.Parse(partes[1], CultureInfo.InvariantCulture);
                var lng = double.Parse(partes[2], CultureInfo.InvariantCulture);
                var fec = DateTime.Parse(partes[5]);

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
}
