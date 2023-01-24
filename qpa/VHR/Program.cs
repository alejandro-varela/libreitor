using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VHR
{
    static class Program
    {
        public static Dictionary<string, string> Lineas = null;
        public static Dictionary<string, Custom> Customs = null;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Líneas
            {
                var cargarLineasRet = LoadList<NameValue<List<int>>>("./config/lineas.json");

                if (cargarLineasRet.Ok)
                {
                    Lineas = new Dictionary<string, string>();
                    foreach (var pepe in cargarLineasRet.Item)
                    {
                        var sValue = string.Join(", ", pepe.Value.Select(n => n.ToString()));
                        Lineas.Add(pepe.Name, sValue);
                    }
                }
                else
                {
                    ShowError($"No se pudieron cargar las líneas: {cargarLineasRet.Error}", "Error de carga");
                    return;
                }
            }

            // Puntos Custom
            {
                foreach (var pathCustom in Directory.GetFiles("./config/", "custom*.json"))
                {
                    var cargarCustomsRet = LoadList<Custom>(pathCustom);

                    if (cargarCustomsRet.Ok)
                    {
                        Customs = new Dictionary<string, Custom>();
                        foreach (var pepe in cargarCustomsRet.Item)
                        {
                            Customs.Add(pepe.Name, pepe);
                        }
                    }
                    else
                    {
                        ShowError($"No se pudo cargar el custom: {cargarCustomsRet.Error}", "Error de carga");
                        return;
                    }
                }
            }

            Application.Run(new FrmMain());
        }

        public static DeserRetVal<T> LoadItem<T>(string pathArchivoJson)
        {
            try
            {
                string json = File.ReadAllText(pathArchivoJson);
                T obj  = JsonConvert.DeserializeObject<T>(json);

                return new DeserRetVal<T>
                {
                    Ok    = true,
                    Error = null,
                    Item  = obj,
                };
            }
            catch (Exception exx)
            {
                return new DeserRetVal<T>
                {
                    Ok    = false,
                    Error = exx.ToString(),
                    Item  = default,
                };
            }
        }

        public static DeserRetVal<List<T>> LoadList<T>(string pathArchivoJson)
        {
            return LoadItem<List<T>>(pathArchivoJson);
        }

        public static DeserRetVal<Dictionary<K, V>> LoadDictionary<K, V>(string pathArchivoJson)
        {
            return LoadItem<Dictionary<K, V>>(pathArchivoJson);
        }

        public static void ShowError(string msg, string title)
        {
            MessageBox.Show(
                msg,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        public class Custom
        {
            public string Name    { get; set; }
            public double Lat     { get; set; }
            public double Lng     { get; set; }
            public double Caption { get; set; }
        }

        public class NameValue<T>
        { 
            public string Name { get; set; }
            public T Value { get; set; }
        }

        public class DeserRetVal<T>
        {
            public bool Ok { get; set; }
            public string Error { get; set; }
            public T Item { get; set; }
        }
    }
}
