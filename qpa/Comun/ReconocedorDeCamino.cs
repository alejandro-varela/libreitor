using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun
{
    // 1) agregar al resultado los puntos del camino
    //    en el procesado posterior se podran vincular intervalos de tiempos con los puntos
    // 2) reconocer los pozos
    //    en el procesado posterior se podran analizar estos pozos geométricamente
    // 

    public class ReconocedorDeCamino
    {
        public static Reconocimento Reconocer(Camino<PuntoHistorico> camino, List<string> patrones)
        {
            // patron de la historia
            var arrNombres = camino.Grupoides.Select(g => g.Nombre).ToArray();
            var patronHistorico = string.Join(string.Empty, arrNombres);

            // patrones para reconocer, los ordeno por tamaño y nombre
            var patronesOrdenados = patrones
                .OrderByDescending(p => p.Length)   // ordeno por tamaño
                .ThenBy(p => p)                     // entonces lexicográficamente
                .ToList()                           // convierto todo en una lista
            ;

            List<ReconocimientoUnidad> ret = new List<ReconocimientoUnidad>();
            int ptr = 0;
            int ptrNombres = 0;

            for (; ; )
            {
                var puntaInicial = patronHistorico[ptr].ToString();

                if (puntaInicial == "?" || puntaInicial == ".")
                {
                    ptr++;
                    if (ptr >= patronHistorico.Length - 1)
                    {
                        break;
                    }
                    continue;
                }

                var patronesPosibles = patronesOrdenados
                    .Where(p => p.StartsWith(puntaInicial))
                    .Distinct()
                    .ToList()
                ;

                if (patronesPosibles.Count == 0)
                {
                    ret.Add(new ReconocimientoUnidadError { IndexNombres = ptr, ErrDescription = $"No hay patrones para '{puntaInicial}'" });
                    ptr++;
                    ptrNombres++;
                    if (ptr >= patronHistorico.Length - 1)
                    {
                        break;
                    }
                    continue;
                }

                string patronElegido = null;
                int indexFinal = 0;
                foreach (var patronPosibleX in patronesPosibles)
                {
                    if (TienePatronPosibleEn(patronHistorico, patronPosibleX, ptr, ref indexFinal))
                    {
                        ret.Add(new ReconocimientoUnidadMatch {
                            IndexNombres        = ptrNombres,
                            Pattern             = patronPosibleX,
                            IndexInicialGrupoide= ptr,
                            IndexFinalGrupoide  = indexFinal
                        });

                        patronElegido = patronPosibleX;
                        break;
                    }
                }

                if (patronElegido == null)
                {
                    ptr++;
                    ptrNombres++;
                    if (ptr >= patronHistorico.Length - 1)
                    {
                        break;
                    }
                    continue; // por ahora estaria al pedo, pero si se agrega algo abajo estamos cubiertos...
                }
                else
                {
                    // ARREGLO1: antes no se tenían en cuenta los patrones de 1 solo caracter...
                    // ARREGLO2: el arreglo 1 estaba mal porque si el patron tenia un solo caracter no adelantaba el contador de nombres

                    if (patronElegido.Length == 1)
                    {
                        ptr++;
                        ptrNombres++;
                    }
                    else
                    {
                        ptr += (indexFinal - ptr);
                        ptrNombres += patronElegido.Length - 1;
                    }

                    if (ptr >= patronHistorico.Length - 1)
                    {
                        break;
                    }
                }
            }

            return new Reconocimento
            {
                Unidades = ret,
            };
        }

        static bool TienePatronPosibleEn(string patronHistorico, string patronPosibleX, int ptr, ref int incrementoPtr)
        {
            StringBuilder sbAcum = new StringBuilder();
            int i = ptr;

            for (; i < patronHistorico.Length; i++)
            {
                if (patronHistorico[i] == '?' || patronHistorico[i] == '.')
                {
                    continue;
                }

                sbAcum.Append(patronHistorico[i]);
                if (sbAcum.ToString() == patronPosibleX)
                {
                    incrementoPtr = i;
                    return true;
                }
            }

            return false;
        }
    }

    public class Reconocimento
    {
        public List<ReconocimientoUnidad> Unidades { get; set; } = new List<ReconocimientoUnidad>();
    }

    public abstract class ReconocimientoUnidad
    {
        public int IndexNombres { get; set; }
    }

    public class ReconocimientoUnidadMatch : ReconocimientoUnidad
    {
        public string Pattern { get; set; }
        public int IndexInicialGrupoide { get; set; }
        public int IndexFinalGrupoide { get; set; }
    }

    public class ReconocimientoUnidadError : ReconocimientoUnidad
    {
        public string ErrDescription { get; set; }
    }
}
