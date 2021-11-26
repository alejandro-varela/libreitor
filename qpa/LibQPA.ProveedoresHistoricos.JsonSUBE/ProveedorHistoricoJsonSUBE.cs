using Comun;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibQPA.ProveedoresHistoricos.JsonSUBE
{
    public class ProveedorHistoricoJsonSUBE : IQPAProveedorPuntosHistoricos
    {
        public string   InputDir    { get; set; } = ".";
        public DateTime FechaDesde  { get; set; } = DateTime.MinValue;
        public DateTime FechaHasta  { get; set; } = DateTime.MaxValue;

        public Dictionary<string, List<PuntoHistorico>> Get()
        {
            // busco subdirectorios fechados...
            var dirsEnFecha = Directory.EnumerateDirectories(InputDir)
                .Where      (d => DameFechaDir(d) >= TruncarFechaYMD(FechaDesde))
                .Where      (d => DameFechaDir(d) <  FechaHasta)
                .ToList     ()
            ;

            // tomo sus archivos...
            var filesss = dirsEnFecha
                .SelectMany (d => Directory.EnumerateFiles(d))
                .Where      (f => DameFechaArchivo(f) >= FechaDesde)
                .Where      (f => DameFechaArchivo(f) <  FechaHasta)
                .OrderBy    (f => f)
                .ToList     ()
            ;

            // creo el diccionario de entrada...
            var ret = new Dictionary<string, List<PuntoHistorico>>();

            // para cada archivo...
            foreach (var fileX in filesss)
            {
                var json = File.ReadAllText(fileX);
                var info = ParseJsonSUBE(json);

                foreach (VehicleContainer container in info)
                {
                    // ignoro las latitudes y longitudes POSITIVAS
                    if (container.Vehicle.Pos.Lat > 0 ||
                        container.Vehicle.Pos.Lng > 0)
                    {
                        continue;
                    }

                    var empresa = DameEmpresaArchivo(fileX);
                    var interno = container.Vehicle.Info.GetIdentCocheSUBEFromLabel();
                    var identificador = $"{empresa}-{interno}";

                    if (!ret.ContainsKey(identificador))
                    {
                        ret.Add(identificador, new List<PuntoHistorico>());
                    }

                    // creo el punto
                    var punto = new PuntoHistorico
                    {
                        Lat = container.Vehicle.Pos.Lat,
                        Lng = container.Vehicle.Pos.Lng,
                        Alt = 0.0,
                        Fecha = container.Vehicle.FechaLocalFromTimeStamp
                    };

                    // si tengo puntos guardados para este identificador
                    if (ret[identificador].Count > 0)
                    {
                        // ignoro el punto si el último es igual
                        if (ret[identificador][^1] == punto)
                        {
                            continue;
                        }

                        // ignoro el punto si su fecha es menor a la última fecha
                        if (ret[identificador][^1].Fecha > punto.Fecha)
                        {
                            continue;
                        }
                    }
                    
                    // agrego el punto
                    ret[identificador].Add(punto);
                }
            }

            return ret;
        }

        private DateTime TruncarFechaYMD(DateTime fecha)
        {
            return new DateTime(
                year    : fecha.Year,
                month   : fecha.Month,
                day     : fecha.Day,
                hour    : 0,
                minute  : 0,
                second  : 0,
                kind    : fecha.Kind
            );
        }

        private DateTime DameFechaDir(string fullDirName)
        {
            if (!Directory.Exists(fullDirName))
            {
                throw new ArgumentException($"El directorio {fullDirName} no existe", nameof(fullDirName));
            }

            var dirName = fullDirName
                .TrimEnd('\\')
                .Split('\\')[^1]
            ;

            var parteFecha = dirName.Split('_')[1];

            return 
                new DateTime(
                    year : int.Parse(parteFecha.Substring(0, 4)),
                    month: int.Parse(parteFecha.Substring(4, 2)),
                    day  : int.Parse(parteFecha.Substring(6, 2))
                );
        }

        private static int DameEmpresaArchivo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"El archivo {filePath} no existe", nameof(filePath));
            }

            var fileName = Path.GetFileName(filePath);

            var parteEmpresa = fileName.Split('_')[0];

            return int.Parse(parteEmpresa);
        }

        private static DateTime DameFechaArchivo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"El archivo {filePath} no existe", nameof(filePath));
            }

            var fileName = Path.GetFileName(filePath);

            var parteFecha = fileName.Split('_')[1];
            var parteHora  = fileName.Split('_')[2];

            return
                new DateTime(
                    year  : int.Parse(parteFecha.Substring(0, 4)),
                    month : int.Parse(parteFecha.Substring(4, 2)),
                    day   : int.Parse(parteFecha.Substring(6, 2)),
                    hour  : int.Parse(parteHora.Substring(0, 2)),
                    minute: int.Parse(parteHora.Substring(2, 2)),
                    second: int.Parse(parteHora.Substring(4, 2))
                );
        }

        public static IEnumerable<VehicleContainer> ParseJsonSUBE(string json)
        {
            JObject data = null;
            try { data = JsonConvert.DeserializeObject(json) as JObject; } catch { }
            if (data == null)
            {
                yield break;
            }

            JArray containers = null;
            try { containers = data["_entity"] as JArray; } catch { }
            if (containers == null)
            {
                yield break;
            }

            foreach (var containerX in containers)
            {
                VehicleContainer container = null;
                try { container = containerX.ToObject<VehicleContainer>(); } catch { }
                if (container != null)
                {
                    yield return container;
                }
            }
        }
    }
}
