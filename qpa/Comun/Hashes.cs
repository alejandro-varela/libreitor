using System.Linq;
using System.Security.Cryptography;

namespace Comun
{
    public class Hashes
    {
        public static string GetSHA1String(byte[] buff)
        {
            var algo = SHA1.Create();

            var arrs = algo
                .ComputeHash(buff)
                .Select(b => b.ToString("X2"))
                .ToArray()
            ;

            var str = string.Join(string.Empty, arrs);

            return str;
        }
    }
}
