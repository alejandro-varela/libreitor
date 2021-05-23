using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace PruebaEFMariaDB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Models.MiDbContext _context;

        public WeatherForecastController(Models.MiDbContext context, ILogger<WeatherForecastController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var resultadosAsync = await _context.VwAuthUsers
                .Where      (x => x.Name.Contains("jandro"))
                .ToListAsync()
            ;

            var pepe = resultadosAsync
                .FirstOrDefault()
                .Name
            ;

            var rng = new Random();
            var arr = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date         = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary      = Summaries[rng.Next(Summaries.Length)] + ":::" + pepe
                })
                .ToArray();

            return arr;
        }
    }
}
