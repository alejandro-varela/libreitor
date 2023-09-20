using System;
using System.Collections.Generic;
using System.Linq;
using Comun;

namespace ExperimentosPuntaDeLinea_3_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var codLineas = new int[] { 159, 163 };
            var dir = "..\\..\\..\\..\\Datos\\ZipRepo\\";
            var fechaInicioCalculo = DateTime.Parse("2023-08-01T00:00:00");

            List<RecorridoLinBan> recorridos = RecorridoLinBan.LeerRecorridosPorArchivos(
                dir,
                codLineas,
                fechaInicioCalculo
            );

            int radio = 500;

            List<PuntaLinea> puntasDeLinea = PuntasDeLinea
                .GetPuntasNombradas(recorridos, radio)
                .ToList()
            ;

            int foo = 0;
        }
    }
}
