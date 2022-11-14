using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace ComunApiCoches
{
    public class FilesHelper
    {
        public static IEnumerable<string> GetPaths(string baseDir, DateTime desde, DateTime hasta)
        {
            int contadorHoras = 0;
            while (true)
            {
                DateTime fechaActual = desde.AddHours(contadorHoras);
                if (fechaActual >= hasta)
                {
                    break;
                }
                yield return FileTimeHelper.CreateFileNameByHour(baseDir, fechaActual);
                contadorHoras++;
            }
        }


        public static (bool, DateTime) GetFechaArchivoMasX(string baseDir, Func<IEnumerable<string>, string> Selector)
        {
            var sAnio = Selector(Directory.GetDirectories(baseDir, "y????").OrderBy(x => x));

            if (sAnio == null)
            {
                return (false, DateTime.MinValue);
            }

            var baseDirConAnio = Path.Combine(baseDir, sAnio);
            var sMes = Selector(Directory.GetDirectories(baseDirConAnio, "mo??").OrderBy(x => x));

            if (sMes == null)
            {
                return (false, DateTime.MinValue);
            }

            var baseDirConMes = Path.Combine(baseDirConAnio, sMes);
            var sDia = Selector(Directory.GetDirectories(baseDirConMes, "d??").OrderBy(x => x));

            if (sDia == null)
            {
                return (false, DateTime.MinValue);
            }

            var baseDirConDia = Path.Combine(baseDirConMes, sDia);
            var sHora = Selector(Directory.GetFiles(baseDirConDia, "h??.*").OrderBy(x => x));

            if (sHora == null)
            {
                return (false, DateTime.MinValue);
            }

            var anio = int.Parse(sAnio.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last().Substring(1, 4));
            var mes  = int.Parse(sMes .Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last().Substring(2, 2));
            var dia  = int.Parse(sDia .Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last().Substring(1, 2));
            var hora = int.Parse(sHora.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last().Substring(1, 2));

            var dt = new DateTime(anio, mes, dia, hora, 0, 0);

            return (true, dt);
        }

        public static (bool, DateTime) GetFechaArchivoMasViejo(string baseDir)
        {
            return GetFechaArchivoMasX(baseDir, Enumerable.FirstOrDefault);
        }

        public static (bool, DateTime) GetFechaArchivoMasNuevo(string baseDir)
        {
            return GetFechaArchivoMasX(baseDir, Enumerable.LastOrDefault);
        }
    }
}
