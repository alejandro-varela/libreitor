using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Comun;
using Pinturas;

namespace PrintRec
{
    public class App
    {
        public int Run(PrintRecSettings settings)
        {
            var dataDir = Path.GetFullPath(settings.DataDir);
   
            var fechaInicio = settings.StartDate;

            var pathways = settings.ParsePathways().ToList();

            var lineas = settings
                .ParsePathways()
                .Select(r => r.Linea)
                .ToList()
            ;

            var recorridosTeoricos = Recorrido.LeerRecorridosPorArchivos(
                dataDir,
                lineas.ToArray(),
                fechaInicio
            );

            // puntos aplanados (todos)
            var todosLosPuntosDeLosRecorridos = recorridosTeoricos.SelectMany(
                (reco) => reco.Puntos,
                (reco, puntoReco) => puntoReco
            );

            // topes
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas = null;

            creadorPuntasNombradas = recorridosTeoricos =>
                PuntasDeLinea
                    .GetPuntasNombradas(
                        recorridos: recorridosTeoricos, 
                        radio: settings.EndpointRadius
                    )
                    .Select(pulin => (IPuntaLinea)pulin)
                    .ToList()
                ;

            // puntas de línea
            List<IPuntaLinea> puntasNombradas = creadorPuntasNombradas(recorridosTeoricos);

            var pintor = new PintorDeRecorrido(
                topes2D: topes2D, 
                granularidad: settings.Granularidad
            );

            pintor.SetColorFondo(settings.ColorFondo);

            // pinto en gris las "líneas"
            foreach (var recorrido in recorridosTeoricos)
            {
                pintor.PintarPuntos(recorrido.Puntos, Color.Gray, size: 3);
            }

            // pinto cada recorrido pedido
            foreach (var pathwayItem in pathways)
            {
                List<PuntoRecorrido> puntos = null;

                foreach (var recorrido in recorridosTeoricos)
                {
                    if (recorrido.Linea   == pathwayItem.Linea &&
                        recorrido.Bandera == pathwayItem.Bandera)
                    {
                        puntos = recorrido.Puntos.ToList();
                    }
                }

                if (puntos == null)
                {
                    continue;
                }

                pintor.PintarPuntos(
                    puntos, 
                    Color.FromName(pathwayItem.Color), 
                    pathwayItem.Stroke
                );

                pintor.PintarPunto(puntos[0], Color.Green, size: 30);
                pintor.PintarPunto(puntos[^1], Color.Red, size: 30);
            }

            foreach (var punteta in puntasNombradas)
            {
                var pl = punteta as PuntaLinea;
                var size = settings.EndpointRadius / Math.Max(settings.Granularidad, 1);
                pintor.PintarRadioNombrado(pl.Punto, pl.Nombre, Color.Blue, size: size);
            }

            pintor
                .Render ()
                .Save   (settings.OutputFilename, ImageFormat.Png)
            ;

            return 0;
        }
    }
}
