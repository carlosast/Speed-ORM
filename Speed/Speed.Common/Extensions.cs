using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class Extensions
    {

        /*
        public static void AddRange(this List<T> list, IEnumerable<T> collection)
        {
            foreach (var item in collection)
                list.Add(item);
        }
        */

        public static DateTime FirstDateOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static int LastDayOfMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        public static DateTime LastDateOfMonth(this DateTime date)
        {
            return  new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        public static int ToInt32(this DataRow row, int columnIndex)
        {
            return Conv.ToInt32(row[columnIndex]);
        }
        public static int ToInt32(this DataRow row, string columnName)
        {
            return Conv.ToInt32(row[columnName]);
        }
        public static long ToInt64(this DataRow row, int columnIndex)
        {
            return Conv.ToInt64(row[columnIndex]);
        }
        public static long ToInt64(this DataRow row, string columnName)
        {
            return Conv.ToInt64(row[columnName]);
        }
        public static string ToString(this DataRow row, int columnIndex)
        {
            return Conv.ToString(row[columnIndex]);
        }
        public static string ToString(this DataRow row, string columnName)
        {
            return Conv.ToString(row[columnName]);
        }


    }
}
