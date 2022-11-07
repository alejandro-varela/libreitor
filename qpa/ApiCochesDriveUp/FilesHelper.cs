using ComunDriveUp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ApiCochesDriveUp
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

        public static IEnumerable<DatosDriveUp> GetDatos(string baseDir, DateTime desde, DateTime hasta)
        {
            int contadorHoras = 0;

            while (true)
            {
                DateTime fechaActual = desde.AddHours(contadorHoras);

                if (fechaActual >= hasta)
                {
                    break;
                }

                foreach (var x in ReadData(baseDir, fechaActual))
                {
                    yield return x;
                }

                contadorHoras++;
            }
        }

        static IEnumerable<DatosDriveUp> ReadData(string baseDir, DateTime dateTime, string extension = "txt")
        {
            string path = FileTimeHelper.CreateFileNameByHour(baseDir, dateTime, extension);

            if (System.IO.File.Exists(path))
            {
                foreach (var sLine in System.IO.File.ReadLines(path))
                {
                    DataWrapperV0 dataWrapperV0 = null;

                    try
                    {
                        dataWrapperV0 = JsonConvert.DeserializeObject<DataWrapperV0>(sLine);
                    }
                    catch
                    {
                        // :P
                    }

                    if (dataWrapperV0 != null)
                    {
                        yield return dataWrapperV0.Data;
                    }
                }
            }

            yield break;
        }
    }
}
