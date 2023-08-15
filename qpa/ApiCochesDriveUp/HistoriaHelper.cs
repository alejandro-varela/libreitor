using ApiCochesDriveUp.Controllers;
using ComunApiCoches;
using ComunDriveUp;
using ComunStreams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ApiCochesDriveUp
{
    public class HistoriaHelper
    {
        public static Dictionary<string, object> DatosADiccionario(DatosDriveUp x)
        {
            return new Dictionary<string, object>
            {
                { "Ficha"       , x.Ficha       },
                { "Lat"         , x.Lat         },
                { "Lng"         , x.Lng         },
                { "FechaLocal"  , x.FechaLocal  },
                { "Recordedat"  , x.Recordedat  },
            };

            //return MapObjectToDict<DatosDriveUp>(
            //    x, 
            //    "Ficha,Lat,Lng,FechaLocal,Recordedat"
            //);
        }

        public static Dictionary<K, V> MapObjectToDict<T, K, V>(
            T @object,
            string commaSeparatedPropertyNames,
            Func<string, K> transformKey,
            Func<object, V> transformValue
        )
        {
            var propertyNames = commaSeparatedPropertyNames.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );

            var ret = new Dictionary<K, V>();
            var type = @object.GetType();

            foreach (var propertyNameX in propertyNames)
            {
                var prop = type.GetProperty(propertyNameX);
                var value = prop.GetValue(@object, null);

                var transformedKey = transformKey(propertyNameX);
                var transformedValue = transformValue(value);

                ret.Add(transformedKey, transformedValue);
            }

            return ret;
        }

        public static Dictionary<string, object> MapObjectToDict<T>(
            T @object,
            string commaSeparatedPropertyNames
        )
        {
            return MapObjectToDict<T, string, object>(
                @object,
                commaSeparatedPropertyNames,
                (pname) => pname,
                (value) => value
            );
        }

        public static TransStream GetCSVStream(string baseDir, string formato, DateTime fechaDesde, DateTime fechaHasta)
        {
            // tomo los paths de los archivos según desde hasta
            var paths = FilesHelper
                .GetPaths(baseDir, fechaDesde, fechaHasta)
                .Where(path => System.IO.File.Exists(path))
                .ToList()
            ;

            // se lo paso a un bolsaStream -> streamReader -> transStream + convertidorCsv
            BolsaStream bolsa = new(paths);
            StreamReader streamReader = new(bolsa);
            Func<string, string, string> convertirACSV = null;

            var conTitulo = formato == "csv";

            var convertidorCSV = new ConvertidorCSV<DatosDriveUp>
            {
                ConTitulo = conTitulo,
                DatosADiccionario = DatosADiccionario
            };

            convertirACSV = convertidorCSV.Convertir;

            TransStream trans = new(
                streamReader,
                convertirACSV,
                () => bolsa.CurrentFile
            );
            
            return trans;
        }
    }
}
