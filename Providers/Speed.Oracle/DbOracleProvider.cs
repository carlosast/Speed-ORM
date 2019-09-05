using System;
using System.Collections.Generic;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Data;
using Speed.Data.MetaData;
using System.IO;
using System.Reflection;
using Speed.Common;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbOracleProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return ":"; } }
        public string ParameterSymbolVar { get { return ":"; } }

        const string sqlSelectColumns =
@"
select
    null TABLE_CATALOG
  , t.tablespace_name TABLE_SCHEMA
  , t.TABLE_NAME
	, COLUMN_NAME 
  , DATA_TYPE
  , column_id ORDINAL_POSITION
  , DATA_DEFAULT COLUMN_DEFAULT
  , NULLABLE IS_NULLABLE
  ,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH
  , DATA_PRECISION NUMERIC_PRECISION
  , DATA_SCALE NUMERIC_SCALE
from
    USER_TAB_COLS c 
    inner join user_tables t on c.table_name = t.table_name
";

        const string sqlSelectColumnsXml =
@"
select
    null TABLE_CATALOG
  , t.tablespace_name TABLE_SCHEMA
  , t.TABLE_NAME
	, COLUMN_NAME 
  , DATA_TYPE
  , column_id ORDINAL_POSITION
  , DATA_DEFAULT COLUMN_DEFAULT
  , NULLABLE IS_NULLABLE
  ,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH
  , DATA_PRECISION NUMERIC_PRECISION
  , DATA_SCALE NUMERIC_SCALE
from
    USER_TAB_COLS c 
    inner join user_tables t on c.table_name = t.table_name
";

        static DbOracleProvider()
        {
            var asm = CheckDdll("Oracle.ManagedDataAccess.dll");
            //if (asm != null)
            //    AppDomain.CurrentDomain.Load(asm);
        }

        static Assembly CheckDdll(string name)
        {
            Assembly asm = null;
            var files = Directory.GetFiles(Sys.AppDirectory, name, SearchOption.AllDirectories).ToList();
            if (files.Count == 0)
                return null;

            if (files.Count == 1)
                return LoadDll(files[0]);

            var file = files.FirstOrDefault(p => Path.GetFileName(Path.GetDirectoryName(p)).Equals("x64", StringComparison.OrdinalIgnoreCase));
            if (IntPtr.Size > 4) // > 32 bits
            {
                if (file != null)
                {
                    files.Remove(file);
                    asm = LoadDll(file);
                    if (asm != null)
                        return asm;
                }
            }

            file = files.FirstOrDefault(p => Path.GetFileName(Path.GetDirectoryName(p)).Equals("x86", StringComparison.OrdinalIgnoreCase));
            if (file != null)
            {
                files.Remove(file);
                asm = LoadDll(file);
                if (asm != null)
                    return asm;
            }

            if (files.Count > 0)
                asm = LoadDll(files[0]);

            return asm;
        }

        static Assembly LoadDll(string path)
        {
            try
            {
                var ass = Assembly.LoadFile(path);
                return ass;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DbOracleProvider(Database db)
        {
            this.db = db;
        }

        ~DbOracleProvider()
        {
            db = null;
        }

        public Type DbType { get { return typeof(OracleDbType); } }

        [ThreadStatic]
        static Dictionary<int, Enum> dbTypes;
        public Dictionary<int, Enum> DbTypes
        {
            get
            {
                if (dbTypes == null)
                    dbTypes = DbUtil.GetTypes<OracleDbType>();
                return dbTypes;
            }
        }

        public bool SupportInformationSchema
        {
            get { return false; }
        }

        public bool SupportBatchStatements
        {
            get { return true; }
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
        {
            OracleConnectionStringBuilder csb = new OracleConnectionStringBuilder(connectionString);
            csb.Pooling = true;
            return csb;
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return CreateConnectionStringBuilder(server, database, null, null, false);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.Oracle, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new OracleConnectionStringBuilder();
            csb.Pooling = true;
            csb.DataSource = server;
            if (!string.IsNullOrWhiteSpace(userId))
                csb.UserID = userId;
            if (!string.IsNullOrWhiteSpace(password))
                csb.Password = password;
            if (port != 0)
                csb.Pooling = true;
            if (integratedSecurity)
                csb.ContextConnection = integratedSecurity;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            OracleConnection cn = new OracleConnection(connectionString);
            cn.Open();
            using (var cmd = cn.CreateCommand())
            {
                // cmd.CommandText = "set dateformat mdy";
                // cmd.ExecuteNonQuery();
                OracleGlobalization SessionGlob = cn.GetSessionInfo();
                // SetSessionInfo updates the Session with the new value
                SessionGlob.DateFormat = "MM/DD/YYYY";
                cn.SetSessionInfo(SessionGlob);
            }
            return cn;
        }

        public DbCommand NewCommand(string commandText)
        {
            var cmd = new OracleCommand(commandText);
            cmd.BindByName = true;
            return cmd;
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new OracleDataAdapter(selectCommand, (OracleConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new OracleDataAdapter((OracleCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, string dataType, object value)
        {
            OracleParameter par = new OracleParameter(parameterName, value);
            par.Direction = ParameterDirection.Input;

            switch (dataType)
            {
                case "CLOB":
                    par.OracleDbType = OracleDbType.Clob;
                    break;
                case "NCLOB":
                    par.OracleDbType = OracleDbType.NClob;
                    break;
            }
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            OracleParameter par = new OracleParameter(parameterName, value);
            par.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            OracleParameter par = new OracleParameter(parameterName, value);
            par.Size = size;
            par.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            OracleParameter par = new OracleParameter(parameterName, value);
            par.DbType = dbType;
            par.Value = value;
            par.Direction = direction;
            cmd.Parameters.Add(par);

            if (size > 0)
                par.Size = size;
            par.Direction = direction;
            par.DbType = dbType;
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value)
        {
            OracleParameter par = new OracleParameter(parameterName, OracleDbType.Blob);
            if (value == null || value == DBNull.Value)
                par.Size = -1;
            par.Direction = ParameterDirection.Input;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            OracleParameter par = new OracleParameter(parameterName, OracleDbType.Blob);
            par.Direction = ParameterDirection.Input;
            par.Size = size;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParRef(DbCommand cmd, string parameterName, ParameterDirection direction, object value)
        {
            var par = new OracleParameter(parameterName, OracleDbType.RefCursor, value, direction);
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParRefCursor(DbCommand cmd, string parameterName, ParameterDirection direction, object value)
        {
            var par = new OracleParameter(parameterName, OracleDbType.RefCursor, value, direction);
            cmd.Parameters.Add(par);
            return par;
        }

        public string GetObjectName(string name, bool quote = true)
        {
            name = getbjectName(name);
            if (string.IsNullOrWhiteSpace(name))
                return name;
            if (name.Contains(" ") && name.IndexOf('"') == -1 || ReservedWords.ContainsKey(name))
                return "\"" + name + "\"";
            else
                return name;
        }

        string getbjectName(string name)
        {
            name = name.RemoveChars(@"#:$/\");
            return name;
        }


        public string GetObjectName(string schemaName, string name, bool quote = true)
        {
            return string.IsNullOrEmpty(schemaName) ? GetObjectName(name) : string.Format("{0}.{1}", GetObjectName(schemaName), GetObjectName(name));
        }

        public string SetTop(string sql, long count)
        {
            sql = "select * from (" + sql + ") TB_X where rownum <= " + count;
            return sql;
        }

        // uso cache, senão fica extremamente lento
        Dictionary<string, Dictionary<string, List<string>>> pkCols;

        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            if (pkCols == null)
            {
                pkCols = new Dictionary<string, Dictionary<string, List<string>>>();

                string sql =
    @"
SELECT cols.owner, cols.table_name, cols.column_name
    FROM user_constraints cons, user_cons_columns cols
--  WHERE cols.table_name = 'ANEXO'
WHERE 
    cons.constraint_type = 'P'
    AND cons.constraint_name = cols.constraint_name
    AND cons.owner = cols.owner
ORDER BY
    cols.owner, cols.table_name, cols.position
";
                using (var reader = db.ExecuteReader(sql))
                {
                    while (reader.Read())
                    {
                        string owner = reader.GetString(0);
                        string tbName = reader.GetString(1);
                        string colName = reader.GetString(2);

                        if (!pkCols.ContainsKey(owner))
                            pkCols.Add(owner, new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase));

                        var dicTables = pkCols[owner];
                        if (!dicTables.ContainsKey(tbName))
                            dicTables.Add(tbName, new List<string>());
                        dicTables[tbName].Add(colName);
                    }
                }
            }

            string[] ret = new string[0];

            if (string.IsNullOrEmpty(schemaName))
            {
                // varre todos os schemas, e pega a primeira tabela com esse nome que existir
                foreach (var pair in pkCols)
                {
                    var dic = pair.Value;
                    if (dic.ContainsKey(tableName))
                        ret = dic[tableName].ToArray();
                }
            }
            else
            {
                if (pkCols.ContainsKey(schemaName) && pkCols[schemaName].ContainsKey(tableName))
                    ret = pkCols[schemaName][tableName].ToArray();
            }
            return ret;

            /*
            string sql =
                //@"SELECT cols.table_name, cols.column_name, cols.position, cons.status, cons.owner
@"SELECT cols.column_name
 FROM user_constraints cons, user_cons_columns cols
 WHERE cols.table_name = '{0}'
 AND cons.constraint_type = 'P'
 AND cons.constraint_name = cols.constraint_name
 AND cons.owner = cols.owner
";
            if (!string.IsNullOrEmpty(schemaName))
                sql += " AND cons.owner = '" + schemaName + "'";
            sql += " ORDER BY cols.table_name, cols.position";

            sql = string.Format(sql, db.Provider.GetObjectName(tableName));
            return db.ExecuteArray1D<string>(sql, 0);
            */
        }

        public string GetSqlIdentityInsert()
        {
            return "[SequenceValue]";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            return null;
            //string sql = string.Format(
            //    "SELECT c.name FROM syscolumns c, sysobjects o WHERE c.id = o.id AND (c.status & 128) = 128 and o.id = OBJECT_ID('{0}');",
            //    tableName);
            //if (schemaName != null)
            //    sql += ", " + db.Provider.GetObjectName(schemaName);

            //return db.ExecuteString(sql);
        }

        public List<string> GetCalculatedColumns(string database, string schemaName, string tableName)
        {
            return new List<string>();
        }

        public List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null)
        {
            // USER_TABLES
            //   TABLE_NAME | TABLESPACE_NAME | CLUSTER_NAME | IOT_NAME |  STATUS | PCT_FREE | PCT_USED | INI_TRANS | MAX_TRANS | INITIAL_EXTENT | NEXT_EXTENT | MIN_EXTENTS | MAX_EXTENTS | PCT_INCREASE | FREELISTS | FREELIST_GROUPS | LOGGING | BACKED_UP | NUM_ROWS | BLOCKS | EMPTY_BLOCKS | AVG_SPACE | CHAIN_CNT | AVG_ROW_LEN | AVG_SPACE_FREELIST_BLOCKS | NUM_FREELIST_BLOCKS |       DEGREE |    INSTANCES |   CACHE | TABLE_LOCK | SAMPLE_SIZE | LAST_ANALYZED | PARTITIONED | IOT_TYPE | TEMPORARY | SECONDARY | NESTED | BUFFER_POOL | FLASH_CACHE | CELL_FLASH_CACHE | ROW_MOVEMENT | GLOBAL_STATS | USER_STATS | DURATION | SKIP_CORRUPT | MONITORING | CLUSTER_OWNER | DEPENDENCIES | COMPRESSION | COMPRESS_FOR | DROPPED | READ_ONLY | SEGMENT_CREATED | RESULT_CACHE
            //   'CUSTOMER' |         'SPEED' |         NULL |     NULL | 'VALID' |       10 |        0 |         1 |       255 |              0 |           0 |           0 |           0 |            0 |         0 |               0 |   'YES' |       'N' |        0 |      0 |            0 |         0 |         0 |           0 |                         0 |                   0 | '         1' | '         1' | '    N' |  'ENABLED' |           0 |          NULL |        'NO' |     NULL |       'N' |       'N' |   'NO' |   'DEFAULT' |   'DEFAULT' |        'DEFAULT' |   'DISABLED' |         'NO' |       'NO' |     NULL |   'DISABLED' |      'YES' |          NULL |   'DISABLED' |  'DISABLED' |         NULL |    'NO' |      'NO' |            'NO' |    'DEFAULT'
            //       'SALE' |         'SPEED' |         NULL |     NULL | 'VALID' |       10 |        0 |         1 |       255 |              0 |           0 |           0 |           0 |            0 |         0 |               0 |   'YES' |       'N' |        0 |      0 |            0 |         0 |         0 |           0 |                         0 |                   0 | '         1' | '         1' | '    N' |  'ENABLED' |           0 |          NULL |        'NO' |     NULL |       'N' |       'N' |   'NO' |   'DEFAULT' |   'DEFAULT' |        'DEFAULT' |   'DISABLED' |         'NO' |       'NO' |     NULL |   'DISABLED' |      'YES' |          NULL |   'DISABLED' |  'DISABLED' |         NULL |    'NO' |      'NO' |            'NO' |    'DEFAULT'
            //'SALE_DETAIL' |         'SPEED' |         NULL |     NULL | 'VALID' |       10 |        0 |         1 |       255 |              0 |           0 |           0 |           0 |            0 |         0 |               0 |   'YES' |       'N' |        0 |      0 |            0 |         0 |         0 |           0 |                         0 |                   0 | '         1' | '         1' | '    N' |  'ENABLED' |           0 |          NULL |        'NO' |     NULL |       'N' |       'N' |   'NO' |   'DEFAULT' |   'DEFAULT' |        'DEFAULT' |   'DISABLED' |         'NO' |       'NO' |     NULL |   'DISABLED' |      'YES' |          NULL |   'DISABLED' |  'DISABLED' |         NULL |    'NO' |      'NO' |            'NO' |    'DEFAULT'

            //db.GetSchema("tables")
            string sql = null;
            if (tableType == null)
                sql = "SELECT TABLE_NAME, 1 IS_TABLE FROM USER_TABLES    UNION     SELECT VIEW_NAME AS TABLE_NAME, 0 IS_TABLE FROM USER_VIEWS";
            else if (tableType.Value == EnumTableType.Table)
                sql = "SELECT TABLE_NAME, 1 IS_TABLE FROM USER_TABLES";
            else // if (tableType.Value == EnumTableType.View)
                sql = "SELECT VIEW_NAME AS TABLE_NAME, 0 IS_TABLE FROM USER_VIEWS";

            List<TableInfo> tables = new List<TableInfo>();

            DataTable tb = db.ExecuteDataTable(sql);

            foreach (DataRow row in tb.Rows)
            {
                TableInfo info = new TableInfo();
                //info.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
                //info.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
                info.TableName = (string)row["TABLE_NAME"];
                info.TableType = Conv.ToBoolean(row["IS_TABLE"]) ? EnumTableType.Table : EnumTableType.View;
                tables.Add(info);
            }

            return tables;
        }

        Dictionary<string, DataTable> tbSchemaColumnsCache;

        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            string sql = null;
            DataTable tb = null;
            if (tbSchemaColumnsCache == null)
            {
                bool usGetSchema = false;

                DataTable tb2 = null;

                //var tc = new TimerCount("usGetSchema = " + usGetSchema);

                if (usGetSchema)
                {
                    try
                    {
                        var tbColsSchema = db.Connection.GetSchema("Columns", new string[] { "TRANSLOGIC" });
                        tb2 = new DataTable();
                        tb2.Columns.Add("TABLE_CATALOG", typeof(string));
                        tb2.Columns.Add("TABLE_SCHEMA", typeof(string));
                        tb2.Columns.Add("TABLE_NAME", typeof(string));
                        tb2.Columns.Add("COLUMN_NAME", typeof(string));
                        tb2.Columns.Add("DATA_TYPE", typeof(string));
                        tb2.Columns.Add("ORDINAL_POSITION", typeof(int));
                        tb2.Columns.Add("COLUMN_DEFAULT", typeof(string));
                        tb2.Columns.Add("IS_NULLABLE", typeof(string));
                        tb2.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
                        tb2.Columns.Add("NUMERIC_PRECISION", typeof(int));
                        tb2.Columns.Add("NUMERIC_SCALE", typeof(int));

                        foreach (DataRow row in tbColsSchema.Rows)
                        {
                            var nRow = tb2.NewRow();
                            nRow["TABLE_CATALOG"] = null;
                            nRow["TABLE_SCHEMA"] = row["OWNER"];
                            nRow["TABLE_NAME"] = row["TABLE_NAME"];
                            nRow["COLUMN_NAME"] = row["COLUMN_NAME"];
                            nRow["DATA_TYPE"] = row["DATATYPE"];
                            nRow["ORDINAL_POSITION"] = row["ID"].ToInt32();
                            nRow["COLUMN_DEFAULT"] = row["DATA_DEFAULT"];
                            nRow["IS_NULLABLE"] = row["NULLABLE"];
                            nRow["CHARACTER_MAXIMUM_LENGTH"] = row["LENGTHINCHARS"].ToInt32();
                            nRow["NUMERIC_PRECISION"] = row["PRECISION"].ToInt32();
                            nRow["NUMERIC_SCALE"] = row["SCALE"].ToInt32();
                            tb2.Rows.Add(nRow);
                        }
                    }
                    catch
                    {
                        tb2 = null;
                    }
                }
                else
                {
                    //sql = "select ORDINAL_POSITION, COLUMN_NAME, TABLE_CATALOG, TABLE_SCHEMA, table_name, DATA_TYPE, COLUMN_DEFAULT, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE from MD_USER_TAB_COLS";
                    sql = sqlSelectColumns;
                    try
                    {
                        tb2 = db.ExecuteDataTable(sql, 0);
                    }
                    catch
                    {
                        tb2 = null;
                    }

                    if (tb2 == null)
                    {
                        // sql = "select column_id ORDINAL_POSITION, COLUMN_NAME, null TABLE_CATALOG, t.tablespace_name TABLE_SCHEMA, t.table_name, DATA_DEFAULT COLUMN_DEFAULT, NULLABLE IS_NULLABLE,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH, DATA_PRECISION NUMERIC_PRECISION, DATA_SCALE NUMERIC_SCALE from USER_TAB_COLS c inner join user_tables t on c.table_name = t.table_name";
                        // retirei o DATA_DEFAULT pq fica extremamente lenta a query
                        sql = "select column_id ORDINAL_POSITION, COLUMN_NAME, null TABLE_CATALOG, t.tablespace_name TABLE_SCHEMA, t.table_name, DATA_TYPE, DATA_DEFAULT COLUMN_DEFAULT, NULLABLE IS_NULLABLE,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH, DATA_PRECISION NUMERIC_PRECISION, DATA_SCALE NUMERIC_SCALE from USER_TAB_COLS c inner join user_tables t on c.table_name = t.table_name";

                        tb2 = db.ExecuteDataTable(sql, 0);
                    }
                }

                //var time = tc.ToString();
                //MessageBox.Show(time);
                //throw new Exception(time);

                var keys = new List<string>();
                foreach (DataRow row in tb2.Rows)
                {
                    string key = $"{row["TABLE_CATALOG"].Trim()}.{row["TABLE_NAME"]}";
                    keys.Add(key);
                }
                keys = keys.Distinct().ToList();

                tbSchemaColumnsCache = new Dictionary<string, DataTable>();
                foreach (var key in keys)
                    tbSchemaColumnsCache.Add(key, tb2.Clone());

                while (tb2.Rows.Count > 0)
                {
                    var index = tb2.Rows.Count - 1;
                    var r = tb2.Rows[index];
                    string key = $"{r["TABLE_CATALOG"].Trim()}.{r["TABLE_NAME"]}";
                    tbSchemaColumnsCache[key].Rows.Add(r.ItemArray);
                    tb2.Rows.RemoveAt(index);
                }
            }

            string k = $"{schemaName}.{tableName}";
            tb = tbSchemaColumnsCache.GetValue(k);

            if (tb == null)
            {
                sql = "select column_id ORDINAL_POSITION, TABLE_NAME, COLUMN_NAME, null TABLE_CATALOG, null TABLE_SCHEMA, DATA_TYPE, DATA_DEFAULT COLUMN_DEFAULT, NULLABLE IS_NULLABLE,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH, DATA_PRECISION NUMERIC_PRECISION, DATA_SCALE NUMERIC_SCALE from USER_tab_COLS c inner join user_views t on c.table_name = t.view_name WHERE t.view_name =" + Conv.ToSqlTextA(tableName);
                if (!string.IsNullOrEmpty(schemaName))
                    sql += " AND TABLE_CATALOG =" + Conv.ToSqlTextA(schemaName);
                tb = db.ExecuteDataTable("select column_id ORDINAL_POSITION, TABLE_NAME, COLUMN_NAME, null TABLE_CATALOG, null TABLE_SCHEMA, DATA_TYPE, DATA_DEFAULT COLUMN_DEFAULT, NULLABLE IS_NULLABLE,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH, DATA_PRECISION NUMERIC_PRECISION, DATA_SCALE NUMERIC_SCALE from USER_tab_COLS c inner join user_views t on c.table_name = t.view_name WHERE t.view_name =" + Conv.ToSqlTextA(tableName));
            }
            return tb;
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
            var seqs = db.ExecuteArray1D<string>("SELECT SEQUENCE_NAME FROM USER_SEQUENCES").ToList();
            var ret = new List<DbSequenceInfo>();
            seqs.ForEach(p => ret.Add(new DbSequenceInfo { SequenceName = p, SchemaName = null }));
            return ret;
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
            // TODO: implementar a conversão de DbType para OracleDbType
            return new OracleParameter(parameter.Name, parameter.Value);
        }

        public string GetInsertXml()
        {
            return "throw new NotSupportedException();";
        }

        public int ExecuteSequenceInt32(string sequenceName)
        {
            return db.ExecuteInt32("SELECT NEXT VALUE FOR {0}", sequenceName);
        }

        public long ExecuteSequenceInt64(string sequenceName)
        {
            return db.ExecuteInt64("SELECT NEXT VALUE FOR {0}", sequenceName);
        }

        public string GetCurrentSchema()
        {
            return db.ExecuteString("select user from dual;");
        }

        public IDbProvider CreateProvider(Database db)
        {
            return new DbOracleProvider(db);
        }

        public DataTable GetSchemaColumns(string commandText)
        {
            throw new NotImplementedException();
        }

        public bool AddUsings(DbColumnInfo col, Dictionary<string, string> usings)
        {
            return false;
        }
        public string GetLastModified()
        {
            string sql =
@"
select to_char(max(last_ddl_time), 'DDMMYYHH:MM:SS') last_ddl_time from user_objects where object_type in ('TABLE', 'VIEW')";
            return db.ExecuteString(sql);
        }

    }

}
