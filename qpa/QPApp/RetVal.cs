using System;

namespace QPApp
{
    public partial class Program
    {
        public class RetVal<T>
        {
            public T Value { get; set; } = default!;
            public bool IsOk { get; set; } = true;
            public string ErrorMessage { get; set; }
            public Exception ErrorException { get; set; }
        }
    }
}
