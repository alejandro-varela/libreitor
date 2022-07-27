using Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibQPA
{
    //public delegate List<int> ConstructorFichasDesdeResultados<TIdent>(List<QPAResult<TIdent>> resultados);

    //public class Calculator
    //{
    //    public object CalculateXFicha(
    //        string idReporte,
    //        string desdeISO8601,
    //        string hastaISO8601,
    //        string lineasPosiblesSeparadasPorComa,
    //        Type tipoPuntaLinea,
    //        Type tipoCreadorPartesHistoricas,
    //        Dictionary<int, List<PuntoHistorico>> puntosXIdentificador,
    //        int granularidadMts = 20,
    //        int radioPuntasDeLineaMts = 200
    //        )
    //    {
    //        var desde = DateTime.Parse(desdeISO8601);
    //        var hasta = DateTime.Parse(hastaISO8601);

    //        // Ahora convierto puntosXIdentificador en infohXIdentificador
    //        var infohXIdentificador = ConvertirPuntosAInformacion(
    //            puntosXIdentificador,
    //            Activator.CreateInstance(tipoCreadorPartesHistoricas) as CreadorPartesHistoricas
    //        );

    //        var (resultadosQPA, reporte) = CalculoGenericoQPA_ConReporte<int>(
    //            idReporte,
    //            desde,
    //            hasta,
    //            lineasPosiblesSeparadasPorComa,
    //            infohXIdentificador,
    //            ConstructorFichas,
    //            tipoPuntaLinea,
    //            granularidadMts,
    //            radioPuntasDeLineaMts
    //        );
    //    }

    //    static List<int> ConstructorFichas(List<QPAResult<int>> resultadosQPA)
    //    {
    //        return resultadosQPA
    //            .Select(rqpa => rqpa.Identificador)
    //            .ToList()
    //        ;
    //    }

    //    static Dictionary<TIdent, InformacionHistorica> ConvertirPuntosAInformacion<TIdent>(
    //        Dictionary<TIdent, List<PuntoHistorico>> puntosXIdentificador,
    //        CreadorPartesHistoricas creadorPartes
    //    )
    //    {
    //        var ret = new Dictionary<TIdent, InformacionHistorica>();

    //        foreach (var kvp in puntosXIdentificador)
    //        {
    //            var infoHistorica = new InformacionHistorica
    //            {
    //                PuntosCrudos = kvp.Value,
    //                CreadorPartes = creadorPartes,
    //            };

    //            ret.Add(kvp.Key, infoHistorica);
    //        }

    //        return ret;
    //    }

    //    ////////////////////////////////////////////////////////////////
    //    ///

    //    (List<QPAResult<TIdent>>, ReporteQPA<TIdent>) CalculoGenericoQPA_ConReporte<TIdent>(
    //        string identificadorReporte,
    //        DateTime desde,
    //        DateTime hasta,
    //        string lineasPosiblesSeparadasPorComa,
    //        Dictionary<TIdent, InformacionHistorica> infohXIdentificador,
    //        ConstructorFichasDesdeResultados<TIdent> constructorFichas,
    //        Type tipoPuntaLinea,
    //        int granularidadMts = 20,
    //        int radioPuntasDeLineaMts = 200
    //    )
    //    {
    //        if (hasta.Subtract(desde).TotalDays == 1)
    //        {
    //            throw new Exception("demasiados dias para el cálculo");
    //        }

    //        if (hasta <= desde)
    //        {
    //            throw new Exception("hasta <= desde");
    //        }

    //        Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas = null;

    //        if (tipoPuntaLinea == typeof(PuntaLinea))
    //        {
    //            creadorPuntasNombradas = recorridosTeoricos =>
    //                PuntasDeLinea
    //                    .GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts)
    //                    .Select(pulin => (IPuntaLinea)pulin)
    //                    .ToList()
    //                ;
    //        }
    //        else if (tipoPuntaLinea == typeof(PuntaLinea2))
    //        {
    //            creadorPuntasNombradas = recorridosTeoricos =>
    //                PuntasDeLinea2
    //                    .GetPuntasNombradas(recorridosTeoricos, radioPuntasDeLineaMts, radioPuntasDeLineaMts * 2)
    //                    .Select(pulin => (IPuntaLinea)pulin)
    //                    .ToList()
    //                ;
    //        }

    //        var resultadosQPA = CalcularQPA<TIdent>(
    //            identificadorReporte,
    //            desde,
    //            hasta,
    //            lineasPosibles: lineasPosiblesSeparadasPorComa.Split(',').Select(s => int.Parse(s)).ToArray(),
    //            proveedorRecorridosTeoricos: new ProveedorVersionesTecnobus(dirRepos: DameMockRepos()), // los recorridos teóricos
    //            infohXIdentificador,
    //            creadorPuntasNombradas,
    //            granularidadMts
    //        );

    //        var reporte = GenerarReporteQPA<TIdent>(
    //            identificadorReporte,
    //            desde,
    //            hasta,
    //            resultadosQPA,
    //            constructorFichas
    //        );

    //        //// poner reporte en un archivo...
    //        //string nombreReporte = $"Reporte__{identificadorReporte}__desde_{desde:yyyyMMdd_HHmmss}__hasta_{hasta:yyyyMMdd_HHmmss}.txt";
    //        //File.WriteAllText(nombreReporte, reporte.ToString());

    //        return (resultadosQPA, reporte);
    //    }

    //    List<QPAResult<TIdent>> CalcularQPA<TIdent>(
    //        string identificador,
    //        DateTime desde,
    //        DateTime hasta,
    //        int[] lineasPosibles,
    //        IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos,
    //        Dictionary<TIdent, InformacionHistorica> infohXIdentificador,
    //        Func<List<RecorridoLinBan>, List<IPuntaLinea>> creadorPuntasNombradas,
    //        int granularidadMts = 20
    //    )
    //    {
    //        identificador ??= string.Empty;

    //        #region Recorridos teóricos

    //        ///////////////////////////////////////////////////////////////////
    //        // recorridos teóricos / topes / puntas nombradas / recopatterns
    //        ///////////////////////////////////////////////////////////////////
    //        var recorridosTeoricos = proveedorRecorridosTeoricos.Get(new QPAProvRecoParams()
    //        {
    //            LineasPosibles = lineasPosibles,
    //            FechaVigencia = desde
    //        })
    //            .Select(reco => SanitizarRecorrido(reco, granularidadMts))
    //            .ToList()
    //        ;

    //        // puntos aplanados (todos)
    //        var todosLosPuntosDeLosRecorridos = recorridosTeoricos.SelectMany(
    //            (reco) => reco.Puntos,
    //            (reco, puntoReco) => puntoReco
    //        );

    //        // topes
    //        var topes2D = Topes2D.CreateFromPuntos(todosLosPuntosDeLosRecorridos);

    //        // puntas de línea
    //        List<IPuntaLinea> puntasNombradas = creadorPuntasNombradas(recorridosTeoricos);

    //        // caminos de los recos, es un diccionario:
    //        // ----------------------------------------
    //        //  -de clave tiene el patrón de recorrido
    //        //  -de valor tiene una lista de pares (linea, bandera) que son las banderas que encajan con ese patrón
    //        var recoPatterns = new Dictionary<string, List<KeyValuePair<int, int>>>();
    //        foreach (var recox in recorridosTeoricos)
    //        {
    //            // creo un camino (su descripción es la clave)
    //            var camino = Camino<PuntoRecorrido>.CreateFromPuntos(puntasNombradas, recox.Puntos);

    //            // TODO: hay que ver el "porqué"
    //            // elimino los patrones de recorrido que tengan un solo char, ej. "A"
    //            // ya que estos patrones producen duraciones negativas
    //            if (camino.Description.Length == 1)
    //            {
    //                continue;
    //            }

    //            // si la clave no está en el diccionario la agrego...
    //            if (!recoPatterns.ContainsKey(camino.Description))
    //            {
    //                recoPatterns.Add(camino.Description, new List<KeyValuePair<int, int>>());
    //            }

    //            // agrego un par (linea, bandera) a la entrada actual...
    //            recoPatterns[camino.Description].Add(new KeyValuePair<int, int>(recox.Linea, recox.Bandera));
    //        }

    //        #endregion

    //        ///////////////////////////////////////////////////////////////////
    //        // Procesamiento de los datos (para todas las fichas)
    //        ///////////////////////////////////////////////////////////////////
    //        var resultadosQPA = ProcesarTodo<TIdent>(
    //            recorridosTeoricos,
    //            topes2D,
    //            puntasNombradas.Select(pu => (IPuntaLinea)pu).ToList(),
    //            recoPatterns,
    //            infohXIdentificador // puntosXIdentificador
    //        );

    //        return resultadosQPA;
    //    }

    //    RecorridoLinBan SanitizarRecorrido(RecorridoLinBan reco, int granularidad)
    //    {
    //        return new RecorridoLinBan
    //        {
    //            Bandera = reco.Bandera,
    //            Linea = reco.Linea,
    //            Puntos = reco.Puntos.HacerGranular(granularidad),
    //        };
    //    }

    //    List<QPAResult<TIdent>> ProcesarTodo<TIdent>(
    //        List<RecorridoLinBan> recorridosTeoricos,
    //        Topes2D topes2D,
    //        List<IPuntaLinea> puntasNombradas,
    //        Dictionary<string, List<KeyValuePair<int, int>>> recoPatterns,
    //        Dictionary<TIdent, InformacionHistorica> infohXIdentificador
    //    )
    //    {
    //        var qpaProcessor = new QPAProcessor();
    //        var resultados = new List<QPAResult<TIdent>>();

    //        foreach (var ident in infohXIdentificador.Keys)
    //        {
    //            try
    //            {
    //                var partesHistoricas = infohXIdentificador[ident].GetPartesHistoricas().ToList();

    //                foreach (ParteHistorica parteHistorica in partesHistoricas)
    //                {
    //                    var resultado = qpaProcessor.Procesar(
    //                        identificador: ident,
    //                        recorridosTeoricos: recorridosTeoricos,
    //                        puntosHistoricos: parteHistorica.Puntos,
    //                        topes2D: topes2D,
    //                        puntasNombradas: puntasNombradas,
    //                        recoPatterns: recoPatterns
    //                    );

    //                    resultado = VelocidadNormal(resultado);
    //                    resultado = DuracionPositiva(resultado);

    //                    //if (resultado.SubCaminos.Any())
    //                    {
    //                        resultados.Add(resultado);
    //                    }
    //                }
    //            }
    //            catch (Exception exx)
    //            {
    //                int foo = 0;
    //            }
    //        } // para cada identificador...

    //        return resultados;
    //    }

    //    private QPAResult<TIdent> VelocidadNormal<TIdent>(QPAResult<TIdent> res)
    //    {
    //        var newSubCaminos = res.SubCaminos
    //            .Where(subCamino =>
    //                subCamino.VelocidadKmhPromedio <= 120 &&
    //                subCamino.VelocidadKmhPromedio >= 5
    //            )
    //            .ToList()
    //        ;

    //        return new QPAResult<TIdent>
    //        {
    //            Granularidad = res.Granularidad,
    //            RecorridosTeoricos = res.RecorridosTeoricos,
    //            Topes2D = res.Topes2D,
    //            Camino = res.Camino,
    //            Identificador = res.Identificador,
    //            SubCaminos = newSubCaminos,
    //        };
    //    }

    //    private QPAResult<TIdent> DuracionPositiva<TIdent>(QPAResult<TIdent> res)
    //    {
    //        var newSubCaminos = res.SubCaminos
    //            .Where(subCamino => subCamino.HoraLlegada > subCamino.HoraSalida)
    //            .ToList()
    //        ;

    //        return new QPAResult<TIdent>
    //        {
    //            Granularidad = res.Granularidad,
    //            RecorridosTeoricos = res.RecorridosTeoricos,
    //            Topes2D = res.Topes2D,
    //            Camino = res.Camino,
    //            Identificador = res.Identificador,
    //            SubCaminos = newSubCaminos,
    //        };
    //    }
    //}
}
