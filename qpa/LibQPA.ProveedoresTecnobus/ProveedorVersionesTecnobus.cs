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
        public string[] DirRepos    { get; set; }
        public bool     RepoRandom  { get; set; }

        public ProveedorVersionesTecnobus(string[] dirRepos, bool repoRandom = false)
        {
            DirRepos    = dirRepos;
            RepoRandom  = repoRandom;
        }

        public IEnumerable<RecorridoLinBan> Get(int[] lineas, DateTime vigenteEn)
        {
            int index = 0; // TODO: hacer random si se pide en RepoRandom...
            return LeerRecorridos(DirRepos[index], lineas, vigenteEn);
        }

        public static List<RecorridoLinBan> LeerRecorridos(string dir, int[] codLineas, DateTime vigenteEn)
        {
            // los archivos estan guardados con el formato verrec
            // nombre vvvvvv yyyy MM dd hh mm ss
            // verrec 000034 2015 12 24 00 00 00
            // primero listo los directorios que tengan que ver con las líneas en cuestion...

            var dirsLineas = codLineas.Select(codLinea => Path.Combine(dir, codLinea.ToString("0000")));
            var ret = new List<RecorridoLinBan>();

            foreach (var dirLinea in dirsLineas)
            {
                var pathVersionRecorridos = Directory
                    .GetFiles(dirLinea)
                    .Where(path => Path.GetFileName(path).StartsWith("verrec"))
                    .Where(path => path.EndsWith(".zip"))
                    .Where(path => GetVerFecha(Path.GetFileName(path)) <= vigenteEn)
                    .OrderByDescending(path => path)
                    .FirstOrDefault()
                ;

                foreach (var recorridoLinBan in LeerRecorridosFromZippedVerRec(pathVersionRecorridos))
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
                year: int.Parse(fileName.Substring(12, 4)),
                month: int.Parse(fileName.Substring(16, 2)),
                day: int.Parse(fileName.Substring(18, 2)),
                hour: int.Parse(fileName.Substring(20, 2)),
                minute: int.Parse(fileName.Substring(22, 2)),
                second: int.Parse(fileName.Substring(24, 2))
            );
        }

        static List<RecorridoLinBan> LeerRecorridosFromZippedVerRec(string pathZippedVerRec)
        {
            var ret = new List<RecorridoLinBan>();

            using FileStream zipStream = File.OpenRead(pathZippedVerRec);
            using ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in zipArchive.Entries)
            {
                if (entry.Name.StartsWith("r"))
                {
                    // 0123456789012
                    // rLLLLBBBB.txt
                    int linea = int.Parse(entry.Name.Substring(1, 4));
                    int bandera = int.Parse(entry.Name.Substring(5, 4));

                    using Stream entryStream = entry.Open();
                    var puntosRecorrido = RecorridosParser.ReadFile(entryStream);

                    var recoLinBan = new RecorridoLinBan
                    {
                        Linea = linea,
                        Bandera = bandera,
                        Puntos = puntosRecorrido,
                    };

                    //yield return recoLinBan;
                    ret.Add(recoLinBan);
                }
            }

            return ret;
        }
    }
}
