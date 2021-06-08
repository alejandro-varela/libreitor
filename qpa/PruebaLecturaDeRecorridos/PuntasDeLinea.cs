using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recorridos;

namespace PruebaLecturaDeRecorridos
{
    public class PuntasDeLinea
    {
        IEnumerable<Punto> Get()
        {
            yield return new Punto();
            yield return new Punto();
            yield return new Punto();
            yield return new Punto();
            yield return new Punto();
            yield return new Punto();
        }
    }
}
