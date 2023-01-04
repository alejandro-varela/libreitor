using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPACreator
{
    public class Reporte
    {
        public IEnumerable<CSVItem> Create(IEnumerable<object> items, CSVFormatter formatter)
        { 
            if (formatter.EnableTitle)
            {
                yield return CreateTitleItem(formatter);
            }

            foreach (var itemX in items)
            {
                yield return CreateNormalItem(formatter, itemX);
            }
        }

        public CSVItem CreateTitleItem(CSVFormatter formatter)
        {
            var arrTitles = formatter.OrderedCells
                .Select(format => format.ResultName ?? string.Empty)
                .ToArray()
            ;
            return new CSVItem { FormattedCells = arrTitles.ToList() };
        }

        public CSVItem CreateNormalItem(CSVFormatter formatter, object item)
        {
            var itemType = item.GetType();
            var values = new List<string>();
            foreach (var format in formatter.OrderedCells)
            {
                var prop  = itemType.GetProperty(format.PropertyName);
                if (prop == null)
                {
                    values.Add(string.Empty);
                }
                else
                {
                    var value = prop.GetValue(item);
                    var mySValue = format.FormatFunction(value);
                    values.Add(mySValue);
                }
            }
            return new CSVItem  { FormattedCells = values };
        }

        public string RenderAllText(IEnumerable<CSVItem> items, CSVFormatter formatter)
        {
            StringBuilder sbReturn = new StringBuilder();
            foreach (var stringX in RenderAllLines(items, formatter))
            {
                sbReturn.AppendLine(stringX);
            }
            return sbReturn.ToString();
        }

        public IEnumerable<string> RenderAllLines(IEnumerable<CSVItem> items, CSVFormatter formatter)
        {
            foreach (var stringX in Render(items, formatter))
            {
                yield return $"{stringX}{formatter.NewLine}";
            }
        }

        public IEnumerable<string> Render(IEnumerable<CSVItem> items, CSVFormatter formatter)
        {
            foreach (var item in items)
            {
                yield return Render(item, formatter);
            }
        }

        public string Render(CSVItem item, CSVFormatter formatter)
        {
            return string.Join(formatter.Separator.ToString(), item.FormattedCells);
        }
    }

    public class CSVItem
    {
        public List<string> FormattedCells { get; set; }
    }

    public class CSVFormatter
    {
        public bool EnableTitle { get; set; }

        public List<CSVCellBuilder> CellBuilders { get; set; }

        public string NewLine { get; set; } = Environment.NewLine;

        public List<CSVCellBuilder> OrderedCells 
        {
            get
            {
                return CellBuilders.OrderBy(f => f.ExplicitOrder).ToList();
            }
        }

        public char Separator { get; set; }
    }

    public class CSVCellBuilder
    {
        // Una función que no formatea
        public static string EmptyFormatFunction(object o) => o.ToString();
        // Esto es por si a alguien se le ocurre explicitar el orden numericamente en vez de "por llegada"
        public int ExplicitOrder { get; set; }
        // El nombre de la propiedad que se desea leer
        public string PropertyName { get; set; }
        // El nombre o título que va a tener esa propiedad en el CSV (si es que el CSV admite títulos)
        public string ResultName { get; set; }
        public string Format { get; set; }
        public Func<object, string> FormatFunction { get; set; } = EmptyFormatFunction;
    }
}
