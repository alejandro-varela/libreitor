using ComunApiCoches;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Copiador_PuntosTecnobus_SmGps_VmCoches
{
    public class Worker
    {
        public async Task Work(ILogger _logger, WorkerOptions _config)
        {
            _logger.Information($"Worker trabajando {DateTimeOffset.Now}");

            foreach (var baseDirX in _config.CopyDirs)
            {
                _logger.Information($"\t{baseDirX} {DateTimeOffset.Now}");

                var existeDirSrc = false;

                try
                {
                    existeDirSrc = Directory.Exists(baseDirX.Src);
                }
                catch (Exception exx)
                {
                    _logger.Error(exx, $"Error al tratar de listar {baseDirX.Src}");
                    break;
                }

                if (!existeDirSrc)
                {
                    _logger.Information($"\tEl directorio {baseDirX.Src} no existe");
                    break;
                }

                var (okDtMasViejo, dtMasViejo) = FilesHelper.GetFechaArchivoMasViejo(baseDirX.Src);

                if (okDtMasViejo)
                {
                    _logger.Information($"\tFecha archivo mas viejo: {dtMasViejo}");

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

                        //if (File.Exists(src) && File.Exists(dest))
                        //{
                        //    // comparo los dos archivos asyncronamente...
                        //    bool archivosIguales = await FileCmpWct(src, dest);
                        //    if (!archivosIguales)
                        //    {
                        //        // si son diferentes
                        //        await CopyFileAsync(src, dest, overwrite: true);
                        //    }
                        //}

                        if (File.Exists(src) && !File.Exists(dest))
                        {
                            // si el archivo solo existe en src...
                            _logger.Information($"Copiando {src} a {dest}");
                            await CopyFileAsync(src, dest);
                        }
                    }
                }
                else
                {
                    _logger.Error($"No pude determinar la fecha del archivo mas viejo de {baseDirX.Src}");
                }
            }
        }

        private async Task<bool> FileCmpWct(string sourceFileName, string destFileName)
        {
            using Stream source = File.OpenRead(sourceFileName);
            using Stream destination = File.OpenRead(destFileName);

            const int BUFFSIZE = 1024;

            while (/*!cancellationToken.IsCancellationRequested*/ true)
            {
                byte[] buffSrc = new byte[BUFFSIZE];
                byte[] buffDst = new byte[BUFFSIZE];

                var leidosSrc = await source.ReadAsync(buffSrc, 0, BUFFSIZE);
                var leidosDst = await destination.ReadAsync(buffDst, 0, BUFFSIZE);

                if (leidosDst != leidosSrc)
                {
                    return false;
                }

                if (leidosSrc <= 0)
                {
                    break;
                }

                int i = 0;
                while (/*!cancellationToken.IsCancellationRequested &&*/ i < leidosSrc)
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

        private async Task CopyFileAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            if (overwrite)
            {
                if (File.Exists(destFileName))
                {
                    File.Delete(destFileName);
                }
            }
            using Stream source = File.OpenRead(sourceFileName);
            using Stream destination = File.Create(destFileName);
            await source.CopyToAsync(destination);
        }

        //private bool FileCmpHash(string src, string dest)
        //{
        //    return FileGetHash(src).SequenceEqual(FileGetHash(dest));
        //}

        //private byte[] FileGetHash(string filename)
        //{
        //    using var md5 = MD5.Create();
        //    using var stream = File.OpenRead(filename);
        //    return md5.ComputeHash(stream);
        //}
    }
}
