using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioCopiador_TecnobusSmGps_VmCoches
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
            return Host
                .CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    // esto es para la conf
                    IConfiguration configuration = hostContext.Configuration;
                    WorkerOptions opts = configuration.GetSection("Configu").Get<WorkerOptions>();
                    services.AddSingleton(opts);
                    // otros
                    services.AddHostedService<Worker>();
                });
        }
    }
}
