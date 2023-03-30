using ComunApiCoches;
using ComunDriveUp;
using ComunStreams;
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
    // ej ayer  http://vm-coches:5000/DatosCochesDriveupAnteriores?diasMenos=1&formato=csv
    // ej ayer https://vm-coches:5001/DatosCochesDriveupAnteriores?diasMenos=1&formato=csv

    [Route("[controller]")]
    [ApiController]
    public class DatosCochesDriveUpAnterioresController : Controller
    {
        private readonly ILogger<DatosCochesDriveUpAnterioresController> _logger;
        private ApiOptions _apiOptions;

        public DatosCochesDriveUpAnterioresController(
            ILogger<DatosCochesDriveUpAnterioresController> logger,
            ApiOptions apiOptions
        )
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

            // si el formato es csv o csvnt
            if (formatoSanitizado == "csv" || formatoSanitizado == "csvnt")
            {
                // tomo los paths de los archivos según desde hasta
                var paths = FilesHelper
                    .GetPaths(_apiOptions.BaseDir, fechaDesde, fechaHasta)
                    .Where(path => System.IO.File.Exists(path))
                    .ToList()
                ;

                // se lo paso a un bolsaStream -> streamReader -> transStream + convertidorCsv
                BolsaStream bolsa = new BolsaStream(paths);
                StreamReader streamReader = new StreamReader(bolsa);
                Func<string, string> convertirACSV = null;

                if (formatoSanitizado == "csv")
                {
                    //convertidorCSV = ConvertidorACsvConTitulo;
                    var convertidorCSV = new ConvertidorCSV<DatosDriveUp>
                    {
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
                var fName = $"driveup_data_{fechaDesde:yyyy_MM_dd}.csv";
                return File(trans, "text/csv", fName);
            }
            else
            {
                // json
                var retVals = FilesHelperForDriveup
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
                { "Ficha"         , x.Ficha       },
                { "Lat"           , x.Lat         },
                { "Lng"           , x.Lng         },
                
                //Pedido por Rafa
                { "AcceleratorPedalPosition", x.AcceleratorPedalPosition },
                { "EngineSpeed"   , x.EngineSpeed },
                { "Speed"         , x.Speed       },
                { "FuelLevel"     , x.FuelLevel   },
                //Fin pedido por Rafa

                //Pedido por Rafa (otra vez)
                { "FuelRate"      , x.FuelRate },
                { "InstantFuelEco", x.InstantFuelEco },
                //Fin pedido por Rafa (otra vez)

                { "FechaLocal"    , x.FechaLocal  },
                { "Recordedat"    , x.Recordedat  },
            };
        }

        string DameAyuda()
        {
            var sbHelp = new StringBuilder();
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Ejemplos de uso:");
            sbHelp.AppendLine("    /DatosCochesDriveUpAnteriores?formato=csv&diasmenos=1");
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
