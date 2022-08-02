using Comun;
using ComunSUBE;
using LibQPA;
using LibQPA.ProveedoresTecnobus;
using LibQPA.ProveedoresVentas.DbSUBE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QPACreator
{
    public delegate List<int> ConstructorFichasDesdeResultados<TIdent>(List<QPAResult<TIdent>> resultados);
    public delegate HashSet<Casillero> FuncCasillerosXLinBan(int linea, int bandera, int granularidad);

    public class Creator
    {
        public class AvisoEventArgs : EventArgs
        {
            public string Mensaje { get; set; }
        }

        public delegate void AvisoEventHandler(object sender, AvisoEventArgs e);

        void Avisar(string sMensaje)
        {
            Aviso?.Invoke(this, new AvisoEventArgs { Mensaje = sMensaje });
        }

        HashSet<string> _boletosReconocidos = new HashSet<string>();
        int DBG_recosmal = 0;
        int DBG_recosbien = 0;

        public CreatorConfiguration Configu { get; private set; }

        public Creator(CreatorConfiguration testConfiguration)
        {
            Configu = testConfiguration;
        }

        public event AvisoEventHandler Aviso;

        public (List<QPAResult<TIdent>>, ReporteQPA<TIdent>) Calculate<TIdent>(
            string idReporte,
            DateTime desde,
            DateTime hasta,
            List<int> lineas,
            Type tipoPuntaLinea,
            Type tipoCreadorPartesHistoricas,
            Dictionary<TIdent, List<PuntoHistorico>> puntosXIdentificador,
            ConstructorFichasDesdeResultados<TIdent> constructorFichasDesdeResultados,
            int granularidadMts = 20,
            int radioPuntasDeLineaMts = 200
        )
        {
            // Ahora convierto puntosXIdentificador en infohXIdentificador
            Avisar("Convirtiendo puntosXIdentificador a infohXIdentificador");
            var infohXIdentificador = ConvertirPuntosAInformacion<TIdent>(
                puntosXIdentificador,
                Activator.CreateInstance(tipoCreadorPartesHistoricas) as CreadorPartesHistoricas
            );
            Avisar("\tOk");

            // Hago el cálculo génerico
            Avisar("Iniciando cálculo genérico");
            var (resultadosQPA, reporte) = CalculoGenericoQPA_ConReporte<TIdent>(
                idReporte,
                desde,
                hasta,
                lineas,
                infohXIdentificador,
                constructorFichasDesdeResultados,
                tipoPuntaLinea,
                granularidadMts,
                radioPuntasDeLineaMts
            );

            return (resultadosQPA, reporte);
        }

        static Dictionary<TIdent, InformacionHistorica> ConvertirPuntosAInformacion<TIdent>(
            Dictionary<TIdent, List<PuntoHistorico>> puntosXIdentificador,
            CreadorPartesHistoricas creadorPartes
        )
        {
            var ret = new Dictionary<TIdent, InformacionHistorica>();

            foreach (var kvp in puntosXIdentificador)
            {
                var infoHistorica = new InformacionHistorica
                {
                    PuntosCrudos = kvp.Value,
                    CreadorPartes = creadorPartes,
                };

                ret.Add(kvp.Key, infoHistorica);
            }

            return ret;
        }

        ////////////////////////////////////////////////////////////////

        (List<QPAResult<TIdent>>, ReporteQPA<TIdent>) CalculoGenericoQPA_ConReporte<TIdent>(
            string identificadorReporte,
            DateTime desde,
            DateTime hasta,
            List<int> lineas,
            Dictionary<TIdent, InformacionHistorica> infohXIdentificador,
            ConstructorFichasDesdeResultados<TIdent> constructorFichas,
            Type tipoPuntaLinea,
            int granularidadMts = 20,
            int radioPuntasDeLineaMts = 200
        )
        {
            if (hasta.Subtract(desde).TotalDays > 1)
            {
                throw new Exception("demasiados dias para el cálculo");
            }

            if (hasta <= desde)
            {
                throw new Exception("hasta <= desde");
            }

            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas = null;

            Avisar($"Usando tipo de punta: {tipoPuntaLinea.Name}");
            if (tipoPuntaLinea == typeof(PuntaLinea))
            {
                creadorPuntasNombradas = recorridosTeoricos =>
                    PuntasDeLinea
                        .GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts)
                        .Select(pulin => (IPuntaLinea)pulin)
                        .ToList()
                    ;
            }
            else if (tipoPuntaLinea == typeof(PuntaLinea2))
            {
                creadorPuntasNombradas = recorridosTeoricos =>
                    PuntasDeLinea2
                        .GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts, radioPuntasDeLineaMts * 2)
                        .Select(pulin => (IPuntaLinea)pulin)
                        .ToList()
                    ;
            }

            Avisar("Calculando resultados QPA");
            var resultadosQPA = CalcularQPA<TIdent>(
                identificadorReporte,
                desde,
                hasta,
                lineasPosibles: lineas.ToArray(),
                proveedorRecorridosTeoricos: new ProveedorVersionesTecnobus(dirRepos: DameMockRepos()), // los recorridos teóricos
                infohXIdentificador,
                creadorPuntasNombradas,
                granularidadMts
            );

            Avisar("Generando reporte QPA");
            var reporte = GenerarReporteQPA<TIdent>(
                identificadorReporte,
                desde,
                hasta,
                resultadosQPA,
                constructorFichas
            );

            //// poner reporte en un archivo...
            //string nombreReporte = $"Reporte__{identificadorReporte}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.txt";
            //File.WriteAllText(nombreReporte, reporte.ToString());

            Avisar("Reporte generado ok");

            return (resultadosQPA, reporte);
        }

        List<QPAResult<TIdent>> CalcularQPA<TIdent>(
            string identificador,
            DateTime desde,
            DateTime hasta,
            int[] lineasPosibles,
            IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos,
            Dictionary<TIdent, InformacionHistorica> infohXIdentificador,
            Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas,
            int granularidadMts = 20
        )
        {
            identificador ??= string.Empty;

            #region Recorridos teóricos

            ///////////////////////////////////////////////////////////////////
            // recorridos teóricos / topes / puntas nombradas / recopatterns
            ///////////////////////////////////////////////////////////////////
            
            Avisar("Procesando recorridos teóricos");
            
            var recorridosTeoricos = proveedorRecorridosTeoricos.Get(new QPAProvRecoParams()
            {
                LineasPosibles = lineasPosibles,
                FechaVigencia = desde
            })
                .Select(reco => SanitizarRecorrido(reco, granularidadMts))
                .ToList()
            ;

            // puntos aplanados (todos)
            var todosLosPuntosDeLosRecorridos = recorridosTeoricos.SelectMany(
                (reco) => reco.Puntos,
                (reco, puntoReco) => puntoReco
            );

            // topes
            var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

            // puntas de línea
            List<IPuntaLinea> puntasNombradas = creadorPuntasNombradas(recorridosTeoricos);

            // caminos de los recos, es un diccionario:
            // ----------------------------------------
            //  -de clave tiene el patrón de recorrido
            //  -de valor tiene una lista de pares (linea, bandera) que son las banderas que encajan con ese patrón
            var recoPatterns = new Dictionary<string, List<KeyValuePair<int, int>>>();
            foreach (var recox in recorridosTeoricos)
            {
                Avisar($"\tProcesando recorrido lin={recox.Linea} ban={recox.Bandera}");

                // creo un camino (su descripción es la clave)
                var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntasNombradas, recox.Puntos);

                // TODO: hay que ver el "porqué"
                // elimino los patrones de recorrido que tengan un solo char, ej. "A"
                // ya que estos patrones producen duraciones negativas
                if (camino.Description.Length == 1)
                {
                    continue;
                }

                // si la clave no está en el diccionario la agrego...
                if (!recoPatterns.ContainsKey(camino.Description))
                {
                    recoPatterns.Add(camino.Description, new List<KeyValuePair<int, int>>());
                }

                // agrego un par (linea, bandera) a la entrada actual...
                recoPatterns[camino.Description].Add(new KeyValuePair<int, int>(recox.Linea, recox.Bandera));
            }

            // muestro los patrones de recorridos:
            foreach (var keyX in recoPatterns.Keys.ToList().OrderBy(k => k))
            {
                Avisar($"RecoPattern {keyX}");
            }

            #endregion

            ///////////////////////////////////////////////////////////////////
            // Procesamiento de los datos (para todas las fichas)
            ///////////////////////////////////////////////////////////////////
            var resultadosQPA = ProcesarTodo<TIdent>(
                recorridosTeoricos,
                topes2D,
                puntasNombradas.Select(pu => (IPuntaLinea)pu).ToList(),
                recoPatterns,
                infohXIdentificador // puntosXIdentificador
            );

            return resultadosQPA;
        }

        public ReporteQPA<TIdent> GenerarReporteQPA<TIdent>(
            string identificador,
            DateTime desde,
            DateTime hasta,
            List<QPAResult<TIdent>> resultadosQPA,
            ConstructorFichasDesdeResultados<TIdent> constructorFichas
        )
        {
            ///////////////////////////////////////////////////////////////////
            // Datos Empresa-Interno / Ficha
            ///////////////////////////////////////////////////////////////////
            var datosEmpIntFicha = new ComunSUBE.DatosEmpIntFicha(new ComunSUBE.DatosEmpIntFicha.Configuration()
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionStringFichasXEmprIntSUBE,
                MaxCacheSeconds = 15 * 60,
            });

            ///////////////////////////////////////////////////////////////////
            // Venta de boletos
            ///////////////////////////////////////////////////////////////////
            Dictionary<int, List<BoletoComun>> boletosXFicha;

            string ARCHIVO_BOLETOS = $"Boletos__{identificador}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.json";

            //var proveedorVentaBoletosConfig = new ProveedorVentaBoletosDbSUBE.Configuracion
            //{
            //    CommandTimeout = 600,
            //    ConnectionString = Configu.ConnectionStringVentasSUBE,
            //    DatosEmpIntFicha = datosEmpIntFicha,
            //    FechaDesde = desde,
            //    FechaHasta = hasta,
            //};

            var proveedorVentaBoletosConfig2 = new ProveedorBoletosSUBE.Configuracion
            {
                CommandTimeout = 600,
                ConnectionString = Configu.ConnectionStringVentasSUBE,
                FechaDesde = desde,
                FechaHasta = hasta,
            };

            //ProveedorVentaBoletosDbSUBE proveedorVentaBoletos;

            //if (File.Exists(ARCHIVO_BOLETOS))
            //{
            //    var json = File.ReadAllText(ARCHIVO_BOLETOS);
            //    boletosXFicha = JsonConvert.DeserializeObject<Dictionary<int, List<BoletoComun>>>(json);
            //    proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(
            //        proveedorVentaBoletosConfig,
            //        boletosXFicha
            //    );
            //}
            //else
            //{
            //    proveedorVentaBoletos = new ProveedorVentaBoletosDbSUBE(proveedorVentaBoletosConfig);
            //    proveedorVentaBoletos.TieneBoletosEnIntervalo(0, DateTime.Now, DateTime.Now); // esto solo es para inicializar
            //    boletosXFicha = proveedorVentaBoletos.BoletosXIdentificador;
            //    File.WriteAllText(
            //        ARCHIVO_BOLETOS,
            //        JsonConvert.SerializeObject(boletosXFicha, Formatting.Indented)
            //    );
            //}

            ProveedorBoletosSUBE proveedorVentaBoletos2 = new ProveedorBoletosSUBE(proveedorVentaBoletosConfig2);

            var resulFichas = constructorFichas(resultadosQPA);

            Dictionary<int, (int, int)> empresaInternoSUBEXFichas = datosEmpIntFicha
                .Get()
                .ToDictionary(x => x.Value, x => x.Key)
            ;

            //var reporte = new CSVReport<string>()
            //{
            //    UsesHeader = true,
            //    Separator = ';',
            //    HeaderBuilder = (sep) => string.Join(sep, new[] { "empresaSUBE", "internoSUBE", "ficha", "linea", "bandera", "inicio", "fin", "cantbol", "cantbolopt" }),
            //    ItemsBuilder = (sep) => CrearItemsCSV(
            //        sep,
            //        resultadosQPA,
            //        resulFichas,
            //        empresaInternoSUBEXFichas,
            //        proveedorVentaBoletos
            //    )
            //};

            //return reporte;

            var lstReporteQPAItems = CrearReporteQPAItems(
                resultadosQPA,
                resulFichas,
                empresaInternoSUBEXFichas,
                proveedorVentaBoletos2
            );

            var reporte = new ReporteQPA<TIdent>()
            {
                Items = lstReporteQPAItems.ToList(),
            };

            return reporte;
        }

        IEnumerable<ReporteQPAItem<TIdent>> CrearReporteQPAItems<TIdent>(
            List<QPAResult<TIdent>> resultadosSUBE,
            List<int> fichasSUBE,
            Dictionary<int, (int, int)> empresaInternoSUBEXFichas,
            /*ProveedorVentaBoletosDbSUBE*/ ProveedorBoletosSUBE proveedorVentaBoletos
        )
        {
            var count = 0;
            var dicCasillerosXLinBan = new Dictionary<(int, int), HashSet<Casillero>>();

            for (int i = 0; i < resultadosSUBE.Count; i++)
            {
                var dameCasillerosXLinBan = new FuncCasillerosXLinBan((int linea, int bandera, int granuCustom) =>
                {
                    var key = (linea, bandera);
                    if (!dicCasillerosXLinBan.ContainsKey(key))
                    {
                        var recorrido = resultadosSUBE[i].RecorridosTeoricos
                            .Where(r => r.Linea == linea && r.Bandera == bandera)
                            .First()
                        ;
                        var casilleros = Geom.PuntosAHashSetCasilleros(
                            recorrido.Puntos,
                            granuCustom,
                            resultadosSUBE[i].Topes2D
                        );
                        dicCasillerosXLinBan[key] = casilleros;
                    }
                    return dicCasillerosXLinBan[key];
                });

                if (resultadosSUBE[i].PorcentajeReconocido > 0) // ex 80%
                {
                    count++;
                    var reporteQPAItem = CrearReporteQPAItem(
                        resultadosSUBE[i],
                        fichasSUBE[i],
                        empresaInternoSUBEXFichas,
                        proveedorVentaBoletos,
                        dameCasillerosXLinBan
                    );

                    if (reporteQPAItem != null)
                    {
                        yield return reporteQPAItem;
                    }
                }
            }
        }

        ReporteQPAItem<TIdent> CrearReporteQPAItem<TIdent>(
    QPAResult<TIdent> qpaResult,
    int ficha,
    Dictionary<int, (int, int)> empresaInternoSUBEXFichas,
    /*ProveedorVentaBoletosDbSUBE*/ ProveedorBoletosSUBE proveedorVentaBoletos,
    FuncCasillerosXLinBan dameCasillerosXLinBan
)
        {
            if (ficha == -1)
            {
                return null;
            }

            var subItems = new List<ReporteQPASubItem<TIdent>>();

            foreach (var subCamino in qpaResult.SubCaminos.ToArray().Reverse())
            {
                // empresa e ident sube...
                var empresaSUBE = empresaInternoSUBEXFichas[ficha].Item1;
                var internoSUBE = empresaInternoSUBEXFichas[ficha].Item2;

                // linea y bandera
                var linBanPuns = subCamino
                    .LineasBanderasPuntuaciones
                    .OrderByDescending(lbp => lbp.Puntuacion)
                    .ToList()
                ;
                var linea = linBanPuns[0].Linea;
                var bandera = linBanPuns[0].Bandera;

                // inicio y fin
                var inicio = subCamino.HoraSalida.ToString("dd/MM/yyyy HH:mm:ss");
                var fin = subCamino.HoraLlegada.ToString("dd/MM/yyyy HH:mm:ss");

                // cant de boletos (naive)
                var cantBoletosNaive = proveedorVentaBoletos
                    .GetBoletosEnIntervalo(new ParEmpresaInterno { Empresa = empresaSUBE, Interno = internoSUBE } /*ficha*/, subCamino.HoraSalida, subCamino.HoraLlegada)
                    .Count()
                ;

                // cant de boletos (optimizada)
                var horaComienzoBoletos = subCamino.SubCaminoAnterior == null ?
                    subCamino.HoraSalida :
                    subCamino.SubCaminoAnterior.HoraLlegada
                ;
                var horaFinBoletos = subCamino.HoraLlegada;

                // BASE DE DATOS DE BOLETOS = DESASTRE
                //  - LA LAT Y LNG INVERTIDA Y ENTERAAAAA    - OK
                //  - BOLETOS QUE TIENEN LAT LNG EN 0        - OK
                //  - ELIMINAR BOLETOS IGUALES               - TODO
                var boletos = proveedorVentaBoletos
                    .GetBoletosEnIntervalo(new ParEmpresaInterno { Empresa = empresaSUBE, Interno = internoSUBE } /*ficha*/, horaComienzoBoletos, horaFinBoletos)
                    .ToList()
                ;
                var deltaGran = 2;
                var boletosOpt = new List<BoletoComun>();

                var DBG_cantBoletosConLatLng = boletos
                    .Where(bolx => bolx.Latitud != 0 && bolx.Longitud != 0)
                    .Count()
                ;
                var DBG_cantBoletosQueEntranEnRecorrido = 0;

                // 1) los boletos que entran en el recorrido
                foreach (var bolx in boletos)
                {
                    bool enReco = Geom.PuntoEnRecorrido(
                        new Punto { Lat = bolx.Latitud, Lng = bolx.Longitud },
                        dameCasillerosXLinBan(linea, bandera, qpaResult.Granularidad * deltaGran),
                        qpaResult.Granularidad * deltaGran,
                        qpaResult.Topes2D
                    );

                    if (enReco && !_boletosReconocidos.Contains(bolx.Id))
                    {
                        boletosOpt.Add(bolx);
                        _boletosReconocidos.Add(bolx.Id);
                        DBG_cantBoletosQueEntranEnRecorrido++;
                    }
                }

                if (DBG_cantBoletosConLatLng > 0 && DBG_cantBoletosQueEntranEnRecorrido == 0)
                {
                    int QUE_bandera = bandera;
                    int QUE_linea = linea;
                    int foo = 0;
                    DBG_recosmal++;
                }
                else
                {
                    DBG_recosbien++;
                }

                // 2) los boletos que están a un radio "grande" de la salida
                //    esto es, unos 250 mts por lo menos
                var puntoSalida = qpaResult.RecorridosTeoricos
                    .Where(recot => recot.Linea == linea && recot.Bandera == bandera)
                    .First()
                    .Puntos
                    .First()
                ;
                foreach (var bolx in boletos)
                {
                    var puntoBoleto = new Punto { Lat = bolx.Latitud, Lng = bolx.Longitud };

                    if ((Haversine.GetDist(puntoSalida, puntoBoleto) <= 250) &&
                        (!_boletosReconocidos.Contains(bolx.Id)))
                    {
                        boletosOpt.Add(bolx);
                        _boletosReconocidos.Add(bolx.Id);
                    }
                }

                // 3) los boletos lat=0 lng=0 que estén en el intervalo de tiempo
                //    mayor o igual al boleto mas viejo que acabo de calcular
                if (boletosOpt.Any())
                {
                    var fechaMasVieja = boletosOpt
                        .OrderBy(b => b.FechaCancelacion)
                        .First()
                        .FechaCancelacion
                    ;

                    foreach (var bolx in boletos)
                    {
                        if ((bolx.Latitud == 0 || bolx.Longitud == 0) &&
                            bolx.FechaCancelacion >= fechaMasVieja &&
                            !_boletosReconocidos.Contains(bolx.Id))
                        {
                            boletosOpt.Add(bolx);
                            _boletosReconocidos.Add(bolx.Id);
                        }
                    }
                }

                // 4) ahora, con la fecha original de inicio (subCamino.HoraSalida)
                //    veremos si entran los boletos con lat=0 lng=0 que quedaron afuera
                foreach (var bolx in boletos)
                {
                    if ((bolx.Latitud == 0 || bolx.Longitud == 0) &&
                        bolx.FechaCancelacion >= subCamino.HoraSalida &&
                        !_boletosReconocidos.Contains(bolx.Id))
                    {
                        var pepe = linea;
                        var pipo = bandera;
                        boletosOpt.Add(bolx);
                        _boletosReconocidos.Add(bolx.Id);
                    }
                }

                // cantidad de boletos "optimizados"
                var cantBoletosOpt = boletosOpt.Count;

                // construcción del subItem
                var subItem = new ReporteQPASubItem<TIdent>
                {
                    EmpresaSUBE = empresaSUBE,
                    InternoSUBE = internoSUBE,
                    Ficha = ficha,
                    Linea = linea,
                    Bandera = bandera,
                    Inicio = subCamino.HoraSalida,
                    Fin = subCamino.HoraLlegada,
                    CantBoletosNaive = cantBoletosNaive,
                    CantBoletosOpt = cantBoletosOpt
                };

                // acumulo subItems
                subItems.Insert(0, subItem);
            }

            return new ReporteQPAItem<TIdent>
            {
                Resultado = qpaResult,
                Items = subItems,
            };
        }


        RecorridoLinBan SanitizarRecorrido(RecorridoLinBan reco, int granularidad)
        {
            return new RecorridoLinBan
            {
                Bandera = reco.Bandera,
                Linea = reco.Linea,
                Puntos = reco.Puntos.HacerGranular(granularidad),
            };
        }

        List<QPAResult<TIdent>> ProcesarTodo<TIdent>(
            List<RecorridoLinBan> recorridosTeoricos,
            Topes2D topes2D,
            List<IPuntaLinea> puntasNombradas,
            Dictionary<string, List<KeyValuePair<int, int>>> recoPatterns,
            Dictionary<TIdent, InformacionHistorica> infohXIdentificador
        )
        {
            var qpaProcessor = new QPAProcessor();
            var resultados = new List<QPAResult<TIdent>>();

            foreach (var ident in infohXIdentificador.Keys)
            {
                try
                {
                    var partesHistoricas = infohXIdentificador[ident].GetPartesHistoricas().ToList();

                    foreach (ParteHistorica parteHistorica in partesHistoricas)
                    {
                        Avisar($"Procesando coche {ident}");

                        var resultado = qpaProcessor.Procesar(
                            identificador: ident,
                            recorridosTeoricos: recorridosTeoricos,
                            puntosHistoricos: parteHistorica.Puntos,
                            topes2D: topes2D,
                            puntasNombradas: puntasNombradas,
                            recoPatterns: recoPatterns
                        );

                        resultado = VelocidadNormal(resultado);
                        resultado = DuracionPositiva(resultado);

                        //if (resultado.SubCaminos.Any())
                        {
                            resultados.Add(resultado);
                        }
                    }
                }
                catch (Exception exx)
                {
                    int foo = 0;
                }
            } // para cada identificador...

            return resultados;
        }

        private QPAResult<TIdent> VelocidadNormal<TIdent>(QPAResult<TIdent> res)
        {
            var newSubCaminos = res.SubCaminos
                .Where(subCamino =>
                    subCamino.VelocidadKmhPromedio <= 120 &&
                    subCamino.VelocidadKmhPromedio >= 5
                )
                .ToList()
            ;

            return new QPAResult<TIdent>
            {
                Granularidad = res.Granularidad,
                RecorridosTeoricos = res.RecorridosTeoricos,
                Topes2D = res.Topes2D,
                Camino = res.Camino,
                Identificador = res.Identificador,
                SubCaminos = newSubCaminos,
            };
        }

        private QPAResult<TIdent> DuracionPositiva<TIdent>(QPAResult<TIdent> res)
        {
            var newSubCaminos = res.SubCaminos
                .Where(subCamino => subCamino.HoraLlegada > subCamino.HoraSalida)
                .ToList()
            ;

            return new QPAResult<TIdent>
            {
                Granularidad = res.Granularidad,
                RecorridosTeoricos = res.RecorridosTeoricos,
                Topes2D = res.Topes2D,
                Camino = res.Camino,
                Identificador = res.Identificador,
                SubCaminos = newSubCaminos,
            };
        }

        private string[] DameMockRepos()
        {
            return new[] { Configu.Repos["MockRepo1"], Configu.Repos["MockRepo2"] };
        }

    }
}
