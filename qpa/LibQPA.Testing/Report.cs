using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibQPA.Testing
{
    public abstract class ReportHeader
    {
        //
    }

    public abstract class ReportItem
    {
        //
    }

    public abstract class Report
    {
        protected string       Header { get; set; }
        protected List<string> Items  { get; set; }

        public override string ToString()
        {
            var sbRet = new StringBuilder();

            if (Header != null)
            {
                sbRet.AppendLine(Header.ToString());
            }

            foreach (var item in Items)
            {
                sbRet.AppendLine(item.ToString());
            }

            return sbRet.ToString();
        }
    }

    public class CSVReport : Report
    {
        public delegate string              HeaderBuilderFunc (char separator);
        public delegate IEnumerable<string> ItemsBuilderFunc  (char separator);

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

            Items = 
                ItemsBuilder(Separator)
                .ToList()
            ;

            return base.ToString();
        }       
    }

    public class CSVReportHeader : ReportHeader
    {
        public List<string> Titulos     { get; set; }
        public char         Separator   { get; set; }

        public override string ToString()
        {
            return string.Join(
                Separator,
                Titulos.ToArray()
            );
        }
    }

    public class CSVReportItem : ReportItem
    {
        public List<string> Values      { get; set; }
        public char         Separator   { get; set; }

        public override string ToString()
        {
            return string.Join(
                Separator,
                Values.ToArray()
            );
        }
    }

    //public class TxtReportHeader : ReportItem
    //{
        
    //}

    //public class TxtReportItem : ReportItem
    //{
    //    public string Value { get; set; }

    //    public override string ToString()
    //    {
    //        return Value;
    //    }
    //}
}
