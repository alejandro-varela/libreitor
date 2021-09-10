using System;
using System.Collections.Generic;
using Comun;

namespace LibQPA
{
    public interface IQPAProveedorRecorridosTeoricos
    {
        List<RecorridoLinBan> Get(int[] lineas, DateTime vigenteEn);
    }
}
