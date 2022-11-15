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
                _logger.LogInformation($"Worker trabajando {DateTimeOffset.Now}");

                foreach (var baseDirX in _config.CopyDirs)
                {
                    _logger.LogInformation($"\t{baseDirX} {DateTimeOffset.Now}");

                    var existeDirSrc = false;

                    try
                    {
                        existeDirSrc = Directory.Exists(baseDirX.Src);
                    }
                    catch (Exception exx)
                    {
                        _logger.LogError(exx, $"Error al tratar de listar {baseDirX.Src}");
                        break;
                    }

                    if (!existeDirSrc)
                    {
                        _logger.LogInformation($"\tEl directorio {baseDirX.Src} no existe");
                        break;
                    }

                    var (okDtMasViejo, dtMasViejo) = FilesHelper.GetFechaArchivoMasViejo(baseDirX.Src);

                    if (okDtMasViejo)
                    {
                        _logger.LogInformation($"\tFecha archivo mas viejo: {dtMasViejo}");

                        var dtHasta = DateTime.Now.Subtract(TimeSpan.FromMinutes(65));
                        var srcPaths = FilesHelper
                            .GetPaths(baseDirX.Src, dtMasViejo, dtHasta)
                            .Select(p => p.Replace('/', '\\'))
                        ;
                        var destPaths = FilesHelper
                            .GetPaths(baseDirX.Dest, dtMasViejo, dtHasta)
                            .Select(p => p.Replace('/', '\\'))
                        ;
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
                                await CopyFileAsyncWct(src, dest, stoppingToken, true);
                            }
                            else if (File.Exists(src) && !File.Exists(dest))
                            {
                                // si el archivo solo existe en src...
                                await CopyFileAsyncWct(src, dest, stoppingToken);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError($"No pude determinar la fecha del archivo mas viejo de {baseDirX.Src}");
                    }
                }
                
                // cada 15 minutos
                await Task.Delay(1000 * 60 * 15, stoppingToken);
            }
        }

        private async Task CopyFileAsyncWct(string sourceFileName, string destFileName, CancellationToken cancellationToken, bool overwrite = false)
        {
            if (overwrite)
            {
                if (File.Exists(destFileName))
                {
                    File.Delete(destFileName);
                }
            }
            using Stream source     = File.OpenRead (sourceFileName);
            using Stream destination= File.Create   (destFileName);
            await source.CopyToAsync(destination, 4096, cancellationToken);
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
