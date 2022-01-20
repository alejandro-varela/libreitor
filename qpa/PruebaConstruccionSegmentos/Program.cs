using Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibQPA.ProveedoresVentas.DbSUBE;

namespace PruebaConstruccionSegmentos
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            string   DIR_REC      = "../../../../Datos/ZipRepo/";
            int[]    lineas       = { 159, 163 };
            DateTime date         = DateTime.Now;
            int      granularidad = 20;
            int      radioPuntas  = 750;

            var recorridos = Recorrido
                .LeerRecorridosPorArchivos(DIR_REC, lineas, date)
                .Select(r => SanitizarRecorrido(r, granularidad))
                .ToList()
            ;

            var todosLosPuntosDeLosRecorridos = recorridos.SelectMany(
                (reco) => reco.Puntos,
                (reco, puntoReco) => puntoReco
            );

            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

            var puntas = PuntasDeLinea2
                .GetPuntasNombradas(recorridos: recorridos, radio: radioPuntas)
                .Select(pulin => (IPuntaLinea) pulin)
                .ToList()
            ;
            
            var dicPuntosXSemirecta     = new Dictionary<string, List<PuntoRecorrido>>();
            var dicCasillerosXSemirecta = new Dictionary<string, HashSet<Casillero>>();
            var dicPuntosXSegmento      = new Dictionary<string, List<PuntoRecorrido>>();
            var dicCasillerosXSegmento  = new Dictionary<string, HashSet<Casillero>>();

            // para cada recorrido
            foreach (Recorrido recorrido in recorridos)
            {
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntas, recorrido.Puntos);
                List<Link<char>> links = GetLinks(camino.DescriptionRawSinSimplificar, recorrido);
                
                // para cada link en el recorrido actual
                foreach (Link<char> linkX in links)
                {
                    var points = GetLinkPoints(linkX, recorrido);

                    // semirectas
                    {
                        var keyName = linkX.NameRay;
                        
                        // puntos
                        {
                            var dict = dicPuntosXSemirecta;
                            if (!dict.ContainsKey(keyName))
                            {
                                dict.Add(keyName, new List<PuntoRecorrido>());
                            }
                            dict[keyName].AddRange(points);
                        }

                        // casilleros
                        {
                            var dict = dicCasillerosXSemirecta;
                            if (!dict.ContainsKey(keyName))
                            {
                                dict.Add(keyName, new HashSet<Casillero>());
                            }
                            var casilleros = points.Select(p => Casillero.Create(topes2D, p, granularidad));
                            foreach (var cas in casilleros)
                            {
                                dict[keyName].Add(cas);
                            }
                        }
                    }

                    // segmentos
                    {
                        var keyName = linkX.NameSegment;

                        // puntos
                        {
                            var dict = dicPuntosXSegmento;
                            if (!dict.ContainsKey(keyName))
                            {
                                dict.Add(keyName, new List<PuntoRecorrido>());
                            }
                            dict[keyName].AddRange(points);
                        }

                        // casilleros
                        { 
                            var dict = dicCasillerosXSegmento;
                            if (!dict.ContainsKey(keyName))
                            {
                                dict.Add(keyName, new HashSet<Casillero>());
                            }
                            var casilleros = points.Select(p => Casillero.Create(topes2D, p, granularidad));
                            foreach (var cas in casilleros)
                            {
                                dict[keyName].Add(cas);
                            }
                        }
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////
            // Datos Empresa-Interno / Ficha
            ///////////////////////////////////////////////////////////////////
            var datosEmpIntFicha = new ComunSUBE.DatosEmpIntFicha(new ComunSUBE.DatosEmpIntFicha.Configuration()
            {
                CommandTimeout = 600,
                ConnectionString = "Data Source=192.168.201.42;Initial Catalog=sube;Persist Security Info=True;User ID=sa;Password=Bondi.amarillo", //Configu.ConnectionStringFichasXEmprIntSUBE,
                MaxCacheSeconds = 15 * 60,
            });

            ///////////////////////////////////////////////////////////////////
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////
            var desde = new DateTime(2022, 1, 10, 0, 0, 0);
            var hasta = new DateTime(2022, 1, 11, 0, 0, 0);

            var proveedorVentaBoletosConfig = new ProveedorVentaBoletosDbSUBE.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = "Data Source=192.168.201.42;Initial Catalog=sube;Persist Security Info=True;User ID=sa;Password=Bondi.amarillo", //Configu.ConnectionStringVentasSUBE,
                DatosEmpIntFicha = datosEmpIntFicha,
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            ProveedorVentaBoletosDbSUBE proveedorVentaBoletosDbSUBE = new ProveedorVentaBoletosDbSUBE(
                proveedorVentaBoletosConfig
            );
            
            var boletosXIdentificador = proveedorVentaBoletosDbSUBE.BoletosXIdentificador;
            //var boletos = boletosXIdentificador[4072];
            //var boletos = boletosXIdentificador[4353];
            var boletos = boletosXIdentificador[4307];

            foreach (var boleto in boletos.Where(b => b.Latitud != 0 && b.Longitud != 0 ))
            {
                var puntoide        = new Punto { Lat = boleto.Latitud, Lng = boleto.Longitud };
                var hacerWriteLine  = false;

                foreach (string kk in dicCasillerosXSemirecta.Keys)
                {
                    var casilleroEnCuestion = Casillero.Create(
                        topes2D,
                        puntoide,
                        granularidad
                    );

                    var contiene = Casillero.HashSetContieneCasilleroFlex(
                        dicCasillerosXSemirecta[kk], 
                        casilleroEnCuestion, 
                        granularidad
                    );

                    if (contiene)
                    {
                        Console.Write($"{kk} ");
                        hacerWriteLine = true;
                    }
                }
                if (hacerWriteLine)
                {
                    Console.WriteLine();
                }
            }

            int foo = 0;
        }

        public static List<Link<char>> GetLinks(string descriptionRawSinSimplificar, Recorrido recorrido)
        {
            var indexes = RepetitionIndexHelper.GetIndexes(descriptionRawSinSimplificar)
                .Where(i => i.Value != '.' && i.Value != '?')
                .ToList()
            ;

            return Engarzar(indexes)
                .Select(tup => new Link<char> { From = tup.Item1, To = tup.Item2 } )
                .ToList()
            ;
        }

        public static List<PuntoRecorrido> GetLinkPoints(Link<char> link, Recorrido recorrido)
        {
            ////       A A A             B  B
            ////       S   E             S  E
            //// 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27
            ////       |--------------------|

            //int from = link.From.StartIndex;
            //int take = (link.To.EndIndex - from) + 1;

            //       A A A             B  B
            //       S   E             S  E
            // 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27
            //             |--------|

            int from = link.From.EndIndex + 1;
            int take = link.To.StartIndex - from;

            return recorrido.Puntos
                .Skip(from)
                .Take(take)
                .ToList()
            ;

        }

        public static bool HasAtLeastNElems<T>(IEnumerable<T> items, int n)
        {
            if (items == null)
            {
                throw new ArgumentException(nameof(items));
            }

            if (n == 0) 
            {
                return true;
            }

            return items.Skip(n-1).Any();
        }

        static IEnumerable<(T,T)> Engarzar<T>(IEnumerable<T> items)
        {
            // si no hay dos elementos al menos me voy
            if (! HasAtLeastNElems(items, 2))
            {
                yield break;
            }

            // tomo el primer elemento
            var first = items.First();

            // para cada item en la secuencia de items (sin el primero)
            foreach (var item in items.Skip(1))
            { 
                yield return (first, item);
                first = item;
            }
        }

        static RecorridoLinBan SanitizarRecorrido(RecorridoLinBan r, int granularidad)
        {
            return new RecorridoLinBan
            {
                Linea   = r.Linea,
                Bandera = r.Bandera,
                Puntos  = r.Puntos.HacerGranular(granularidad).ToList(),
            };
        }
        
    }
}
