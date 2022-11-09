using System.Collections.Generic;
using System.IO;

namespace ComunStreams
{
    public class BolsaStream : Stream
    {
        public enum BOMSkippingStrategy
        { 
            SkipAlways,
            SkipMiddle,
            DontSkip,
        }

        List<string> _archivos;
        int _archivoIndex = 0;
        int _archivoPtr = 0;

        const int MAIN_BUFF_SIZE = 1_048_576; // 1 mega
        byte[] _mainBuff = new byte[MAIN_BUFF_SIZE];
        int _mainBuffLen = 0;
        int _mainBuffPtr = 0;

        public BOMSkippingStrategy SkipBOM { get; set; } = BOMSkippingStrategy.SkipAlways;

        // main buffer
        // [0123456789] vacio sin iniciar, len = 0, ptr = 0
        //  ^  
        //
        // [0123456789] tiene = len > ptr ; disponible = len - ptr
        //  ^          
        //
        // [0123456789] no tiene = len <= ptr
        //            ^
        //
        // cada vez que se rellena:
        //  poner ptr en 0
        //  poner len en lo que se haya leído
        //

        public BolsaStream(List<string> paths)
        {
            _archivos = paths;
        }

        bool HayAlgoEnElMainBuffer()
        {
            return _mainBuffLen > _mainBuffPtr;
            // o podría ser...
            // BytesDisponiblesEnElMainBuffer() > 0
        }

        int BytesDisponiblesEnElMainBuffer()
        {
            int resultado = _mainBuffLen - _mainBuffPtr;
            return resultado;
        }

        bool RellenarElMainBuffer()
        {
            // pongo el apuntador en el primer byte del _mainBuff
            // siempre...
            _mainBuffPtr = 0;

            // forever
            for (; ; )
            {
                // si el index de archivos es igual a la cantidad de
                // archivos, ya no tengo archivos... retorno false
                if (_archivoIndex == _archivos.Count)
                {
                    return false;
                }

                if (System.IO.File.Exists(_archivos[_archivoIndex]))
                {
                    using (FileStream fs = File.OpenRead(_archivos[_archivoIndex]))
                    {
                        // adelanto el puntero del stream hasta lo que ya leí...
                        fs.Seek(_archivoPtr, SeekOrigin.Begin);

                        // trato de leer algo...
                        _mainBuffLen = fs.Read(_mainBuff, 0, MAIN_BUFF_SIZE);
                    }
                }

                if (_mainBuffLen <= 0)
                {
                    // si no lei nada de el archivo actual:
                    // incremento el index de archivos
                    _archivoIndex++;
                    _archivoPtr = 0;
                }
                else
                {
                    // incremento el puntero del archivo...
                    _archivoPtr += _mainBuffLen;

                    // leí algo...
                    return true;
                }
            }
        }

        int TransferirBytes(byte[] buff, int offset, int count)
        {
            int disponibles = BytesDisponiblesEnElMainBuffer();
            int transferidos = 0;

            for (int i = 0; i < disponibles && i < count; i++)
            {
                buff[offset + i] = _mainBuff[_mainBuffPtr];
                _mainBuffPtr += 1;
                transferidos += 1;
            }

            return transferidos;
        }

        public override int Read(byte[] buff, int offset, int count)
        {
            if (!HayAlgoEnElMainBuffer())
            {
                if (RellenarElMainBuffer())
                {
                    if ((SkipBOM == BOMSkippingStrategy.SkipAlways) || 
                        (SkipBOM == BOMSkippingStrategy.SkipMiddle && _archivoIndex > 0))
                    {
                        // BOM FEBBBF (UTF8)
                        if (_mainBuff[0] == 0xEF &&
                            _mainBuff[1] == 0xBB &&
                            _mainBuff[2] == 0xBF)
                        {
                            _mainBuffPtr += 3;
                        }

                        // BOM EFFF
                        if (
                            _mainBuff[0] == 0xFE &&
                            _mainBuff[1] == 0xFF)
                        {
                            _mainBuffPtr += 2;
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }

            int transferidos = TransferirBytes(buff, offset, count);

            return transferidos;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => 0L;

        public override long Position { get => 0L; set => _ = value; }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin) => 0L;

        public override void SetLength(long value) { }

        public override void Write(byte[] buffer, int offset, int count) { }
    }
}
