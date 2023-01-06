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
            string localFileName = ArgsHelper.SafeGetArgVal(misArgs, "local", "").ToLower();

            // Remote
            var remoteFileName = ArgsHelper.SafeGetArgVal(misArgs, "remote", Path.GetFileName(localFileName)).ToLower();

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
                Usage();
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
                var strRequestUri = "http://192.168.201.74:5010/ExisteReporte?rfname=" + remoteFileName;
                var strRequestUriEscapada = Uri.EscapeUriString(strRequestUri);
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(strRequestUriEscapada);

                if (!response.IsSuccessStatusCode)
                {
                    return 1;
                }

                var json = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<RespuestaExisteReporte>(json);

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

            if (!subirArchivo)
            {
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
                    var jsonete = await response.Content.ReadAsStringAsync();
                    return 1;
                }
            }

            // loguear que el archivo se ha subido ok
            return 0;
        }

        static void Usage()
        {
            //
        }
    }

    public class RespuestaExisteReporte
    {
        public bool Existe { get; set; }
        public string FName { get; set; }
        public string Sha1 { get; set; }
    }
}
