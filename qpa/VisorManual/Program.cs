﻿using System;
using System.Collections.Generic;
using Comun;
using Pinturas;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.ComponentModel.DataAnnotations;

namespace VisorManual
{
    class Program
    {
        static void Main(string[] args)
        {
            // -Poner centroide comun en todas las versiones
            // -Poner leyenda
            // -Mostrar cambios en reporte
            // -Mostrar cambios geométricamente

            //var versiones = new HashSet<int> { 18, 19, 20, 21 };
            //var dataDir   = "../../../../Datos/ZipRepo/0127";
            //var outputDir = "../../../Vistas/RecorridosLinea41/";

            var versiones = new HashSet<int> { 45, 46, 47 };
            var dataDir   = "../../../../Datos/ZipRepo/0163";
            var outputDir = "../../../Vistas/RecorridosLinea163Moreno/";

            foreach (var verrecPath in Directory.GetFiles(dataDir, "verrec*.zip"))
            {
                // nombre de archivo de versión
                // index   012345678901234567890123456789
                // leyenda ppppppVVVVVVyyyyMMddHHmmss.ext
                // ejemplo verrec00001820200203000000.zip

                var fileName = Path.GetFileName(verrecPath);

                if (!Regex.IsMatch(fileName, "^verrec[0-9]{20}\\.zip$"))
                {
                    continue;
                }

                var fileVersion = int.Parse(fileName.Substring(6, 6));

                if (versiones.Contains(fileVersion))
                {
                    PintarRecorridos(
                        version   : fileVersion,
                        verrecPath: verrecPath,
                        outputDir : outputDir
                    );
                }

                int faa = 0;
            }

            int foo = 0;
        }

        static void PintarRecorridos(
            int version,
            string verrecPath, 
            string outputDir
        )
        {
            List<RecorridoLinBan> recorridos = Recorrido.LeerRecorridosFromZippedVerRec(verrecPath);
            List<PuntoRecorrido> todosLosPuntos = recorridos.SelectMany(r => r.Puntos).ToList();
            Topes2D topes2D = Topes2D.CreateFromPuntos(todosLosPuntos);
            for (int i = 0; i < recorridos.Count; i++)
            {
                PintorDeRecorrido pintorDeRecorrido = new PintorDeRecorrido(topes2D, granularidad: 20);
                pintorDeRecorrido.SetColorFondo(Color.Black);
                for (int j = 0; j < recorridos.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    pintorDeRecorrido.PintarPuntos(recorridos[j].Puntos, Color.Gray, size: 1);
                }
                pintorDeRecorrido.PintarPuntos(recorridos[i].Puntos, Color.Lime, size: 3);

                var bmp = pintorDeRecorrido.Render();
                var picFileName = string.Format(
                    "r{0:0000}{1:0000}_v{2:000000}.png", 
                    recorridos[i].Linea, 
                    recorridos[i].Bandera,
                    version
                );
                var pic = Path.Combine(outputDir, picFileName);
                bmp.Save(filename: pic, ImageFormat.Png);
            }
            int foo = 0;
        }

        static void Main_MapaConLetras_20220720_Mapa203(string[] args)
        {
            var dirData = Path.GetFullPath("../../../Vistas/MapaConLetras/20220720_MAPA203/");

            var fechaInicio = new DateTime(2022, 07, 20);

            var recorridosTeoricos = Recorrido.LeerRecorridosPorArchivos(dirData, new int[] { 159, 163 }, fechaInicio);

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
                    .GetPuntasNombradas(recorridosTeoricos, radio: 800)
                    .Select(pulin => (IPuntaLinea)pulin)
                    .ToList()
                ;

            // puntas de línea
            List<IPuntaLinea> puntasNombradas = creadorPuntasNombradas(recorridosTeoricos);

            var pintor = new PintorDeRecorrido(topes2D, granularidad: 20);

            pintor.SetColorFondo(Color.Silver);

            foreach (var recorrido in recorridosTeoricos)
            {
                var color = recorrido.Linea == 159 ? Color.Orange : Color.DarkGreen /*Color.FromArgb(30, Color.DarkGreen)*/ ;
                pintor.PintarPuntos(recorrido.Puntos, color, size: 3);
            }

            foreach (var punteta in puntasNombradas)
            {
                var pl = punteta as PuntaLinea;
                pintor.PintarRadioNombrado(pl.Punto, pl.Nombre, Color.Blue, size: 80);
            }

            pintor.Render().Save(Path.Combine(dirData, "salida.png"), ImageFormat.Png);
        }

        static void ConfundeBandera_20220721_060000_F4357(string[] args)
        {
            // 4357
            // 21 JUL 2022

            // MOPA 2772
            // MOCA 2773

            var dirData = Path.GetFullPath( "../../../Vistas/ConfundeBandera/20220721_060000_F4357/" );

            var archivoPuntos = Path.Combine(dirData, "puntos4357.csv");

            var desde = new DateTime(2022, 07, 21, 6, 0, 0);
            var hasta = new DateTime(2022, 07, 21, 7, 0, 0);

            var puntosHisto = File.ReadLines(archivoPuntos)
                .Where(sLine => GetFecha(sLine, ';', 5) >= desde && GetFecha(sLine, ';', 5) < hasta)
                .Select(sLine => new Punto { Lat = GetDouble(sLine, ';', 1), Lng = GetDouble(sLine, ';', 2) })
                .ToList()    
            ;

            var fechaInicio = new DateTime(2022, 07, 22);

            var recorridos = Recorrido.LeerRecorridosPorArchivos(dirData, new int[] { 163 }, fechaInicio);

            var recoMOPA = recorridos.Where(reco => reco.Bandera == 2772).FirstOrDefault();
            var recoMOCA = recorridos.Where(reco => reco.Bandera == 2773).FirstOrDefault();

            var todosLosPuntos = new List<Punto>();
            todosLosPuntos.AddRange(recoMOPA.Puntos);
            todosLosPuntos.AddRange(recoMOCA.Puntos);

            // cargar con los puntos historicos recorridos...
            // cargar con los recorridos de la linea en cuestión...
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntos);
            var pintor = new PintorDeRecorrido(topes2D, granularidad: 20);

            var bitmap = pintor
                .PintarPuntos(recoMOPA.Puntos, Color.FromArgb(65, Color.Blue), size: 5)
                .PintarPuntos(recoMOCA.Puntos, Color.FromArgb(35, Color.Red ), size: 5)
                .PintarPuntos(puntosHisto    , Color.FromArgb(90, Color.Violet), size: 7)
                .Render()
            ;

            bitmap.Save("c:/users/avarela/desktop/salida.png", ImageFormat.Png);
        }

        static double GetDouble(string s, char sep, int index)
        {
            var partes = s.Split(sep);
            return double.Parse(partes[index], CultureInfo.InvariantCulture);
        }

        static DateTime GetFecha(string s, char sep, int index)
        {
            var partes = s.Split(sep);
            return DateTime.Parse(partes[index]);
        }
    }
}
