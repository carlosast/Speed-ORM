using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data.Common;
using System.Data;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    class DbOracleProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return ""; } }
        public string ParameterSymbolVar { get { return ":"; } }

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
            var par = new OracleParameter(parameterName, OracleDbType.Ref, value, direction);
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
            if (string.IsNullOrWhiteSpace(name))
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
            sql = "select * from (" + sql + ") TB_X where rownum <= " + count;
            return sql;
        }

        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            string sql =
                //@"SELECT cols.table_name, cols.column_name, cols.position, cons.status, cons.owner
@"SELECT cols.column_name
 FROM all_constraints cons, all_cons_columns cols
 WHERE cols.table_name = '{0}'
 AND cons.constraint_type = 'P'
 AND cons.constraint_name = cols.constraint_name
 AND cons.owner = cols.owner
 ORDER BY cols.table_name, cols.position";

            sql = string.Format(sql, db.Provider.GetObjectName(tableName));

            return db.ExecuteArray1D<string>(sql, 0);
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

        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            var tb = db.ExecuteDataTable("select column_id ORDINAL_POSITION, null TABLE_CATALOG, t.tablespace_name TABLE_SCHEMA, DATA_DEFAULT COLUMN_DEFAULT, NULLABLE IS_NULLABLE,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH, DATA_PRECISION NUMERIC_PRECISION, DATA_SCALE NUMERIC_SCALE, c.* from USER_TAB_COLS c inner join user_tables t on c.table_name = t.table_name WHERE t.TABLE_NAME = " + Conv.ToSqlTextA(tableName));
            if (tb.Rows.Count == 0)
                tb = db.ExecuteDataTable("select column_id ORDINAL_POSITION, null TABLE_CATALOG, null TABLE_SCHEMA, DATA_DEFAULT COLUMN_DEFAULT, NULLABLE IS_NULLABLE,CHAR_COL_DECL_LENGTH CHARACTER_MAXIMUM_LENGTH, DATA_PRECISION NUMERIC_PRECISION, DATA_SCALE NUMERIC_SCALE, c.* from USER_tab_COLS c inner join user_views t on c.table_name = t.view_name WHERE t.view_name =" + Conv.ToSqlTextA(tableName));
            return tb;
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

    }

}
