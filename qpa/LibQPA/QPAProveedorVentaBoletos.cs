using Comun;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        _boletosXIdentificador = LeerOrigenDeDatos();
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

        public abstract Dictionary<IdType, List<BoletoComun>> LeerOrigenDeDatos();

        /////////////////////////////////////////////////////////////
        // Métodos para usar el proveedor de manera segura         //
        /////////////////////////////////////////////////////////////

        public List<BoletoComun> GetBoletos(IdType identificador)
        {
            if (!BoletosXIdentificador.ContainsKey(identificador))
            {
                return new List<BoletoComun>();
            }

            return BoletosXIdentificador[identificador];
        }

        public List<BoletoComun> GetBoletosEnIntervalo(IdType identificador, DateTime fechaComienzo, DateTime fechaFin)
        {
            return GetBoletos(identificador)
                .Where(boleto =>
                    boleto.FechaCancelacion >= fechaComienzo &&
                    boleto.FechaCancelacion <  fechaFin
                )
                .ToList()
            ;
        }

        public bool TieneBoletosEnIntervalo(IdType identificador, DateTime horaComienzo, DateTime horaFin)
        {
            return GetBoletosEnIntervalo(identificador, horaComienzo, horaFin).Any();
        }
    }
}
