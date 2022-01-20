using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Comun
{
    public class PuntasDeLinea2
    {
        // las puntas se podrían agrupar usando el algoritmo "#" y mirando la superficie del cuadrado enmarcado
        public static IEnumerable<PuntaLinea2> GetPuntasNombradas(IEnumerable<RecorridoLinBan> recorridos, int radio = 200, int radioAgrupacion = 400)
        {
            var indexNombre = 0;
            var nombres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

            foreach (var ptx in GetPuntasDeLinea(recorridos, radio, radioAgrupacion))
            {
                yield return new PuntaLinea2
                {
                    Nombre          = nombres[indexNombre].ToString(),
                    Puntos          = ptx.Puntos,
                    Radio           = ptx.Radio,
                    RadioAgrupacion = ptx.RadioAgrupacion,
                };

                indexNombre++;
            }


            //var puntas = new List<PuntaLinea2>();
            //int n = 0;
            //foreach (var recorrido in recorridos)
            //{
            //    n = AgregarSiNoEstaPresente(recorrido.Linea, recorrido.Bandera, puntas, recorrido.PuntoSalida, n, radio);
            //    n = AgregarSiNoEstaPresente(recorrido.Linea, recorrido.Bandera, puntas, recorrido.PuntoLlegada, n, radio);
            //}
            //return puntas;
        }

        //static int AgregarSiNoEstaPresente(int linea, int bandera, List<PuntaLinea2> puntas, PuntoRecorrido punto, int n, int radio)
        //{
        //    foreach (var puntaX in puntas)
        //    {
        //        if (EnPunta(puntaX, punto, radio))
        //        {
        //            puntaX.Varios.Add(new Tuple<int, int>(linea, bandera));
        //            return n;
        //        }
        //    }

        //    var varios = new List<Tuple<int, int>>();
        //    varios.Add(new Tuple<int, int>(linea, bandera));
        //    var nombre = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz"[n].ToString();
        //    puntas.Add(new PuntaLinea2 { Punto = punto, Nombre = nombre, Varios = varios, Radio = radio });

        //    return n + 1;
        //}

        private static bool EnPunta(PuntaLinea2 puntaX, PuntoRecorrido punto, int radio)
        {
            return puntaX.Puntos
                .Select (puntoPunta => Haversine.GetDist(puntoPunta, punto))
                .Any    (dist => dist < radio)
            ;
        }

        public static IEnumerable<PuntaLinea2> GetPuntasDeLinea(IEnumerable<RecorridoLinBan> recorridosRBus, int radioPuntas, int radioAgrupacion)
        {
            var puntos = new List<PuntoRecorrido>();
            foreach (var rec in recorridosRBus)
            {
                puntos.Add(rec.PuntoSalida );
                puntos.Add(rec.PuntoLlegada);
            }

            var parejas = new List<(PuntoRecorrido, PuntoRecorrido)>();
            var conjuntos = new List<HashSet<PuntoRecorrido>>();

            for (int i = 0; i < puntos.Count; ++i)
            {
                for (int j = i+1; j < puntos.Count; ++j)
                {
                    var pi = puntos[i];
                    var pj = puntos[j];

                    if (Haversine.GetDist(pi, pj) <= radioAgrupacion)
                    {           
                        parejas.Add((pi, pj));

                        HashSet<PuntoRecorrido> conji = conjuntos.FirstOrDefault(conj => conj.Contains(pi));
                        HashSet<PuntoRecorrido> conjj = conjuntos.FirstOrDefault(conj => conj.Contains(pj));

                        if (conji == null && conjj == null)
                        {
                            var conjx = new HashSet<PuntoRecorrido>();
                            conjx.Add(pi);
                            conjx.Add(pj);
                            conjuntos.Add(conjx);
                            //Console.Write("Caso1 ");
                        }
                        else if (conji != null && conjj == null)
                        {
                            conji.Add(pj);
                            //Console.Write("Caso2 ");
                        }
                        else if (conji == null && conjj != null)
                        {
                            conjj.Add(pi);
                            //Console.Write("Caso3 ");
                        }
                        else if (conji == conjj)
                        {
                            int foo = 0;
                            //Console.Write("Caso4 ");
                        }
                        else if (conji != conjj)
                        {
                            var conjx = new HashSet<PuntoRecorrido>();
                            conjx.UnionWith(conji);
                            conjx.UnionWith(conjj);
                            conjuntos.Add(conjx);
                            conjuntos.Remove(conji);
                            conjuntos.Remove(conjj);
                            //Console.Write("Caso5 ");
                        }
                    }
                }
            }

            foreach (var conj in conjuntos)
            {
                yield return new PuntaLinea2
                {
                    Puntos = conj.ToList(),
                    Radio  = radioPuntas,
                    RadioAgrupacion = radioAgrupacion,
                };
            }
        }
    }
}
