using System.Drawing;
using Comun;

namespace Pinturas
{
    public class TareaDePinturaPunto : ITareaDePintura
    {
        public Punto    Punto       { get; set; }
        public Color    Color       { get; set; }
        public int      Size        { get; set; }
        public Topes2D  Topes2D     { get; set; }
        public int      Granularidad{ get; set; }

        public void Paint(Graphics g)
        {
            var casillero = Casillero.Create(Topes2D, Punto, Granularidad);

            if (Size == 1)
            {
                g.FillRectangle(
                    new SolidBrush(Color),
                    casillero.IndexHorizontal,
                    casillero.IndexVertical,
                    1,
                    1
                );
            }
            else
            {
                int despX = (Size / 2);
                int despY = (Size / 2);

                Rectangle rect = new Rectangle()
                {
                    X = casillero.IndexHorizontal - despX,
                    Y = casillero.IndexVertical - despY,
                    Width = Size,
                    Height = Size,
                };

                var brush = new SolidBrush(Color);

                if (Size > 4)
                {
                    g.FillEllipse(brush, rect);
                }
                else
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }
    }   
}
