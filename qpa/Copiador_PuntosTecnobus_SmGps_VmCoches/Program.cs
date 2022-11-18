using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Copiador_PuntosTecnobus_SmGps_VmCoches
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Logger...
            var loggerConfiguiration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "log.txt",     // nombre original
                    fileSizeLimitBytes: 1024 * 1024,   // tamaño maximo en bytes
                    rollOnFileSizeLimit: true,          // rodar cuando llegue al límite de tamaño
                    retainedFileCountLimit: 10,            // guardar x archivos
                    encoding: Encoding.UTF8  // codificar en utf8
                )
            ;

            var logger = loggerConfiguiration.CreateLogger();

            // Configuración...
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false)
            ;

            IConfiguration configuration = builder.Build();

            var workerOptions = configuration
                .GetSection("Configu")
                .Get<WorkerOptions>()
            ;

            // Trabajo...
            await new Worker().Work(logger, workerOptions);
        }
    }
}
