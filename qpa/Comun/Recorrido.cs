using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System;

namespace Comun
{
    public class Recorrido
    {
        public IEnumerable<PuntoRecorrido> Puntos { get; set; }

        public PuntoRecorrido PuntoSalida
        {
            get
            {
                if (Puntos != null)
                {
                    return Puntos.First();
                }

                return null;
            }
        }

        public PuntoRecorrido PuntoLlegada
        {
            get
            {
                if (Puntos != null)
                {
                    return Puntos.Last();
                }

                return null;
            }
        }

        // inst 
        public int DameCuentaMasCercanaA(Punto punto)
        {
            return DameCuentaMasCercanaA(punto, Puntos);
        }

        // static
        public static int DameCuentaMasCercanaA(Punto punto, IEnumerable<PuntoRecorrido> puntos)
        {
            var minDist = double.MaxValue;
            var cuenta  = -1;

            foreach (var prec in puntos)
            {
                var dist = Haversine.GetDist(prec, punto);

                if (dist < minDist)
                {
                    cuenta = prec.Cuenta;
                    minDist = dist;
                }
            }

            return cuenta;
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
                    int linea   = int.Parse(entry.Name.Substring(1, 4));
                    int bandera = int.Parse(entry.Name.Substring(5, 4));

                    using Stream entryStream = entry.Open();
                    var puntosRecorrido = RecorridosParser.ReadFile(entryStream);

                    var recoLinBan = new RecorridoLinBan
                    {
                        Linea   = linea,
                        Bandera = bandera,
                        Puntos  = puntosRecorrido,
                    };

                    //yield return recoLinBan;
                    ret.Add(recoLinBan);
                }
            }

            return ret;
        }

        public static List<RecorridoLinBan> LeerRecorridosPorArchivos(string dir, int[] codLineas, DateTime fechaInicioCalculo)
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
                    .Where(path => GetVerFecha(Path.GetFileName(path)) <= fechaInicioCalculo)
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
    }
}
