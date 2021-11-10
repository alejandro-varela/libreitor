using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Comun;
using Pinturas;

namespace PintorRecorridosGenerico
{
    class Program
    {
        static void Main(string[] args)
        {
            // params:
            var lineas      = new int[] { 166 , 167 };
            var date        = DateTime.Now;
            var dir         = "../../../../Datos/ZipRepo/";
            var granu       = 20;
            var radioPuntas = 200;

            // recorridos y puntos
            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos(dir, lineas, date);

            var todosLosPuntosRec = recorridosRBus
                .SelectMany(
                    (reco) => reco.Puntos.HacerGranular(granu),
                    (reco, punto) => new PuntoRecorridoLinBan(punto, reco.Linea, reco.Bandera)
                )
                .ToList()
            ;

            // puntas de línea
            var puntas = PuntasDeLinea.GetPuntasNombradas(
                recorridosRBus, 
                radioPuntas
            );

            var puntas2 = PuntasDeLinea2.GetPuntasDeLinea(
                recorridosRBus,                     // una lista de recorridos
                radioPuntas,                        // el radio de detección de cada item de la punta de línea (puede ser mas que uno)
                radioAgrupacion: radioPuntas * 2    // la máxima distancia que pueden tener las puntas de línea para ser incluidas en un mismo grupo entre si
            )
            .ToList();

            // inicializar objetos
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosRec);

            // pintar (ver problema index 6)
            foreach (var rec in recorridosRBus)
            {
                var pintor = new PintorDeRecorrido(topes2D, granu)

                    //.SetColorFondo(Color.Aquamarine)
                    .SetColorFondo(Color.Gray)

                    //.PintarPuntos(recorridosRBus.Select(rec => rec.PuntoSalida), Color.Fuchsia, 15)
                    //.PintarPuntos(recorridosRBus.Select(rec => rec.PuntoLlegada), Color.Fuchsia, 15)

                    //.PintarPuntos(rec.Puntos, Color.Lime, 3)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 166), Color.DarkGray)
                    .PintarPuntos(todosLosPuntosRec.Where(pr => pr.Linea == 167), Color.DarkGray)

                //.PintarRadio(recorridosRBus[index].Puntos.First(), Color.Yellow, 800 / granu)
                //.PintarRadio(recorridosRBus[index].Puntos.Last (), Color.Yellow, 800 / granu)
                //.PintarRadios(puntas.Select(pu => pu.Punto), Color.Aqua, radioPuntas / granu)
                //.PintarRadiosNombrados(puntas.Select(pu => (pu.Punto, pu.Nombre)), Color.LightBlue, radioPuntas / (granu / 2))

                ;

                var random = new Random(Environment.TickCount);
                var colori = 40;
                foreach (PuntaLinea2 pun2 in puntas2)
                {
                    //var colorRandom = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    var colorRandom = Color.FromKnownColor((KnownColor)colori);
                    pintor.PintarPuntos(pun2.Puntos, Color.FromArgb(255, colorRandom), radioPuntas * 2 / granu);
                    pintor.PintarPuntos(pun2.Puntos, Color.Black, 3);
                    pintor.PintarPuntos(pun2.Puntos, Color.White, 1);
                    colori += 1;
                }

                pintor.PintarPuntos(rec.Puntos, Color.FromArgb(50, Color.Lime), 3);

                var bitmap = pintor.Render();

                bitmap.Save($"rec_{rec.Linea:0000}_{rec.Bandera:0000}.png", ImageFormat.Png);

                int fin = 0;
            }
        }
    }
}
