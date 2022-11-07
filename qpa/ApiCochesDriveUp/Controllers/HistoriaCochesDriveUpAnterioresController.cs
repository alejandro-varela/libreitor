using ComunDriveUp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApiCochesDriveUp.Controllers
{
    // ej ayer https://vm-coches:5001/HistoriaCochesDriveupAnteriores?diasMenos=1&formato=csv

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
        public IActionResult Get(string formato, string diasMenos, string filtro)
        {
            // formato => formatoSanitizado
            var (formatoOk, formatoSanitizado) = ValidadorComun.ProcesarFormato(formato);

            if (!formatoOk)
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro \"formato\" debe ser alguno de estos valores: { ValidadorComun.GetFormatosPosibles() }") +
                    DameAyuda()
                );
            }

            // diasMenos => nDiasMenos
            var (diasMenosOk, nDiasMenos) = ValidadorComun.ProcesarDiasMenos(diasMenos);

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

            // si el formato es csv o csvnt
            if (formatoSanitizado == "csv" || formatoSanitizado == "csvnt")
            {
                // tomo los paths de los archivos según desde hasta
                var paths = FilesHelper
                    .GetPaths(_apiOptions.BaseDir, fechaDesde, fechaHasta)
                    .Where   (path => System.IO.File.Exists(path))
                    .ToList  ()
                ;

                // se lo paso a un bolsaStream -> streamReader -> transStream + convertidorCsv
                BolsaStream  bolsa        = new BolsaStream (paths);
                StreamReader streamReader = new StreamReader(bolsa);
                Func<string, string> convertirACSV = null;

                if (formatoSanitizado == "csv")
                {
                    //convertidorCSV = ConvertidorACsvConTitulo;
                    var convertidorCSV = new ConvertidorCSV<DatosDriveUp> { 
                        ConTitulo = true, 
                        DatosADiccionario = DatosADiccionario 
                    };
                    convertirACSV = convertidorCSV.Convertir;
                }
                else if (formatoSanitizado == "csvnt")
                {
                    //convertidorCSV = ConvertidorACsvSinTitulo;
                    var convertidorCSV = new ConvertidorCSV<DatosDriveUp>
                    {
                        ConTitulo = false,
                        DatosADiccionario = DatosADiccionario
                    };
                    convertirACSV = convertidorCSV.Convertir;
                }

                TransStream trans = new TransStream(streamReader, convertirACSV);
                var fName = $"driveup_{fechaDesde:yyyy_MM_dd}.csv";
                return File(trans, "text/csv", fName);
            }
            else
            {
                // json
                var retVals = FilesHelper
                    .GetDatos(_apiOptions.BaseDir, fechaDesde, fechaHasta)
                    .Select(x => DatosADiccionario(x))
                ;

                return Ok(retVals);
            }
        }

        Dictionary<string, object> DatosADiccionario(DatosDriveUp x)
        {
            return new Dictionary<string, object>
            {
                { "Ficha"       , x.Ficha       },
                { "Lat"         , x.Lat         },
                { "Lng"         , x.Lng         },
                { "FechaLocal"  , x.FechaLocal  },
                { "Recordedat"  , x.Recordedat  },
            };
        }

        string DameAyuda()
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
