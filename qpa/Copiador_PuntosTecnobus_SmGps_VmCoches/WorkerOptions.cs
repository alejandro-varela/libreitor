using System.Collections.Generic;

namespace Copiador_PuntosTecnobus_SmGps_VmCoches
{
    public class WorkerOptions
    {
        public List<CopyDir> CopyDirs { get; set; }
    }

    public class CopyDir
    {
        public string Src { get; set; }
        public string Dest { get; set; }

        public override string ToString()
        {
            return $"Src={Src} Dest={Dest}";
        }
    }
}
