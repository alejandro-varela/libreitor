using System;
using System.Collections.Generic;
using System.IO;

namespace Comun
{
    public class RecorridosParser
    {
        public static IEnumerable<PuntoRecorrido> ReadFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                foreach (PuntoRecorrido prx in ReadFile(stream))
                {
                    yield return prx;
                }
            }
        }

        public static List<PuntoRecorrido> ReadFile(Stream stream)
        {
            var ret = new List<PuntoRecorrido>();
            
            //byte[] buffer = new byte[16];
            //while (16 == stream.Read(buffer, 0, buffer.Length))
            //{
            //    var p = PuntoRecorrido.CreateFromBuffer(buffer);
            //    ret.Add(p);
            //}

            for (; ; )
            {
                var buffer = ReadXBytes(16, stream);
                if (buffer == null)
                {
                    break;
                }
                var p = PuntoRecorrido.CreateFromBuffer(buffer);
                ret.Add(p);
            }

            return ret;
        }

        public static byte[] ReadXBytes(int n, Stream stream)
        {
            byte[] buffer = new byte[n];

            for (int i = 0; i < n; i++)
            {
                int leido = stream.ReadByte();

                if (leido == -1)
                {
                    return null;
                }

                buffer[i] = (byte) leido;
            }

            return buffer;
        }
    }
}
