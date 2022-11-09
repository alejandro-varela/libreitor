using ComunApiCoches;
using ComunStreams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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

            // *** tengo que leer de tres lugares diferentes en vez de uno...
            // *** tengo que sumar los tres archivos y ordenarlos...

            var archivosPorFecha = new Dictionary<DateTime, List<string>>();
            foreach (var baseDirX in _apiOptions.BaseDirs)
            {
                var files = FilesHelper.GetPaths(baseDirX, fechaDesde, fechaHasta);

                foreach (var fileX in files)
                {
                    var (okFileDateTime, fileDateTime) = FileTimeHelper.GetFileDameTime(fileX);

                    if (okFileDateTime)
                    {
                        if (!archivosPorFecha.ContainsKey(fileDateTime))
                        {
                            archivosPorFecha.Add(
                                fileDateTime, // la clave del diccionario es la fecha
                                new List<string>() // el valor del diccionario es una lista de nombres de archivo
                            );
                        }

                        // ingreso nombre de archivo con la fecha en la que pertenece
                        archivosPorFecha[fileDateTime].Add(fileX);
                    }
                }
            }

            // >>> básicamente tengo que mergear 3 bolsas
            
            // creo las bolsas
            List<BolsaStream> bolsas = new List<BolsaStream>();
            foreach (var baseDirX in _apiOptions.BaseDirs)
            {
                var files = FilesHelper.GetPaths(baseDirX, fechaDesde, fechaHasta);
                var bolsa = new BolsaStream(files.ToList());
                bolsas.Add(bolsa);
            }

            // creo los StreamReaders para cada bolsa
            var textReaders = bolsas
                .Select(b => (TextReader) new StreamReader(b))
                .ToList()
            ;

            // creo el merge de bolsas
            MergeTextReader<DateTime> mergeTextReader = new MergeTextReader<DateTime>(
                textReaders,
                SelectorOrdenDatetime
            );

            // aca debo convertir el MergeTextReader a un stream...
            TransStream transStream = new TransStream(
                mergeTextReader, 
                FuncionTransformadora
            );

            //return Ok(archivosPorFecha.Keys);
            var fName = $"tecnobussmg_{fechaDesde:yyyy_MM_dd}.csv";
            return File(transStream, "text/csv", fName);
        }

        private string FuncionTransformadora(string sRenglon)
        {
            // 0000;1111111111111111;2222222222222222;3333333333333333333
            // 4241;33.0381507873535;60.6574440002441;2022-11-09 05:00:03
            var partes = sRenglon.Split(";", StringSplitOptions.RemoveEmptyEntries);

            // ficha
            var sFicha = partes[0];
            
            // lat
            var sLat   = partes[1];
            if (sLat == "0")
            {
                return null;
            }
            if (!sLat.StartsWith("-"))
            {
                sLat = "-" + sLat;
            }
            sLat = sLat.PadRight(9, '0').Substring(0, 8);

            // lng
            var sLng   = partes[2];
            if (sLng == "0")
            {
                return null;
            }
            if (!sLng.StartsWith("-"))
            {
                sLng = "-" + sLng;
            }
            sLng = sLng.PadRight(9, '0').Substring(0, 8);

            // date
            var sDate  = partes[3];

            var sNuevoRenglon = $"{sFicha};{sLat};{sLng};{sDate}";

            return sNuevoRenglon;
        }
        private DateTime SelectorOrdenDatetime(string sRenglon)
        {
            // 0000;1111111111111111;2222222222222222;3333333333333333333
            // 4241;33.0381507873535;60.6574440002441;2022-11-09 05:00:03
            var partes = sRenglon.Split(";", StringSplitOptions.RemoveEmptyEntries);
            return DateTime.Parse(partes[3]);
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
