using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Speed.Common
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class Conv
    {

        public static Dictionary<string, object> Defaults;
        public static Dictionary<string, string> DefaultsTexts;
        private const string sep = "----------------------------------------------------------------";

        static CultureInfo culturePort = new CultureInfo("pt-BR");

        static Conv()
        {

            Defaults = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            Defaults.Add("Boolean", "false");
            Defaults.Add("Byte", 0);
            Defaults.Add("Char", '\0');
            Defaults.Add("DateTime", DateTime.MinValue);
            Defaults.Add("Decimal", 0);
            Defaults.Add("Double", 0);
            Defaults.Add("Guid", Guid.Empty);
            Defaults.Add("Int16", 0);
            Defaults.Add("Int32", 0);
            Defaults.Add("Int64", 0);
            Defaults.Add("SByte", 0);
            Defaults.Add("Single", 0);
            Defaults.Add("TimeSpan", 0);
            Defaults.Add("UInt16", 0);
            Defaults.Add("UInt32", 0);
            Defaults.Add("UInt64", 0);
            Defaults.Add("String", null);

            Defaults.Add("Boolean?", null);
            Defaults.Add("Byte?", null);
            Defaults.Add("Char?", "null'");
            Defaults.Add("DateTime?", null);
            Defaults.Add("Decimal?", null);
            Defaults.Add("Double?", null);
            Defaults.Add("Guid?", null);
            Defaults.Add("Int16?", null);
            Defaults.Add("Int32?", null);
            Defaults.Add("Int64?", null);
            Defaults.Add("SByte?", null);
            Defaults.Add("Single?", null);
            Defaults.Add("TimeSpan?", null);
            Defaults.Add("UInt16?", null);
            Defaults.Add("UInt32?", null);
            Defaults.Add("UInt64?", null);

            Defaults.Add("Boolean[]", null);
            Defaults.Add("Byte[]", null);
            Defaults.Add("Char[]", "null'");
            Defaults.Add("DateTime[]", null);
            Defaults.Add("Decimal[]", null);
            Defaults.Add("Double[]", null);
            Defaults.Add("Guid[]", null);
            Defaults.Add("Int16[]", null);
            Defaults.Add("Int32[]", null);
            Defaults.Add("Int64[]", null);
            Defaults.Add("SByte[]", null);
            Defaults.Add("Single[]", null);
            Defaults.Add("TimeSpan[]", null);
            Defaults.Add("UInt16[]", null);
            Defaults.Add("UInt32[]", null);
            Defaults.Add("UInt64[]", null);



            DefaultsTexts = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            DefaultsTexts.Add("Boolean", "false");
            DefaultsTexts.Add("Byte", "0");
            DefaultsTexts.Add("Char", "'\0'");
            DefaultsTexts.Add("DateTime", "DateTime.MinValue");
            DefaultsTexts.Add("Decimal", "0");
            DefaultsTexts.Add("Double", "0");
            DefaultsTexts.Add("Guid", "Guid.Empty");
            DefaultsTexts.Add("Int16", "0");
            DefaultsTexts.Add("Int32", "0");
            DefaultsTexts.Add("Int64", "0");
            DefaultsTexts.Add("SByte", "0");
            DefaultsTexts.Add("Single", "0");
            DefaultsTexts.Add("TimeSpan", "0");
            DefaultsTexts.Add("UInt16", "0");
            DefaultsTexts.Add("UInt32", "0");
            DefaultsTexts.Add("UInt64", "0");
            DefaultsTexts.Add("String", "null");

            DefaultsTexts.Add("Boolean?", "null");
            DefaultsTexts.Add("Byte?", "null");
            DefaultsTexts.Add("Char?", "null'");
            DefaultsTexts.Add("DateTime?", "null");
            DefaultsTexts.Add("Decimal?", "null");
            DefaultsTexts.Add("Double?", "null");
            DefaultsTexts.Add("Guid?", "null");
            DefaultsTexts.Add("Int16?", "null");
            DefaultsTexts.Add("Int32?", "null");
            DefaultsTexts.Add("Int64?", "null");
            DefaultsTexts.Add("SByte?", "null");
            DefaultsTexts.Add("Single?", "null");
            DefaultsTexts.Add("TimeSpan?", "null");
            DefaultsTexts.Add("UInt16?", "null");
            DefaultsTexts.Add("UInt32?", "null");
            DefaultsTexts.Add("UInt64?", "null");

            DefaultsTexts.Add("Boolean[]", "null");
            DefaultsTexts.Add("Byte[]", "null");
            DefaultsTexts.Add("Char[]", "null'");
            DefaultsTexts.Add("DateTime[]", "null");
            DefaultsTexts.Add("Decimal[]", "null");
            DefaultsTexts.Add("Double[]", "null");
            DefaultsTexts.Add("Guid[]", "null");
            DefaultsTexts.Add("Int16[]", "null");
            DefaultsTexts.Add("Int32[]", "null");
            DefaultsTexts.Add("Int64[]", "null");
            DefaultsTexts.Add("SByte[]", "null");
            DefaultsTexts.Add("Single[]", "null");
            DefaultsTexts.Add("TimeSpan[]", "null");
            DefaultsTexts.Add("UInt16[]", "null");
            DefaultsTexts.Add("UInt32[]", "null");
            DefaultsTexts.Add("UInt64[]", "null");

            DefaultsTexts.Add("SqlHierarchyId", "SqlHierarchyId");
        }

        public static bool IsNull(this object value)
        {
            return value == null || value == DBNull.Value;
        }

        public static bool IsNullOrEmpty(this object value)
        {
            if (value == null || value == DBNull.Value)
                return true;
            else
                return string.IsNullOrEmpty(Conv.ToString(value));
        }

        public static bool IsEmpty(this object value)
        {
            return Conv.ToString(value).Length == 0;
        }

        public static object NeDef(object value, object defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            else
                return value;
        }

        public static object NeDefEmpty(object value, object defaultValue)
        {
            if (value == null || value == DBNull.Value || value == "")
                return defaultValue;
            else
                return value;
        }

        public static bool ToBoolean(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            if ((value is bool))
                return (bool)value;

            if (IsNull(value))
                return false;

            if (In(value, true, false))
                return Convert.ToBoolean(ToInt32(value, culture));

            if (In(ToString(value, culture).ToUpper(), "TRUE", "Y", "YES", "S", "SIM"))
                return true;

            if (In(ToString(value, culture).ToUpper(), "FALSE", "N", "NO", "NAO", "NÃO"))
                return false;

            return Convert.ToBoolean(Conv.ToInt32(value, culture), culture);
        }

        public static bool? ToBooleanN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            if ((value is bool || value is bool?))
                return (bool?)value;

            if (IsNull(value))
                return (bool?)null;

            if (In(value, true, false))
                return Convert.ToBoolean(ToInt32N(value, null, culture));

            if (In(ToString(value, culture).ToUpper(), "TRUE", "Y", "YES", "S", "SIM"))
                return true;

            if (In(ToString(value, culture).ToUpper(), "FALSE", "N", "NO", "NAO", "NÃO"))
                return false;

            return Convert.ToBoolean(Conv.ToInt32(value, culture), culture);
        }

        public static Int32 ToInt32(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            return ToInt32(value, 0, culture);
        }

        public static Int32 ToInt32(this object value, int defaultValue, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDef(value, defaultValue);
            try
            {
                return Convert.ToInt32(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static Int32? ToInt32N(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            return ToInt32N(value, null, culture);
        }

        public static Int32? ToInt32N(this object value, int? defaultValue, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDef(value, defaultValue);
            if (IsNull(value))
                return (Int32?)null;
            try
            {
                return Convert.ToInt32(ret, culture);
            }
            catch
            {
                return (Int32?)null;
            }

        }

        public static Int16 ToInt16(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToInt16(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static UInt16 ToUInt16(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToUInt16(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static UInt32 ToUInt32(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToUInt32(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static Int16? ToInt16N(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, null);
            if (IsNull(value))
                return (Int16?)null;
            try
            {
                return Convert.ToInt16(ret, culture);
            }
            catch
            {
                return (Int16?)null;
            }
        }

        public static UInt16? ToUInt16N(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, null);
            if (IsNull(value))
                return (UInt16?)null;
            try
            {
                return Convert.ToUInt16(ret, culture);
            }
            catch
            {
                return (UInt16?)null;
            }
        }

        public static UInt32? ToUInt32N(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, null);
            if (IsNull(value))
                return (UInt32?)null;
            try
            {
                return Convert.ToUInt32(ret, culture);
            }
            catch
            {
                return (UInt32?)null;
            }
        }

        public static UInt64 ToUInt64(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToUInt64(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static UInt64? ToUInt64N(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, null);
            if (IsNull(value))
                return (UInt64?)null;
            try
            {
                return Convert.ToUInt64(ret, culture);
            }
            catch
            {
                return (UInt64?)null;
            }
        }

        public static double ToDouble(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToDouble(ret, culture);
            }
            catch
            {
                return 0;
            }

        }

        public static double? ToDoubleN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, null);
            if (IsNull(value))
                return (double?)null;
            try
            {
                return Convert.ToDouble(ret, culture);
            }
            catch
            {
                return (double?)null;
            }

        }

        public static decimal ToDecimal(this object value)
        {
            if (value == null)
            {
                return 0;
            }

            decimal result;

            if (decimal.TryParse(value.ToString(), out result))
            {
                return result;
            }

            return 0;
        }

        public static decimal ToDecimal(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToDecimal(ret, culture);
            }
            catch
            {
                return 0;
            }

        }

        public static decimal? ToDecimalN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, null);
            if (IsNull(value))
                return (decimal?)null;
            try
            {
                return Convert.ToDecimal(ret, culture);
            }
            catch
            {
                return (decimal?)null;
            }
        }

        public static byte ToByte(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToByte(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static sbyte ToSByte(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToSByte(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static Int64 ToInt64(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToInt64(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static Int64? ToInt64N(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            if (IsNull(value))
                return (Int64?)null;
            try
            {
                return Convert.ToInt64(ret, culture);
            }
            catch
            {
                return (Int64?)null;
            }
        }

        public static DateTime ToDateTime(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDef(value, DateTime.MinValue);
            try
            {
                return Convert.ToDateTime(ret, culture);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime? ToDateTimeN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDef(value, null);
            if (IsNull(value))
                return (DateTime?)null;
            try
            {
                return Convert.ToDateTime(ret, culture);
            }
            catch
            {
                return (DateTime?)null;
            }
        }

        public static DateTime? ToDateTimePort(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            else
                return Convert.ToDateTime(value, culturePort);
        }

        public static long ToLong(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            return ToInt64(value, culture);
        }

        public static long? ToLongN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            return ToInt64N(value, culture);
        }

        public static float ToFloat(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToSingle(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static float? ToFloatN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, null);
            if (IsNull(value))
                return (long?)null;
            try
            {
                return Convert.ToSingle(ret, culture);
            }
            catch
            {
                return (float?)null;
            }
        }

        public static Single ToSingle(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            try
            {
                return Convert.ToSingle(ret, culture);
            }
            catch
            {
                return 0;
            }
        }

        public static Single? ToSingleN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDefEmpty(value, 0);
            if (IsNull(value))
                return (Single?)null;
            try
            {
                return Convert.ToSingle(ret, culture);
            }
            catch
            {
                return (Single?)null;
            }
        }

        public static string ToString(object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDef(value, "");
            try
            {
                return Convert.ToString(ret, culture);
            }
            catch
            {
                return "";
            }
        }

        public static string ToStringN(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDef(value, null);
            if (ret == null)
                return null;
            try
            {
                return Convert.ToString(ret, culture);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Faz um ToString seguido por Trim
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Trim(this object value)
        {
            return ToString(value).Trim();
        }


        public static char ToChar(this object value, CultureInfo culture = null)
        {
            culture = culture ?? Culture;
            object ret = NeDef(value, '\0');
            try
            {
                return Convert.ToChar(ret);
            }
            catch
            {
                return '\0';
            }
        }

        public static string ToSqlText(object value)
        {
            return Conv.ToString(value).Replace("'", "''");
        }

        /// Trata as aspas simples no meio da string
        /// Isso evita Sql Injection
        public static string ToSqlText(object value, int length)
        {
            return Left(Conv.ToString(value), length).Replace("'", "''");
        }

        /// <summary>
        /// Trata as aspas simples no meio da string e contatena aspas simples no início e final.
        /// Isso evita Sql Injection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlTextA(object value)
        {
            return "'" + ToSqlText(value) + "'";
        }

        /// <summary>
        /// Trata as aspas simples no meio da string e contatena aspas simples no início e final.
        /// Isso evita Sql Injection
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToSqlTextA(object value, int length)
        {
            return "'" + ToSqlText(value, length) + "'";
        }

        public static string Left(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            if (text.Length > length)
                return text.Substring(0, Math.Min(text.Length, length));
            else
                return text;
        }

        //Function to get string from end
        public static string Right(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            if (length > 0)
                return text.Substring(text.Length - length, length);
            else
                return text;
        }

        /// <summary>
        /// Trunca uma string, mas não quebra palavras. Procura o primeiro espaço e trunca ali
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Trunc(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            if (text.Length == 0 || text.Length <= maxLength)
                return text;

            int index = maxLength - 1;
            while (text[index] != ' ')
                index--;

            if (index == 0)
                return Conv.Left(text, maxLength);
            else
            {
                return text.Substring(0, index);
            }
        }

        public static TimeSpan? ToTimeSpan(object time)
        {
            string value = ToString(time);
            if (string.IsNullOrEmpty(value) || value.IndexOf(':') == -1)
                return null;

            var values = value.Split(':');
            return new TimeSpan(
                values.Length > 0 ? ToInt32(values[0]) : 0,
                values.Length > 1 ? ToInt32(values[1]) : 0,
                values.Length > 2 ? ToInt32(values[2]) : 0
                );
        }

        /// <summary>
        /// Trunca uma string, mas não quebra palavras. Procura o primeiro espaço e trunca ali
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <param name="addToEnd">Texto à ser inserido no final, caso text.Length > maxLength. Exemplo: " ..."</param>
        /// <returns></returns>
        public static string Trunc(string text, int maxLength, string addToEnd)
        {
            if (text.Length == 0 || text.Length <= maxLength)
                return text;

            int index = maxLength - 1;
            while (text[index] != ' ')
                index--;

            if (index == 0)
                return Conv.Left(text, maxLength) + addToEnd;
            else
                return text.Substring(0, index) + addToEnd;
        }

        public static bool IsDbNull(object value)
        {
            return value == DBNull.Value;
        }

        /// <summary>
        /// Contatena um array de strings
        /// Não acrescenta o último endSeparator
        /// </summary>
        /// <param name="endSeparator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Concat(string endSeparator, params object[] values)
        {
            return Concat(null, endSeparator, values);
        }

        /// <summary>
        /// Contatena um array de strings
        /// Não acrescenta o último endSeparator
        /// </summary>
        /// <param name="beforeSeparator"></param>
        /// <param name="endSeparator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Concat(string beforeSeparator, string endSeparator, params object[] values)
        {
            return Concat(beforeSeparator, endSeparator, false, values);
        }

        public static string Concat(string beforeSeparator, string endSeparator, bool addLastSeparator, params object[] values)
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (beforeSeparator != null)
                    text.Append(beforeSeparator);
                text.Append(Conv.ToString(values[i]));
                if ((endSeparator != null) && (addLastSeparator | (i < values.Length - 1 && endSeparator != null)))
                    text.Append(endSeparator);
            }
            return text.ToString();
        }

        public static string ConcatAndBrackets(string separator, params object[] values)
        {
            return "(" + Concat(separator, values) + ")";
        }

        public static object ToObject(object value)
        {
            return value;
        }

        public static bool In(object value, params object[] values)
        {
            if (values == null)
                return false;

            string val = ToString(value);
            for (int i = 0; i < values.Length; i++)
                if (val == ToString(values[i]))
                    return true;
            return false;

        }

        public static bool InArray<T>(T value, T[] values)
        {
            if (values == null)
                return false;

            for (int i = 0; i < values.Length; i++)
                if (value.Equals(values[i]))
                    return true;
            return false;
        }

        public static T Generic<T>(object value, object defaultValue)
        {
            if (IsNull(value))
                return (T)defaultValue;
            else
                return (T)value;
        }

        public static byte ToSqlBit(object value)
        {
            return ToSqlBit(value, 0);
        }

        public static byte ToSqlBit(object value, int defaultValue)
        {
            if (IsNull(value))
                return Convert.ToByte(defaultValue);
            value = Convert.ToInt16(value);

            return (Conv.ToInt32(value, defaultValue) == 0 ? (byte)0 : (byte)1);
        }

        public static string ToSqlDate(DateTime? date)
        {
            return date.HasValue ? "'" + date.Value.ToString("yyyy-MM-dd") + "'" : "NULL";
        }

        public static string ToSqlDateTime(DateTime? date)
        {
            return date.HasValue ? "'" + date.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL";
        }

        /// <summary>
        /// Retorna uma expressão SQL, já tratando quando os valores são nulos ou não.
        /// Se allDay=true, inclui 00:00:00 para date1 e 23:59:59 para date2
        /// Se date1=null && date2=null, retorna null
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="includeHours"></param>
        /// <param name="allDay"></param>
        /// <returns></returns>
        public static string ToSqlDateRange(string columnName, DateTime? date1, DateTime? date2, bool includeHours, bool allDay = false)
        {
            string sql;
            if (allDay)
            {
                if (date1.HasValue)
                    date1 = date1.Value.Date;
                if (date2.HasValue)
                    date2 = new DateTime(date2.Value.Year, date2.Value.Month, date2.Value.Day, 23, 59, 59);
            }

            string value1 = includeHours | allDay ? ToSqlDateTime(date1) : ToSqlDate(date1);
            string value2 = includeHours | allDay ? ToSqlDateTime(date2) : ToSqlDate(date2);

            if (date1.HasValue && date2.HasValue)
                sql = string.Format("({0} >= {1} and {0} <= {2})", columnName, value1, value2);
            else if (date1.HasValue && !date2.HasValue)
                sql = string.Format("({0} >= {1}", columnName, value1);
            else if (!date1.HasValue && date2.HasValue)
                sql = string.Format("({0} <= {1}", columnName, value2);
            else // if (!date1.HasValue && !date2.HasValue)
                sql = null;

            return sql;
        }

        public static double[] ToDouble(double?[] values)
        {
            double[] ret = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
                ret[i] = Convert.ToDouble(values[i]);
            return ret;
        }

        public static int TrimLength(object value)
        {
            return Conv.Trim(value).Length;
        }

        public static bool HasData(object value)
        {
            return Conv.Trim(value).Length > 0;
        }

        public static int Length(object value)
        {
            return Conv.ToString(value).Length;
        }

        public static DateTime RemoveMilliseconds(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
        }

        /// <summary>
        /// Exemplo: double? x = Cast(typeof(double?), 1.0);
        ///          int y = .Cast(typeof(int), 1.2345);
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Cast(Type type, object obj)
        {
            return GetConverter(type, obj)(obj);
        }
        private static readonly IDictionary<PairOfTypes, Func<object, object>> converters =
            new Dictionary<PairOfTypes, Func<object, object>>();

        private static readonly ParameterExpression convParameter =
            Expression.Parameter(typeof(object), "val");

        // This is the method with the "guts" of the implementation
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static Func<object, object> GetConverter(Type targetType, object val)
        {
            var fromType = val != null ? val.GetType() : typeof(object);
            var key = new PairOfTypes(fromType, targetType);
            Func<object, object> res;
            if (converters.TryGetValue(key, out res))
            {
                return res;
            }
            res = (Func<object, object>)Expression.Lambda(
                Expression.Convert(
                    Expression.Convert(
                        Expression.Convert(
                            convParameter
                        , fromType
                        )
                    , targetType
                    )
                , typeof(object)
                )
            , convParameter
            ).Compile();
            converters.Add(key, res);
            return res;
        }

        // This class provides Equals and GetHashCode
        // for a pair of System.Type objects.
        private class PairOfTypes
        {
            private readonly Type first;
            private readonly Type second;
            public PairOfTypes(Type first, Type second)
            {
                this.first = first;
                this.second = second;
            }
            public override int GetHashCode()
            {
                return 31 * first.GetHashCode() + second.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                if (obj == this)
                {
                    return true;
                }
                var other = obj as PairOfTypes;
                if (other == null)
                {
                    return false;
                }
                return first.Equals(other.first)
                    && second.Equals(other.second);
            }
        }

        public static Color SunColor
        {
            get { return Color.FromName("fff5f2"); }
        }

        #region Converter

        public static CultureInfo Culture = new CultureInfo("en-US");

        public static decimal EnuToNumber(string text)
        {
            if (Conv.HasData(text))
                return Convert.ToDecimal(text, Culture);
            else
                return 0;
        }

        public static int EnuToInt32(string text)
        {
            if (Conv.HasData(text))
                return Convert.ToInt32(text, Culture);
            else
                return 0;
        }

        public static string EnuFromNumber(decimal value)
        {
            return Convert.ToString(value, Culture);
        }

        public static string EnuFromDateTime(DateTime value)
        {
            return Convert.ToString(value, Culture);
        }

        public static DateTime EnuToDateTime(object value)
        {
            return Convert.ToDateTime(value, Culture);
        }

        public static decimal ConvertTextTimeToNumber(string text)
        {
            var vs = text.Split(':');
            var ts = new TimeSpan(0, I(vs[0]), I(vs[1]), I(vs[2]), I(vs[3]) * 10);
            return (decimal)ts.TotalSeconds;
        }

        public static string ConvertNumberToTextTime(decimal value)
        {
            long ticks = Convert.ToInt64(value * (decimal)Math.Pow(10, 9) / 100);
            var ts = new TimeSpan(ticks);

            //var ts2 = new TimeSpan(0, 1, 2, 3, 4);

            return string.Format("{0}:{1}:{2}:{3}",
                S(ts.Hours), S(ts.Minutes), S(ts.Seconds), S(ts.Milliseconds));
        }

        static int I(string value)
        {
            return Conv.ToInt32(value);
        }

        static string S(long value)
        {
            return StringUtil.Left(string.Format("{0:00}", value), 2);
        }

        #endregion Converter

        /// <summary>
        /// Retorna só a parte da Data, removendo horas, minutos e segundos
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDateOnly1(DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        /// Retorna a Data, com o tempo como 23:59:59:999
        /// Usado para filtros em query
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDateOnly2(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static string ExtractOnlyNumbers(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            string ret = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c >= '0' && c <= '9')
                    ret += c;
            }
            return ret;
        }

        /// <summary>
        /// Calcula o número de páginas
        /// </summary>
        /// <param name="count">Número de items</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns></returns>
        public static int CalcPageSize(int count, int pageSize)
        {
            if (count % pageSize == 0)
                return count / pageSize + 1;
            else
                return count / pageSize;
        }

        static Random r = new Random();
        public static string GeneratePassword(int length)
        {
            string password = "";
            int v1 = (int)'a';
            int v2 = (int)'z';

            for (int i = 0; i < length; i++)
                password += (char)r.Next(v1, v2);
            return password;
        }

        public static List<T> GetList<T>(params T[] pars)
        {
            return new List<T>(pars);
        }

        public static List<string> GetList(params string[] pars)
        {
            return new List<string>(pars);
        }

        /// <summary>
        /// Retorna uma data. Método útil para trabalhar no fdormato dd/MM/yy
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime DMY(int day, int month, int year)
        {
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Converte um objecto para sua representação string, no formato Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToString(object obj)
        {
            String json;
            using (var stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(stream, obj);
                json = Encoding.UTF8.GetString(stream.ToArray());
                return json;
            }
        }

        /// <summary>
        /// Converte um objecto para sua representação byte, no formato Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToBytes(object obj)
        {
            using (var stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(stream, obj);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Converte uma string, no formato Json, para um objeto
        /// </summary>
        /// <param name="json">Representação json (string) do objeto</param>
        /// <returns></returns>
        public static T StringToObject<T>(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Converte um byte[], no formato Json, para um objeto
        /// </summary>
        /// <param name="json">Representação json (byte[]) do objeto</param>
        /// <returns></returns>
        public static T ByteToObject<T>(byte[] json)
        {
            using (var stream = new MemoryStream(json))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Gera uma chava com os valores de keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetKey(params object[] keys)
        {
            // retira os nulls
            string[] values = new string[keys.Length];
            for (int i = 0; i < values.Length; i++)
                values[i] += Conv.ToString(keys[i]);

            return string.Join(".", values);
        }

        public static bool Contains(this string s1, string s2, StringComparison comparison)
        {
            return s1.IndexOf(s2, comparison) >= 0;
        }

        /// <summary>
        /// Insentitive case
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool ContainsI(this string s1, string s2)
        {
            return s1.IndexOf(s2, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        /// <summary>
        /// Retorna uma string com os caracteres e o valor asc destes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCharInfo(string value)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(value);
            b.AppendLine();
            foreach (var c in value)
            {
                b.Append(c);
                b.Append(' ');
                b.Append((int)c);
                b.AppendLine();
            }
            return b.ToString();
        }

        /// <summary>
        /// Retorna a mensagem de erro de uma exception.
        /// Se InnerException não for null, acrestanta-a tb, recursivamente
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="includeStack"></param>
        /// <returns></returns>
        public static string GetErrorMessage(Exception ex, bool includeStack)
        {
            System.Text.StringBuilder b = new System.Text.StringBuilder();

            var ex2 = ex;
            int count = 0;
            do
            {
                count++;
                if (b.Length > 0)
                    b.AppendLine(sep);
                b.AppendLine(ex2.Message);
                ex2 = ex2.InnerException;
            }
            while (ex2 != null && count <= 20);

            if (includeStack)
            {
                b.AppendLine();
                b.AppendLine("StackTrace");
                b.AppendLine();

                ex2 = ex;
                count = 0;
                do
                {
                    count++;
                    if (b.Length > 0)
                        b.AppendLine(sep);
                    b.AppendLine(ex2.Message);
                    if (includeStack && ex2.StackTrace != null)
                        b.AppendLine(ex2.StackTrace);
                    ex2 = ex2.InnerException;
                }
                while (ex2 != null && count <= 20);
            }

            return b.ToString();
        }

        /// <summary>
        /// Converte um objeto num DataTable, com 2 colunas, Nome (nome da propriedade) e Valor (valor da propriedade)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataTable ObjectToDataTable(object value)
        {
            List<PropertyInfo> props = new List<PropertyInfo>();
            foreach (PropertyInfo prop in value.GetType().GetProperties())
                props.Add(prop);

            DataTable tb = new DataTable();

            tb.Columns.Add("Nome");
            tb.Columns.Add("Valor");

            foreach (var prop in props)
            {
                DataRow row = tb.NewRow();
                row["Nome"] = prop.Name;
                row["Valor"] = Conv.ToString(prop.GetValue(value, null));
                tb.Rows.Add(row);
            }
            return tb;
        }

        /// <summary>
        /// Checa se uma string é numérica
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            Int64 val;
            return Int64.TryParse(value, out val);
        }

        /// <summary>
        /// Checa se um char é numérico
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(char value)
        {
            byte val;
            return byte.TryParse(value.ToString(), out val);
        }

        public static string Unquote(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (value.StartsWith("\""))
                value = value.Remove(0, 1);
            if (value.EndsWith("\""))
                value = value.Remove(value.Length - 1, 1);
            return value;
        }

        public static string Quote(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (!value.StartsWith("\""))
                value = "\"" + value;
            if (!value.EndsWith("\""))
                value += "\"";
            return value;
        }

        /// <summary>
        /// Dá um trim em todas as propriedades string. Se o valor for String.IsNullOrEmpty, seta null
        /// </summary>
        /// <param name="list"></param>
        public static void TrimObject(System.Collections.IEnumerable list)
        {
            foreach (var item in list)
                TrimObject(item);
        }

        /// <summary>
        /// Dá um trim em todas as propriedades string. Se o valor for String.IsNullOrEmpty, seta null
        /// </summary>
        /// <param name="obj"></param>
        public static void TrimObject(object obj)
        {
            if (obj == null)
                return;

            var props = obj.GetType().GetProperties();
            foreach (var prop in props)
            {
                try
                {
                    if (!prop.PropertyType.IsPrimitive)
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            var val = prop.GetValue(obj, null) as string;
                            if (prop.CanWrite)
                            {
                                if (!string.IsNullOrEmpty(val))
                                    prop.SetValue(obj, val.Trim(), null);
                                else
                                    prop.SetValue(obj, null, null);
                            }
                        }
                        else
                        {
                            try
                            {
                                var val2 = prop.GetValue(obj, null);
                                if (obj != val2 && !prop.PropertyType.IsPrimitive && !prop.PropertyType.IsSealed)
                                    TrimObject(val2);
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            foreach (var field in obj.GetType().GetFields().Where(p => !p.IsStatic))
            {
                if (!field.FieldType.IsPrimitive)
                {
                    if (field.FieldType == typeof(string))
                    {
                        var val = field.GetValue(obj) as string;
                        if (val != null)
                            field.SetValue(obj, val.Trim());
                    }
                    else
                        if (!field.IsLiteral)
                    {
                        var val2 = field.GetValue(obj);
                        if (obj != val2 && !field.FieldType.IsPrimitive && !field.FieldType.IsSealed)
                            TrimObject(val2);
                    }
                }
            }

        }

        /// <summary>
        /// Converte para um DataTable, mas trata todos as DataColumns como string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTableStr<T>(IList<T> list)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, "".GetType()); //Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            table.BeginLoadData();
            object[] values = new object[props.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (props[i].PropertyType.ToString().ToLower().Contains("decimal"))
                    {
                        if (props[i] != null && props[i].GetValue(item) != null)
                            values[i] = props[i].GetValue(item).ToString().Replace(".", ",") ?? "0,00";
                    }
                    else
                    {
                        try
                        {
                            values[i] = RemoveSpecialCharacters(props[i].GetValue(item).ToString());
                        }
                        catch
                        {
                            values[i] = "";
                        }
                    }
                }
                table.Rows.Add(values);
            }
            table.EndLoadData();
            return table;
        }

        /// <summary>
        /// Converte para um DataTable, mas trata todos as DataColumns como string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            var entityType = typeof(T);

            // Lists of type System.String and System.Enum (which includes enumerations and structs) must be handled differently 
            // than primitives and custom objects (e.g. an object that is not type System.Object).
            if (entityType == typeof(String))
            {
                var dataTable = new DataTable(entityType.Name);
                dataTable.Columns.Add(entityType.Name);

                // Iterate through each item in the list. There is only one cell, so use index 0 to set the value.
                foreach (T item in list)
                {
                    var row = dataTable.NewRow();
                    row[0] = item;
                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }
            else if (entityType.BaseType == typeof(Enum))
            {
                var dataTable = new DataTable(entityType.Name);
                dataTable.Columns.Add(entityType.Name);

                // Iterate through each item in the list. There is only one cell, so use index 0 to set the value.
                foreach (string namedConstant in Enum.GetNames(entityType))
                {
                    var row = dataTable.NewRow();
                    row[0] = namedConstant;
                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }

            // Check if the type of the list is a primitive type or not. Note that if the type of the list is a custom 
            // object (e.g. an object that is not type System.Object), the underlying type will be null.
            var underlyingType = Nullable.GetUnderlyingType(entityType);
            var primitiveTypes = new List<Type>
    {
        typeof (Byte),
        typeof (Char),
        typeof (Decimal),
        typeof (Double),
        typeof (Int16),
        typeof (Int32),
        typeof (Int64),
        typeof (SByte),
        typeof (Single),
        typeof (UInt16),
        typeof (UInt32),
        typeof (UInt64),
    };

            var typeIsPrimitive = primitiveTypes.Contains(underlyingType);

            // If the type of the list is a primitive, perform a simple conversion.
            // Otherwise, map the object's properties to columns and fill the cells with the properties' values.
            if (typeIsPrimitive)
            {
                var dataTable = new DataTable(underlyingType.Name);
                dataTable.Columns.Add(underlyingType.Name);

                // Iterate through each item in the list. There is only one cell, so use index 0 to set the value.
                foreach (T item in list)
                {
                    var row = dataTable.NewRow();
                    row[0] = item;
                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }
            else
            {
                // TODO:
                // 1. Convert lists of type System.Object to a data table.
                // 2. Handle objects with nested objects (make the column name the name of the object and print "system.object" as the value).

                var dataTable = new DataTable(entityType.Name);
                var propertyDescriptorCollection = TypeDescriptor.GetProperties(entityType);

                // Iterate through each property in the object and add that property name as a new column in the data table.
                foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
                {
                    // Data tables cannot have nullable columns. The cells can have null values, but the actual columns themselves cannot be nullable.
                    // Therefore, if the current property type is nullable, use the underlying type (e.g. if the type is a nullable int, use int).
                    var propertyType = Nullable.GetUnderlyingType(propertyDescriptor.PropertyType) ?? propertyDescriptor.PropertyType;
                    dataTable.Columns.Add(propertyDescriptor.Name, propertyType);
                }

                // Iterate through each object in the list adn add a new row in the data table.
                // Then iterate through each property in the object and add the property's value to the current cell.
                // Once all properties in the current object have been used, add the row to the data table.
                foreach (T item in list)
                {
                    var row = dataTable.NewRow();

                    foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
                    {
                        var value = propertyDescriptor.GetValue(item);
                        row[propertyDescriptor.Name] = value ?? DBNull.Value;
                    }

                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }
        }

        /*
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            var props = typeof(T).GetProperties();
            DataTable table = new DataTable();
            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Length];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        */

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (
                    Char.IsLetterOrDigit(c)
                    || c == '.' || c == '_' || c == '-'
                    || c == ' ' || c == '$' || c == '\\'
                    || c == '/' || c == '*'
                    )
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveAccents(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            else
            {
                byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(input);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
        }

        #region Xml
        static public string SerializeToXml(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        static public T DeSerializeFromXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static void SerializeToFile(object obj, string fileName)
        {
            File.WriteAllText(fileName, SerializeToXml(obj));
        }

        public static T DeSerializeFromFile<T>(string fileName)
        {
            return DeSerializeFromXml<T>(File.ReadAllText(fileName));
        }

        #endregion Xml

        public static string GetIn<T>(IEnumerable<T> values, bool quote = false)
        {
            if (quote)
                return string.Join(",", values.Select(p => $"'{p}'"));
            else
                return string.Join(",", values);
        }

    }


}
