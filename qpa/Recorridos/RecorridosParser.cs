﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Recorridos
{
    public class RecorridosParser
    {
        public static IEnumerable<PuntoRecorrido> ReadFile(string path)
        {
            using var stream = File.OpenRead(path);

            foreach (PuntoRecorrido prx in ReadFile(stream))
            {
                yield return prx;
            }
        }

        public static IEnumerable<PuntoRecorrido> ReadFile(Stream stream)
        {
            byte[] buffer = new byte[16];

            while (16 == stream.Read(buffer, 0, buffer.Length))
            {
                var p = PuntoRecorrido.CreateFromBuffer(buffer);
                yield return p;
            }
        }
    }
}
