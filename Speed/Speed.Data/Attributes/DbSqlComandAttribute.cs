using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbSqlCommandAttribute : Attribute
    {

        public string Sql { get; set; }

        public DbSqlCommandAttribute(string sql)
        {
            this.Sql = sql;
        }

    }

}
