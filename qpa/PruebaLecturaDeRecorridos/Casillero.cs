namespace PruebaLecturaDeRecorridos
{
    public class Casillero
    {
        public int IndexHorizontal { get; set; }
        public int IndexVertical   { get; set; }

        public override string ToString()
        {
            return $"c[{IndexHorizontal},{IndexVertical}]";
        }
    }
}