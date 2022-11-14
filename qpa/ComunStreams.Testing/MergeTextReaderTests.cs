using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace ComunStreams.Testing
{
    public class MergeTextReaderTests
    {
        [Fact]
        public void Prueba1()
        {
            var textReader1 = File.OpenText("./Data/a.txt");
            var textReader2 = File.OpenText("./Data/b.txt");

            List<TextReader> textReaders = new List<TextReader> { 
                textReader1,
                textReader2 
            };

            Func<string, string> fnSelectorOrden = (sRenglon) =>
            {
                return sRenglon;
            };

            var mergeTextReader = new MergeTextReader<string>(
                textReaders,
                fnSelectorOrden
            );

            for (; ; )
            {
                var sRenglon = mergeTextReader.ReadLine();

                if (sRenglon == null)
                {
                    break;
                }
            }
        }
    }
}
