using ComunDriveUp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiCochesDriveUp.Controllers
{
    public class ConvertidorCSV<TipoADiccionario>
    {
        private bool _esPrimeraVez = true; // sirve para poner el título de las columnas

        public bool ConTitulo { get; set; }

        public Func<DatosDriveUp, Dictionary<string, object>> DatosADiccionario { get; set; }

        public string Convertir(string json)
        {
            try
            {
                // creo una instancia del json
                var obj = JsonConvert.DeserializeObject<DataWrapperV0>(json);

                // necesito la hora del archivo!!!

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

                if (_esPrimeraVez && ConTitulo)
                {
                    _esPrimeraVez = false;
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
