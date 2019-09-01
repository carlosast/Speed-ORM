using System;
using System.Collections.Generic;
// using System.Linq;
using System.Text;
using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;
using System.Data;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    class DbFirebirdProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "@"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        static DbFirebirdProvider()
        {
            ////var ret = Sys.LoadLibraryW("./x86/fbembed.dll");
            Sys.CheckFiles(
                "fbembed.dll",
                "firebird.msg",
                "ib_util.dll",
                "icudt30.dll",
                "icuin30.dll",
                "icuuc30.dll",
                "IDPLicense.txt",
                "IPLicense.txt",
                "Microsoft.VC80.CRT.manifest",
                "msvcp80.dll",
                "msvcr80.dll"
                );
        }

        public DbFirebirdProvider(Database db)
        {
            this.db = db;
        }

        ~DbFirebirdProvider()
        {
            db = null;
        }

        public Type DbType { get { return typeof(FbDbType); } }

        [ThreadStatic]
        static Dictionary<int, Enum> dbTypes;
        public Dictionary<int, Enum> DbTypes
        {
            get
            {
                if (dbTypes == null)
                    dbTypes = DbUtil.GetTypes<FbDbType>();
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

        //public string CreateConnectionString(string server, string database, string userId, string password)
        //{
        //    FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
        //    csb.Server = server;
        //    csb.Database = database;
        //    csb.UserID = userId;
        //    csb.Password = password;
        //    return csb.ConnectionString;
        //}

        //public string CreateConnectionString(string server, string database)
        //{
        //    FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
        //    csb.Server = server;
        //    csb.Database = database;
        //    // csb.IntegratedSecurity = true;
        //    return csb.ConnectionString;
        //}

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
        {
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder(connectionString);
            return csb;
        }
        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.Firebird, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return CreateConnectionStringBuilder(server, database, "SYSDBA", "masterkey");
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new FbConnectionStringBuilder();

            if (!string.IsNullOrEmpty(server))
                csb.DataSource = server;
            if (!string.IsNullOrEmpty(database))
                csb.Database = database;
            if (!string.IsNullOrEmpty(userId))
                csb.UserID = userId;
            if (!string.IsNullOrEmpty(password))
                csb.Password = password;
            if (port != 0)
                csb.Port = port;
            csb.Pooling = true;
            if (embedded)
                csb.ServerType = FbServerType.Embedded;
            csb.Pooling = true;
            //csb.ServerType = embedded ? FbServerType.Embedded : FbServerType.Default;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            FbConnection cn = new FbConnection(connectionString);
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
            return new FbCommand(commandText);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new FbDataAdapter(selectCommand, (FbConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new FbDataAdapter((FbCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            return ((FbCommand)cmd).Parameters.AddWithValue(parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            var par = ((FbCommand)cmd).Parameters.AddWithValue(parameterName, value);
            par.Size = size;
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            FbParameter par = new FbParameter(parameterName, value);
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
            FbParameter par = new FbParameter(parameterName, FbDbType.Binary);
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            FbParameter par = new FbParameter(parameterName, FbDbType.Binary);
            par.Value = value;
            par.Size = size;
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
            if (name.Contains(" ") && name.IndexOf('"') == -1 || ReservedWords.ContainsKey(name))
                return "\"" + name + "\"";
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
                sql = sql.Insert(pos, "select first " + count);
            }
            return sql;
        }

        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            //string sql = string.Format(
            //    "SELECT column_name FROM information_schema.key_column_usage where table_schema = '{0}' and constraint_name = 'PRIMARY' and table_name = '{1}' order by ordinal_position",
            //    database, tableName);

            string sql = string.Format(
@"select b.rdb$field_name
from rdb$relation_constraints a,
rdb$index_segments b
where UPPER(a.rdb$relation_name) = UPPER('{0}')
and a.rdb$constraint_type = 'PRIMARY KEY'
and b.rdb$index_name = a.rdb$index_name
order by a.rdb$index_name, b.rdb$field_position", tableName);

            var ret = db.ExecuteArray1D<string>(sql);
            // vem espaços
            for (int i = 0; i < ret.Length; i++)
                ret[i] = ret[i].Trim();
            return ret;

        }

        public string GetSqlIdentityInsert()
        {
            return "LAST_INSERT_ID()";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            return null;
            //string sql = string.Format(
            //    "select column_name from information_schema.columns where table_schema = '{0}' and table_name = '{1}' and extra like '%auto_increment%'",
            //    database, tableName);
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
            //return db.GetAllTables(tableSchema, tableType);
            tableSchema = Conv.Unquote(tableSchema);

            string sql =
@"select null TABLE_CATALOG, null TABLE_SCHEMA, rdb$relation_name TABLE_NAME, 'BASE TABLE' TABLE_TYPE
 from rdb$relations
 where rdb$view_blr is null 
 and (rdb$system_flag is null or rdb$system_flag = 0)
UNION
 select null TABLE_CATALOG, null TABLE_SCHEMA, rdb$relation_name TABLE_NAME, 'VIEW' TABLE_TYPE
 from rdb$relations
 where rdb$view_blr is not null 
 and (rdb$system_flag is null or rdb$system_flag = 0);
 ";

            //if (string.IsNullOrEmpty(tableSchema) && tableType == null)
            //    sql = "select * from information_schema.tables order by table_name;";
            //else if (!string.IsNullOrEmpty(tableSchema) && tableType == null)
            //    sql = "select * from information_schema.tables where table_schema = '" + tableSchema + "' order by table_name;";
            //else if (string.IsNullOrEmpty(tableSchema) && tableType != null)
            //    sql = "select * from information_schema.tables where table_type = '" + (tableType.Value == EnumTableType.Table ? "BASE TABLE" : "") + "' order by table_name;";
            //else // if (!string.IsNullOrEmpty(tableSchema) && tableType != null)
            //    sql = "select * from information_schema.tables where table_schema = '" + tableSchema + "' and table_type = '" + (tableType.Value == EnumTableType.Table ? "BASE TABLE" : "") + "' order by table_name;";

            DataTable tb = db.ExecuteDataTable(sql);
            if (tableType != null)
                tb.DefaultView.RowFilter = "TABLE_TYPE = '" + (tableType == EnumTableType.Table ? "BASE TABLE" : "VIEW") + "'";

            List<TableInfo> tables = new List<TableInfo>();
            foreach (DataRowView row in tb.DefaultView)
            {
                TableInfo ti = new TableInfo();
                ti.TableSchema = Conv.Trim(row["TABLE_SCHEMA"]);
                ti.TableCatalog = Conv.Trim(row["TABLE_CATALOG"]);
                ti.TableName = Conv.Trim(row["TABLE_NAME"]);
                ti.TableType = Conv.Trim(row["TABLE_TYPE"]) == "BASE TABLE" ? EnumTableType.Table : EnumTableType.View;
                tables.Add(ti);
            }
            return tables;
        }

        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            if (schemaName == "")
                schemaName = null;
            if (tableName == "")
                tableName = null;
            return db.GetSchema("Columns", new string[] { null, schemaName, tableName, null });
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
            var names = db.ExecuteArray1D<string>("SELECT RDB$GENERATOR_NAME FROM RDB$GENERATORS WHERE RDB$SYSTEM_FLAG=0");
            var list = new List<DbSequenceInfo>();
            foreach (var name in names)
                list.Add(new DbSequenceInfo { SequenceName = name.Trim() });
            return list;
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
            // TODO: implementar a conversão de FbDbType para SqlDbType
            return new FbParameter(parameter.Name, parameter.Value);
        }

        public string GetInsertXml()
        {
            return "throw new NotSupportedException();";
        }

        public int ExecuteSequenceInt32(string sequenceName)
        {
            return db.ExecuteInt32("select gen_id({0},1) FROM RDB$DATABASE", sequenceName);
        }

        public long ExecuteSequenceInt64(string sequenceName)
        {
            return db.ExecuteInt64("select gen_id({0},1) FROM RDB$DATABASE", sequenceName);
        }

    }

}
