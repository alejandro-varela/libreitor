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

                            if (File.Exists(src) && File.Exists(dest))
                            {
                                // comparo los dos archivos asyncronamente...
                                bool archivosIguales = await FileCmpWct(src, dest, stoppingToken);

                                if (!archivosIguales)
                                {
                                    // si son diferentes
                                    await CopyFileAsyncWct(src, dest, stoppingToken, overwrite: true);
                                }
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
                _logger.LogInformation("##################### Esperandooo #####################");
                await Task.Delay(1000 * 60 * 15, stoppingToken);
            }
        }

        private async Task<bool> FileCmpWct(string sourceFileName, string destFileName, CancellationToken cancellationToken)
        {
            using Stream source     = File.OpenRead(sourceFileName);
            using Stream destination= File.OpenRead(destFileName);

            const int BUFFSIZE = 1024;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                byte[] buffSrc = new byte[BUFFSIZE];
                byte[] buffDst = new byte[BUFFSIZE];

                var leidosSrc = await source     .ReadAsync(buffSrc, 0, BUFFSIZE, cancellationToken);
                var leidosDst = await destination.ReadAsync(buffDst, 0, BUFFSIZE, cancellationToken);

                if (leidosDst != leidosSrc)
                {
                    return false;
                }

                if (leidosSrc <= 0)
                {
                    break;
                }

                int i = 0;
                while (!cancellationToken.IsCancellationRequested && i < leidosSrc)
                {
                    if (buffSrc[i] != buffDst[i])
                    {
                        return false;
                    }
                    i++;
                }
            }

            return true;
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
            await source.CopyToAsync(destination, cancellationToken);
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
