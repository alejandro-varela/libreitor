using ComunApiCoches;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCochesTecnobusSmGps.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HistoriaCochesTecnobusSmGpsAnterioresController : Controller
    {
        private readonly ILogger<HistoriaCochesTecnobusSmGpsAnterioresController> _logger;
        private readonly ApiOptions _apiOptions;

        public HistoriaCochesTecnobusSmGpsAnterioresController(ILogger<HistoriaCochesTecnobusSmGpsAnterioresController> logger, ApiOptions apiOptions)
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(string formato, string diasMenos, string filtro)
        {
            var validador = new ValidadorComun();

            // formato => formatoSanitizado
            var (formatoOk, formatoSanitizado) = validador.ProcesarFormato(formato);

            if (!formatoOk)
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro \"formato\" debe ser alguno de estos valores: { validador.FormatosPosibles }") +
                    DameAyuda()
                );
            }

            // diasMenos => nDiasMenos
            var (diasMenosOk, nDiasMenos) = validador.ProcesarDiasMenos(diasMenos);

            if (!diasMenosOk)
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro diasMenos debe ser un número entero positivo pero era: {diasMenos}") +
                    DameAyuda()
                );
            }

            // calculo fechas desde hasta según los días menos
            DateTime fechaDesde = DateTime.Now.AddDays(-nDiasMenos).Date;
            DateTime fechaHasta = fechaDesde.AddDays(1);

            // tengo que leer de tres lugares diferentes en vez de uno...
            // tengo que sumar los tres archivos y ordenarlos...

            return Ok();
        }

        string DameAyuda()
        {
            var sbHelp = new StringBuilder();
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Ejemplos de uso:");
            sbHelp.AppendLine("    /HistoriaCochesTecnobusSmGpsAnteriores?formato=csv&diasmenos=1");
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
