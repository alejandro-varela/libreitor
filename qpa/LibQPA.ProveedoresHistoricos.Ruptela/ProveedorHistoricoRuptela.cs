using Comun;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LibQPA.ProveedoresHistoricos.Ruptela
{
    public class ProveedorHistoricoRuptela : IQPAProveedorPuntosHistoricos<int>
    {
        string _nombreArchivoCSV = "C:/Users/avarela/Desktop/Ruptela/CSV_Prueba/Posicionamiento.csv";

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public Dictionary<int, List<PuntoHistorico>> Get()
        {
            var registros = File.ReadLines(_nombreArchivoCSV)
                .Skip   (1)
                .Select (line => ParseRuptelaLine(line))
                .Where  (rrup => rrup.Fecha >= FechaDesde && rrup.Fecha <= FechaHasta)
                .OrderBy(rrup => rrup.Ficha)
                .ThenBy (rrup => rrup.Fecha)
            ;

            var dic = new Dictionary<int, List<PuntoHistorico>>();

            foreach (var rx in registros)
            {
                if (! dic.ContainsKey(rx.Ficha))
                {
                    dic.Add(rx.Ficha, new List<PuntoHistorico>());
                }

                dic[rx.Ficha].Add(new PuntoHistorico { 
                    Fecha = rx.Fecha, Lat = rx.Lat, Lng = rx.Lng, 
                });
            }

            return dic;
        }

        static RegistroRuptela ParseRuptelaLine(string line, char sep = ',')
        {
            var partes = line.Split(',');

            if (partes.Length != 4)
            {
                throw new Exception("No es una línea ruptela válida");
            }

            // reconozco la ficha...
            var parteFicha = partes[0];

            if (parteFicha[0] != 'F')
            {
                throw new Exception("No se puede leer la ficha en la línea");
            }

            int ficha = 0;

            for (int i = 1; ; i++)
            {
                char c = parteFicha[i];

                if (c >= '0' && c <= '9')
                {
                    ficha = ((ficha * 10) + (c - '0'));
                }
                else
                {
                    break;
                }
            }

            // reconozco la fecha (que es UTC)
            var fecha = DateTime.Parse(
                partes[1] + "Z"
            );

            // reconozco la lat
            var lat = double.Parse(
                partes[2].Replace(',', '.'),
                CultureInfo.InvariantCulture
            );

            // reconozco la lng
            var lng = double.Parse(
                partes[3].Replace(',', '.'),
                CultureInfo.InvariantCulture
            );

            return new RegistroRuptela
            {
                Ficha = ficha,
                Fecha = fecha,
                Lat = lat,
                Lng = lng,
            };
        }

        public class RegistroRuptela
        {
            public int Ficha { get; set; }
            public DateTime Fecha { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }

            public override string ToString()
            {
                return $"{Ficha} {Fecha:s} [Lat={Lat:000.0000}, Lng={Lng:000.0000}]";
            }
        }

    }
}
