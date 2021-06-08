using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecorridosResumidos
{
    public class Camino
    {
        public readonly ReadOnlyCollection<PuntoCamino> Puntos;

        public Camino(IList<PuntoCamino> puntosCamino)
        {
            Puntos = new ReadOnlyCollection<PuntoCamino>(puntosCamino);
        }


    }
}
