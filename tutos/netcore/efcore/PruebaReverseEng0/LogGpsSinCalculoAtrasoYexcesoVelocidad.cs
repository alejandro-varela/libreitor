using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaReverseEng0
{
    public partial class LogGpsSinCalculoAtrasoYexcesoVelocidad
    {
        public long Uid { get; set; }
        public int Id { get; set; }
        public int IdGrupo { get; set; }
        public int IdRegistro { get; set; }
        public int NroFicha { get; set; }
        public string NroInterno { get; set; }
        public int? NroEquipo { get; set; }
        public int CodLinea { get; set; }
        public DateTime FechaHora { get; set; }
        public int? Servicio { get; set; }
        public int? CodTipoDia { get; set; }
        public int? CodBandera { get; set; }
        public string NroLegajo { get; set; }
        public float Latitud { get; set; }
        public float Longitud { get; set; }
        public decimal Velocidad { get; set; }
        public int? MetrosAcumulados { get; set; }
        public int? EstadoCoche { get; set; }
        public DateTime? FechaInsert { get; set; }
    }
}
