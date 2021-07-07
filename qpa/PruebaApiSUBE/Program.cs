using Comun;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;

namespace PruebaApiSUBE
{
    class Program
    {
        // api de larreta
        // https://apitransporte.buenosaires.gob.ar/colectivos/vehiclePositions?json=1&agency_id=49&client_id=4aa160cfad8f4b10bbc4a0ce8d2296f3&client_secret=9a448561738644B5ac6Eba7705bD31C3

        static void Main(string[] args)
        {
            Uri uri         = ConstruirUri();
            var uriString   = uri.ToString(); // debug
            //var json        = GetString(uri);
            var json = File.ReadAllText("..\\..\\..\\ejemplo.json");

            foreach (var container in ParseRespuesta(json))
            {
                Console.WriteLine($"{container.Vehicle.Pos.Lat} {container.Vehicle.Pos.Lng} {container.Vehicle.Info.Label}");
            }

            int esto_es_para_debuguear = 0; // debug
        }

        public static IEnumerable<VehicleContainer> ParseRespuesta(string json)
        {
            JObject data =
                JsonConvert.DeserializeObject(json)
                as JObject;

            var containers = data["_entity"] as JArray;

            foreach (var containerX in containers)
            {
                var container = containerX.ToObject<VehicleContainer>();
                yield return container;
            }
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
        }

        public class Vehicle
        {
            [JsonProperty("_position")]
            public Position Pos { get; set; }

            [JsonProperty("_vehicle")]
            public VehicleInfo Info { get; set; }
        }

        public class VehicleContainer
        {
            [JsonProperty("_vehicle")]
            public Vehicle Vehicle { get; set; }
        }

        private static Uri ConstruirUri()
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host   = "apitransporte.buenosaires.gob.ar",
                Path   = "/colectivos/vehiclePositions"
            }
                .AddQuery("json", "1")
                .AddQuery("agency_id", "49")
                .AddQuery("client_id", "4aa160cfad8f4b10bbc4a0ce8d2296f3")
                .AddQuery("client_secret", "9a448561738644B5ac6Eba7705bD31C3")
            ;

            var uri = uriBuilder.Uri;

            return uri;
        }

        static string GetString (Uri uri)
        {
            using WebClient webClient = new();

            string json = webClient.DownloadString(uri);

            return json;
        }
    }

    public static class UriBuilderExtensions
    {
        public static UriBuilder AddQuery(this UriBuilder uriBuilder, string paramName, string paramValue)
        {
            var queryNVC = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryNVC[paramName] = paramValue;
            uriBuilder.Query = queryNVC.ToString(); // <-- este ToString no es el normal
            return uriBuilder;
        }
    }
}
