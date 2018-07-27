using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data.OleDb;
using System.Data;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    class DbOleDbProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "?"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        static DbOleDbProvider()
        {
            //Sys.LoadLibraryW("./OleDb.Data.dll");
        }

        public DbOleDbProvider(Database db)
        {
            this.db = db;
        }

        ~DbOleDbProvider()
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
            return BuildConnectionString(EnumDbProviderType.Access, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return CreateConnectionStringBuilder(server, database, null, null, false, 0);
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new OleDbConnectionStringBuilder();
            csb.DataSource = server;

            string file = (server ?? "").Trim();
            if (System.IO.Path.GetExtension(file).ToLower().Replace(".", "") == "mdb")
                csb.Provider = "Microsoft.Jet.OLEDB.4.0";
            else
                csb.Provider = "Microsoft.ACE.OLEDB.12.0";
            csb.DataSource = server;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            OleDbConnection cn = new OleDbConnection(connectionString);
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
            sql = sql.Remove(pos, "select".Length);
            sql = sql.Insert(pos, "select top " + count);
            return sql;
        }

        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            var tb = ((OleDbConnection)db.Connection).GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new object[] { null, null, tableName });
            string[] ret = new string[tb.Rows.Count];
            for (int i = 0; i < tb.Rows.Count; i++)
                ret[i] = (string)tb.Rows[i]["COLUMN_NAME"]; ;
            return ret;
        }

        public string GetSqlIdentityInsert()
        {
            return "@@IDENTITY";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            var tbCols = db.GetSchema("columns", new string[] { null, null, tableName, null });
            var rows = tbCols.Select("COLUMN_FLAGS=90");
            if (rows.Length > 0)
                return (string)rows[0]["COLUMN_NAME"];
            return null;
        }

        public List<string> GetCalculatedColumns(string database, string schemaName, string tableName)
        {
            var ret = new List<string>();
            // TODO: implementar GetCalculatedColumns pro access

            //var tbCols = db.GetSchema("columns", new string[] { null, null, tableName, null });
            //var rows = tbCols.Select("COLUMN_FLAGS=90");
            //foreach (var row in rows)
            //    ret.Add((string)row["COLUMN_NAME"]);
            return ret;
        }

        public List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null)
        {
            DataTable tableViews;
            if (tableType == null)
                tableViews = db.Connection.GetSchema("tables");
            else if (tableType.Value == EnumTableType.Table)
                tableViews = db.Connection.GetSchema("tables", new string[] { null, null, null, "TABLE" });
            else // if (tableType.Value == EnumTableType.View)
                tableViews = db.Connection.GetSchema("tables", new string[] { null, null, null, "VIEW" });

            List<TableInfo> tables = new List<TableInfo>();

            foreach (DataRow row in tableViews.Rows)
            {
                string type = (string)row["TABLE_TYPE"];
                if (type == "TABLE" || type == "VIEW")
                {
                    TableInfo tb = new TableInfo();
                    tb.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
                    tb.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
                    tb.TableName = (string)row["TABLE_NAME"];
                    tb.TableType = type == "TABLE" ? EnumTableType.Table : EnumTableType.View;
                    tables.Add(tb);
                }
            }
            return tables;
        }

        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            return db.GetSchemaColumnsGeneric2(schemaName, tableName);
        }

        public List<DbReferencialConstraintInfo> GetParentRelations(string schemaName, string tableName)
        {
            return db.GetParentRelationsGeneric(schemaName, tableName);
            //var tb = ((OleDbConnection)db.Connection).GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, null, null });

            //foreach (DataRow row in tb.Rows)
            //{
            //    string pkTableName = (string)row["PK_TABLE_NAME"];
            //    string fkTableName = (string)row["FK_TABLE_NAME"];
            //    string pkColumnName = (string)row["PK_COLUMN_NAME"];
            //    string fkColumnName = (string)row["FK_COLUMN_NAME"];
            //}

            //tb.ToString();
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
            return new DbOleDbProvider(db);
        }

        public TimeSpan GetTimeSpan(DbDataReader reader, int ordinal)
        {
            return ((OleDbDataReader)reader).GetTimeSpan(ordinal);
        }

        public DictionarySchemaTable<List<DbReferencialConstraintInfo>> GetReferentialContraints()
        {
            throw new NotImplementedException();
        }

    }

}
