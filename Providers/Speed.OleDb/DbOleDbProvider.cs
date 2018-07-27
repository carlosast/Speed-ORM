using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.Common;
using System.Data;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class OleDbProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "@"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        public OleDbProvider(Database db)
        {
            this.db = db;
        }

        ~OleDbProvider()
        {
            db = null;
        }

        public Type DbType { get { return typeof(OleDbType); } }

        [ThreadStatic]
        static Dictionary<int, Enum> dbTypes;
        public Dictionary<int, Enum> DbTypes
        {
            get
            {
                if (dbTypes == null)
                    dbTypes = DbUtil.GetTypes<OleDbType>();
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
            OleDbConnectionStringBuilder csb = new OleDbConnectionStringBuilder(connectionString);
            return csb;
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.OleDb, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return BuildConnectionString(EnumDbProviderType.OleDb, server, database, null, null, false, 0, false);
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            OleDbConnectionStringBuilder csb = new OleDbConnectionStringBuilder();
            csb.DataSource = server;
            csb.FileName = database;
            csb.Provider = provider;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            OleDbConnection cn = new OleDbConnection(connectionString);
            cn.Open();
            return cn;
        }

        public DbCommand NewCommand(string commandText)
        {
            return new OleDbCommand(commandText);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new OleDbDataAdapter(selectCommand, (OleDbConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new OleDbDataAdapter((OleDbCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            return ((OleDbCommand)cmd).Parameters.AddWithValue(parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            var par = ((OleDbCommand)cmd).Parameters.AddWithValue(parameterName, value);
            par.Size = size;
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            OleDbParameter par = new OleDbParameter(parameterName, value);
            par.DbType = dbType;
            par.Direction = direction;
            par.Value = value;
            if (size > 0)
                par.Size = size;
            par.Direction = direction;
            par.DbType = dbType;
            cmd.Parameters.Add(par);
            return par;
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value)
        {
            OleDbParameter par = new OleDbParameter(parameterName, SqlDbType.Binary);
            if (value == null || value == DBNull.Value)
                par.Size = -1;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            OleDbParameter par = new OleDbParameter(parameterName, SqlDbType.Binary);
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
                sql = sql.Insert(pos, "select top " + count);
                return sql;
            }
            else
            {
                return "set ROWCOUNT " + count + "\r\n" + sql + "\r\nset ROWCOUNT 0";
            }
        }

        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            string sql = string.Format("sp_pkeys {0}", db.Provider.GetObjectName(tableName));
            if (schemaName != null && Conv.HasData(db.Provider.GetObjectName(schemaName)))
                sql += ", " + db.Provider.GetObjectName(schemaName);

            return db.ExecuteArray1D<string>(sql, 3);
        }

        public string GetSqlIdentityInsert()
        {
            return "SCOPE_IDENTITY()";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            string sql = string.Format(
                "SELECT c.name FROM syscolumns c, sysobjects o WHERE c.id = o.id AND (c.status & 128) = 128 and o.id = OBJECT_ID('{0}');",
                tableName);
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
            // TODO: implementar a conversão de OleDbType para SqlDbType
            return new OleDbParameter(parameter.Name, parameter.Value);
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

        public IDbProvider CreateProvider(Database db)
        {
            return new OleDbProvider(db);
        }

        public TimeSpan GetTimeSpan(DbDataReader reader, int ordinal)
        {
            throw new NotImplementedException();
            //return ((OleDbProvider)reader).GetTimeSpan(ordinal);
        }

        public DictionarySchemaTable<List<DbReferencialConstraintInfo>> GetReferentialContraints()
        {
            throw new NotImplementedException();
        }

        public List<DbReferencialConstraintInfo> GetParentRelations(string schemaName, string tableName)
        {
            return db.GetParentRelationsGeneric(schemaName, tableName);
        }
    }

}
