using System;

namespace ApiCochesDriveUp
{
    public class CocheDriveUp
    {
        public DateTime FechaRecep { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Plate { get; set; }
        public int Ficha
        {
            get
            {
                return int.Parse( Plate.Substring(1, 4) );
            }
            set
            { 
                // :P
            }
        }
    }
}
