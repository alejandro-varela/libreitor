using Comun;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace LibQPA.ProveedoresTecnobus
{
    public class ProveedorVersionesTecnobus : IQPAProveedorRecorridosTeoricos
    {
        public string DirRepoLocal { get; set; }

        public ProveedorVersionesTecnobus(string dirRepoLocal)
        {
            DirRepoLocal = dirRepoLocal;
        }

        public IEnumerable<RecorridoLinBan> Get(QPAProvRecoParams getParams)
        {
            return LeerRecorridos(DirRepoLocal, getParams.LineasPosibles, getParams.FechaVigencia, getParams.ConPuntos);
        }

        public static List<RecorridoLinBan> LeerRecorridos(string dir, int[] codLineas, DateTime vigenteEn, bool conPuntos = true)
        {
            // los archivos estan guardados con el formato verrec
            // nombre vvvvvv yyyy MM dd hh mm ss
            // verrec 000034 2015 12 24 00 00 00
            // primero listo los directorios que tengan que ver con las líneas en cuestion...

            var dirsLineas = codLineas.Select(codLinea => Path.Combine(dir, codLinea.ToString("0000")));
            var ret = new List<RecorridoLinBan>();

            foreach (var dirLinea in dirsLineas)
            {
                if (!Directory.Exists(dirLinea))
                {
                    throw new Exception($"No existe el directorio {dirLinea}");
                }

                var files = Directory.GetFiles(dirLinea, "verrec*.zip");

                var pathVersionRecorridos = files
                    .Where(path => GetVerFecha(Path.GetFileName(path)) <= vigenteEn)
                    .OrderByDescending(path => path)
                    .FirstOrDefault()
                ;

                if (string.IsNullOrEmpty(pathVersionRecorridos) || 
                    !File.Exists(pathVersionRecorridos))
                {
                    throw new Exception($"No existe archivo de recorridos en {dirLinea} para la fecha de vigencia {vigenteEn}");
                }

                foreach (var recorridoLinBan in LeerRecorridosFromZippedVerRec(pathVersionRecorridos, conPuntos))
                {
                    //yield return recorridoLinBan;
                    ret.Add(recorridoLinBan);
                }
            }

            return ret;
        }

        static DateTime GetVerFecha(string fileName)
        {
            // 000000 000011 1111 11 11 22 22 22
            // 012345 678901 2345 67 89 01 23 45
            // verrec 000034 2015 12 24 00 00 00

            return new DateTime(
                year  : int.Parse(fileName.Substring(12, 4)),
                month : int.Parse(fileName.Substring(16, 2)),
                day   : int.Parse(fileName.Substring(18, 2)),
                hour  : int.Parse(fileName.Substring(20, 2)),
                minute: int.Parse(fileName.Substring(22, 2)),
                second: int.Parse(fileName.Substring(24, 2))
            );
        }

        static List<RecorridoLinBan> LeerRecorridosFromZippedVerRec(string pathZippedVerRec, bool conPuntos = true)
        {
            // 012345678901234567890123456789
            // 0         1         2
            // verrec00003420220912000000.zip
            var verFileName = Path.GetFileName(pathZippedVerRec);
            int version = int.Parse(verFileName.Substring(10, 2));

            var fechaActivacion = new DateTime(
                year: int.Parse(verFileName.Substring(12, 4)),
                month: int.Parse(verFileName.Substring(16, 2)),
                day: int.Parse(verFileName.Substring(18, 2)),
                hour: int.Parse(verFileName.Substring(20, 2)),
                minute: int.Parse(verFileName.Substring(22, 2)),
                second: int.Parse(verFileName.Substring(24, 2)),
                kind: DateTimeKind.Local
            );

            var ret = new List<RecorridoLinBan>();

            using (FileStream zipStream = File.OpenRead(pathZippedVerRec))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zipArchive.Entries)
                    {
                        if (entry.Name.StartsWith("r"))
                        {
                            // 0123456789012
                            // 0         1
                            // rLLLLBBBB.txt
                            int linea = int.Parse(entry.Name.Substring(1, 4));
                            int bandera = int.Parse(entry.Name.Substring(5, 4));
                            
                            using (Stream entryStream = entry.Open())
                            {
                                List<PuntoRecorrido> puntosRecorrido;

                                if (conPuntos)
                                {
                                    puntosRecorrido = RecorridosParser.ReadFile(entryStream);
                                }
                                else
                                {
                                    puntosRecorrido = new List<PuntoRecorrido>();
                                }

                                var recoLinBan = new RecorridoLinBan
                                {
                                    Version = version,
                                    FechaActivacion = fechaActivacion,
                                    Linea   = linea,
                                    Bandera = bandera,
                                    Puntos  = puntosRecorrido,
                                };

                                //yield return recoLinBan;
                                ret.Add(recoLinBan);
                            }
                        }
                    }
                }
            }

            return ret;
        }
    }
}
