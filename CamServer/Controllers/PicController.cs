using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// curl -k -d "x=5000" -X POST https://localhost:5001/Pic/

namespace CamServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PicController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PicController> _logger;

        public PicController(ILogger<PicController> logger)
        {
            _logger = logger;
        }

        // [HttpGet]
        // public IEnumerable<WeatherForecast> Get()
        // {
        //     var rng = new Random();
        //     return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //     {
        //         Date = DateTime.Now.AddDays(index),
        //         TemperatureC = rng.Next(-20, 75),
        //         Summary = Summaries[rng.Next(Summaries.Length)]
        //     })
        //     .ToArray();
        // }

        [HttpGet]
        public string Get([FromQuery] int x)
        {
            return (x * 2).ToString();
        }

        [HttpPost]
        public string Post([FromForm] int x)
        {
            return (x * 3).ToString();
        }
    }
}
