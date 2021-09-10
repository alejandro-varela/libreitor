using System;
using System.Collections.Generic;
using System.Text;

namespace LibQPA
{
    public class QPAConfiguration
    {
        public int GranularidadMts { get; set; } = 20;
        public int RadioPuntasMts  { get; set; } = 800;
        //public DateTime FechaDesde { get; set; } = DateTime.MinValue;
        //public DateTime FechaHasta { get; set; } = DateTime.MinValue;
        public IQPAProveedorRecorridosTeoricos ProveedorRecorridosTeoricos  { get; set; }
        public IQPAProveedorPuntosHistoricos   ProveedorPuntosHistoricos    { get; set; }

        public QPAConfiguration SetGranularidadMts(int granularidadMts)
        {
            GranularidadMts = granularidadMts;
            return this;
        }

        public QPAConfiguration SetRadioPuntasMts(int radioPuntasMts)
        {
            RadioPuntasMts = radioPuntasMts;
            return this;
        }

        //public QPAConfiguration SetIntervalorDesdeHasta(DateTime fechaDesde, DateTime fechaHasta)
        //{
        //    FechaDesde = fechaDesde;
        //    FechaHasta = fechaHasta;
        //    return this;
        //}

        public QPAConfiguration SetProveedorPuntosHistoricos(IQPAProveedorPuntosHistoricos proveedorPuntosHistoricos)
        {
            ProveedorPuntosHistoricos = proveedorPuntosHistoricos;
            return this;
        }

        public QPAConfiguration SetProveedorRecorridosTeoricos(IQPAProveedorRecorridosTeoricos proveedorRecorridosTeoricos)
        {
            ProveedorRecorridosTeoricos = proveedorRecorridosTeoricos;
            return this;
        }

        public QPAProcessor BuildProcessor()
        {
            return new QPAProcessor(this);
        }
    }
}
