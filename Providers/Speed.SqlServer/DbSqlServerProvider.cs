using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using Speed.Data;
using System.Data.SqlClient;
using Speed.Data.MetaData;
using System.Linq;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbSqlServerProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "@"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        public DbSqlServerProvider(Database db)
        {
            this.db = db;
        }

        ~DbSqlServerProvider()
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
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            return csb;
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return BuildConnectionString(EnumDbProviderType.SqlServer, server, database, null, null, false, 0, false);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.SqlServer, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new SqlConnectionStringBuilder();
            csb.DataSource = server;
            csb.InitialCatalog = database;
            if (!integratedSecurity)
            {
                if (!string.IsNullOrWhiteSpace(userId))
                    csb.UserID = userId;
                if (!string.IsNullOrWhiteSpace(password))
                    csb.Password = password;
            }
            if (integratedSecurity)
                csb.IntegratedSecurity = integratedSecurity;

            // Speed usa isto
            csb.MultipleActiveResultSets = true;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            cn.Open();
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "set dateformat mdy";
                cmd.ExecuteNonQuery();
            }
            return cn;
        }

        public DbCommand NewCommand(string commandText)
        {
            return new SqlCommand(commandText);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new SqlDataAdapter(selectCommand, (SqlConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new SqlDataAdapter((SqlCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, string dataType, object value)
        {
            return ((SqlCommand)cmd).Parameters.AddWithValue(parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            var par = ((SqlCommand)cmd).Parameters.AddWithValue(parameterName, value);
            par.Size = size;
            return par;
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            var par = ((SqlCommand)cmd).Parameters.AddWithValue(parameterName, value);
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            SqlParameter par = new SqlParameter(parameterName, value);
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
            SqlParameter par = new SqlParameter(parameterName, SqlDbType.Binary);
            if (value == null || value == DBNull.Value)
                par.Size = -1;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            SqlParameter par = new SqlParameter(parameterName, SqlDbType.Binary);
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
            if (string.IsNullOrEmpty(name))
                return name;
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

        /*
        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            string sql = string.Format("sp_pkeys {0}", db.Provider.GetObjectName(tableName));
            if (schemaName != null && Conv.HasData(db.Provider.GetObjectName(schemaName)))
                sql += ", " + db.Provider.GetObjectName(schemaName);

            return db.ExecuteArray1DOfRows<string>(sql, 3);
        }
        */
        DictionarySchemaTable<List<string>> bufferPKs;
        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            // ganho fantástico de performance
            if (bufferPKs == null)
            {
                string sql =
@"select s.name as TABLE_SCHEMA, t.name as TABLE_NAME
     , c.name as COLUMN_NAME, ic.key_ordinal AS ORDINAL_POSITION
  from sys.key_constraints as k
  join sys.tables as t
    on t.object_id = k.parent_object_id
  join sys.schemas as s
    on s.schema_id = t.schema_id
  join sys.index_columns as ic
    on ic.object_id = t.object_id
   and ic.index_id = k.unique_index_id
  join sys.columns as c
    on c.object_id = t.object_id
   and c.column_id = ic.column_id
   where k.type_desc = 'PRIMARY_KEY_CONSTRAINT'
 order by TABLE_SCHEMA, TABLE_NAME, k.type_desc, ORDINAL_POSITION;
";
                bufferPKs = new DictionarySchemaTable<List<string>>(StringComparer.InvariantCultureIgnoreCase);
                string lastName = null;
                using (var dr = db.ExecuteReader(sql))
                {
                    while (dr.Read())
                    {
                        string tbName = GetObjectName(dr.GetString(0), dr.GetString(1));
                        string columnName = dr.GetString(2);
                        if (tbName != lastName)
                            bufferPKs.Add(tbName, new List<string>());
                        bufferPKs[tbName].Add(columnName);
                        lastName = tbName;
                    }
                }
            }

            var pk = bufferPKs.Find(db, schemaName, tableName);
            if (pk != null)
                return pk.ToArray();
            else
                return null;
        }

        public string GetSqlIdentityInsert()
        {
            return "SCOPE_IDENTITY()";
        }

        public string GetIdentityColumn1(string database, string schemaName, string tableName)
        {
            string sql = string.Format(
            "SELECT c.name FROM syscolumns c, sysobjects o WHERE c.id = o.id AND (c.status & 128) = 128 and o.id = OBJECT_ID('{0}')",
            tableName, !string.IsNullOrEmpty(schemaName) ? "schema_name(o.uid) = '" + db.Provider.GetObjectName(schemaName) + "'" : "");
            return db.ExecuteString(sql);
        }

        Dictionary<string, string> bufferIdentity;
        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            string schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;
            string key = $"{schema}.{tableName}";

            if (bufferIdentity == null)
            {
                string sql = string.Format(
                "SELECT SCHEMA_NAME(o.uid) SchemaName, o.name, c.name FROM syscolumns c, sysobjects o WHERE c.id = o.id AND (c.status & 128) = 128;",
                //tableName, !string.IsNullOrEmpty(schemaName) ? "schema_name(o.uid) = '" + db.Provider.GetObjectName(schemaName) : "");

                bufferIdentity = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase));
                using (var dr = db.ExecuteReader(sql))
                {
                    while (dr.Read())
                    {
                        try
                        {
                            bufferIdentity.Add($"{dr.GetString(0)}.{dr.GetString(1)}", dr.GetString(2));
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                }
            }
            if (bufferIdentity.ContainsKey(key))
                return bufferIdentity[key];
            else
                return null;
        }

        Dictionary<string, List<string>> bufferCalculatedColumns;
        public List<string> GetCalculatedColumns(string database, string schemaName, string tableName)
        {
            if (bufferCalculatedColumns == null)
            {
                string sql = string.Format(
                "select SCHEMA_NAME(o.uid) 'SchemaName', object_name(c.id) 'TableName', c.name 'ColumnName' from syscolumns c inner join sysobjects o on c.id = o.id where iscomputed = 1",
                //tableName, !string.IsNullOrEmpty(schemaName) ? "schema_name(o.uid) = '" + db.Provider.GetObjectName(schemaName) : "");

                bufferCalculatedColumns = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase));
                using (var dr = db.ExecuteReader(sql))
                {
                    while (dr.Read())
                    {
                        string _schemaName = dr.GetString(0);
                        string _tableName = dr.GetString(1);
                        string _columnName = dr.GetString(2);
                        string name = GetObjectName(_schemaName, _tableName);
                        if (!bufferCalculatedColumns.ContainsKey(name))
                            bufferCalculatedColumns.Add(name, new List<string>());
                        bufferCalculatedColumns[name].Add(_columnName);
                    }
                }
            }

            string key = GetObjectName(schemaName, tableName);

            if (bufferCalculatedColumns.ContainsKey(key))
                return bufferCalculatedColumns[key];
            else
                return new List<string>();
        }

        public List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null)
        {
            //var tables = db.GetAllTables(tableSchema, tableType);
            string sql;

            List<TableInfo> tables = new List<TableInfo>();

            try
            {
                sql = "select * from (select DB_NAME() table_catalog, schema_name(schema_id) schema_name, name table_name, type table_type, modify_date from sys.objects where type in ('U', 'V')) T ";
                var w = new List<string>();
                if (string.IsNullOrEmpty(tableSchema))
                    w.Add("schema_name = '" + tableSchema);
                if (tableType == EnumTableType.Table)
                    w.Add("table_type = 'U'");
                else if (tableType == EnumTableType.Table)
                    w.Add("table_type = 'V'");

                if (w.Any())
                {
                    sql += string.Join(" AND ", w);
                }

                using (var dr = db.ExecuteReader(sql))
                {
                    while (dr.Read())
                    {
                        TableInfo tb = new TableInfo();
                        tb.TableCatalog = dr.GetString(0) as string;
                        tb.TableSchema = dr.GetString(1) as string;
                        tb.TableName = dr.GetString(2);
                        tb.TableType = dr.GetString(3) == "U" ? EnumTableType.Table : EnumTableType.View;
                        if (!dr.IsDBNull(4))
                            tb.DateModified = dr.GetDateTime(4);
                        if (tb.TableName.ToLower() != "sysdiagrams")
                            tables.Add(tb);
                    }
                }
            }
            catch
            {
                //var tables = db.GetAllTables(tableSchema, tableType);
                if (string.IsNullOrEmpty(tableSchema) && tableType == null)
                    sql = "select * from information_schema.tables order by table_name;";
                else if (!string.IsNullOrEmpty(tableSchema) && tableType == null)
                    sql = "select * from information_schema.tables where table_schema = '" + tableSchema + "' order by table_name;";
                else if (string.IsNullOrEmpty(tableSchema) && tableType != null)
                    sql = "select * from information_schema.tables where table_type = '" + (tableType.Value == EnumTableType.Table ? "BASE TABLE" : "") + "' order by table_name;";
                else // if (!string.IsNullOrEmpty(tableSchema) && tableType != null)
                    sql = "select * from information_schema.tables where table_schema = '" + tableSchema + "' and table_type = '" + (tableType.Value == EnumTableType.Table ? "BASE TABLE" : "") + "' order by table_name;";

                using (var dr = db.ExecuteReader(sql))
                {
                    while (dr.Read())
                    {
                        TableInfo tb = new TableInfo();
                        tb.TableSchema = (string)dr["TABLE_SCHEMA"];
                        tb.TableCatalog = (string)dr["TABLE_CATALOG"];
                        tb.TableName = (string)dr["TABLE_NAME"];
                        tb.TableType = (string)dr["TABLE_TYPE"] == "BASE TABLE" ? EnumTableType.Table : EnumTableType.View;
                        if (tb.TableName.ToLower() != "sysdiagrams")
                            tables.Add(tb);
                    }
                }
            }

            return tables;
        }

        Dictionary<string, DataTable> bufferSchemaColumns;
        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            return db.GetSchemaColumnsGeneric(schemaName, tableName);
        }

        public List<DbReferencialConstraintInfo> GetParentRelations(string schemaName, string tableName)
        {
            return null;
            //return db.GetParentRelationsGeneric(schemaName, tableName);
        }

        public DataTable GetDataTypes()
        {
            return db.Connection.GetSchema("DataTypes");
        }

        public List<DbSequenceInfo> GetSequences()
        {
            try
            {
                using (var r = db.ExecuteReader("SELECT schema_name(schema_id) schema_name, name FROM SYS.SEQUENCES"))
                {
                    List<DbSequenceInfo> list = new List<DbSequenceInfo>();
                    while (r.Read())
                    {
                        DbSequenceInfo rec = new DbSequenceInfo();
                        rec.SchemaName = r.GetString(0);
                        rec.SequenceName = r.GetString(1);
                        list.Add(rec);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                return new List<DbSequenceInfo>();
            }
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
            var par = new SqlParameter(parameter.Name, parameter.Value);
            if (parameter.Size.HasValue)
                par.Size = parameter.Size.Value;
            par.Direction = parameter.Direction;
            par.DbType = parameter.DbType;
            return par;
        }

        public string GetInsertXml()
        {
            return
@"
        List<[TypeName]> values = (List<[TypeName]>)instance;
        
        // create DataTable
        DataTable tb = new DataTable('Table');
[InsertDataTableAddColumns]
        foreach (var value in values)
        {
            DataRow row = tb.NewRow();
[InsertDataTableSetColumns]
            tb.Rows.Add(row);
        }

string sql = 
@'
declare @xmlData xml = @Xml
BEGIN
    DECLARE @idInt int;

    EXEC sp_xml_preparedocument @idInt OUTPUT, @xmlData;

    INSERT INTO [TableName]([InsertColumns])
    SELECT [InsertColumns]
    FROM OPENXML (@idInt, [DocumentElement])
    WITH 
    (
        [InsertDataTableWithColumns]
    )

    select CAST( SCOPE_IDENTITY() - @@ROWCOUNT + 1 AS INT);
    exec sp_xml_removedocument @idInt;
END;';

        using (System.IO.StringWriter swStringWriter = new System.IO.StringWriter())
        {
            // Datatable as XML format 
            tb.WriteXml(swStringWriter);
            // Datatable as XML string 
            string xml = swStringWriter.ToString();
            using (var cmd = db.NewCommand(sql, 0))
            {
                db.AddParameter(cmd, '@xml', DbType.Xml, xml, ParameterDirection.Input);
                int firstId = Convert.ToInt32(cmd.ExecuteScalar());
                return firstId;
            }
        }
";
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
            return new DbSqlServerProvider(db);
        }

        public bool AddUsings(DbColumnInfo col, Dictionary<string, string> usings)
        {
            return false;
        }

        public string GetLastModified()
        {
            string sql =
@"
select CONVERT(NVARCHAR(20), MAX(modify_date), 20) From sys.objects where type in ('U', 'V')
";
            return db.ExecuteString(sql, 300);
        }

        public TimeSpan GetTimeSpan(DbDataReader reader, int ordinal)
        {
            return ((SqlDataReader)reader).GetTimeSpan(ordinal);
        }

    }

}
