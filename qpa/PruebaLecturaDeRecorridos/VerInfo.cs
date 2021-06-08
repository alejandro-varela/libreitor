using System.IO;

namespace PruebaLecturaDeRecorridos
{
    public class VerInfo
    {
        public static VerInfo FromFile(string path)
        {
            if (! File.Exists(path))
            {
                return null;
            }

            // nombrevvvvvvyyyyMMddhhmmss
            return null;
        }
    }
}