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

        static IDbProvider ProvSqlServer;
        static IDbProvider ProvPostgreSql;
        static IDbProvider ProvOracle;
        static IDbProvider ProvOleDb;
        static IDbProvider ProvOdbc;
        static IDbProvider ProvSqlServerCompact;
        static IDbProvider ProvMySql;
        static IDbProvider ProvFirebird;
        static IDbProvider ProvMariaDB;
        static IDbProvider ProvAccess;
        static IDbProvider ProvSQLite;

        public static IDbProvider GetFactory(Database db, EnumDbProviderType providerType)
        {
            IDbProvider provider = null;
            switch (providerType)
            {
                case EnumDbProviderType.SqlServer:

                    if (ProvSqlServer == null)
                        ProvSqlServer = AssemblyHelper.GetProvider("Speed.SqlServer.dll", db);
                    provider = ProvSqlServer;
                    break;

                //case EnumDbProviderType.SqlServerCe:
                //    provider = new DbSqlServerCeProvider(db);
                //    db.commandTimeout = 0;
                //    break;
                case EnumDbProviderType.MySql:
                    if (ProvMySql == null)
                        ProvMySql = AssemblyHelper.GetProvider("Speed.MySql.dll", db);
                    provider = ProvMySql;
                    break;

                case EnumDbProviderType.Firebird:
                    if (ProvFirebird == null)
                        ProvFirebird = AssemblyHelper.GetProvider("Speed.Firebird.dll", db);
                    provider = ProvFirebird;
                    break;

                case EnumDbProviderType.MariaDB:
                    if (ProvMariaDB == null)
                        ProvMariaDB = AssemblyHelper.GetProvider("Speed.MariaDB.dll", db);
                    provider = ProvMariaDB;
                    break;

                //case EnumDbProviderType.Firebird:
                //    provider = new DbFirebirdProvider(db);
                //    break;

                case EnumDbProviderType.Oracle:
                    if (ProvOracle == null)
                        ProvOracle = AssemblyHelper.GetProvider("Speed.Oracle.dll", db);
                    provider = ProvOracle;
                    break;

                case EnumDbProviderType.OleDb:
                    if (ProvOleDb == null)
                        ProvOleDb = AssemblyHelper.GetProvider("Speed.OleDb.dll", db);
                    provider = ProvOleDb;
                    break;

                case EnumDbProviderType.PostgreSQL:
                    if (ProvPostgreSql == null)
                        ProvPostgreSql = AssemblyHelper.GetProvider("Speed.ProvPostgreSql.dll", db);
                    provider = ProvOracle;
                    break;

                case EnumDbProviderType.Access:
                    if (ProvAccess == null)
                        ProvAccess = AssemblyHelper.GetProvider("Speed.Access.dll", db);
                    provider = ProvAccess;
                    break;

                case EnumDbProviderType.SQLite:
                    if (ProvSQLite == null)
                        ProvSQLite = AssemblyHelper.GetProvider("Speed.ProvSQLite.dll", db);
                    provider = ProvSQLite;
                    break;

                default:
                    throw new NotImplementedException();
            }
            return provider;
        }

        public static IDbProvider CreateProvider(Database db, EnumDbProviderType providerType)
        {
            return GetFactory(db, providerType).CreateProvider(db);
        }

    }

}
