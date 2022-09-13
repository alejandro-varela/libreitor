using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Comun
{
    public class Historia
    {
        public int Ficha { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public List<List<PuntoHistorico>> SubHistorias { get; set; }
        public List<PuntoHistorico> Puntos { get; set; }

        public static IEnumerable<PuntoHistorico> GetRaw(int ficha, DateTime desde, DateTime hasta, PuntosHistoricosGetFromCSVConfig config)
        {
            List<PuntoHistorico> puntosHistoricos = new List<PuntoHistorico>();

            double latInverter = config.InvertLat ? -1 : 1;
            double lngInverter = config.InvertLng ? -1 : 1;

            foreach (string line in File.ReadLines("2021-09-02.csv"))
            {
                // parto la linea
                string[] lineParts = line.Split(new string[] { config.Separator }, StringSplitOptions.None);

                // leo ficha
                int fichaX = int.Parse(lineParts[config.FichaPos]);
                if (fichaX != ficha)
                {
                    continue;
                }

                // leo la fecha
                DateTime fechaHoraX = DateTime.Parse(lineParts[config.FechaHoraPos]);
                if (fechaHoraX < desde || fechaHoraX > hasta)
                {
                    continue;
                }

                // leo lat lng
                double latX = double.Parse(lineParts[config.LatitudPos]) * latInverter;
                double lngX = double.Parse(lineParts[config.LongitudPos]) * lngInverter;

                if (latX == 0 || lngX == 0)
                {
                    continue;
                }

                puntosHistoricos.Add(new PuntoHistorico
                {
                    Fecha = fechaHoraX,
                    Lat = latX,
                    Lng = lngX,
                });
            }

            return puntosHistoricos;
        }
    }
    
    public class PuntosHistoricosGetConfig
    {
        public bool InvertLat { get; set; }
        public bool InvertLng { get; set; }
        public bool ExcluirCeros { get; set; } = true;
    }

    public class PuntosHistoricosGetFromCSVConfig : PuntosHistoricosGetConfig
    {
        public int FichaPos     { get; set; } = 2;
        public int FechaHoraPos { get; set; } = 3;
        public int LatitudPos   { get; set; } = 4;
        public int LongitudPos  { get; set; } = 5;
        public string Separator { get; set; } = "\t";
        public bool HasTitles { get; set; } = false;
    }
}
