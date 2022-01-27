using Comun;
using System.Collections.Generic;

namespace LibQPA
{
    public abstract class QPAProveedorVentaBoletos<IdType>
    {
        private readonly object _lockInterno = new object();
        private Dictionary<IdType, List<BoletoComun>> _boletosXIdentificador;

        public Dictionary<IdType, List<BoletoComun>> BoletosXIdentificador
        {
            get
            {
                lock (_lockInterno)
                {
                    if (_boletosXIdentificador == null)
                    {
                        _boletosXIdentificador = LeerDB();
                    }
                }

                return _boletosXIdentificador;
            }
            set
            {
                lock (_lockInterno)
                {
                    _boletosXIdentificador = value;
                }
            }
        }

        public abstract Dictionary<IdType, List<BoletoComun>> LeerDB();
    }
}
