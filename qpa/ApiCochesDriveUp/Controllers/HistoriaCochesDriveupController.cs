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
            // formato
            if (string.IsNullOrEmpty(formato))
            {
                formato = "csv";
            }
            var formatosPosibles = new string[] { "csv", "csvnt" };
            if (!formatosPosibles.Contains((formato ?? "").Trim().ToLower()))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError(
                        $"El parámetro \"formato\" debe ser alguno de estos valores: {string.Join(',', formatosPosibles.ToArray())}") +
                    GetHelpText()
                );
            }

            // fecha desde
            if (!DateTime.TryParse(desde, out DateTime fechaDesde))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro \"desde\" debe tener el formato ISO8601") +
                    GetHelpText()
                );
            }

            // fecha hasta
            if (string.IsNullOrEmpty(hasta))
            {
                var fechaDesdeMasUnDia = fechaDesde.AddDays(1);
                hasta = $"{fechaDesdeMasUnDia.Year:0000}-{fechaDesdeMasUnDia.Month:00}-{fechaDesdeMasUnDia.Day:00}T00:00:00";
            }
            if (!DateTime.TryParse(hasta, out DateTime fechaHasta))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro \"hasta\" debe tener el formato ISO8601") +
                    GetHelpText()
                );
            }

            // crear stream y devolverlo
            var csvStream = HistoriaHelper.GetCSVStream(
                _apiOptions.BaseDir, 
                formato, 
                fechaDesde, 
                fechaHasta
            );

            var fName = $"driveup_desde_{fechaDesde:yyyy_MM_dd_HH_mm_ss}_hasta_{fechaHasta:yyyy_MM_dd_HH_mm_ss}.csv";

            return File(csvStream, "text/csv", fName);
        }

        static string GetHelpText()
        {
            var sbHelp = new StringBuilder();
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Ejemplos de uso:");
            sbHelp.AppendLine("    /HistoriaCochesDriveUp?desde=2023-08-10");
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Aclaraciones:");
            sbHelp.AppendLine("    Parámetro \"formato\":");
            sbHelp.AppendLine("        csv con título (es el valor por defecto):");
            sbHelp.AppendLine("          ej: formato=csv");
            sbHelp.AppendLine("        csv sin título:");
            sbHelp.AppendLine("          ej: formato=csvnt");
            sbHelp.AppendLine("    Parámetro \"desde\":");
            sbHelp.AppendLine("        ej: desde=2023-08-10");
            //sbHelp.AppendLine("        ej: desde=2023-08-10T15:30:05");
            sbHelp.AppendLine("    Parámetro \"hasta\":");
            sbHelp.AppendLine("        ej: desde=2023-08-11");
            //sbHelp.AppendLine("        ej: desde=2023-08-11T20:45:10");

            return sbHelp.ToString();
        }
    }
}
