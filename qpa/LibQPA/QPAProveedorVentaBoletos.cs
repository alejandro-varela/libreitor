using System.Collections.Generic;

namespace LibQPA
{
    public abstract class QPAProveedorVentaBoletos<TBoleto>
    {
        private readonly object _lockInterno = new object();
        private Dictionary<int, List<TBoleto>> _ventasXIdentificador;

        public Dictionary<int, List<TBoleto>> BoletosXIdentificador
        {
            get
            {
                lock (_lockInterno)
                {
                    if (_ventasXIdentificador == null)
                    {
                        _ventasXIdentificador = LeerDB();
                    }
                }

                return _ventasXIdentificador;
            }
        }

        public abstract Dictionary<int, List<TBoleto>> LeerDB();
    }
}
