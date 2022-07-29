using System;
using System.Collections.Generic;
using Comun;
using Pinturas;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;

namespace VisorManual
{
    class Program
    {
        static void Main()
        { 
            //
        }

        // ConfundeBandera 20220721_060000_F4357
        static void Main000(string[] args)
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
