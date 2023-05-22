using System;
using System.Collections.Generic;

namespace LibQPA
{
    public class QPAProvRecoParams
    {
        public int[] LineasPosibles { get; set; }
        public DateTime FechaVigencia { get; set; }
        public bool ConPuntos { get; set; } = true;
    }
}
