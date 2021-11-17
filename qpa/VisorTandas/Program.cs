using System;
using System.Collections.Generic;
using LibQPA.ProveedoresHistoricos.JsonSUBE;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using Comun;
using System.IO;

namespace VisorTandas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var file in Directory.GetFiles("D:\\EstadosCoches\\Agency49\\49_20211014"))
            {
                var vc = ProveedorHistoricoJsonSUBE.ParseJsonSUBE(File.ReadAllText(file))
                   .Where(vc => vc.Vehicle.Info.Label.Contains("304-"))
                   .FirstOrDefault();

                //Console.WriteLine(file);
                if (vc != null)
                { 
                    Console.WriteLine($"{Path.GetFileName(file)} {vc.Vehicle.TimeStamp} {vc.Vehicle.Pos.Lat} {vc.Vehicle.Pos.Lng}");
                }
            }

            var desde = new DateTime(2021, 10, 14, 0, 0, 0);
            var hasta = new DateTime(2021, 10, 15, 0, 0, 0);

            var proveedorPtsHistoJsonSUBE = new ProveedorHistoricoJsonSUBE
            {
                InputDir = @"D:\EstadosCoches\Agency49\",
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            var ptsHistoSUBEPorIdent = proveedorPtsHistoJsonSUBE.Get();
            var puntos = ptsHistoSUBEPorIdent["49-304"];
            var minutas = new DateTime?[1440];
            var pututas = new PuntoHistorico[1440];

            var json = JsonConvert.SerializeObject(puntos);

            foreach (var puntoX in puntos)
            {
                var index = (int)puntoX.Fecha.Subtract(desde).TotalMinutes;
                minutas[index] = puntoX.Fecha;
                pututas[index] = puntoX;
            }

            using Bitmap bmp = new SeqViewer<DateTime?>()
                .SetIsNoValuePredicate(x => !x.HasValue)
                .SetFilter((cur) => cur.HasValue ? Color.Black : Color.Gray)
                .SetSeparator(height: 1)
                .SetFilter((prev, cur) => prev.HasValue ? (prev.Value == cur.Value ? Color.Green : Color.YellowGreen) : Color.YellowGreen)
                .SetSeparator(height: 1)

                //.SetFilter((prev, cur) => prev.HasValue ? (prev.Value == cur.Value ? Color.Green : Color.YellowGreen) : Color.YellowGreen)
                //.SetSeparator(height: 1)

                .SetFilter((prev, cur) => prev.HasValue ? (prev.Value == cur.Value ? Color.Gray : Color.YellowGreen) : Color.YellowGreen)
                .Render(minutas)
            ;

            using Bitmap bm2 = new SeqViewer<PuntoHistorico>()
                .SetIsNoValuePredicate(x => x == null)
                .SetFilter((cur) => cur != null ? Color.Black : Color.Gray)
                
                .SetSeparator(height: 1)
                .SetFilter((prev, cur) => prev!=null ? (prev.Lat == cur.Lat && prev.Lng == cur.Lng ? Color.Green : Color.YellowGreen) : Color.YellowGreen)

                .SetSeparator(height: 1)
                .SetFilter((prev, cur) => prev != null ? (prev.Lat == cur.Lat && prev.Lng == cur.Lng ? Color.Gray : Color.YellowGreen) : Color.YellowGreen)

                .Render(pututas)
            ;

            bmp.Save("output.png", ImageFormat.Png);
            bm2.Save("outpu2.png", ImageFormat.Png);

            int foo = 0;
        }
    }
}
