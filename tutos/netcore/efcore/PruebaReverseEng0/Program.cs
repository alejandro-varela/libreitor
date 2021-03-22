using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PruebaReverseEng0
{
    class Program
    {
        static void Main(string[] args)
        {
            using (logsTecnobusContext ctx = new logsTecnobusContext())
            {
                var logs = ctx.LogGpsSinCalculoAtrasoYexcesoVelocidads
                    .ToList()
                    .Take(100)
                    .AsQueryable<LogGpsSinCalculoAtrasoYexcesoVelocidad>()
                ;

                var iq = (
                    from x in ctx.LogGpsSinCalculoAtrasoYexcesoVelocidads
                    select x
                ).Take(100);

                foreach (var log in logs)
                {
                    Console.WriteLine($"línea: {log.CodLinea,-10} | bandera: {log.CodBandera,-15}");
                }

                var sql_iq = iq.ToQueryString();
                Console.WriteLine("sql iq   = " + sql_iq);

                var sql_logs = logs.ToQueryString();
                Console.WriteLine("sql logs = " + sql_logs);
            }
        }
    }
}
