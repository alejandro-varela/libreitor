using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibQPA
{
    public enum QPATipoSubCamino
    {
        MATCH = 0,
        ERR   = 1,
        //DESCANSO = 2,
        //CUSTOM = 3,
    }

    public class QPASubCamino
    {
        public Camino<PuntoHistorico>   CaminoPadre                 { get; set; }
        public QPASubCamino             SubCaminoAnterior           { get; set; }
        public QPATipoSubCamino         Tipo                        { get; set; } = QPATipoSubCamino.MATCH;
        public string                   Patron                      { get; set; }
        public DateTime                 HoraSalida                  { get; set; }
        public DateTime                 HoraLlegada                 { get; set; }
        public List<PuntoHistorico>     PuntosEntreSalidaYLlegada   { get; set; }
        public List<LineaBanderaPuntuacion> LineasBanderasPuntuaciones { get; set; }
        public int                      PatronIndexInicial          { get; set; }
        public int                      PatronIndexFinal            { get; set; }
        

        public List<PuntoHistorico> PuntosHistoricos
        {
            get
            {
                var ret = new List<PuntoHistorico>();
                for (int i = PatronIndexInicial; i <= PatronIndexFinal; i++)
                {
                    var grupoide = CaminoPadre.Grupoides[i];
                    var puntos = grupoide.Nodos.Select(pc => pc.PuntoAsociado);
                    ret.AddRange(puntos);   
                }
                return ret;
            }
        }

        public double DuracionHoras
        {
            get 
            {
                return Duracion.TotalHours;
            }
        }

        public TimeSpan Duracion
        {
            get
            {
                var duracion = HoraLlegada - HoraSalida;
                return duracion;
            }
        }

        public double KmsRecorridos
        {
            get
            {
                var metrosRecorridos = 0.0;
                if (PuntosEntreSalidaYLlegada.Any())
                {
                    var puntoAnterior = PuntosEntreSalidaYLlegada.First();
                    foreach (var puntoX in PuntosEntreSalidaYLlegada.Skip(1))
                    {
                        metrosRecorridos += Haversine.GetDist(puntoAnterior, puntoX);
                        puntoAnterior = puntoX;
                    }
                }
                return metrosRecorridos / 1000.0;
            }
        }

        public double VelocidadKmhPromedio
        {
            get
            {
                var kilometrosXHora = KmsRecorridos / DuracionHoras;
                return kilometrosXHora;
            }
        }

        public override string ToString()
        {
            return Patron ?? "";
        }
    }
}
