namespace ModeladoControlDeFlota
{
    public class BanderaInfo
    {
        public int Codigo { get; set; }
        public PuntaDeLinea PuntaSaLida  { get; set; }
        public PuntaDeLinea PuntaLlegada { get; set; }

        public static BanderaInfo CreateFromRecorrido()
        {
            return new BanderaInfo();
        }
    }
}
