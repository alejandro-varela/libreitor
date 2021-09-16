using System;
using System.Collections.Generic;

namespace LibQPA
{
    public enum QPATipoSubCamino
    {
        MATCH = 0,
        ERR = 1,
        DESCANSO = 2,
        CUSTOM = 3,
    }

    public class QPASubCamino
    {
        public QPATipoSubCamino Tipo { get; set; } = QPATipoSubCamino.MATCH;
        
        public string Patron { get; set; }

        public DateTime HoraComienzo { get; set; }
        public DateTime HoraFin { get; set; }
        public TimeSpan Duracion { get; set; }

        public List<LineaBanderaPuntuacion> LineasBanderasPuntuaciones { get; set; }
        public int PatronIndexInicial { get; set; }
        public int PatronIndexFinal { get; set; }

        public override string ToString()
        {
            return Patron ?? "";
        }

    }
}
