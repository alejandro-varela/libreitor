using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace QPApp
{
    public class BajadorDeArchivo
    {
        string   _nombreArchivoBajando      = string.Empty;
        bool     _yaBajado                  = false;
        DateTime _ultimoMovimiento;

        public string Error { get; private set; }

        public bool BajarArchivo(Uri remoteUri, string pathArchivoLocal)
        {
            pathArchivoLocal = Path.GetFullPath(pathArchivoLocal).Replace('\\', '/');

            _nombreArchivoBajando   = Path.GetFileName(pathArchivoLocal).Replace('\\', '/');
            var dirArchivoBajando   = Path.GetDirectoryName(pathArchivoLocal).Replace('\\', '/');
            var pathArchivoTemporal = Path.Combine(
                dirArchivoBajando,
                $"{Path.GetFileNameWithoutExtension(pathArchivoLocal)}.tmp"
            ).Replace('\\', '/');

            try
            {
                _ultimoMovimiento = DateTime.Now;
                _yaBajado = false;

                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                
                WebClient client = new WebClient();
                client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                client.DownloadFileAsync(remoteUri, pathArchivoTemporal);

                for (; ; )
                {
                    Thread.Sleep(100);

                    if ((DateTime.Now - _ultimoMovimiento).TotalSeconds >= 15)
                    {
                        try { File.Delete(pathArchivoTemporal); } catch { }
                        client.Dispose();
                        Error = "Red lenta o desconectada";
                        return false;
                    }

                    if (_yaBajado)
                    {                        
                        client.Dispose();
                        break;
                    }
                }

                if (File.Exists(pathArchivoLocal))
                {
                    File.Delete(pathArchivoLocal);
                }

                File.Move(pathArchivoTemporal, pathArchivoLocal);

                //for (int i = 0; i < 50; i++)
                //{
                //    Thread.Sleep(100);
                //    if (File.Exists(pathArchivoLocal))
                //    {
                //        return true;
                //    }
                //}
                //Error = $"No puedo crear el archivo {pathArchivoLocal}";
                //return false;

                return true;
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
                try { File.Delete(pathArchivoTemporal); } catch { }
                return false;
            }
        }

        void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(
                "Bajando archivo {0}, {1} bytes recibidos",
                _nombreArchivoBajando,
                e.BytesReceived
            );

            _ultimoMovimiento = DateTime.Now;

            if (e.ProgressPercentage == 100)
            {
                _yaBajado = true;
            }
        }

        bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // si todo ok...
            return true;
        }
    }
}
