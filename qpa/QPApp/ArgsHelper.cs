using System;
using System.Collections.Generic;

namespace QPApp
{
    public class ArgsHelper
    {
        public static string SafeGetArgVal(Dictionary<string, string> dic, string argName, string @default, Predicate<string> validator = null)
        {
            if (dic.ContainsKey(argName.ToLower()))
            {
                var val = dic[argName.ToLower()];

                if (validator == null || validator(val))
                {
                    return val ?? "";
                }
            }

            return @default;
        }

        public static Dictionary<string, string> CreateDictionary(string[] args, char sep = '=')
        {
            var dic = new Dictionary<string, string>();

            foreach (var argX in args)
            {
                if (argX.Contains(sep.ToString()))
                {
                    var partes = argX.Split(sep);

                    if (partes.Length == 2)
                    {
                        dic.Add(
                            partes[0].ToLower().Trim(),
                            partes[1].Trim()
                        );
                    }
                }
                else
                {
                    dic.Add(argX, string.Empty);
                }
            }

            return dic;
        }
    }
}
