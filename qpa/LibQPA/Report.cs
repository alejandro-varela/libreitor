using Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibQPA
{
    //empresaSUBE; internoSUBE; ficha; linea; bandera; inicio; fin; cantbol; cantbolopt
    //132; 3696; 3696; 167; 1038; 02/03/2022 13:25:48; 02/03/2022 13:47:50; 7; 339
    //132; 3696; 3696; 167; 3082; 02/03/2022 13:52:50; 02/03/2022 14:05:20; 0; 0
    //132; 3696; 3696; 167; 3097; 02/03/2022 17:17:54; 02/03/2022 17:35:56; 0; 0
    //132; 3696; 3696; 167; 2793; 02/03/2022 17:37:58; 02/03/2022 18:36:26; 55; 49

    public class ReporteQPASubItem<TIdent>
    {
        public int      EmpresaSUBE     { get; set; }
        public int      InternoSUBE     { get; set; }
        public int      Ficha           { get; set; }
        public int      Linea           { get; set; }
        public int      Bandera         { get; set; }
        public DateTime Inicio          { get; set; }
        public DateTime Fin             { get; set; }
        public int      CantBoletosNaive{ get; set; }
        public int      CantBoletosOpt  { get; set; }
    }

    public class ReporteQPAItem<TIdent>
    {
        public QPAResult<TIdent>                Resultado   { get; set; }
        public List<ReporteQPASubItem<TIdent>>  Items       { get; set; }
    }

    public class ReporteQPA<TIdent>
    {
        public List<ReporteQPAItem<TIdent>> Items { get; set; }

        public List<ReporteQPASubItem<TIdent>> ListaAplanadaSubItems
        { 
            get
            {
                return Items
                    .SelectMany(item => item.Items)
                    .ToList()
                ;
            }
        }
    }

    public abstract class Report<ItemType>
    {
        protected List<ItemType> Items  { get; set; }

        //public override string ToString()
        //{
        //    var sbRet = new StringBuilder();

        //    if (Header != null)
        //    {
        //        sbRet.AppendLine(Header.ToString());
        //    }

        //    foreach (var item in Items)
        //    {
        //        sbRet.AppendLine(item.ToString());
        //    }

        //    return sbRet.ToString();
        //}
    }

    // Lo voy a borrar en el futuro
    public class CSVReport<ItemType> : Report<ItemType>
    {
        public delegate string                HeaderBuilderFunc (char separator);
        public delegate IEnumerable<ItemType> ItemsBuilderFunc  (char separator);

        public string               Header          { get; set; }
        public bool                 UsesHeader      { get; set; }
        public char                 Separator       { get; set; }
        public HeaderBuilderFunc    HeaderBuilder   { get; set; }
        public ItemsBuilderFunc     ItemsBuilder    { get; set; }

        public override string ToString()
        {
            if (UsesHeader)
            {
                Header = HeaderBuilder(Separator);
            }

            Items = ItemsBuilder(Separator)
                .ToList()
            ;

            return base.ToString();
        }
    }

}
