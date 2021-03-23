using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PruebaReverseEng0
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>(optional: false, reloadOnChange: true)
                // .AddJsonFile("appsettings.json")
                // .AddEnvironmentVariables()
                // .AddCommandLine(args)
                .Build();

            using (logsTecnobusContext ctx = new logsTecnobusContext(configuration))
            {
                // var logs = ctx.LogGpsSinCalculoAtrasoYexcesoVelocidads
                //     .ToList()
                //     .Take(100)
                // ;

                var bandes = new int[] { 183, 344, 484, 677, 333 };

                var iq = (
                    from x in ctx.LogGpsSinCalculoAtrasoYexcesoVelocidads
                    where 
                        bandes.Contains(x.CodBandera ?? -1) &&
                        (x.NroEquipo ?? -1) >= 1000000 &&
                        (x.NroEquipo ?? -1) <= 4000000 &&
                        true
                    select x
                )
                .Take(100);

                foreach (var log in iq)
                {
                    Console.WriteLine($"equipo: {log.NroEquipo,-10} línea: {log.CodLinea,-10} | bandera: {log.CodBandera,-15}");
                }

                var sql_iq = iq.ToQueryString();
                Console.WriteLine("sql iq   = " + sql_iq);
            }
        }
    }
}
