using ComunApis;
using ComunStreams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

// https://vm-coches:5003/HistoriaCochesPicoBusAnteriores?formato=csv&diasmenos=1

namespace ApiCochesPicoBus.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HistoriaCochesPicoBusAnterioresController : Controller
    {
        private readonly ILogger<HistoriaCochesPicoBusAnterioresController> _logger;
        private readonly ApiOptions _apiOptions;

        public HistoriaCochesPicoBusAnterioresController(ILogger<HistoriaCochesPicoBusAnterioresController> logger, ApiOptions apiOptions)
        {
            _logger = logger;
            _apiOptions = apiOptions;
        }

        [HttpGet]
        public IActionResult Get(string formato, string diasMenos, string filtro)
        {
            // verifico que exista el formato
            var formatos = new List<string>()
            {
                "json",
                "csv",
                "csvnt",
            };

            // si no viene el formato asumo csv
            if (string.IsNullOrEmpty(formato))
            {
                formato = "csv";
            }

            // formateo el formato :P
            formato = formato.Trim().ToLower();

            // valido que exista el formato
            if (!formatos.Contains(formato))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro \"formato\" debe ser alguno de estos: {formatos}") +
                    DameAyuda()
                );
            }

            // valido que exista la cantidad de días menos
            if (string.IsNullOrEmpty(diasMenos) || !StringEsNumeroEntero(diasMenos))
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"El parámetro diasMenos debe ser un número entero positivo pero era: {diasMenos}") +
                    DameAyuda()
                );
            }

            ///////////////////////////////////////////////////////////////////
            // trabajo principal

            // calculo los días
            int cantDiasMenos = int.Parse(diasMenos);
            DateTime fechaDesde = DateTime.Now.AddDays(-cantDiasMenos).Date;
            DateTime fechaHasta = fechaDesde.AddDays(1);

            if (formato == "csv" || formato == "csvnt")
            {
                // tomo los paths de los archivos según desde hasta
                var paths = FilesHelper
                    .GetPaths(_apiOptions.BaseDir, fechaDesde, fechaHasta)
                    .Where(path => System.IO.File.Exists(path))
                    .ToList()
                ;

                // transformer...
                var primeraVez = true;
                var acomodador = new AcomodadorLineaCsvPicoBus1();
                Func<string, string> transformer = (s) =>
                {
                    string ret = string.Empty;
                    if (primeraVez && formato == "csv")
                    {
                        // poner título acá
                        ret += acomodador.GetTitulo().Trim();
                        ret += "\n";
                        primeraVez = false;
                    }
                    // si la linea s está ok...
                    if (acomodador.Validar(s))
                    {
                        // acomodar la línea como quiero aca...
                        ret += acomodador.Acomodar(s);
                    }
                    // retorno lo que se haya armado...
                    return ret;
                };

                // pongo los paths en un bolsaStream
                var bolsaStream  = new BolsaStream (paths); // TODO: lo cambie para que no pinche cuando los archivos no existen
                var streamReader = new StreamReader(bolsaStream);
                var transStream  = new TransStream (streamReader, transformer); // TODO: lo cambie para que no le de bola a los nulos

                // return
                var fName = $"picobus_{fechaDesde:yyyy_MM_dd}.csv";
                return File(transStream, "text/csv", fName);
            }
            else
            {
                return BadRequest(
                    MensajesDeErrorHelper.CrearMensajeError($"Error con los parámetros indicados") +
                    DameAyuda()
                );
            }
        }

        bool CharEsDigito(char c)
        {
            return c >= '0' && c <= '9';
        }

        bool StringEsNumeroEntero(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }

            foreach (char c in s)
            {
                if (!CharEsDigito(c))
                {
                    return false;
                }
            }

            return true;
        }

        string DameAyuda()
        {
            var sbHelp = new StringBuilder();
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Ejemplos de uso:");
            sbHelp.AppendLine("    /HistoriaCochesPicoBusAnteriores?formato=csv&diasmenos=1");
            sbHelp.AppendLine("    /HistoriaCochesPicoBusAnteriores?formato=csv&diasmenos=2");
            sbHelp.AppendLine("");
            sbHelp.AppendLine("Aclaraciones:");
            sbHelp.AppendLine("    Parámetro \"DiasMenos\":");
            sbHelp.AppendLine("        ej: diasMenos=1                    es todo el día de ayer");
            sbHelp.AppendLine("        ej: diasMenos=2                    es todo el día de anteayer");
            sbHelp.AppendLine("        y así sucesivamente");
            //sbHelp.AppendLine("    Parámetro \"Filtro\":");
            //sbHelp.AppendLine("        ej: filtro=campo1,campo2,campo3    lista de campos que se quiere ver");
            sbHelp.AppendLine("    Parámetro \"Formato\":");
            sbHelp.AppendLine("        ej: formato=csv                    csv con título (es el valor por defecto)");
            sbHelp.AppendLine("        ej: formato=csvnt                  csv sin título");
            //sbHelp.AppendLine($"    FileName Temporal: {Path.GetTempFileName()}");
            //sbHelp.AppendLine($"    FileName Random  : {Path.GetRandomFileName()}");
            return sbHelp.ToString();
        }

    }

    public abstract class AcomodadorLineaCsv
    {
        public abstract string GetTitulo();
        public abstract bool Validar(string s);
        public abstract string Acomodar(string s);
    }

    public class AcomodadorLineaCsvPicoBus1 : AcomodadorLineaCsv
    {
        public char Sep { get; set; } = ';';

        public override string GetTitulo()
        {
            return "Ficha;Id;Lat;Lng;FechaLlegadaLocal;FechaDeProduccion";
        }

        public override bool Validar(string s)
        {
            // son 6 elementos
            //    0       1         2         3                   4                   5
            // 4441;3000678;-34.54301;-58.71907;2022-08-11 23:46:29;2022-08-11 23:46:29

            // no puede ser null o empty
            if (string.IsNullOrEmpty(s))
            { 
                return false;
            }

            var partes = s.Split(Sep);

            // debe tener 6 partes
            if (partes.Length != 6)
            {
                return false;
            }

            // las partes deben ser de determinado tipo...
            try
            {
                var foo0 = int.Parse(partes[0]);
                var foo1 = int.Parse(partes[1]);
                var foo2 = double.Parse(partes[2], CultureInfo.InvariantCulture);
                var foo3 = double.Parse(partes[3], CultureInfo.InvariantCulture);
                var foo4 = DateTime.Parse(partes[4]);
                var foo5 = DateTime.Parse(partes[5]);
            }
            catch
            {
                // algo estaba mal...
                return false;
            }

            // todo está bien
            return true;
        }

        public override string Acomodar(string s)
        {
            ////Original
            ////$"{ficha};{id};{sLat};{sLng};{sAhora};{sProdu}\n"
            ////    0  1   2   3                 4                     5
            ////Ficha;Id;Lat;Lng;FechaLlegadaLocal;Recordedat=FechaLocal;
            ////4441;3000678;-34.54301;-58.71907;2022-08-11 23:46:29;2022-08-11 23:46:29

            ////Para Imitar
            ////    0   1   2          3          4                 5
            ////Ficha;Lat;Lng;FechaLocal;Recordedat;FechaLlegadaLocal
            ////4377;-34.5546266;-58.725265;2022-07-20 00:00:00;2022-07-20 03:00:00;2022-07-20 00:00:00

            //var orig = s.Split(Sep);

            //var imit = new string[6];
            //imit[0] = orig[0];
            //imit[1] = orig[2];
            //imit[2] = orig[3];
            //imit[3] = orig[4];
            //imit[4] = orig[4];
            //imit[5] = orig[4];

            //// retornar todo...
            //return string.Join(Sep, imit);

            return s;
        }
    }
}
