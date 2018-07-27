using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Speed.Data
{

    public static class ExtSqlServer
    {

        public static TimeSpan GetTimeSpan(this DbDataReader reader, int ordinal)
        {
            return ((SqlDataReader)reader).GetTimeSpan(ordinal);
        }

        public static DateTimeOffset GetDateTimeOffset(this DbDataReader reader, int ordinal)
        {
            return ((SqlDataReader)reader).GetDateTimeOffset(ordinal);
        }


    }
}
