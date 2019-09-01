using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class DataExtensions
    {

        public static uint GetUInt32(this DbDataReader r, int ordinal)
        {
            return (uint)r.GetInt32(ordinal);
        }
        public static uint GetUInt32(this SqlDataReader r, int ordinal)
        {
            return (uint)r.GetInt32(ordinal);
        }
        public static uint GetUInt32(this MySqlDataReader r, int ordinal)
        {
            return (uint)r.GetInt32(ordinal);
        }

    }

}
