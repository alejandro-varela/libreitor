using Recorridos;
using System.Collections.Generic;
using System.Drawing;

namespace PruebaLecturaDeRecorridos
{
    public partial class PintorDeRecorrido
    {
        // Límites
        public Topes2D Topes2D { get; private set; }
        public int Granularidad { get; private set; }

        // Tamaño
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Color de fondo
        public Color ColorFondo { get; private set; } = Color.White;
        public List<ITareaDePintura> Tareas { get; private set; }

        public PintorDeRecorrido(Topes2D topes2D, int granularidad)
        {
            Topes2D = topes2D;
            Granularidad = granularidad;
            Width = topes2D.GetAnchoGranular(granularidad);
            Height = topes2D.GetAlturaGranular(granularidad);
            Tareas = new();
        }

        public PintorDeRecorrido SetColorFondo(Color color)
        {
            ColorFondo = color;
            return this;
        }

        public PintorDeRecorrido PintarPunto(Punto punto, Color color, int size = 1)
        {
            Tareas.Add(
                new TareaDePinturaPunto {
                    Punto = punto,
                    Color = color,
                    Size = size,
                    Granularidad = Granularidad,
                    Topes2D = Topes2D
                }
            );
            return this;
        }

        public PintorDeRecorrido PintarPuntos(IEnumerable<Punto> puntos, Color color, int size = 1)
        {
            foreach (var p in puntos)
            {
                PintarPunto(p, color, size);
            }

            return this;
        }

        public PintorDeRecorrido PintarRadio(Punto centro, Color color, int size = 10)
        {
            Tareas.Add(
                new TareaDePinturaRadio {
                    Centro = centro,
                    Color = color,
                    Size = size,
                    Granularidad = Granularidad,
                    Topes2D = Topes2D
                }
            );

            return this;
        }

        public PintorDeRecorrido PintarRadios(IEnumerable<Punto> centros, Color color, int size = 10)
        {
            foreach (var c in centros)
            {
                PintarRadio(c, color, size);
            }

            return this;
        }

        public Bitmap Render()
        {
            // tamaño
            var bitmap = new Bitmap(Width, Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // fondo
                g.FillRectangle(new SolidBrush(ColorFondo), new Rectangle(0, 0, Width, Height));

                // tareas de puntos...
                foreach (ITareaDePintura itarea in Tareas)
                {
                    itarea.Paint(g);
                }
            }

            return bitmap;
        }
    }
}
