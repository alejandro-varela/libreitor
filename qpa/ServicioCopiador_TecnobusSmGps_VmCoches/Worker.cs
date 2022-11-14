using ComunApiCoches;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ServicioCopiador_TecnobusSmGps_VmCoches
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker>_logger;
        private readonly WorkerOptions  _config;

        public Worker(ILogger<Worker> logger, WorkerOptions config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                foreach (var baseDirX in _config.CopyDirs)
                {
                    if (!Directory.Exists(baseDirX.Src))
                    {
                        break;
                    }

                    var (ok, dtMasViejo) = FilesHelper.GetFechaArchivoMasViejo(baseDirX.Src);

                    if (ok)
                    {
                        _logger.LogInformation(dtMasViejo.ToString());

                        var dtHasta  = DateTime.Now.Subtract(TimeSpan.FromMinutes(65));
                        var srcPaths = FilesHelper.GetPaths(baseDirX.Src , dtMasViejo, dtHasta);
                        var destPaths= FilesHelper.GetPaths(baseDirX.Dest, dtMasViejo, dtHasta);
                        var tupPaths = Enumerable.Zip(srcPaths, destPaths);

                        foreach (var (src, dest) in tupPaths)
                        {
                            var esteDir = Path.GetDirectoryName(dest);
                            if (!Directory.Exists(esteDir))
                            {
                                Directory.CreateDirectory(esteDir);
                            }

                            if (File.Exists(src) && File.Exists(dest) && !FileCmpHash(src, dest))
                            {
                                // si existen los dos archivos pero son diferentes por dentro...
                                File.Copy(src, dest, true);
                            }
                            else if (File.Exists(src) && !File.Exists(dest))
                            {
                                // si el archivo solo existe en src...
                                File.Copy(src, dest);
                            }
                        }
                    }
                }
                
                // cada 15 minutos
                await Task.Delay(1000 * 60 * 15, stoppingToken);
            }
        }

        private bool FileCmpHash(string src, string dest)
        {
            return FileGetHash(src).SequenceEqual(FileGetHash(dest));
        }

        private byte[] FileGetHash(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filename);
            return md5.ComputeHash(stream);
        }
    }
}
