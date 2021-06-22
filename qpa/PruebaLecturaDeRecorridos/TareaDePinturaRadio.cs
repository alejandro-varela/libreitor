using Recorridos;
using System.Drawing;

namespace PruebaLecturaDeRecorridos
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

            int despX = (Size / 2);
            int despY = (Size / 2);

            g.DrawEllipse(
                pen: new Pen(Color), 
                x: casillero.IndexHorizontal - despX, 
                y: casillero.IndexVertical   - despY, 
                width: Size, 
                height: Size
            ); 
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
                X = casillero.IndexHorizontal - despX*2,
                Y = casillero.IndexVertical - despY*2,
            };

            g.DrawString(Nombre, font, brush, punto);

            base.Paint(g);
        }
    }
}