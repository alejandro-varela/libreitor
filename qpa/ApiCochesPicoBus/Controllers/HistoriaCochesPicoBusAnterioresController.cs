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

        /*
        public class TransStream : Stream
        {
            StreamReader        _streamReader;
            Func<string, string>_transformer;
            List<byte>          _sobra = new List<byte>(128 * 1024);

            public string NewLine { get; set; } = "\n";

            static string Identidad(string s)
            {
                return s;
            }

            public TransStream(StreamReader streamReader, Func<string, string> transformer)
            {
                _streamReader = streamReader;
                _transformer = transformer ?? Identidad;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_sobra.Count == 0)
                {
                    while (_sobra.Count < count)
                    {
                        var sLine = _streamReader.ReadLine();

                        if (sLine == null)
                        {
                            break;
                        }
                        else
                        {
                            var sTransLine = _transformer(sLine);
                            if (!string.IsNullOrEmpty(sTransLine))
                            {
                                _sobra.AddRange(Encoding.UTF8.GetBytes(sTransLine));
                                _sobra.AddRange(Encoding.UTF8.GetBytes(NewLine));
                            }
                        }
                    }
                }

                var cant = Math.Min(count, _sobra.Count);
                _sobra.CopyTo(0, buffer, offset, cant);
                _sobra.RemoveRange(0, cant);
                return cant;
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => 0;

            public override long Position { get => 0; set => _ = value; }

            public override void Flush() { }

            public override long Seek(long offset, SeekOrigin origin) { return 0L; }

            public override void SetLength(long value) { }

            public override void Write(byte[] buffer, int offset, int count) { }
        }

        public class BolsaStream : Stream
        {
            List<string> _archivos;
            int _archivoIndex = 0;
            int _archivoPtr = 0;

            const int MAIN_BUFF_SIZE = 1_048_576; // 1 mega
            byte[] _mainBuff = new byte[MAIN_BUFF_SIZE];
            int _mainBuffLen = 0;
            int _mainBuffPtr = 0;

            // main buffer
            // [0123456789] vacio sin iniciar, len = 0, ptr = 0
            //  ^  
            //
            // [0123456789] tiene = len > ptr ; disponible = len - ptr
            //  ^          
            //
            // [0123456789] no tiene = len <= ptr
            //            ^
            //
            // cada vez que se rellena:
            //  poner ptr en 0
            //  poner len en lo que se haya leído
            //

            public BolsaStream(List<string> paths)
            {
                _archivos = paths;
            }

            bool HayAlgoEnElMainBuffer()
            {
                return _mainBuffLen > _mainBuffPtr;
                // o podría ser...
                // BytesDisponiblesEnElMainBuffer() > 0
            }

            int BytesDisponiblesEnElMainBuffer()
            {
                int resultado = _mainBuffLen - _mainBuffPtr;
                return resultado;
            }

            bool RellenarElMainBuffer()
            {
                // pongo el apuntador en el primer byte del _mainBuff
                // siempre...
                _mainBuffPtr = 0;

                // forever
                for (; ; )
                {
                    // si el index de archivos es igual a la cantidad de
                    // archivos, ya no tengo archivos... retorno false
                    if (_archivoIndex == _archivos.Count)
                    {
                        return false;
                    }

                    if (System.IO.File.Exists(_archivos[_archivoIndex]))
                    {
                        using (FileStream fs = System.IO.File.OpenRead(_archivos[_archivoIndex]))
                        {
                            // adelanto el puntero del stream hasta lo que ya leí...
                            fs.Seek(_archivoPtr, SeekOrigin.Begin);

                            // trato de leer algo...
                            _mainBuffLen = fs.Read(_mainBuff, 0, MAIN_BUFF_SIZE);
                        }
                    }

                    if (_mainBuffLen <= 0)
                    {
                        // si no lei nada de el archivo actual:
                        // incremento el index de archivos
                        _archivoIndex++;
                        _archivoPtr = 0;
                    }
                    else
                    {
                        // incremento el puntero del archivo...
                        _archivoPtr += _mainBuffLen;

                        // leí algo...
                        return true;
                    }
                }
            }

            int TransferirBytes(byte[] buff, int offset, int count)
            {
                int disponibles = BytesDisponiblesEnElMainBuffer();
                int transferidos = 0;

                for (int i = 0; i < disponibles && i < count; i++)
                {
                    buff[offset + i] = _mainBuff[_mainBuffPtr];
                    _mainBuffPtr += 1;
                    transferidos += 1;
                }

                return transferidos;
            }

            public override int Read(byte[] buff, int offset, int count)
            {
                if (!HayAlgoEnElMainBuffer())
                {
                    if (!RellenarElMainBuffer())
                    {
                        return 0;
                    }
                }

                int transferidos = TransferirBytes(buff, offset, count);

                return transferidos;
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => 0L;

            public override long Position { get => 0L; set => _ = value; }

            public override void Flush() { }

            public override long Seek(long offset, SeekOrigin origin) => 0L;

            public override void SetLength(long value) { }

            public override void Write(byte[] buffer, int offset, int count) { }
        }
        */
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
