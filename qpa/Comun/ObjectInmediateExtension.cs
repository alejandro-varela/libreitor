using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Comun
{
    public static class ObjectInmediateExtension
    {
        public static void SaveJson(this object obj, string fileName)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            System.IO.File.WriteAllText(fileName, json);
            Debug.Print("Written");
            Debug.Print($"Dir: {System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(fileName))}");
        }

        public static void SaveJsonCSharpString(this object obj, string fileName)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            json = json.Replace("\"", "\\\"");
            json = $"\"{json}\"";
            System.IO.File.WriteAllText(fileName, json);
            Debug.Print("Written");
            Debug.Print($"Dir: {System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(fileName))}");
            Debug.Print($"Dir: {System.IO.Path.GetFullPath(fileName)}");
        }

        public static void SaveJsonCSharpVariable(this object obj, string fileName, string varName)
        {
            if (string.IsNullOrEmpty(varName))
            {
                varName = Path.GetFileNameWithoutExtension(fileName);
            }

            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            json = json.Replace("\"", "\\\"");

            var partes = json.Split("\r\n");

            using (var fs = System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate))
            {
                var stringBuilderName = $"{varName}StringBuilder";

                // region
                {
                    var region = Encoding.UTF8.GetBytes($"#region {varName}\r\n");
                    fs.Write(region, 0, region.Length);
                }

                // declaración del stringBuilder
                {
                    var sbDecl = Encoding.UTF8.GetBytes($"var {stringBuilderName} = new StringBuilder();\r\n");
                    fs.Write(sbDecl, 0, sbDecl.Length);
                }

                // appendLine(s)
                foreach (var parteX in partes)
                {
                    var renglon = $"{stringBuilderName}.AppendLine(\"{parteX}\");\r\n";
                    var buffer = Encoding.UTF8.GetBytes(renglon);
                    fs.Write(buffer, 0, buffer.Length);
                }

                // declaración de la variable en si
                {
                    var varDecl = Encoding.UTF8.GetBytes($"var {varName} = {stringBuilderName}.ToString();\r\n");
                    fs.Write(varDecl, 0, varDecl.Length);
                }

                // endregion
                {
                    var endRegion = Encoding.UTF8.GetBytes($"#endregion {varName}\r\n");
                    fs.Write(endRegion, 0, endRegion.Length);
                }

                fs.Flush();
            }

            //System.IO.File.WriteAllText(fileName, json);
            Debug.Print("Written");
            Debug.Print($"Dir: {Path.GetDirectoryName(Path.GetFullPath(fileName))}");
            Debug.Print($"Dir: {Path.GetFullPath(fileName)}");
        }

        public static string GetJson(this object obj)
        {
            return GetJson(obj, false);
        }

        public static string GetJson(this object obj, bool indented)
        {
            return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
        }

        public static void PrintJson(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            Debug.Print(json);
        }
    }
}


