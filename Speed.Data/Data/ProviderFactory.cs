using Speed.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Speed.Data
{

    public static class ProviderFactory
    {

        static IDbProvider SqlServer;
        static IDbProvider Oracle;
        static IDbProvider OleDb;
        static IDbProvider Odbc;
        static IDbProvider SqlServerCompact;
        static IDbProvider MySql;
        static IDbProvider Access;
        static IDbProvider Postgres;

        public static IDbProvider GetFactory(Database db, EnumDbProviderType providerType)
        {
            IDbProvider provider = null;
            switch (providerType)
            {
                case EnumDbProviderType.SqlServer:
                    if (SqlServer == null)
                        SqlServer = AssemblyHelper.GetProvider("Speed.SqlServer.dll", db);
                    provider = SqlServer;
                    break;

                case EnumDbProviderType.Oracle:
                    if (Oracle == null)
                        Oracle = AssemblyHelper.GetProvider("Speed.Oracle.dll", db);
                    provider = Oracle;
                    break;

                case EnumDbProviderType.Access:
                    if (Access == null)
                        Access = AssemblyHelper.GetProvider("Speed.Access.dll", db);
                    provider = Access;
                    break;

                case EnumDbProviderType.MySql:
                    if (MySql == null)
                        MySql = AssemblyHelper.GetProvider("Speed.MySql.dll", db);
                    provider = MySql;
                    break;

                case EnumDbProviderType.PostgreSQL:
                    if (Postgres == null)
                        Postgres = AssemblyHelper.GetProvider("Speed.PostgreSQL.dll", db);
                    provider = Postgres;
                    break;

                    //case EnumDbProviderType.SqlServerCe:
                    //    provider = new DbSqlServerCeProvider(db);
                    //    db.commandTimeout = 0;
                    //    break;
                    //case EnumDbProviderType.MySql:
                    //    provider = new DbMySqlProvider(db);
                    //    break;
                    //case EnumDbProviderType.MariaDB:
                    //    provider = new DbMariaDBProvider(db);
                    //    break;
                    //case EnumDbProviderType.Firebird:
                    //    provider = new DbFirebirdProvider(db);
                    //    break;
                    //case EnumDbProviderType.OleDb:
                    //    provider = new DbOleDbProvider(db);
                    //    break;
                    //case EnumDbProviderType.PostgreSQL:
                    //    provider = new DbPostgreSqlProvider(db);
                    //    break;
                    //case EnumDbProviderType.Access:
                    //    provider = new DbAccessProvider(db);
                    //    break;
                    //case EnumDbProviderType.SQLite:
                    //    provider = new DbSQLiteProvider(db);
                    //    break;
            }
            return provider;
        }

        public static IDbProvider CreateProvider(Database db, EnumDbProviderType providerType)
        {
            return GetFactory(db, providerType).CreateProvider(db);
        }

    }

}
