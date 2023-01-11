using Comun;
using ComunQPApp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace QPAppUploaderHttp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            #region Parsing de argumentos

            Dictionary<string, string> misArgs = ArgsHelper.CreateDictionary(args);
            DateTime ahora = DateTime.Now;

            // Local
            string localFileName = ArgsHelper.SafeGetArgVal(misArgs, "local", "");

            // Remote
            var remoteFileName = ArgsHelper.SafeGetArgVal(misArgs, "remote", Path.GetFileName(localFileName));

            #endregion

            #region Validación de argumentos

            if (string.IsNullOrEmpty(localFileName))
            {
                Usage();
                return 1;
            }

            #endregion

            // Me fijo si existe el archivo local
            if (!File.Exists(localFileName))
            {
                Console.WriteLine($"No existe el archivo local {localFileName}");
                return 1;
            }

            // Obtengo los bytes del archivo
            var localFileBuff = File.ReadAllBytes(localFileName);

            // Calculo el hash local del archivo
            var hashArchivoLocal = Hashes.GetSHA1String(localFileBuff);

            // Me fijo si existe el archivo remoto
            bool existeArchivoRemoto = false;
            bool hashLocalIgualAHashRemoto = false;

            {
                var strRequestUri = "http://192.168.201.74:5010/ExisteReporte?fileName=" + remoteFileName;
                var strRequestUriEscapada = Uri.EscapeUriString(strRequestUri);
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(strRequestUriEscapada);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"ExisteReporte: { response.StatusCode }");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    return 1;
                }

                var json = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<RespuestaExisteReporte>(json);

                Console.WriteLine(json);

                if (resp.Existe)
                {
                    hashLocalIgualAHashRemoto = hashArchivoLocal == resp.Sha1;
                    existeArchivoRemoto = true;
                }
            }

            // Determino si hay que subir el archivo...
            bool subirArchivo = 
                !existeArchivoRemoto ||     // Si no existe el archivo...
                !hashLocalIgualAHashRemoto  // Si el hash no es el mismo...
            ;

            Console.WriteLine($"Existe archivo remoto {existeArchivoRemoto}");
            Console.WriteLine($"Hashes iguales        {hashLocalIgualAHashRemoto}");

            if (!subirArchivo)
            {
                Console.WriteLine($"No hace falta subir el archivo porque ya existe");
                return 0;
            }

            // Subo el archivo local al destino remoto
            {
                var strRequestUri = "http://192.168.201.74:5010/SubirReporte";
                var strRequestUriEscapada = Uri.EscapeUriString(strRequestUri);

                var form = new MultipartFormDataContent();
                using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(localFileName));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(fileContent, "File", Path.GetFileName(localFileName));
                form.Add(new StringContent(remoteFileName), "Name");
                form.Add(new StringContent("json file"), "Description");

                var httpClient = new HttpClient();
                var response = await httpClient.PostAsync(strRequestUriEscapada, form);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"SubirReporte: { response.StatusCode }");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    return 1;
                }
            }

            // loguear que el archivo se ha subido ok
            Console.WriteLine("Reporte subido OK");
            return 0;
        }

        static void Usage()
        {
            Console.WriteLine("QPAppUploaderHtpp ");
            Console.WriteLine();
            Console.WriteLine("    Uso: ");
            Console.WriteLine("        QPAppUploaderHtpp local=NombreArchivoLocal [remote=NombreArchivoRemoto]");
            Console.WriteLine();
            Console.WriteLine("    Ejemplos: ");
            Console.WriteLine("        QPAppUploaderHtpp local=/home/juana/arch.txt remote=archivo1234.txt");
            Console.WriteLine("        QPAppUploaderHtpp local=/home/juana/arch.txt");
            Console.WriteLine();
            Console.WriteLine("    Notas: ");
            Console.WriteLine("        Si remote no es especificado el nombre del archivo se saca de local, ");
            Console.WriteLine("        sin los directorios de ruta ");
        }
    }

    public class RespuestaExisteReporte
    {
        public bool Existe { get; set; }
        public string FName { get; set; }
        public string Sha1 { get; set; }
    }
}
