using Comun;
using System.Drawing;

namespace Pinturas
{
    public class TareaDePinturaRadio : ITareaDePintura
    {
        public int Size { get; set; }
        public Color Color { get; set; }
        public Punto Centro { get; set; }
        public int Granularidad { get; set; }
        public Topes2D Topes2D { get; set; }

        public virtual void Paint(Graphics g)
        {
            var casillero = Casillero.Create(Topes2D, Centro, Granularidad);

            // para centrar objetos
            int despX = (Size / 2);
            int despY = (Size / 2);

            int x = casillero.IndexHorizontal;
            int y = casillero.IndexVertical;

            g.DrawEllipse(
                pen: new Pen(Color), 
                x: x - despX,
                y: y - despY, 
                width: Size, 
                height: Size
            );

            var rect = new Rectangle(x, y, 1, 1);
            g.FillRectangle(new SolidBrush(Color), rect);
        }
    }

    public class TareaDePinturaRadioNombrado : TareaDePinturaRadio
    {
        public string Nombre { get; set; }

        public override void Paint(Graphics g)
        {
            var casillero = Casillero.Create(Topes2D, Centro, Granularidad);

            int despX = (Size / 2);
            int despY = (Size / 2);

            var font  = new Font("Tahoma", 14);
            var brush = new SolidBrush(Color);
            var punto = new Point
            {
                X = casillero.IndexHorizontal,
                Y = casillero.IndexVertical - despY,
            };

            g.DrawString(Nombre, font, brush, punto);

            base.Paint(g);
        }
    }
}