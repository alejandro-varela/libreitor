using Recorridos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace PruebaLecturaDeRecorridos
{
    public class PuntoHistorico
    { 
        public DateTime Fecha { get; set; }
        public Punto    Punto { get; set; }
    }

    public class Historia
    {
        public int Ficha { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public List<PuntoHistorico> PuntosHistoricos { get; set; }

        public static Historia GetFromDB(int ficha, DateTime desde, DateTime hasta)
        {
            // TODO:
            return null;
        }

        public static Historia GetFromCSV(int ficha, DateTime desde, DateTime hasta, HistoriaGetFromCSVConfig config)
        {
            List<PuntoHistorico> puntos = new List<PuntoHistorico>();

            double latInverter = config.InvertLat ? -1 : 1;
            double lngInverter = config.InvertLng ? -1 : 1;

            foreach (string line in File.ReadLines("C:\\Users\\alejandro\\Desktop\\CSV\\2021-06-02.csv"))
            {
                // parto la linea
                string[] lineParts = line.Split(config.Separator);

                // leo ficha
                int     fichaX = int.Parse(lineParts[config.FichaPos]);
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
                double  latX = double.Parse(lineParts[config.LatitudPos ]) * latInverter;
                double  lngX = double.Parse(lineParts[config.LongitudPos]) * lngInverter;

                puntos.Add(new PuntoHistorico { 
                    Fecha = fechaHoraX,
                    Punto = new Punto { 
                        Lat = latX,
                        Lng = lngX,
                    }
                });
            }

            return new Historia()
            {
                Ficha = ficha,
                Desde = desde,
                Hasta = hasta,
                PuntosHistoricos = puntos,
            };
        }

        public static Historia GetFromCSV(int ficha, DateTime desde, DateTime hasta)
        {
            return GetFromCSV(ficha, desde, hasta, new HistoriaGetFromCSVConfig());
        }
    }

    public class HistoriaGetConfig
    {
        public bool InvertLat { get; set; }
        public bool InvertLng { get; set; }
    }

    public class HistoriaGetFromCSVConfig : HistoriaGetConfig
    {
        public int FichaPos     { get; set; } = 2;
        public int FechaHoraPos { get; set; } = 3;
        public int LatitudPos   { get; set; } = 4;
        public int LongitudPos  { get; set; } = 5;
        public string Separator { get; set; } = ";";
        public bool HasTitles { get; set; } = false;
    }
}
