using Comun;
using ComunSUBE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibQPA.ProveedoresVentas.DbSUBE;
using LibQPA.ProveedoresHistoricos.DbSUBE;
using System.IO;

namespace PruebaConstruccionSegmentos
{
    class Program
    {
        static DatosEmpIntFicha             _datosEmpIntFicha;
        static ProveedorVentaBoletosDbSUBE  _proveedorVentaBoletosDbSUBE;
        static ProveedorHistoricoDbSUBE     _proveedorHistoricoDbSUBE;

        static void Main(string[] args)
        {
            var desde = new DateTime(2022, 1, 10, 0, 0, 0);
            var hasta = new DateTime(2022, 1, 11, 0, 0, 0);

            //var poropopop = DamePuntosPrueba(default, default, default);

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

            //////////////////////////////////////////////////////////////////
            // Construyo diccionarios de:
            //   Puntos x Segmento
            //   Puntos x Semirectas

            var dicPuntosXSemirecta     = new Dictionary<string, List<PuntoRecorrido>>();
            var dicCasillerosXSemirecta = new Dictionary<string, HashSet<Casillero>>();
            var dicPuntosXSegmento      = new Dictionary<string, List<PuntoRecorrido>>();
            var dicCasillerosXSegmento  = new Dictionary<string, HashSet<Casillero>>();

            // para cada recorrido
            foreach (RecorridoLinBan recorrido in recorridos)
            {
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntas, recorrido.Puntos);
                List<Link<char>> links = GetLinks(camino.DescriptionRawSinSimplificar, recorrido);

                Console.WriteLine($"{ recorrido.Linea } { recorrido.Bandera } { LinksToString(links) }");

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

            //////////////////////////////////////////////////////////////////
            // Puntos históricos: averiguo que pasó en este lapso de tiempo
            //  -con esto obtendré un diccionario de tipo:
            //   Dictionary<ParEmpresaInterno,List<Puntos>>
            //  -debo filtrar por empresa, probablemente en el proveedor
            //  -para cada cole, busco su coleccion de semirectas
            //  -interpreto las semirectas
            //  -fin

            IniciarProveedorPuntosKms(desde, hasta);
            DamePuntosFromTablaKms(49, 123, desde, hasta);
            
            //IniciarDatosEmpIntFicha();
            //IniciarProveedorVentasSUBE(desde, hasta);
            //var boletosXIdentificador = _proveedorVentaBoletosDbSUBE.BoletosXIdentificador;
            //var boletos = boletosXIdentificador[4307];

            //foreach (var boleto in boletos.Where(b => b.Latitud != 0 && b.Longitud != 0 ))
            //{
            //    var puntoide        = new Punto { Lat = boleto.Latitud, Lng = boleto.Longitud };
            //    var hacerWriteLine  = false;

            //    foreach (string kk in dicCasillerosXSemirecta.Keys)
            //    {
            //        var casilleroEnCuestion = Casillero.Create(
            //            topes2D,
            //            puntoide,
            //            granularidad
            //        );

            //        var contiene = Casillero.HashSetContieneCasilleroFlex(
            //            dicCasillerosXSemirecta[kk], 
            //            casilleroEnCuestion, 
            //            granularidad
            //        );

            //        if (contiene)
            //        {
            //            Console.Write($"{kk} ");
            //            hacerWriteLine = true;
            //        }
            //    }
            //    if (hacerWriteLine)
            //    {
            //        Console.WriteLine();
            //    }
            //}

            int foo = 0;
        }

        private static void IniciarDatosEmpIntFicha()
        {
            ///////////////////////////////////////////////////////////////////
            // Datos Empresa-Interno / Ficha
            ///////////////////////////////////////////////////////////////////
            _datosEmpIntFicha = new ComunSUBE.DatosEmpIntFicha(new ComunSUBE.DatosEmpIntFicha.Configuration()
            {
                CommandTimeout = 600,
                ConnectionString = File.ReadAllText("connstring_sube.txt").Trim(),
                MaxCacheSeconds = 15 * 60,
            });
        }

        private static void IniciarProveedorVentasSUBE(DateTime desde, DateTime hasta)
        {
            ///////////////////////////////////////////////////////////////////
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////
            var proveedorVentaBoletosConfig = new ProveedorVentaBoletosDbSUBE.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = File.ReadAllText("connstring_sube.txt").Trim(),
                DatosEmpIntFicha = _datosEmpIntFicha,
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            _proveedorVentaBoletosDbSUBE = new ProveedorVentaBoletosDbSUBE(
                proveedorVentaBoletosConfig
            );
        }

        private static void IniciarProveedorPuntosKms(DateTime desde, DateTime hasta)
        {
            ProveedorHistoricoDbSUBE.Configuracion config = new ProveedorHistoricoDbSUBE.Configuracion
            {
                ConnectionStringPuntos = File.ReadAllText("connstring_sube.txt").Trim(),
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            _proveedorHistoricoDbSUBE = new ProveedorHistoricoDbSUBE(config);
        }

        static List<(PuntoHistorico, bool, BoletoComun)> DamePuntosPrueba(int identificador, DateTime desde, DateTime hasta)
        {
            var ret = new List<(PuntoHistorico, bool, BoletoComun)>();
            var fbase = DateTime.Now;

            // boletos
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.0), Lat=32, Lng=60 }, true, new BoletoComun { FechaCancelacion = fbase.AddHours(1.0) }));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.2), Lat=32, Lng=60 }, true, new BoletoComun { FechaCancelacion = fbase.AddHours(1.2) }));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.4), Lat=32, Lng=60 }, true, new BoletoComun { FechaCancelacion = fbase.AddHours(1.4) }));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.6), Lat=32, Lng=60 }, true, new BoletoComun { FechaCancelacion = fbase.AddHours(1.6) }));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.8), Lat=32, Lng=60 }, true, new BoletoComun { FechaCancelacion = fbase.AddHours(1.8) }));

            // puntos kms
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.1), Lat=32, Lng=60 }, false, null));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.3), Lat=32, Lng=60 }, false, null));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.5), Lat=32, Lng=60 }, false, null));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.7), Lat=32, Lng=60 }, false, null));
            ret.Add((new PuntoHistorico { Fecha = fbase.AddHours(1.9), Lat=32, Lng=60 }, false, null));

            // ordenamos por fecha
            return ret
                .OrderBy(t => t.Item1.Fecha)
                .ToList()
            ;
        }

        static List<(PuntoHistorico,bool,BoletoComun)> DamePuntos(int empresa, int interno, DateTime desde, DateTime hasta)
        {
            var ret = new List<(PuntoHistorico, bool, BoletoComun)>();

            List<BoletoComun> boletos = _proveedorVentaBoletosDbSUBE.BoletosXIdentificador[_datosEmpIntFicha.GetFicha(empresa, interno)];

            foreach (var boleto in boletos)
            {
                var puntoHistorico = new PuntoHistorico {
                    Lat = boleto.Latitud,
                    Lng = boleto.Longitud,
                    Fecha = boleto.FechaCancelacion,
                };

                var tripla = (puntoHistorico, true, boleto);

                ret.Add(tripla);
            }

            // aca debo tomar los puntos de la vista de kms...
            List<PuntoHistorico> puntosKms = DamePuntosFromTablaKms(empresa, interno, desde, hasta);

            foreach (var puntoKms in puntosKms)
            {
                var tripla = (puntoKms, false, (BoletoComun) null);
                ret.Add(tripla);
            }
            
            // aca debo ordenar todo por fecha...
            return ret.OrderBy(t => t.Item1.Fecha).ToList();
        }

        static List<PuntoHistorico> DamePuntosFromTablaKms(int empresa, int interno, DateTime desde, DateTime hasta)
        {
            var pepe = _proveedorHistoricoDbSUBE.Get()
                .OrderBy(kvp => kvp.Key.Empresa)
                .ThenBy (kvp => kvp.Key.Interno)
                .ToList ()
            ;

            var pape = pepe
                .Where(kvp => kvp.Key.Empresa == 49)
                .ToList()
            ;

            var pipo = pepe
                .Where(kvp => kvp.Key.Empresa == 49 && kvp.Key.Interno == 4388)
                .FirstOrDefault()
            ;

            // 49 4388

            return null;
        }

        static string LinksToString(List<Link<char>> links)
        {
            var sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.Append(link.NameRay);
            }
            return sb.ToString();
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
