using ComunApiCoches;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServicioCopiador_TecnobusSmGps_VmCoches
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var (ok1, dtMasViejo) = FilesHelper.GetFechaArchivoMasViejo("\\\\sm-gps-1\\datosposicionamiento");
                var (ok2, dtMasNuevo) = FilesHelper.GetFechaArchivoMasNuevo("\\\\sm-gps-1\\datosposicionamiento");

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation(dtMasViejo.ToString());
                _logger.LogInformation(dtMasNuevo.ToString());

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
