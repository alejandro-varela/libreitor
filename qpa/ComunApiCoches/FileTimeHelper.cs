using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComunApiCoches
{
    public class FileTimeHelper
    {
        public readonly object WriteLock = new object();
        public string CurrentPath { get; private set; } = string.Empty;

        private static string CreateDirectoryNameByDay(DateTime dateTime)
        {
            return $"y{dateTime.Year:0000}/mo{dateTime.Month:00}/d{dateTime.Day:00}/";
        }

        public static string CreateFileNameByHour(string baseDir, DateTime dateTime, string extension = "txt")
        {
            string nombreArchivo =
                CreateDirectoryNameByDay(dateTime).TrimEnd('/') + $"/h{dateTime.Hour:00}"
            ;

            if (!string.IsNullOrEmpty(extension))
            {
                nombreArchivo += $".{extension}";
            }

            string path =
                Path.Combine(baseDir ?? "", nombreArchivo)
                .Replace("\\", "/")
            ;

            return path;
        }

        public Tuple<bool, Exception> WriteToFile(string directorioBase, DateTime dateTime, string sData)
        {
            try
            {
                lock (WriteLock)
                {
                    string path = CreateFileNameByHour(directorioBase, dateTime);

                    if (path != CurrentPath)
                    {
                        string dir = Path.GetDirectoryName(path);

                        if (dir != null && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            CurrentPath = path;
                        }
                    }

                    File.AppendAllText(path, sData);
                }

                return new Tuple<bool, Exception>(true, null);
            }
            catch (Exception exx)
            {
                return new Tuple<bool, Exception>(false, exx);
            }
        }

        public static (bool, DateTime) GetFileDameTime(string fileName)
        {
            var regExpHora = new Regex("^h[0-9]{2}");
            var regExpDia  = new Regex("^d[0-9]{2}$");
            var regExpMes  = new Regex("^mo[0-9]{2}$");
            var regExpAnio = new Regex("^y[0-9]{4}$");

            var partes = fileName.Split('/', '\\');
            var hora = -1;
            var dia = -1;
            var mes = -1;
            var anio = -1;

            for (int i = partes.Length - 1; i >= 0; i--)
            {
                if (partes[i].StartsWith("h") && regExpHora.IsMatch(partes[i]))
                {
                    hora = int.Parse(partes[i].Substring(1, 2));
                }

                if (partes[i].StartsWith("d") && regExpDia.IsMatch(partes[i]))
                {
                    dia = int.Parse(partes[i].Substring(1));
                }

                if (partes[i].StartsWith("mo") && regExpMes.IsMatch(partes[i]))
                {
                    mes = int.Parse(partes[i].Substring(2));
                }

                if (partes[i].StartsWith("y") && regExpAnio.IsMatch(partes[i]))
                {
                    anio = int.Parse(partes[i].Substring(1));
                    break;
                }
            }

            if (anio == -1 || mes == -1 || dia == -1 || hora == -1)
            {
                return (false, DateTime.MinValue);
            }

            return (true, new DateTime(anio, mes, dia, hora, 0, 0));
        }
    }
}
