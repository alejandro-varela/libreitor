using System;
using System.Collections.Generic;
using System.Text;

namespace ComunDriveUp
{
    public class JsonParser
    {
        public class Info
        {
            //
        }

        int     _index  = 0;
        string  _json   = string.Empty;

        public Info ParsedInfo { get; set; } = new Info();

        public JsonParser(string json)
        {
            _json  = (json ?? string.Empty).Trim();
            _index = 0;
        }

        public static Tuple<bool, Info> Parse(string json)
        {
            var parser = new JsonParser(json);
            int depth  = 0;

            if (!parser.LeerValor(depth))
            {
                return Tuple.Create(false, parser.ParsedInfo);
            }

            return Tuple.Create(parser.EnFinal(), parser.ParsedInfo);
        }

        #region Helpers
        char? Actual()
        {
            if (_index >= _json.Length)
            {
                return null;
            }
            else
            {
                return _json[_index];
            }
        }

        bool EnFinal()
        {
            //  012345
            // "abcdef" len=6
            return Actual() == null;
        }

        void Avanzar()
        {
            _index += 1;
        }

        void LeerEspaciosEnBlanco()
        {
            for (; ; )
            {
                bool esBlanco =
                    Actual() == ' '   ||
                    Actual() == '\r'  ||
                    Actual() == '\n'  ||
                    Actual() == '\t'
                ;

                if (esBlanco)
                {
                    Avanzar();
                }
                else
                {
                    break;
                }
            }
        }

        bool Leer(char c)
        {
            if (Actual() == c)
            {
                Avanzar();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Valor
        bool LeerValor(int depth)
        {
            return
                LeerString()        ||
                LeerObjeto(depth)   ||
                LeerArreglo(depth)  ||
                LeerNull()          ||
                LeerBoolean()       ||
                LeerNumero()
            ;
        }
        #endregion

        #region Arreglo
        bool LeerCorcheteInicial()
        {
            return Leer('[');
        }

        bool LeerCorcheteFinal()
        {
            return Leer(']');
        }

        bool LeerArreglo(int depth)
        {
            // ej: []
            // ej: [ ]
            // ej: [ 1 , 2 , 3 ]
            // ej: [ true, false ]
            // ej: [1,"mama",true]

            if (!LeerCorcheteInicial())
            {
                return false;
            }

            LeerEspaciosEnBlanco();

            if (LeerCorcheteFinal())
            {
                return true;
            }

            do
            {
                LeerEspaciosEnBlanco();

                if (!LeerValor(depth + 1))
                {
                    return false;
                }

                LeerEspaciosEnBlanco();
            }
            while (LeerSeparador());

            if (!LeerCorcheteFinal())
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Null
        bool LeerNull()
        {
            if (Actual() == 'n') { Avanzar(); } else { return false; }
            if (Actual() == 'u') { Avanzar(); } else { return false; }
            if (Actual() == 'l') { Avanzar(); } else { return false; }
            if (Actual() == 'l') { Avanzar(); } else { return false; }
            return true;
        }
        #endregion

        #region Boolean
        bool LeerBoolean()
        {
            if (Actual() == 'f')
            {
                Avanzar();
                if (Actual() == 'a') { Avanzar(); } else { return false; }
                if (Actual() == 'l') { Avanzar(); } else { return false; }
                if (Actual() == 's') { Avanzar(); } else { return false; }
                if (Actual() == 'e') { Avanzar(); } else { return false; }
                return true;
            }
            else if (Actual() == 't')
            {
                Avanzar();
                if (Actual() == 'r') { Avanzar(); } else { return false; }
                if (Actual() == 'u') { Avanzar(); } else { return false; }
                if (Actual() == 'e') { Avanzar(); } else { return false; }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Numero
        bool LeerSignoNegativo()
        {
            return Leer('-');
        }

        bool LeerSignoPositivoONegativo()
        {
            if (Leer('-'))
            {
                return true;
            }

            if (Leer('+'))
            {
                return true;
            }

            return false;
        }

        bool LeerDigito(out char? digito)
        {
            if (Actual() >= '0' && Actual() <= '9')
            {
                digito = Actual();
                Avanzar();
                return true;
            }
            else
            {
                digito = null;
                return false;
            }
        }

        bool LeerDigito()
        {
            if (Actual() >= '0' && Actual() <= '9')
            {
                Avanzar();
                return true;
            }
            else
            {
                return false;
            }
        }

        bool LeerPuntoDecimal()
        {
            return Leer('.');
        }

        bool LeerE()
        {
            if (Leer('e'))
            {
                return true;
            }

            if (Leer('E'))
            {
                return true;
            }

            return false;
        }

        bool LeerNumero()
        {
            LeerSignoNegativo();

            StringBuilder sbParteEntera = new StringBuilder();
            char? digitoParteEntera;

            if (LeerDigito(out digitoParteEntera))
            {
                sbParteEntera.Append(digitoParteEntera ?? '\0');
            }
            else
            {
                return false;
            }

            while (LeerDigito(out digitoParteEntera))
            {
                sbParteEntera.Append(digitoParteEntera ?? '\0');
            }

            // evito que los números tengan 0 a la izquierda
            // si su parte entera tiene mas de 1 caracter...
            if (sbParteEntera.Length > 1 && sbParteEntera[0] == '0')
            {
                return false;
            }

            if (LeerPuntoDecimal())
            {
                if (!LeerDigito())
                {
                    return false;
                }

                while (LeerDigito())
                    ;
            }

            if (LeerE())
            {
                LeerSignoPositivoONegativo();

                if (!LeerDigito())
                {
                    return false;
                }

                while (LeerDigito())
                    ;
            }

            return true;
        }
        #endregion

        #region String
        bool LeerComilla()
        {
            return Leer('"');
        }

        bool LeerCaracteresDeCadenaJson()
        {
            while (Actual() != '"')
            {
                if (Actual() == '\\')
                {
                    // es un caracter de escape, debo avanzar un char
                    Avanzar();

                    // actuo dependiendo del actual
                    switch (Actual())
                    {
                        case '"':
                        case '\\':
                        case '/':
                        case 'b':
                        case 'f':
                        case 'n':
                        case 'r':
                        case 't':
                            {
                                Avanzar();
                                break;
                            }
                        case 'u':
                            {
                                Avanzar();
                                if (EsCaracterHexadecimal(Actual())) Avanzar(); else return false;
                                if (EsCaracterHexadecimal(Actual())) Avanzar(); else return false;
                                if (EsCaracterHexadecimal(Actual())) Avanzar(); else return false;
                                if (EsCaracterHexadecimal(Actual())) Avanzar(); else return false;
                            }
                            break;
                        default:
                            {
                                // lo que vino después de la barra invertida
                                // no es un caracter de escape válido
                                return false;
                            }
                    }
                }
                else if (Actual() == null)
                {
                    // esto puede pasar si el json es solo una cadena y no termina 
                    // por ej:  json = "hola como te
                    return false;
                }
                else
                {
                    // cualquier caracter que NO sea barra invertida (\) NI comilla (")
                    Avanzar();
                }
            }

            return true;
        }

        static bool EsCaracterHexadecimal(char? posibleCaracterHexadecimal)
        {
            if (posibleCaracterHexadecimal == null)
            {
                return false;
            }

            return 
                (posibleCaracterHexadecimal >= '0' && posibleCaracterHexadecimal <= '9') ||
                (posibleCaracterHexadecimal >= 'A' && posibleCaracterHexadecimal <= 'F') ||
                (posibleCaracterHexadecimal >= 'a' && posibleCaracterHexadecimal <= 'f')
            ;
        }

        bool LeerString()
        {
            if (!LeerComilla())
            {
                return false;
            }

            if (!LeerCaracteresDeCadenaJson())
            {
                return false;
            }

            if (!LeerComilla())
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Objeto
        bool LeerLlaveInicial()
        {
            return Leer('{');
        }

        bool LeerDosPuntos()
        {
            return Leer(':');
        }

        bool LeerSeparador()
        {
            return Leer(',');
        }

        bool LeerLlaveFinal()
        {
            return Leer('}');
        }

        bool LeerObjeto(int depth)
        {
            // ej: { "nombre" : "pepe" , "edad" : 42.05 , "esProgramatore" : true }

            if (!LeerLlaveInicial())
            {
                return false;
            }

            LeerEspaciosEnBlanco();

            if (LeerLlaveFinal())
            {
                return true;
            }

            do
            {
                LeerEspaciosEnBlanco();

                if (!LeerString())
                {
                    return false;
                }

                LeerEspaciosEnBlanco();

                if (!LeerDosPuntos())
                {
                    return false;
                }

                LeerEspaciosEnBlanco();

                if (!LeerValor(depth + 1))
                {
                    return false;
                }

                LeerEspaciosEnBlanco();

            } while (LeerSeparador());

            LeerLlaveFinal();

            return true;
        }
        #endregion
    }

}

