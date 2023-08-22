using ComunApiCoches;
using ComunDriveUp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiCochesDriveUp.Controllers
{
    public class ConvertidorCSV<T>
    {
        private bool _primeraVez = true; // sirve para poner el título de las columnas
        public bool ConTitulo { get; set; }
        public Func<DatosDriveUp, Dictionary<string, object>> DatosADiccionario { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public string Convertir(string json, string filePath)
        {
            try
            {
                // creo una instancia del json
                var obj = JsonConvert.DeserializeObject<DataWrapperV0>(json);

                // fecha - hora que representa el archivo
                var (okFileDateTime, fileDateTime) = FileTimeHelper.GetFileDateTime(filePath);

                if (!okFileDateTime)
                {
                    return "";
                }

                if (fileDateTime.Hour  != obj.Data.FechaLocal.Hour  ||
                    fileDateTime.Day   != obj.Data.FechaLocal.Day   ||
                    fileDateTime.Month != obj.Data.FechaLocal.Month ||
                    fileDateTime.Year  != obj.Data.FechaLocal.Year)
                {
                    return "";
                }

                if ((obj.Data.FechaLocal < FechaDesde) ||
                    (obj.Data.FechaLocal > FechaHasta))
                {
                    return "";
                }

                // creo un diccionario de esos datos
                var dic = DatosADiccionario(obj.Data);

                // agrego la fecha local
                var fechaLlegadaLocal = obj.RecvUtc.ToLocalTime();
                dic.Add("FechaLlegadaLocal", fechaLlegadaLocal);

                // construyo los datos de un renglon csv
                var sRenglonCSV = string.Empty;
                var lstDatos    = new List<string>();

                foreach (var value in dic.Values)
                {
                    if (value.GetType() == typeof(DateTime))
                    {
                        lstDatos.Add(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else if (value.GetType() == typeof(float) || value.GetType() == typeof(double))
                    {
                        lstDatos.Add(value.ToString().Replace(',', '.'));
                    }
                    else
                    {
                        lstDatos.Add(value.ToString());
                    }
                }

                sRenglonCSV = string.Join(';', lstDatos.ToArray());

                if (_primeraVez && ConTitulo)
                {
                    _primeraVez = false;
                    var tituloCSV = string.Join(';', dic.Keys);
                    return $"{tituloCSV}\n{sRenglonCSV}";
                }
                else
                {
                    return sRenglonCSV;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
