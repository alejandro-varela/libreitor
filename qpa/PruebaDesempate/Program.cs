using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaDesempate
{
    class Program
    {
        static void Main(string[] args)
        {
            // 2 Junio 2021
            var fechaConsulta = new DateTime(2021, 6, 2, 0, 0, 0);

            // recorridos de '159' (203-PILAR) y '163' (203-MORENO)
            var recorridosRBus = Recorrido.LeerRecorridosPorArchivos("../../../../Datos/ZipRepo/", new int[] { 159, 163 }, fechaConsulta)
                .Select(reco => SanitizarRecorrido(reco))
                .ToList()
            ;

            // puntos aplanados (todos)
            var todosLosPuntosDeLosRecorridos = recorridosRBus.SelectMany(
                (reco) => reco.Puntos,
                (reco, puntoReco) => puntoReco
            );

            // topes
            var topes2d = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

            // puntas de línea
            var puntas = PuntasDeLinea.GetPuntasNombradas(recorridosRBus, 800);

            // caminos de los recos
            foreach (var recox in recorridosRBus)
            {
                var camino = Camino.CreateFromRecorrido(puntas, recox);
                Console.WriteLine($"{recox.Linea,-3} {recox.Bandera, -4} {camino.Description}");
            }

            // historia real
            var puntosHistoricos = Historia.GetRaw(
                3850, 
                fechaConsulta, 
                fechaConsulta.AddDays(1), 
                new PuntosHistoricosGetFromCSVConfig
                {
                    ExcluirCeros = true,
                    InvertLat = true,
                    InvertLng = true,
                }
            );

            // camino histórico
            var caminoHistorico = Camino.CreateFromPuntos(puntas, puntosHistoricos);
            Console.WriteLine(caminoHistorico.Description);

            int foo = 0;
        }

        // TODO: pasar esta función a RecorridoLinBan
        static RecorridoLinBan SanitizarRecorrido(RecorridoLinBan reco)
        {
            return new RecorridoLinBan
            {
                Bandera = reco.Bandera,
                Linea   = reco.Linea,
                Puntos  = reco.Puntos.HacerGranular(20),
            };
        }
    }
}
