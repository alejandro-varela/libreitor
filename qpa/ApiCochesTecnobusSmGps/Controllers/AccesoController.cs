using ComunApiCoches;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCochesTecnobusSmGps.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class AccesoController : Controller
    {
        private readonly ILogger<AccesoController> _logger;
        private ApiOptions _apiOptions;

        public AccesoController(
            ILogger<AccesoController> logger,
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

            // construyo el resultado
            var resultado = new List<List<string>>();
            foreach (var baseDirX in _apiOptions.BaseDirs)
            {
                var paths = FilesHelper
                    .GetPaths(baseDirX, fechaDesde, fechaHasta)
                    .Select(path => path.Replace('/', '\\'))
                    .Select(path => path + Probar(path))
                    //.Where      (path => System.IO.File.Exists(path))
                    .ToList()
                ;
                resultado.Add(paths);
            }
            
            return Ok(resultado);
        }

        private string Probar(string path)
        {
            try
            {
                var pepe = System.IO.File
                    .ReadLines(path)
                    .FirstOrDefault()
                ;
                return pepe;
            }
            catch (Exception exx) 
            {
                return exx.Message;
            }
        }
    }
}
