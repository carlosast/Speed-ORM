using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Speed.Data.MetaData;
using Npgsql;
using NpgsqlTypes;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    class DbPostgreSqlProvider : IDbProvider
    {

        Database db;
        public string ParameterSymbol { get { return "@"; } }
        public string ParameterSymbolVar { get { return ParameterSymbol; } }

        public DbPostgreSqlProvider(Database db)
        {
            this.db = db;
        }

        ~DbPostgreSqlProvider()
        {
            db = null;
        }

        public Type DbType { get { return typeof(NpgsqlDbType); } }

        [ThreadStatic]
        static Dictionary<int, Enum> dbTypes;
        public Dictionary<int, Enum> DbTypes
        {
            get
            {
                if (dbTypes == null)
                    dbTypes = DbUtil.GetTypes<NpgsqlDbType>();
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
        //    NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder();
        //    csb.Host = server;
        //    csb.Database = database;
        //    csb.UserName = userId;
        //    csb.Password = password;
        //    return csb.ConnectionString;
        //}

        //public string CreateConnectionString(string server, string database)
        //{
        //    NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder();
        //    csb.Host = server;
        //    csb.Database = database;
        //    // csb.IntegratedSecurity = true;
        //    return csb.ConnectionString;
        //}

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
        {
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(connectionString);
            return csb;
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database)
        {
            return BuildConnectionString(EnumDbProviderType.PostgreSQL, server, database, null, null, false, 0, false);
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            return BuildConnectionString(EnumDbProviderType.PostgreSQL, server, database, userId, password, integratedSecurity, port, embedded);
        }

        public DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var csb = new NpgsqlConnectionStringBuilder();
            csb.Host = server;
            csb.Database = database;
            if (!string.IsNullOrWhiteSpace(userId))
                csb.UserName = userId;
            if (!string.IsNullOrWhiteSpace(password))
                csb.Password = password;
            if (integratedSecurity)
                csb.IntegratedSecurity = integratedSecurity;
            if (port > 0)
                csb.Port = (int)port;
            return csb;
        }

        public System.Data.Common.DbConnection NewConnection(string connectionString)
        {
            NpgsqlConnection cn = new NpgsqlConnection(connectionString);
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
            return new NpgsqlCommand(commandText);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn)
        {
            return new NpgsqlDataAdapter(selectCommand, (NpgsqlConnection)cn);
        }

        public System.Data.Common.DbDataAdapter CreateDataAdapter(System.Data.Common.DbCommand cmd)
        {
            return new NpgsqlDataAdapter((NpgsqlCommand)cmd);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            return ((NpgsqlCommand)cmd).Parameters.AddWithValue(parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            var par = ((NpgsqlCommand)cmd).Parameters.AddWithValue(parameterName, value);
            par.Size = size;
            return par;
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int size = 0)
        {
            NpgsqlParameter par = new NpgsqlParameter(parameterName, value);
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
            NpgsqlParameter par = new NpgsqlParameter(parameterName, NpgsqlDbType.Array);
            if (value == null || value == DBNull.Value)
                par.Size = -1;
            par.Value = value;
            cmd.Parameters.Add(par);
            return par;
        }

        public DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size)
        {
            NpgsqlParameter par = new NpgsqlParameter(parameterName, NpgsqlDbType.Array);
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
                name = "\"" + name + "\"";

            //quote = true;

            if (quote)
            {
                if (!name.StartsWith("\""))
                    name = "\"" + name;
                if (!name.EndsWith("\""))
                    name += "\"";
            }
            return name;
        }

        public string GetObjectName(string schemaName, string name, bool quote = true)
        {
            return string.IsNullOrEmpty(schemaName) ? GetObjectName(name, quote) : string.Format("{0}.{1}", GetObjectName(schemaName, quote), GetObjectName(name, quote));
        }

        public string SetTop(string sql, long count)
        {
            return sql + "\r\nLIMIT " + count;
        }

        // TODO: aqui no DbNpgsqlProvider é criado um inner join correto com todas as colunas necessárias entre 
        // key_column_usage e table_constraints. Ver se nos demais providers está correto
        public string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName)
        {
            string sql = null;

            schemaName = Conv.Unquote(schemaName);
            tableName = Conv.Unquote(tableName);

            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                sql = string.Format(
                     "select column_name,ordinal_position from information_schema.key_column_usage k inner join information_schema.table_constraints c on k.constraint_catalog = c.constraint_catalog and k.table_schema = c.table_schema and k.constraint_name = c.constraint_name and k.table_name = c.table_name where c.table_schema = '{0}' and c.table_name = '{1}' and c.constraint_type = 'PRIMARY KEY' order by ordinal_position",
                    //"SELECT distinct column_name FROM information_schema.key_column_usage where table_schema = '{0}' and constraint_name = 'PRIMARY' and table_name = '{1}' order by ordinal_position",
                     schemaName, tableName);
            }
            else
            {
                sql = string.Format(
                     "select column_name,ordinal_position from information_schema.key_column_usage k inner join information_schema.table_constraints c on k.constraint_catalog = c.constraint_catalog and k.table_schema = c.table_schema and k.constraint_name = c.constraint_name and k.table_name = c.table_name where c.table_name = '{0}' and c.constraint_type = 'PRIMARY KEY' order by ordinal_position",
                     tableName);
            }
            return db.ExecuteArray1D<string>(sql);
        }

        public string GetSqlIdentityInsert()
        {
            return "[SequenceValue]";
        }

        public string GetIdentityColumn(string database, string schemaName, string tableName)
        {
            //string sql = string.Format(
            //"select column_name from information_schema.columns where " + GetTableFilter(schemaName, tableName) + " and extra like '%auto_increment%'",
            //schemaName, tableName);
            //return db.ExecuteString(sql);
            return null;
        }

        public List<string> GetCalculatedColumns(string database, string schemaName, string tableName)
        {
            return new List<string>();
        }

        public List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null)
        {
            tableSchema = Conv.Unquote(tableSchema);

            //return db.GetAllTables(tableSchema, tableType, false);
            bool useDatabase = false;
            string defaultSchemas = "'pg_toast', 'pg_toast_toast_temp_1', 'pg_toast_temp_1', 'information_schema', 'pg_catalog'";

            string filerSchema = null;
            if (string.IsNullOrWhiteSpace(tableSchema))
                filerSchema = "not table_schema in (" + defaultSchemas + ")";

            string sql;
            if (string.IsNullOrEmpty(tableSchema) && tableType == null)
                sql = "select * from information_schema.tables " + (filerSchema == null ? "" : "where " + filerSchema) + " order by table_name;";
            else if (!string.IsNullOrEmpty(tableSchema) && tableType == null)
                sql = "select * from information_schema.tables where table_schema = '" + tableSchema + (filerSchema == null ? "" : "and " + filerSchema) + "' order by table_name;";
            else // if (!string.IsNullOrEmpty(tableSchema) && tableType != null)
                sql = "select * from information_schema.tables where table_schema = '" + tableSchema + (filerSchema == null ? "" : "and " + filerSchema) + "' and table_type = '" + (tableType.Value == EnumTableType.Table ? "BASE TABLE" : "") + "' order by table_name;";

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
            var list = new List<DbSequenceInfo>();

            string sql =
@"SELECT s1.nspname as tableschema, t1.relname AS tablename, a.attname columnname,  s2.nspname || '.' || t2.relname AS sequencename
 FROM pg_depend AS d
 JOIN pg_class AS t1 ON t1.oid = d.refobjid
 JOIN pg_class AS t2 ON t2.oid = d.objid
 JOIN pg_namespace AS s1 ON s1.oid = t1.relnamespace
 JOIN pg_namespace AS s2 ON s2.oid = t2.relnamespace
 JOIN pg_attribute AS a ON a.attrelid = d.refobjid AND a.attnum = d.refobjsubid
 WHERE t1.relkind = 'r'
 AND t2.relkind = 'S';
";

            using (var r = db.ExecuteReader(sql))
            {
                while (r.Read())
                {
                    var rec = new DbSequenceInfo();
                    rec.SchemaName = Conv.ToString(r["tableschema"]);
                    rec.TableName = Conv.ToString(r["TableName"]);
                    rec.SequenceName = Conv.ToString(r["SequenceName"]);
                    rec.ColumnName = Conv.ToString(r["ColumnName"]);
                    list.Add(rec);
                }
            }
            return list;
        }

        public List<DbSequenceInfo> GetSequences_OLD()
        {
            var list = new List<DbSequenceInfo>();

            string sql =
                // @"select x.nspname as ''SchemaName'', x.relname as ''SequenceName'', x.attnum as ''#'', x.attname as ''Column'', x.''Type'', case x.attnotnull when true then ''NOT NULL'' else '''' end as ''NULL?''
@"select x.nspname as ''SchemaName'', x.relname as ''SequenceName'', x.attname as ''ColumnName'', r.conname as ''ConstraintName'', d.adsrc as ''Default''
 from (
 SELECT 
c.oid, a.attrelid, a.attnum, n.nspname, c.relname, a.attname, pg_catalog.format_type(a.atttypid, a.atttypmod) as ''Type'', a.attnotnull
FROM pg_catalog.pg_attribute a, pg_namespace n, pg_class c
 WHERE a.attnum > 0
 AND NOT a.attisdropped
 AND a.attrelid = c.oid
 and c.relkind not in ('S','v')
 and c.relnamespace = n.oid
 and n.nspname not in ('pg_catalog','pg_toast','information_schema') and relkind = 'r'
 ) x
 left join pg_attrdef d on d.adrelid = x.attrelid and d.adnum = x.attnum
 left join pg_constraint r on r.conrelid = x.oid and r.conkey[1] = x.attnum
 left join pg_class f on r.confrelid = f.oid
 left join pg_namespace fn on f.relnamespace = fn.oid
 where
	r.contype = 'p'
 order by 1,2,3
".Replace("''", "\"");

            using (var r = db.ExecuteReader(sql))
            {
                while (r.Read())
                {
                    var rec = new DbSequenceInfo();
                    rec.SchemaName = Conv.ToString(r["SchemaName"]);
                    rec.SequenceName = Conv.ToString(r["SequenceName"]);
                    rec.ColumnName = Conv.ToString(r["ColumnName"]);
                    //rec.ConstraintName = Conv.ToString(r["ConstraintName"]);
                    //rec.Default = Conv.ToString(r["Default"]);
                    list.Add(rec);
                }
            }
            return list;
        }

        string GetTableFilter(string schemaName, string tableName)
        {
            schemaName = Conv.Unquote(schemaName);
            tableName = Conv.Unquote(tableName);

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
            var par = new NpgsqlParameter(parameter.Name, parameter.DbType);
            par.Value = parameter.Value;
            par.Direction = parameter.Direction;
            if (parameter.Size.HasValue)
                par.Size = parameter.Size.Value;
            return par;
        }

        public string GetInsertXml()
        {
            return "throw new NotSupportedException();";
        }

        public int ExecuteSequenceInt32(string sequenceName)
        {
            return db.ExecuteInt32("Select nextval({0})", sequenceName);
        }

        public long ExecuteSequenceInt64(string sequenceName)
        {
            return db.ExecuteInt64("Select nextval({0})", sequenceName);
        }

    }

}
