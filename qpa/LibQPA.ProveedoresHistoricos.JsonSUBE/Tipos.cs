using Newtonsoft.Json;
using System;

namespace LibQPA.ProveedoresHistoricos.JsonSUBE
{
    public class PosicionSUBE
    {
        public int Ficha { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class Position
    {
        [JsonProperty("_latitude")]
        public double Lat { get; set; }

        [JsonProperty("_longitude")]
        public double Lng { get; set; }

        [JsonProperty("_speed")]
        public double Vel { get; set; } // averiguar en que unidad está
    }

    public class VehicleInfo
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_label")]
        public string Label { get; set; }

        public string GetIdentCocheSUBEFromLabel()
        {
            return Label.Split('-')[0];
        }

        public string GetLineaSUBEFromLabel()
        {
            return Label.Split('-')[1];
        }
    }

    public class Vehicle
    {
        [JsonProperty("_position")]
        public Position Pos { get; set; }

        [JsonProperty("_vehicle")]
        public VehicleInfo Info { get; set; }

        [JsonProperty("_timestamp")]
        public int TimeStamp { get; set; }

        public DateTime FechaUTCFromTimeStamp
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0)
                    .AddSeconds(TimeStamp);
            }
        }

        public DateTime FechaLocalFromTimeStamp
        {
            get
            {
                return FechaUTCFromTimeStamp.ToLocalTime();
            }
        }
    }

    public class VehicleContainer
    {
        [JsonProperty("_vehicle")]
        public Vehicle Vehicle { get; set; }
    }
}

