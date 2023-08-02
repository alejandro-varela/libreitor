using System;
using System.Collections.Generic;
using System.Text;

namespace ServicioProductorDatosDriveUp
{
    public class WorkerOptions
    {
        public ReaderConfig ReaderConfig { get; set; }
        public OutputConfig OutputConfig { get; set; }
        public ProxyConfig ProxyConfig { get; set; }
        public bool ProxyEnable { get; set; }
    }

    public class ReaderConfig
    { 
        public string Address { get; set; }
        public string AuthToken { get; set; }
        public int MaxReadBuffer { get; set; } = 16384;
    }

    public class OutputConfig
    { 
        public string BaseDir { get; set; }
        public int MaxSegsBack { get; set; } = 86400;
    }

    public class ProxyCredentials
    {
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ProxyConfig
    {
        public string ProxyAddress { get; set; }
        public bool BypassProxyOnLocal { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public ProxyCredentials ProxyCredentials { get; set; }
    }
}
