using System;
using System.Collections.Generic;
using System.Text;

namespace Comun
{
    public class DateTimeHelper
    {
        public static IEnumerable<ParDesdeHasta> GetFromToDayPairs(
            DateTime from, 
            DateTime to
        )
        {
            var startDay = from.Date;
            var finalDay = to == to.Date ? to.Date : to.AddDays(1).Date;

            for (int i = 0; ; i++)
            {
                var fromX = startDay.AddDays(i);
                if (fromX == finalDay)
                {
                    break;
                }
                var toX = fromX.AddDays(1);
                yield return new ParDesdeHasta(fromX, toX);
            }
        }

        public class ParDesdeHasta : Tuple<DateTime, DateTime>
        {
            public DateTime Desde { get { return Item1; } }

            public DateTime Hasta { get { return Item2; } }

            public ParDesdeHasta(DateTime desde, DateTime hasta) : base(desde, hasta)
            {
                //
            }
        }
    }
}
