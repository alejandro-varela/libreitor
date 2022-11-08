using ComunApiCoches;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCochesDriveUp.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AccesoDriveupController: Controller
    {
        private readonly ILogger<AccesoDriveupController> _logger;
        private ApiOptions _apiOptions;

        public AccesoDriveupController(
            ILogger<AccesoDriveupController> logger,
            ApiOptions apiOptions
        )
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(string diasMenos)
        {
            var nDiasMenos = int.Parse(diasMenos ?? "0");
            
            // calculo fechas desde hasta según los días menos
            DateTime fechaDesde = DateTime.Now.AddDays(-nDiasMenos).Date;
            DateTime fechaHasta = fechaDesde.AddDays(1);

            var paths = FilesHelper
                .GetPaths(_apiOptions.BaseDir, fechaDesde, fechaHasta)
                .Where(path => System.IO.File.Exists(path))
                .ToList()
            ;

            return Ok(paths);
        }
    }
}
