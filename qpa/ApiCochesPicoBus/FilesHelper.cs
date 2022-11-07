using System.Collections.Generic;
using System;

namespace ApiCochesPicoBus
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

    }
}
