using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

// https://www.campusmvp.es/recursos/post/como-manejar-trazas-en-net-core-con-serilog.aspx

namespace ServicioProductorDatosDriveUp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Workingdir
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            // Run
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var loggerConfiguiration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path                   : "log.txt",     // nombre original
                    fileSizeLimitBytes     : 1024 * 1024,   // tamaño maximo en bytes
                    rollOnFileSizeLimit    : true,          // rodar cuando llegue al límite de tamaño
                    retainedFileCountLimit : 10,            // guardar x archivos
                    encoding               : Encoding.UTF8  // codificar en utf8
                )
            ;
            
            var logger = loggerConfiguiration.CreateLogger();

            var builder = Host
                .CreateDefaultBuilder(args)
                .UseSystemd()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    // esto es para la conf
                    IConfiguration configuration = hostContext.Configuration;
                    WorkerOptions opts = configuration.GetSection("Configu").Get<WorkerOptions>();
                    services.AddSingleton(opts);
                    // otros
                    services.AddHostedService<Worker>();
                })
                .ConfigureLogging((loggingBuilder) =>
                {
                    loggingBuilder.ClearProviders();
                })
                .UseSerilog(logger)
            ;

            return builder;
        }
    }
}
