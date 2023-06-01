using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace PrintRec
{
    public class Program
    {
        public static int Main(string[] args)
        {
            //using IHost host = CreateHostBuilder(args).Build();
            //using IServiceScope scope = host.Services.CreateScope();
            //IServiceProvider serviceProvider = scope.ServiceProvider;

            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    //.AddEnvironmentVariables("PREC_") // si no las lee reiniciar VS
                    //.AddUserSecrets(userSecretsId)
                    .AddCommandLine(args)
                    .Build()
                ;

                // si queremos una sección sería
                // var settings = configuration.GetRequiredSection("NombreSeccion").Get<PrintRecSettings>();
                
                var settings = configuration.Get<PrintRecSettings>();

                //var app = serviceProvider.GetRequiredService<App>();
                var app = new App();
                
                var retVal = app.Run(settings);

                return retVal;
            }
            catch (Exception ex)
            {
                ShowErrorLine(ex.ToString());
                return -1;
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSingleton<App>();
                })
            ;
        }

        public static void ShowErrorLine(string sErr)
        { 
            ShowLineWithColor(ConsoleColor.Red, sErr);
        }

        public static void ShowLineWithColor(ConsoleColor color, string text)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }
    }
}
