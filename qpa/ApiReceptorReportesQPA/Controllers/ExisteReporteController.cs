using Comun;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiReceptorReportesQPA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExisteReporteController : Controller
    {
        ILogger<ExisteReporteController> _logger;
        ApiOptions _apiOptions;

        public ExisteReporteController(
            ILogger<ExisteReporteController> logger,
            ApiOptions apiOptions
        )
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(string rfname)
        {
            var path = Path.Combine(_apiOptions.ReportesBaseDir, rfname);
            var existe = System.IO.File.Exists(path);

            if (existe)
            {
                var buff = System.IO.File.ReadAllBytes(path);
                return Ok(new RespuestaExisteReporte { Existe = true, FName = rfname, Sha1 = Hashes.GetSHA1String(buff) });
            }
            else
            {
                return Ok(new RespuestaExisteReporte { Existe = false, FName = rfname });
            }

        }
    }

    public class RespuestaExisteReporte
    {
        public bool Existe { get; set; }
        public string FName { get; set; }
        public string Sha1 { get; set; }
    }
}
