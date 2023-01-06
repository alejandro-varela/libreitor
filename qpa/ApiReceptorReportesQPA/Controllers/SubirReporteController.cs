using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiReceptorReportesQPA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubirReporteController : Controller
    {
        ILogger<SubirReporteController> _logger;
        ApiOptions _apiOptions;

        public SubirReporteController(
            ILogger<SubirReporteController> logger,
            ApiOptions apiOptions
        )
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpPost]
        public IActionResult Post(
            [FromForm] FileInformation fileInformation
        )
        {
            var dirBase = _apiOptions.ReportesBaseDir;
            var newFile = System.IO.Path.Combine(dirBase, fileInformation.Name);

            try
            {
                using (var wstream = System.IO.File.Create(newFile))
                using (var rstream = fileInformation.File.OpenReadStream())
                {
                    for (; ; )
                    {
                        int byteLeido = rstream.ReadByte();
                        if (byteLeido == -1)
                        {
                            break;
                        }
                        wstream.WriteByte((byte)byteLeido);
                    }
                }

                return Ok(new SubirReporteRespuesta
                {
                    Error = false,
                });
            }
            catch (Exception exx)
            {
                return StatusCode(500, new SubirReporteRespuesta
                {
                    Error = true,
                    MsgError = exx.Message,
                });
            }
        }
    }

    public class FileInformation
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }

    public class SubirReporteRespuesta
    {
        public bool Error { get; set; }
        public string MsgError { get; set; }
    }
}
