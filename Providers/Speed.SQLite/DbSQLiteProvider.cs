using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbSQLiteProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "@"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        public DbSQLiteProvider(Database db)
        {
            this.db = db;
        }

        ~DbSQLiteProvider()
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
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder(connectionString);
            return csb;
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return BuildConnectionString(EnumDbProviderType.SQLite, server, database, null, null, false, 0, false);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.SQLite, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new SQLiteConnectionStringBuilder();
            if (string.IsNullOrWhiteSpace(server))
                server = database;
            csb.DataSource = server;
            //csb.Database = database;
            if (!string.IsNullOrWhiteSpace(password))
                csb.Password = password;
            csb.Pooling = true;
            csb.FailIfMissing = true;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            SQLiteConnection cn = new SQLiteConnection(connectionString);
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
            return new SQLiteCommand(commandText);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new SQLiteDataAdapter(selectCommand, (SQLiteConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new SQLiteDataAdapter((SQLiteCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            return ((SQLiteCommand)cmd).Parameters.AddWithValue(parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            var par = ((SQLiteCommand)cmd).Parameters.AddWithValue(parameterName, value);
            par.Size = size;
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            SQLiteParameter par = new SQLiteParameter(parameterName, value);
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
            SQLiteParameter par = new SQLiteParameter(parameterName, System.Data.DbType.Binary);
            if (value == null || value == DBNull.Value)
                par.Size = -1;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            SQLiteParameter par = new SQLiteParameter(parameterName, System.Data.DbType.Binary);
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
            if (name == null)
                ToString();
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
            var tbCols = db.GetSchema("columns", new string[] { null, null, tableName, null });
            var rows = tbCols.Select("PRIMARY_KEY=1");
            string[] ret;
            if (rows.Length > 0)
            {
                ret = new string[rows.Length];
                for (int i = 0; i < rows.Length; i++)
                    ret[i] = (string)rows[i]["COLUMN_NAME"];
            }
            else
                ret = new string[0];
            return ret;
        }

        public string GetSqlIdentityInsert()
        {
            return "last_insert_rowid()";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            var tbCols = db.GetSchema("columns", new string[] { null, null, tableName, null });
            var rows = tbCols.Select("AUTOINCREMENT=1");
            if (rows.Length > 0)
                return (string)rows[0]["COLUMN_NAME"];
            return null;
        }

        public List<string> GetCalculatedColumns(string database, string schemaName, string tableName)
        {
            var ret = new List<string>();
            // TODO: implementar GetCalculatedColumns pro SQLite
            return new List<string>();
        }

        public List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null)
        {
            DataTable tbTables = null, tbViews = null;
            if (tableType == null)
            {
                tbTables = db.Connection.GetSchema("Tables");
                tbViews = db.Connection.GetSchema("Views");
            }
            else if (tableType.Value == EnumTableType.Table)
                tbTables = db.Connection.GetSchema("Tables");
            else // if (tableType.Value == EnumTableType.View)
                tbViews = db.Connection.GetSchema("Views");

            List<TableInfo> tables = new List<TableInfo>();

            if (tbTables != null)
            {
                foreach (DataRow row in tbTables.Rows)
                {
                    TableInfo tb = new TableInfo();
                    tb.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
                    tb.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
                    tb.TableName = (string)row["TABLE_NAME"];
                    tb.TableType = EnumTableType.Table;
                    if (tb.TableName.ToLower() != "sqlite_sequence")
                        tables.Add(tb);
                }
            }

            if (tbViews != null)
            {
                foreach (DataRow row in tbViews.Rows)
                {
                    TableInfo tb = new TableInfo();
                    tb.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
                    tb.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
                    tb.TableName = (string)row["TABLE_NAME"];
                    tb.TableType = EnumTableType.View;
                    tables.Add(tb);
                }
            }

            return tables;
        }

        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            return db.GetSchemaColumnsGeneric2(schemaName, tableName);
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
            return new SQLiteParameter(parameter.Name, parameter.Value);
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
            return new DbSQLiteProvider(db);
        }

        public TimeSpan GetTimeSpan(DbDataReader reader, int ordinal)
        {
            throw new NotImplementedException();
            //return ((SQLiteDataReader)reader).GetTimeSpan(ordinal);
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
