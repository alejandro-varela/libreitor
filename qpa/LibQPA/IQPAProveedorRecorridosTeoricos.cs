using System;
using System.Collections.Generic;
using Comun;

namespace LibQPA
{
    public interface IQPAProveedorRecorridosTeoricos
    {
        IEnumerable<RecorridoLinBan> Get(int[] lineas, DateTime vigenteEn);
    }
}
