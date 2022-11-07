using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCochesDriveUp.Controllers
{
    public class ValidadorComun
    {
        /////////////////////////////////////////////////////////////
        // Formato

        public static (bool, string) ProcesarFormato(string formato)
        {
            if (string.IsNullOrEmpty(formato))
            {
                return (true, "csv");
            }

            var formatosPosibles = GetFormatosPosibles();

            var formatoSanitizado = (formato ?? "")
                .Trim()
                .ToLower()
            ;

            if (!formatosPosibles.Contains(formatoSanitizado))
            {
                return (false, formato);
            }

            return (true, formatoSanitizado);
        }

        /////////////////////////////////////////////////////////////
        // DiasMenos

        public static (bool, int) ProcesarDiasMenos(string diasMenos)
        {
            var diasMenosSanitizado = (diasMenos ?? "")
                .Trim()
                .ToLower()
            ;

            if (StringEsNumeroEntero(diasMenosSanitizado))
            {
                return (true, int.Parse(diasMenosSanitizado));
            }
            else
            {
                return (false, 0);
            }
        }

        /////////////////////////////////////////////////////////////
        // Helpers

        public static List<string> GetFormatosPosibles()
        {
            return new List<string>()
            {
                "json",
                "csv",
                "csvnt",
            };
        }

        public static bool CharEsDigito(char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool StringEsNumeroEntero(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }

            foreach (char c in s)
            {
                if (!CharEsDigito(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
