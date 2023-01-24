using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHR
{
    public class Pintor
    {
        public string Nombre { get; private set; }
        public Action<Graphics> Accion { get; private set; }
        public bool Habilitado { get; set; }

        public Pintor(string nombre, Action<Graphics> accion)
        {
            Nombre = nombre;
            Accion = accion;
        }
    }
}
