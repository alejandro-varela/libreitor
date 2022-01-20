using System;
using System.Linq;

namespace PruebaConstruccionSegmentos
{
    public class Link<T>
    {
        public string NameRay
        {
            get 
            {
                return $"{From.Value}->{To.Value}";
            }
        }

        public string NameSegment
        {
            get
            {
                if (string.Compare(From.Value.ToString(), To.Value.ToString()) == -1)
                {
                    return $"{From.Value}--{To.Value}";
                }
                else
                {
                    return $"{To.Value}--{From.Value}";
                }
            }
        }

        public IndexInfo<T> From { get; set; }
        public IndexInfo<T> To   { get; set; }
    }
}
