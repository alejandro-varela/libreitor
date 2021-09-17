namespace LibQPA
{
    public class LineaBandera
    {
        public int Linea      { get; set; }
        public int Bandera    { get; set; }
    }

    public class LineaBanderaPuntuacion : LineaBandera
    {
        public int Puntuacion { get; set; }

        public override string ToString()
        {
            return $"Lin={Linea} Ban={Bandera} {Puntuacion} pts";
        }
    }
}
