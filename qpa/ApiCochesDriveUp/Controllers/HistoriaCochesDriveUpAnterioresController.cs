using ComunApiCoches;
using ComunDriveUp;
using ComunStreams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApiCochesDriveUp.Controllers
{
    // ej ayer http://server:5000/HistoriaCochesDriveupAnteriores?diasMenos=1&formato=csv

    [Route("[controller]")]
    [ApiController]
    public class HistoriaCochesDriveUpAnterioresController : Controller
    {
        private readonly ILogger<HistoriaCochesDriveUpAnterioresController> _logger;
        private ApiOptions _apiOptions;

        public HistoriaCochesDriveUpAnterioresController(
            ILogger<HistoriaCochesDriveUpAnterioresController> logger,
            ApiOptions apiOptions
        )
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(string formato, string diasMenos)
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

            // días menos
            bool diasMenosParseOk = int.TryParse(diasMenos, out int nDiasMenos);
            if (! diasMenosParseOk || nDiasMenos < 0)
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro diasMenos debe ser un número entero positivo pero era: {diasMenos}") +
                    GetHelpText()
                );
            }

            // calculo fechas desde/hasta según los días menos
            DateTime fechaDesde = DateTime.Now.AddDays(-nDiasMenos).Date;
            DateTime fechaHasta = fechaDesde.AddDays(1);

            // crear stream y devolverlo
            var csvStream = HistoriaHelper.GetCSVStream(
                _apiOptions.BaseDir,
                formato,
                fechaDesde,
                fechaHasta
            );

            var fName = $"driveup_{fechaDesde:yyyy_MM_dd}.csv";

            return File(csvStream, "text/csv", fName);
        }

        static string GetHelpText()
        {
            var sbHelp = new StringBuilder();
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Ejemplos de uso:");
            sbHelp.AppendLine("    /HistoriaCochesDriveUpAnteriores?formato=csv&diasmenos=1");
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Aclaraciones:");
            sbHelp.AppendLine("    Parámetro \"DiasMenos\":");
            sbHelp.AppendLine("        ej: diasMenos=1                    es todo el día de ayer");
            sbHelp.AppendLine("        ej: diasMenos=2                    es todo el día de anteayer");
            sbHelp.AppendLine("        y así sucesivamente");
            sbHelp.AppendLine("    Parámetro \"Formato\":");
            sbHelp.AppendLine("        ej: formato=csv                    csv con título (es el valor por defecto)");
            sbHelp.AppendLine("        ej: formato=csvnt                  csv sin título");
            return sbHelp.ToString();
        }
    }
}
