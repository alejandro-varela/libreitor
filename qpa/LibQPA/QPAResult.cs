using Comun;
using System;
using System.Collections.Generic;

namespace LibQPA
{
    public class QPAResult
    {
        public Camino<PuntoHistorico> Camino { get; set; }
        public List<QPASubCamino> SubCaminos { get; set; }

        public override string ToString()
        {
            var descrCamino = Camino?.Description ?? string.Empty;
            return $"{Convert.ToInt32(PorcentajeReconocido)}% {descrCamino}";
        }

        public double PorcentajeReconocido
        {
            get
            {
                if (Camino == null || string.IsNullOrEmpty(Camino.Description))
                {
                    return 0;
                }

                double lenTotal = Camino.Description.Length;
                double lenRecon = LargoReconocido;
                return (lenRecon * 100.0) / lenTotal;
            }
        }

        public int LargoReconocido 
        {
            /*
             
            129839012830
                       0123
            ---------------  suma-1
            1298390128300123 suma

            123
              32819
                  9123
            ----------   suma-2
            123328199123 suma

            1234
               4567
                  78
                   89
            ---------!!! suma-3
            123445677889 suma...

            si hay 1 sub recon el largo es la suma -0?
            si hay 2 sub recon el largo es la suma -1?
            si hay 3 sub recon el largo es la suma -2?
            si hay 4 sub recon el largo es la suma -3?

            el largo de lo reconocido es la suma de todas los largos de los sub reconocidos menos (la cantidad de los sub -1)
            si los subs son 0 entonces el largo es 0
             
            */

            get
            {
                if (SubCaminos.Count == 0)
                {
                    return 0;
                }

                var acum = 0;
                foreach (var subCamino in SubCaminos)
                {
                    acum += subCamino.Patron.Length;
                }

                return acum - (SubCaminos.Count - 1);
            }
        }
    }
}
