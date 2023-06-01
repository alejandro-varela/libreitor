using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;

namespace PrintRec
{
    public class PrintRecSettings
    {
        public string   DataDir         { get; set; }

        public int      EndpointRadius  { get; set; }

        public Color    ColorFondo      { get; set; }

        public int      Granularidad    { get; set; }

        public DateTime StartDate       { get; set; }

        public string   OutputFilename  { get; set; }

        public string   Pathways        { get; set; }

        public List<Pathway> ParsePathways()
        {
            if (string.IsNullOrEmpty(Pathways))
            {
                return new List<Pathway>();
            }

            try
            {
                var jSerOpts = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Pathway>>(Pathways, jSerOpts);
            }
            catch (Exception exx)
            {
                return new List<Pathway>();
            }
        }

        public PrintRecSettings()
        {
            ColorFondo      = Color.Silver;
            DataDir         = ".";
            EndpointRadius  = 500;
            Granularidad    = 20;
            OutputFilename  = "output.png";
            StartDate       = DateTime.Now;
        }
    }
}
