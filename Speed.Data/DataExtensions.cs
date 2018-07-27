using System;
using System.Data.Common;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class DataExtensions
    {

        public static UInt64 GetUInt64(this DbDataReader r, int ordinal)
        {
            return (UInt64)r.GetInt64(ordinal);
        }
        //public static UInt64 GetUInt64(this SqlDataReader r, int ordinal)
        //{
        //    return (UInt64)r.GetInt64(ordinal);
        //}
        //public static UInt64 GetUInt64(this MySqlDataReader r, int ordinal)
        //{
        //    return (UInt64)r.GetInt64(ordinal);
        //}

        public static uint GetUInt32(this DbDataReader r, int ordinal)
        {
            return (uint)r.GetInt32(ordinal);
        }
        //public static uint GetUInt32(this SqlDataReader r, int ordinal)
        //{
        //    return (uint)r.GetInt32(ordinal);
        //}
        //public static uint GetUInt32(this MySqlDataReader r, int ordinal)
        //{
        //    return (uint)r.GetInt32(ordinal);
        //}

        public static UInt16 GetUInt16(this DbDataReader r, int ordinal)
        {
            return (UInt16)r.GetInt16(ordinal);
        }
        //public static UInt16 GetUInt16(this SqlDataReader r, int ordinal)
        //{
        //    return (UInt16)r.GetInt16(ordinal);
        //}
        //public static UInt16 GetUInt16(this MySqlDataReader r, int ordinal)
        //{
        //    return (UInt16)r.GetInt16(ordinal);
        //}

    }

}
