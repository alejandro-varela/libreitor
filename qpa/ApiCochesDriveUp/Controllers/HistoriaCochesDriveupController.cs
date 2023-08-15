using ComunApiCoches;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;

namespace ApiCochesDriveUp.Controllers
{
    // ej ayer http://server:5000/HistoriaCochesDriveup&formato=csv

    [Route("[controller]")]
    [ApiController]
    public class HistoriaCochesDriveUpController : Controller
    {
        private readonly ILogger<HistoriaCochesDriveUpController> _logger;
        private ApiOptions _apiOptions;

        public HistoriaCochesDriveUpController(
            ILogger<HistoriaCochesDriveUpController> logger,
            ApiOptions apiOptions
        )
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(string formato, string desde, string hasta)
        {
            // validar formato
            var formatosPosibles = new string[] { "csv", "csvnt" };
            if (!formatosPosibles.Contains((formato ?? "").Trim().ToLower()))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError(
                        $"El parámetro \"formato\" debe ser alguno de estos valores: {string.Join(',', formatosPosibles.ToArray())}") +
                    GetHelpText()
                );
            }

            // validar fecha desde
            if (!DateTime.TryParse(desde, out DateTime fechaDesde))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro \"desde\" debe tener el formato ISO8601") +
                    GetHelpText()
                );
            }

            // validar fecha hasta
            if (!DateTime.TryParse(hasta, out DateTime fechaHasta))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro \"hasta\" debe tener el formato ISO8601") +
                    GetHelpText()
                );
            }

            // crear stream y devolverlo
            var trans = HistoriaHelper.GetCSVStream(
                _apiOptions.BaseDir, 
                formato, 
                fechaDesde, 
                fechaHasta
            );

            var fName = $"driveup_desde_{fechaDesde:yyyy_MM_dd_HH_mm_ss}_hasta_{fechaHasta:yyyy_MM_dd_HH_mm_ss}.csv";

            return File(trans, "text/csv", fName);
        }

        static string GetHelpText()
        {
            var sbHelp = new StringBuilder();
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Ejemplos de uso:");
            sbHelp.AppendLine("    /HistoriaCochesDriveUp?formato=csv");
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Aclaraciones:");
            sbHelp.AppendLine("    Parámetro \"Desde\":");
            //sbHelp.AppendLine("        ej: diasMenos=1                    es todo el día de ayer");
            //sbHelp.AppendLine("        ej: diasMenos=2                    es todo el día de anteayer");
            //sbHelp.AppendLine("        y así sucesivamente");
            sbHelp.AppendLine("    Parámetro \"Hasta\":");
            //sbHelp.AppendLine("        ej: formato=csv                    csv con título (es el valor por defecto)");
            //sbHelp.AppendLine("        ej: formato=csvnt                  csv sin título");
            return sbHelp.ToString();
        }
    }
}
