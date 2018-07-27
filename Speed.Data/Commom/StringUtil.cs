using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class StringUtil
    {
        /// <summary>
        /// Remove a acentuação de um texto
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAccents(this string text)
        {
            string accents = "ÁÍÓÚÉÄÏÖÜËÀÌÒÙÈÃÕÂÎÔÛÊáíóúéäïöüëàìòùèãõâîôûêÇç";
            string replace = "AIOUEAIOUEAIOUEAOAIOUEaioueaioueaioueaoaioueCc";
            for (int i = 0; i < accents.Length; i++)
                text = text.Replace(accents[i], replace[i]);
            return text;
        }

        public static string Left(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            else
                return text.Substring(0, Math.Min(length, text.Length));
        }

        public static bool In(this string text, params string[] listToSearch)
        {
            foreach (var c in listToSearch)
                if (c == text)
                    return true;
            return false;
        }

        public static string Right(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            else
            {
                int min = Math.Min(length, text.Length);
                return text.Substring(text.Length - min, min);
            }
        }

        /// <summary>
        /// Retorna o texto à direita de text
        /// </summary>
        /// <param name="str"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Right(this string str, string text)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            int pos = str.IndexOf(text);
            if (pos == -1)
                return null;
            else
                return Right(str, str.Length - (pos + text.Length));
        }

        /// <summary>
        /// Gera uma string randômica
        /// </summary>
        /// <returns></returns>
        public static string Randomize(int length)
        {
            Random rLetra = new Random();
            string Alfabeto = "ab12cd4efg7hij8klm7n0op9q=rstu3vx50z";
            int iPosicao;
            string ret = "";
            for (int i = 0; i < length; i++)
            {
                iPosicao = rLetra.Next(0, Alfabeto.Length);
                ret += Alfabeto[iPosicao];
            }
            return ret;
        }

        /*

        public static string Left(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            else
            {
                return text.Substring(0, Math.Min(length, text.Length));
            }
        }

        public static string Right(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            else
            {
                int min = Math.Min(length, text.Length);
                return text.Substring(text.Length - min, min);
            }
        }
        */

        /// <summary>
        /// Retorna o nº de ocorrências do caracter 'c' dentro de 'text'
        /// </summary>
        /// <param name="text"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int CountChar(this string text, char c)
        {
            int count = 0;
            if (String.IsNullOrEmpty(text))
                return 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == c)
                    count++;
            }
            return count;
        }

        public static string Concat(this string[] text, string separator)
        {
            return Concat(text, "", "", separator, false);
        }

        public static string Concat(this string[] text)
        {
            return Concat(text, "", "", ", ", false);
        }

        public static string Concat(this string[] text, string separator, bool trim)
        {
            return Concat(text, "", "", separator, trim);
        }

        public static string Concat(this string[] text, string prefix, string sufix, string separator)
        {
            return Concat(text, prefix, sufix, separator, false);
        }

        public static string Concat(this string[] text, string prefix, string sufix, string separator, bool trim)
        {
            string ret = "";
            for (int i = 0; i < text.Length; i++)
            {
                string s = trim ? text[i].Trim() : text[i];
                if (i > 0) ret += separator;
                ret += prefix + s + sufix;
            }
            return ret;
        }

        public static string Capitalize(this string value, bool toLower = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                if (value.Length <= 1)
                    value = value.ToUpper();
                else
                    value = value.Substring(0, 1).ToUpper() +
                        (toLower ? value.Substring(1, value.Length - 1).ToLower() : value.Substring(1, value.Length - 1));
            }
            return value;
        }

        /// <summary>
        /// Converte a primeira letra pra maiúscula
        /// Se toLower = truee, faz ToLower da segunda letra em diante
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toLower"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string value, bool removeSpaces = true)
        {
            string[] parts;
            if (removeSpaces)
                parts = value.Split(new char[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            else
                parts = value.Split(new char[] { '_', }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                bool toLower = parts.Length > 0;
                value = "";
                for (int i = 0; i < parts.Length; i++)
                    value += Capitalize(parts[i], toLower);
            }
            if (Conv.IsNumeric(value[0]))
                value = "_" + value;
            return value;
        }

        public static string ToPascalCase2(this string value)
        {
            string[] parts= value.Split(new char[] { ' ', }, StringSplitOptions.None);

            if (parts.Length > 0)
            {
                value = "";
                for (int i = 0; i < parts.Length; i++)
                    value += Capitalize(parts[i], true) + " ";
            }
            if (Conv.IsNumeric(value[0]))
                value = "_" + value;
            return value.Trim();
        }

        /// <summary>
        /// Converte um texto pra Camel Case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string value)
        {
            string[] parts = value.Split(new char[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                value = "";
                for (int i = 0; i < parts.Length; i++)
                    value += Capitalize(parts[i]);
            }
            value = value[0].ToString().ToLower() + value.Substring(1, value.Length - 1);
            if (Conv.IsNumeric(value[0]))
                value = "_" + value;
            return value;
        }

        public static string Reverse(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            StringBuilder b = new StringBuilder(value.Length);
            for (int i = value.Length - 1; i >= 0; i--)
                b.Append(value[i]);
            return b.ToString();
        }

        public static int GetOnlyNumbers(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            else
            {
                string ret = "";
                foreach (var c in value)
                    if (c >= '0' && c <= '9')
                        ret += c;
                return Conv.ToInt32(ret);
            }
        }

        public static string Replace(this string text, string find, string replace, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            int pos = 0;
            do
            {
                pos = text.IndexOf(find, pos, comparisonType);
                if (pos >= 0)
                {
                    text = text.Remove(pos, find.Length);
                    text = text.Insert(pos, replace);
                    //pos = text.IndexOf(find, comparisonType);
                    pos += replace.Length;
                }
            } while (pos >= 0);
            return text;
        }

        public static string ReplaceInsensitive(this string text, string find, string replace)
        {
            return Replace(text, find, replace, StringComparison.InvariantCultureIgnoreCase);
        }

        public static Boolean IsValidEmail(String email)
        {
            String expresion;
            email = Conv.Trim(email);
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static Regex reGetLinks = new Regex(@"(http:\/\/([\w.]+\/?)\S*)", RegexOptions.IgnoreCase);
        public static string GetLinks(String text)
        {
            text = reGetLinks.Replace(text, "<a href=\"$1\" target=\"_blank\">$1</a>");
            return text;
        }

        public static bool EqualsICIC(this string text, string value)
        {
            if (text == null)
                return text == value;
            else
                return text.Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }

#if !SILVERLIGHT
        public static string ConvertToString(this object value)
        {
            TypeConverter converter =
              TypeDescriptor.GetConverter(value.GetType());

            // Can converter convert this type to string?   
            if (converter.CanConvertTo(typeof(string)))
            {
                // Convert it   
                return converter.ConvertTo(value,
                        typeof(string)) as string;
            }
            return value.ToString();
        }
#endif

        /// <summary>
        /// Convert um range no formato texto para uma matriz numérica
        /// Pode-se usar espaços, ',' ou ';' para separador, tanto faz. O '-' indica um range
        /// Ex: 1 2 7-21;25, 31  56; 89 - 90
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        public static int[] ConvertPageTextToNumbers(string pages)
        {
            pages = pages.Trim().Replace(',', ' ');
            pages = pages.Replace(';', ' ');
            pages = pages.Replace("  ", " ");
            List<int> ret = new List<int>();
            string[] ps = pages.Split(' ');
            foreach (var p in ps)
            {
                if (p.IndexOf('-') < 0)
                {
                    ret.Add(Convert.ToInt32(p));
                }
                else
                {
                    string[] pa = p.Split('-');
                    if (pa.Length == 2)
                    {
                        var p1 = Convert.ToInt32(pa[0]);
                        var p2 = Convert.ToInt32(pa[1]);
                        for (int i = p1; i <= p2; i++)
                            ret.Add(i);
                    }
                    else
                    {
                        throw new Exception("Invalid range: " + p);
                    }
                }
            }
            return ret.ToArray();
        }

        public static string NullIfEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
                str = null;
            return str;
        }

    }

}
