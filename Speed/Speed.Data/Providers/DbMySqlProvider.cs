using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    class DbMySqlProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "?"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        static DbMySqlProvider()
        {
            //Sys.LoadLibraryW("./MySql.Data.dll");
        }

        public DbMySqlProvider(Database db)
        {
            this.db = db;
        }

        ~DbMySqlProvider()
        {
            db = null;
        }

        public Type DbType { get { return typeof(MySqlDbType); } }

        [ThreadStatic]
        static Dictionary<int, Enum> dbTypes;
        public Dictionary<int, Enum> DbTypes
        {
            get
            {
                if (dbTypes == null)
                    dbTypes = DbUtil.GetTypes<MySqlDbType>();
                return dbTypes;
            }
        }

        public bool SupportInformationSchema
        {
            get { return true; }
        }

        public bool SupportBatchStatements
        {
            get { return true; }
        }

        //public string CreateConnectionString(string server, string database, string userId, string password)
        //{
        //    MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder();
        //    csb.Server = server;
        //    csb.Database = database;
        //    csb.UserID = userId;
        //    csb.Password = password;
        //    return csb.ConnectionString;
        //}

        //public string CreateConnectionString(string server, string database)
        //{
        //    MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder();
        //    csb.Server = server;
        //    csb.Database = database;
        //    // csb.IntegratedSecurity = true;
        //    return csb.ConnectionString;
        //}

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
        {
            MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder(connectionString);
            return csb;
        }
        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.MySql, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder();
            csb.Server = server;
            csb.Database = database;
            return csb;
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            csb.Server = server;
            csb.Database = database;
            if (!string.IsNullOrWhiteSpace(userId))
                csb.UserID = userId;
            if (!string.IsNullOrWhiteSpace(password))
                csb.Password = password;
            if (integratedSecurity)
                csb.IntegratedSecurity = integratedSecurity;
            if (port > 0)
                csb.Port = (uint)port;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            MySqlConnection cn = new MySqlConnection(connectionString);
            cn.Open();
            //using (var cmd = cn.CreateCommand())
            //{
            //    cmd.CommandText = "set date_format = '%m-%d-%Y'";
            //    cmd.ExecuteNonQuery();
            //}
            return cn;
        }

        public DbCommand NewCommand(string commandText)
        {
            return new MySqlCommand(commandText);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new MySqlDataAdapter(selectCommand, (MySqlConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new MySqlDataAdapter((MySqlCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            return ((MySqlCommand)cmd).Parameters.AddWithValue(parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            var par = ((MySqlCommand)cmd).Parameters.AddWithValue(parameterName, value);
            par.Size = size;
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            MySqlParameter par = new MySqlParameter(parameterName, value);
            par.DbType = dbType;
            par.Value = value;
            par.Direction = direction;
            if (size > 0)
                par.Size = size;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value)
        {
            MySqlParameter par = new MySqlParameter(parameterName, MySqlDbType.Binary);
            if (value == null || value == DBNull.Value)
                par.Size = -1;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            MySqlParameter par = new MySqlParameter(parameterName, MySqlDbType.Binary);
            par.Size = size;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParRef(DbCommand cmd, string parameterName, ParameterDirection direction, object value)
        {
            throw new NotSupportedException();
        }

        public DbParameter AddParRefCursor(DbCommand cmd, string parameterName, ParameterDirection direction, object value)
        {
            throw new NotSupportedException();
        }

        public string GetObjectName(string name, bool quote = true)
        {
            if (name.Contains(" ") && name.IndexOf('`') == -1 || ReservedWords.ContainsKey(name))
                return "`" + name + "`";
            else
                return name;
        }

        public string GetObjectName(string schemaName, string name, bool quote = true)
        {
            return string.IsNullOrEmpty(schemaName) ? GetObjectName(name) : string.Format("{0}.{1}", GetObjectName(schemaName), GetObjectName(name));
        }

        public string SetTop(string sql, long count)
        {
            return sql + "\r\nLIMIT " + count;
        }

        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            string sql = null;
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                sql = string.Format(
                     "SELECT distinct column_name FROM information_schema.key_column_usage where table_schema = '{0}' and constraint_name = 'PRIMARY' and table_name = '{1}' order by ordinal_position",
                     database, tableName);
            }
            else
            {
                sql = string.Format(
                     "SELECT column_name FROM information_schema.key_column_usage where constraint_name = 'PRIMARY' and table_name = '{0}' order by ordinal_position",
                     tableName);
            }
            return db.ExecuteArray1D<string>(sql);
        }

        public string GetSqlIdentityInsert()
        {
            return "LAST_INSERT_ID()";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            string sql = string.Format(
            "select column_name from information_schema.columns where " + GetTableFilter(schemaName, tableName) + " and extra like '%auto_increment%'",
            schemaName, tableName);
            return db.ExecuteString(sql);
        }

        public List<string> GetCalculatedColumns(string database, string schemaName, string tableName)
        {
            return new List<string>();
        }

        public List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null)
        {
            //return db.GetAllTables(tableSchema ?? db.DatabaseName, tableType, true);
            bool useDatabase = true;
            tableSchema = tableSchema ?? db.DatabaseName;
            string sql;
            if (string.IsNullOrEmpty(tableSchema) && tableType == null)
            {
                sql =
@"select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, 'VIEW' TABLE_TYPE from information_schema.VIEWS 
union 
select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME,          TABLE_TYPE from information_schema.TABLES where TABLE_TYPE = 'BASE TABLE'";
            }
            else if (!string.IsNullOrEmpty(tableSchema) && tableType == null)
            {
                sql =
@"select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, 'VIEW' TABLE_TYPE from information_schema.VIEWS  where table_schema = {0}
union 
select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME,          TABLE_TYPE from information_schema.TABLES where table_schema = {0}";
                sql = string.Format(sql, Conv.ToSqlTextA(tableSchema));
            }
            else // if (!string.IsNullOrEmpty(tableSchema) && tableType != null)
            {
                if (tableType == EnumTableType.Table)
                    sql = string.Format("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME,          TABLE_TYPE from information_schema.TABLES where table_schema = {0} and TABLE_TYPE = 'BASE TABLE'", Conv.ToSqlTextA(tableSchema));
                else
                    sql = string.Format("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, 'VIEW' TABLE_TYPE from information_schema.VIEWS  where table_schema = {0}", Conv.ToSqlTextA(tableSchema));
            }

            List<TableInfo> tables = new List<TableInfo>();
            using (var dr = db.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    TableInfo tb = new TableInfo();
                    tb.TableSchema = useDatabase ? db.DatabaseName : (string)dr["TABLE_SCHEMA"];
                    tb.TableCatalog = (string)dr["TABLE_CATALOG"];
                    tb.TableName = (string)dr["TABLE_NAME"];
                    tb.TableType = (string)dr["TABLE_TYPE"] == "BASE TABLE" ? EnumTableType.Table : EnumTableType.View;
                    tables.Add(tb);
                }
            }
            tables = tables.OrderBy(p => p.TableSchema).ThenBy(p => p.TableName).ToList();
            return tables;
        }

        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            return db.GetSchemaColumnsGeneric(schemaName, tableName);
        }

        public List<DbReferencialConstraintInfo> GetParentRelations(string schemaName, string tableName)
        {
            return db.GetParentRelationsGeneric(schemaName, tableName);
        }

        public DataTable GetDataTypes()
        {
            return db.Connection.GetSchema("DataTypes");
        }

        public List<DbSequenceInfo> GetSequences()
        {
            return new List<DbSequenceInfo>();
        }

        string GetTableFilter(string schemaName, string tableName)
        {

            string sql = "";
            if (!string.IsNullOrEmpty(schemaName))
                sql += " TABLE_SCHEMA = " + Conv.ToSqlTextA(schemaName);
            if (!string.IsNullOrEmpty(sql))
                sql += " and ";
            sql += " TABLE_NAME = " + Conv.ToSqlTextA(tableName);
            return sql;

        }

        static Dictionary<string, string> reservedWords;
        public Dictionary<string, string> ReservedWords
        {
            get
            {
                if (reservedWords == null)
                    reservedWords = db.GetReservedWordsGeneric();
                return reservedWords;
            }
        }

        private static Dictionary<string, DbDataType> dataTypes;
        /// <summary>
        /// Mapeamento dos tipos do banco de dados com os do .NET
        /// </summary>
        public Dictionary<string, DbDataType> DataTypes
        {
            get
            {
                if (dataTypes == null)
                    dataTypes = db.GetDataTypesGeneric();
                return dataTypes;
            }
        }

        public IDbDataParameter Convert(Parameter parameter)
        {
            // TODO: implementar a conversão de MySlDbType para SqlDbType
            return new MySqlParameter(parameter.Name, parameter.Value);
        }

        public string GetInsertXml()
        {
            return "throw new NotSupportedException();";
        }

        public int ExecuteSequenceInt32(string sequenceName)
        {
            throw new NotSupportedException();
        }

        public long ExecuteSequenceInt64(string sequenceName)
        {
            throw new NotSupportedException();
        }

    }

}
