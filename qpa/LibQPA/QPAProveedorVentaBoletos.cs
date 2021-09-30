using System.Collections.Generic;

namespace LibQPA
{
    public abstract class QPAProveedorVentaBoletos<TBoleto>
    {
        private readonly object _lockInterno = new object();
        private Dictionary<int, List<TBoleto>> _boletosXIdentificador;

        public Dictionary<int, List<TBoleto>> BoletosXIdentificador
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

        public abstract Dictionary<int, List<TBoleto>> LeerDB();
    }
}
