using System;
using System.Collections.Generic;
using System.Text;

namespace ServicioCopiador_TecnobusSmGps_VmCoches
{
    public class WorkerOptions
    {
        public List<CopyDir> CopyDirs { get; set; }
    }

    public class CopyDir
    {
        public string Src { get; set; }
        public string Dest { get; set; }
    }
}
