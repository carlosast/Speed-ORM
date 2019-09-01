using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Data;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    class DbSqlServerCeProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "@"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        public DbSqlServerCeProvider(Database db)
        {
            this.db = db;
        }

        ~DbSqlServerCeProvider()
        {
            db = null;
        }

        public Type DbType { get { return typeof(SqlDbType); } }

        [ThreadStatic]
        static Dictionary<int, Enum> dbTypes;
        public Dictionary<int, Enum> DbTypes
        {
            get
            {
                if (dbTypes == null)
                    dbTypes = DbUtil.GetTypes<SqlDbType>();
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

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
        {
            return new SqlCeConnectionStringBuilder(connectionString);
        }
        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return BuildConnectionString(EnumDbProviderType.SqlServerCe, server, database, null, null, false, 0, false);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.SqlServerCe, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new SqlCeConnectionStringBuilder();
            //string cs = string.Format(@"Data Source={0}; Password={1}; Persist Security Info=False",
            //    database, password);
            csb.DataSource = server;
            if (string.IsNullOrWhiteSpace(server))
                server = database;
            if (!string.IsNullOrWhiteSpace(password))
                csb.Password = password;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            SqlCeConnection cn = new SqlCeConnection(connectionString);
            cn.Open();
            //using (var cmd = cn.CreateCommand())
            //{
            //    cmd.CommandText = "set dateformat mdy";
            //    cmd.ExecuteNonQuery();
            //}
            return cn;
        }

        public DbCommand NewCommand(string commandText)
        {
            return new SqlCeCommand(commandText);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new SqlCeDataAdapter(selectCommand, (SqlCeConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new SqlCeDataAdapter((SqlCeCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            return ((SqlCeCommand)cmd).Parameters.AddWithValue(parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            var par = ((SqlCeCommand)cmd).Parameters.AddWithValue(parameterName, value);
            par.Size = size;
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            SqlCeParameter par = new SqlCeParameter(parameterName, value);
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
            SqlCeParameter par = new SqlCeParameter(parameterName, SqlDbType.Binary);
            if (value == null || value == DBNull.Value)
                par.Size = -1;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            SqlCeParameter par = new SqlCeParameter(parameterName, SqlDbType.Binary);
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
            if (name.Contains(" ") && name.IndexOf('[') == -1 || ReservedWords.ContainsKey(name))
                return "[" + name + "]";
            else
                return name;
        }

        public string GetObjectName(string schemaName, string name, bool quote = true)
        {
            return string.IsNullOrEmpty(schemaName) ? GetObjectName(name) : string.Format("{0}.{1}", GetObjectName(schemaName), GetObjectName(name));
        }

        public string SetTop(string sql, long count)
        {
            var pos = sql.IndexOf("select", StringComparison.InvariantCultureIgnoreCase);
            if (pos > -1)
            {
                sql = sql.Remove(pos, "select".Length);
                sql = sql.Insert(pos, "select top(" + count + ")");
                return sql;
            }
            else
            {
                return "set ROWCOUNT " + count + "\r\n" + sql + "\r\nset ROWCOUNT 0";
            }
        }

        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            //string sql = string.Format("sp_pkeys {0}", db.Provider.GetObjectName(tableName));
            //if (schemaName != null && Conv.HasData(db.Provider.GetObjectName(schemaName)))
            //    sql += ", " + db.Provider.GetObjectName(schemaName);
            string sql = "select COLUMN_NAME from INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME =  " + Conv.ToSqlTextA(tableName) +
                "ORDER BY ORDINAL_POSITION";

            return db.ExecuteArray1D<string>(sql);
        }

        public string GetSqlIdentityInsert()
        {
            return "SCOPE_IDENTITY()";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            string sql = string.Format(
                //"SELECT c.name FROM syscolumns c, sysobjects o WHERE c.id = o.id AND (c.status & 128) = 128 and o.id = OBJECT_ID('{0}');",
                "select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME =  '{0}' AND NOT AUTOINC_INCREMENT IS NULL",
                tableName);
            //string sql = "select * from INFORMATION_SCHEMA.PROVIDER_TYPES WHERE TABLE_NAME";

            if (schemaName != null)
                sql += ", " + db.Provider.GetObjectName(schemaName);

            return db.ExecuteString(sql);
        }

        public List<string> GetCalculatedColumns(string database, string schemaName, string tableName)
        {
            return new List<string>();
        }

        public List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null)
        {
            return db.GetAllTables(tableSchema, tableType);
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
            // TODO: implementar a conversão de DbType para SqlDbType
            return new System.Data.SqlServerCe.SqlCeParameter(parameter.Name, parameter.Value);
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
