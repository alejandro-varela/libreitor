using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ComunStreams
{
    public class MergeTextReader<MergeOrderType> : TextReader
    {
        class Lector
        { 
            public TextReader TextReader { get; set; }
            public string Valor { get; set; }
        }

        bool _iniciarEstructura = true;

        List<Lector> _estructura = null;

        Func<string, MergeOrderType> _selectorOrden = null;

        public MergeTextReader(List<TextReader> textReaders, Func<string, MergeOrderType> selectorOrden)
        {
            _selectorOrden = selectorOrden;
            _estructura    = textReaders
                .Select(tr => new Lector
                {
                    TextReader  = tr,
                })
                .ToList()
            ;
        }

        public override string ReadLine()
        {
            if (_iniciarEstructura)
            {
                foreach (var lectorX in _estructura)
                {
                    lectorX.Valor = lectorX.TextReader.ReadLine();
                }

                _iniciarEstructura = false;
            }

            var lectorElegido = _estructura
                .Where          (lectorX => lectorX.Valor != null)
                .OrderBy        (lectorX => _selectorOrden(lectorX.Valor))
                .FirstOrDefault ()
            ;

            if (lectorElegido == null)
            {
                return null;
            }

            var retVal = lectorElegido.Valor;

            if (retVal != null)
            {
                lectorElegido.Valor = lectorElegido.TextReader.ReadLine();
            }

            return retVal;
        }
    }
}
