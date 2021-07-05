using System;
using System.Collections.Generic;
using System.Linq;

namespace Comun
{
    public static class GrupoidePuntoHistoricoExtensiones
    {
        public static DateTime GetFechaComienzo(this Grupoide<PuntoHistorico> grupoide)
        {
            if (grupoide.Nodos == null ||
                grupoide.Nodos.Count == 0)
            {
                return DateTime.MinValue;
            }

            return grupoide
                .Nodos[0]
                .PuntoAsociado
                .Fecha
            ;
        }

        public static DateTime GetFechaFinal(this Grupoide<PuntoHistorico> grupoide)
        {
            if (grupoide.Nodos == null ||
                grupoide.Nodos.Count == 0)
            {
                return DateTime.MinValue;
            }

            return grupoide
                .Nodos[grupoide.Nodos.Count - 1]
                .PuntoAsociado
                .Fecha
            ;
        }

        public static List<Descanso> GetDescansos(this Grupoide<PuntoHistorico> grupoide)
        {
            // OJO! para que los desplazamientos sean realistas deben ser GRANULARES

            if (grupoide.Nodos == null) return new List<Descanso>(); // no hay puntos...
            if (grupoide.Nodos.Count < 2) return new List<Descanso>(); // no se pueden determinar intervalos con un solo punto

            var desplazamientosCrudos = DameDesplazamientosGrupoide(grupoide);
            var desplazamientosDiscretos = DiscretizarDesplazamientos(desplazamientosCrudos);
            var verDesplazamientosDiscretos = string.Join(string.Empty, DiscretizarDesplazamientos(desplazamientosCrudos).ToArray());

            var agrupados = desplazamientosDiscretos
                .AgruparConContador((c1, c2) => c1 == c2)
                .ToList()
            ;

            var ret = new List<Descanso>();

            // creo los descansos con las agrupaciones calculadas
            foreach (var x in agrupados)
            {
                if (x.Elemento == '_')
                {
                    var f1 = grupoide.Nodos[x.SourceIndex].PuntoAsociado.Fecha;
                    var f2 = grupoide.Nodos[x.SourceIndex + x.Count].PuntoAsociado.Fecha;
                    ret.Add(new Descanso() { InicioDescanso = f1, FinDescanso = f2 });
                }
            }

            return ret;
        }

        public static PuntoCamino<PuntoHistorico> GetPuntoMasCentral(this Grupoide<PuntoHistorico> grupoide)
        {
            var minDist = double.MaxValue;
            PuntoCamino<PuntoHistorico> puntoCamino = null;
            foreach (PuntoCamino<PuntoHistorico> pc in grupoide.Nodos)
            {
                var dist = Haversine.GetDist(pc.PuntoAsociado, grupoide.PuntaLinea.Centroide);

                if (dist < minDist)
                {
                    puntoCamino = pc;
                }
            }
            return puntoCamino;
        }

        public static IEnumerable<char> DiscretizarDesplazamientos(IEnumerable<double> desplazamientos)
        {
            foreach (var despl in desplazamientos)
            {
                if (despl > 1)
                {
                    yield return '|';
                }
                else if (despl > 0.6)
                {
                    yield return ':';
                }
                else
                {
                    yield return '_';
                }
            }
        }

        static IEnumerable<double> DameDesplazamientosGrupoide(Grupoide<PuntoHistorico> grupoide)
        {
            // Un desplazamiento es una relación que hay entre dos puntos,
            // para ser claros podemos expresarlo como la velocidad en metros / segundo de un punto a otro.
            // Esta función devuelve una cantidad de desplazamientos igual a
            // la (cantidad_de_nodos_del_grupoide - 1), porque es una relación entre dos nodos.
            // Ej:
            //      despl1     despl2      <- desplazamientos    (2 despls)
            // nodo1      nodo2      nodo3 <- nodos del grupoide (3 nodos )

            PuntoHistorico anterior = null;

            foreach (var nodoX in grupoide.Nodos)
            {
                if (anterior == null)
                {
                    anterior = nodoX.PuntoAsociado;
                    continue;
                }

                var distMetros = Haversine.GetDist(anterior, nodoX.PuntoAsociado);
                var distSegundos = (nodoX.PuntoAsociado.Fecha - anterior.Fecha).TotalSeconds;

                anterior = nodoX.PuntoAsociado;

                yield return distMetros / distSegundos;
            }
        }
    }

    public class Descanso
    { 
        public DateTime InicioDescanso  { get; set; }
        public DateTime FinDescanso     { get; set; }
        public TimeSpan DuracionDescanso{ get { return FinDescanso - InicioDescanso; } }
    }
}
