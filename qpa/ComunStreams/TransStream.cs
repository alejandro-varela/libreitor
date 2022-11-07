using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ComunStreams
{
    public class TransStream : Stream
    {
        StreamReader        _streamReader;
        Func<string, string>_transformer;
        List<byte>          _sobra = new List<byte>(128 * 1024);

        public string NewLine { get; set; } = "\n";

        static string Identidad(string s)
        {
            return s;
        }

        public TransStream(StreamReader streamReader, Func<string, string> transformer)
        {
            _streamReader = streamReader;
            _transformer = transformer ?? Identidad;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_sobra.Count == 0)
            {
                while (_sobra.Count < count)
                {
                    var sLine = _streamReader.ReadLine();

                    if (sLine == null)
                    {
                        break;
                    }
                    else
                    {
                        var sTransLine = _transformer(sLine);
                        if (!string.IsNullOrEmpty(sTransLine))
                        {
                            _sobra.AddRange(Encoding.UTF8.GetBytes(_transformer(sLine)));
                            _sobra.AddRange(Encoding.UTF8.GetBytes(NewLine));
                        }
                    }
                }
            }

            var cant = Math.Min(count, _sobra.Count);
            _sobra.CopyTo(0, buffer, offset, cant);
            _sobra.RemoveRange(0, cant);
            return cant;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => 0;

        public override long Position { get => 0; set => _ = value; }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin) { return 0L; }

        public override void SetLength(long value) { }

        public override void Write(byte[] buffer, int offset, int count) { }
    }
}
