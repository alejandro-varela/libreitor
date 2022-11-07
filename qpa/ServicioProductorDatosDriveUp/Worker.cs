#nullable enable

using ComunDriveUp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicioProductorDatosDriveUp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker>        _logger;
        private readonly WorkerOptions          _options;

        public Worker(ILogger<Worker> logger, WorkerOptions workerOptions)
        {
            //var tracee = logger.IsEnabled(LogLevel.Trace);
            //var debuge = logger.IsEnabled(LogLevel.Debug);
            //var infore = logger.IsEnabled(LogLevel.Information);
            //var warnie = logger.IsEnabled(LogLevel.Warning);
            //var errore = logger.IsEnabled(LogLevel.Error);
            //var critee = logger.IsEnabled(LogLevel.Critical);

            //logger.LogTrace("Log TRACE");
            //logger.LogDebug("Log DEBUG");
            //logger.LogInformation("Log INFO");
            //logger.LogWarning("Log WARN");
            //logger.LogError("Log ERROR");
            //logger.LogCritical("Log CRIT");

            _logger             = logger;
            _options            = workerOptions;

            logger.LogInformation("Constructor...");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start Async...");
            _logger.LogInformation("Servicio Iniciado");

            _logger.LogInformation("--------------- CONFIG ---------------");
            
            _logger.LogInformation($"Proxy");
            _logger.LogInformation($"\tProxyEnable           : {_options.ProxyEnable}");

            if (_options.ProxyEnable)
            {
                _logger.LogInformation($"\tProxyAddress          : {_options.ProxyConfig.ProxyAddress}");
                _logger.LogInformation($"\tBypassProxyOnLocal    : {_options.ProxyConfig.BypassProxyOnLocal}");
                _logger.LogInformation($"\tUseDefaultCredentials : {_options.ProxyConfig.UseDefaultCredentials}");
            }
            
            _logger.LogInformation($"ReaderConfig");
            _logger.LogInformation($"\tAddress               : {_options.ReaderConfig.Address}");
            _logger.LogInformation($"\tMaxReadBuffer         : {_options.ReaderConfig.MaxReadBuffer} bytes");

            _logger.LogInformation("Output");
            _logger.LogInformation($"\tBaseDir               : {_options.OutputConfig.BaseDir}");
            
            _logger.LogInformation("--------------------------------------");
            
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop Async...");
            _logger.LogInformation("Servicio Parado");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Ejecutando el trabajo");
            
            int MILLIS_TIMEOUT_LECTURA  = 5_000;
            int MILLIS_REINTENTO_CONN   = 1_000;

            var codificacion  = Encoding.UTF8;
            var maxReadBuffer = _options.ReaderConfig.MaxReadBuffer; // ej 16384;
            var authToken     = _options.ReaderConfig.AuthToken;     // ej "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjaWQiOjI2NCwiaWF0IjoxNjUzMDUzMjEzLCJpc3MiOiIxNTUyIiwibG5nIjoiZXMiLCJyb2wiOjE3fQ.lBa0IaKcAaMNrcr3ZVP7yabiK8VFbeG1JYFaI6H3RQ4";
            var addrRemote    = _options.ReaderConfig.Address;       // ej "https://api.driveup.info/stream";
            var requestUri    = new Uri(addrRemote);
            var primeraVez    = true;
            var headers       = new Dictionary<string, string>
            {
                { "x-driveup-token", authToken }
            };
            var fileTimeHelper = new FileTimeHelper();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (primeraVez)
                {
                    primeraVez = false;
                }
                else
                {
                    // fuerzo el garbage collector
                    GC.Collect();
                    // espero un momento (en caso de fallo de red esto se ejecutaría muy rápido varias veces ahogando la CPU)
                    await Task.Delay(millisecondsDelay: MILLIS_REINTENTO_CONN);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("Iniciando httpClient");

                // creo el cliente (httpClient)
                using var httpHandler = new HttpClientHandler
                { 
                    Proxy = _options.ProxyEnable ? GetWebProxy(_options) : null
                };
                using var httpClient = new HttpClient(httpHandler);
                
                // agrego headers al cliente
                foreach (var parHeader in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(parHeader.Key, parHeader.Value);
                }

                try
                {
                    // stream
                    _logger.LogInformation("Obteniendo stream del httpClient");
                    using var stream = await httpClient.GetStreamAsync(requestUri);

                    // leo el stream 
                    var buff = new byte[maxReadBuffer];
                    int read = 0;

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var canTokSrc = new CancellationTokenSource(millisecondsDelay: MILLIS_TIMEOUT_LECTURA);

                        try
                        {
                            var sw = Stopwatch.StartNew();
                            var start = Environment.TickCount;
                            var cntobjs = 0;

                            read = await stream.ReadAsync(buff, canTokSrc.Token);
                            _logger.LogDebug($"\t{read} bytes leídos");

                            if (read <= 0)
                            {
                                _logger.LogError($"No se leyeron datos del stream");
                                break;
                            }

                            var stringa = codificacion.GetString(buff, 0, read);
                            var partes  = stringa.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            cntobjs += partes.Length;

                            _logger.LogDebug($"\tPartes: {partes.Length}");

                            foreach (var parteX in partes)
                            {
                                _logger.LogDebug($"\t\t{parteX}");

                                if (EsJsonValido(parteX))
                                {
                                    //Console.WriteLine(parteX);
                                    Procesar(parteX, fileTimeHelper);
                                }
                            }

                            var elap = Environment.TickCount - start;
                            sw.Stop();

                            _logger.LogDebug($"{sw.ElapsedMilliseconds} millis | {cntobjs} objs");
                        }
                        catch (TaskCanceledException)
                        {
                            _logger.LogError("Timeout leyendo el stream");
                            break;
                        }
                        catch (Exception exx)
                        {
                            _logger.LogError($"Excepción {exx}");
                            break;
                        }
                        finally
                        {
                            canTokSrc.Dispose();
                        }
                    }
                }
                catch (Exception exx)
                {
                    LoguearExcepcion(exx, _logger);
                }
            }

            _logger.LogInformation("Cancellation Token Requested...");
        }

        private void LoguearExcepcion<T>(Exception exx, ILogger<T> logger)
        {
            var sException = exx.ToString();
            logger.LogError($"{sException}");
        }

        private static bool EsJsonValido(string sJson)
        {
            try
            {
                JsonConvert.DeserializeObject(sJson);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Procesar(string sJson, FileTimeHelper fileTimeHelper)
        {
            try
            {
                var driveupData = JsonConvert.DeserializeObject<DatosDriveUp>(sJson);

                DataWrapperV0 dataWrapperV0 = new DataWrapperV0
                {
                    RecvUtc = DateTime.UtcNow,
                    Data = driveupData,
                };

                var jsonV0 = JsonConvert.SerializeObject(dataWrapperV0);

                if (jsonV0 == null)
                {
                    // tiro una except...
                    // TODO:
                }
                else
                {
                    // eso es lo que vamos a evitar...
                    // la escritura será directa
                    // _datosRecibidos?.Enqueue(dataWrapperV0);

                    // escribo directo en el archivo...
                    int ficha = 0;
                    
                    if (dataWrapperV0.Data != null)
                    {
                        ficha = dataWrapperV0.Data.Ficha;
                    }
                    
                    //redis.HashSet("DU-WrappedDataXFicha", ficha, jsonV0);
                    //redis.HashSet("DU-WrappedDataXPlate", dataWrapperV0.Data.Plate, jsonV0);

                    // escribo los datos en archivos
                    var (escrArchOk, escrArchExx) = fileTimeHelper.WriteToFile(
                        _options.OutputConfig.BaseDir, DateTime.Now, $"{jsonV0}\r\n"
                    );

                    if (escrArchOk)
                    {
                        //redis.StringSet("DU-LastFSWrite-Ok-DateTime", DateTime.Now.ToString("yyyy-MM-ss HH:mm:ss"));
                    }
                    else
                    {
                        //redis.StringSet("DU-LastFSWrite-Err-DateTime", DateTime.Now.ToString("yyyy-MM-ss HH:mm:ss"));
                        //redis.StringSet("DU-LastFSWrite-Err-Description", (escrArchExx?.ToString() ?? ""));
                    }
                }
            }
            catch
            {
                //_logger.LogError("No pude procesar: " + sJson);
            }
        }

        static IWebProxy GetWebProxy(WorkerOptions options)
        {
            var networkCredential = new NetworkCredential
            {
                Domain   = options.ProxyConfig.ProxyCredentials.Domain,
                UserName = options.ProxyConfig.ProxyCredentials.UserName,
                Password = options.ProxyConfig.ProxyCredentials.Password,
            };

            var proxy = new WebProxy
            {
                Address               = new Uri(options.ProxyConfig.ProxyAddress),
                BypassProxyOnLocal    = options.ProxyConfig.BypassProxyOnLocal,
                UseDefaultCredentials = options.ProxyConfig.UseDefaultCredentials,
                Credentials           = networkCredential,
            };

            return proxy;
        }
    }
}
