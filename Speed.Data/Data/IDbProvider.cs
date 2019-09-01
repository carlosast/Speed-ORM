using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Speed.Data.MetaData;

namespace Speed.Data
{

    public interface IDbProvider
    {
        Type DbType { get; }
        Dictionary<int, Enum> DbTypes { get; }
        bool SupportInformationSchema { get; }
        bool SupportBatchStatements { get; }
        IDbProvider CreateProvider(Database db);

        Dictionary<string, string> ReservedWords { get; }
        string ParameterSymbol { get; }
        string ParameterSymbolVar { get; }
        //string CreateConnectionString(string server, string database, string userId, string password);
        //string CreateConnectionString(string server, string database);
        DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString);
        DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null);
        DbConnectionStringBuilder CreateConnectionStringBuilder(string server, string database);
        DbConnection NewConnection(string connectionString);
        DbCommand NewCommand(string commandText);
        DbDataAdapter CreateDataAdapter(string selectCommand, DbConnection cn);
        DbDataAdapter CreateDataAdapter(DbCommand cmd);
        DbParameter AddWithValue(DbCommand cmd, string parameterName, object value);
        DbParameter AddWithValue(DbCommand cmd, string parameterName, string propertyType, object value);
        DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size);
        DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, ParameterDirection direction, object value, int length);
        DbParameter AddParBinary(DbCommand cmd, string parameterName, object value);
        DbParameter AddParBinary(DbCommand cmd, string parameterName, object value, int size);
        DbParameter AddParRef(DbCommand cmd, string parameterName, ParameterDirection direction, object value);
        DbParameter AddParRefCursor(DbCommand cmd, string parameterName, ParameterDirection direction, object value);
        string GetObjectName(string name, bool quote = true);
        string GetObjectName(string schemaName, string name, bool quote = true);
        string SetTop(string sql, long count);
        string[] GetPrimaryKeyColumns(string database, string schemaName, string tableName);
        string GetIdentityColumn(string database, string schemaName, string tableName);
        List<string> GetCalculatedColumns(string database, string schemaName, string tableName);
        /// <summary>
        /// Retorna o comando sql para retornar o valor do da coluna identity do último insert
        /// </summary>
        /// <returns></returns>
        string GetSqlIdentityInsert();
        List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null);

        DataTable GetSchemaColumns(string schemaName, string tableName);
        List<DbReferencialConstraintInfo> GetParentRelations(string schemaName, string tableName);
        DataTable GetDataTypes();
        List<DbSequenceInfo> GetSequences();
        DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null);

        /// <summary>
        /// Types do banco de dados. Ainda não usado
        /// </summary>
        Dictionary<string, DbDataType> DataTypes { get; }
        IDbDataParameter Convert(Parameter parameter);
        string GetInsertXml();
        int ExecuteSequenceInt32(string sequenceName);
        long ExecuteSequenceInt64(string sequenceName);

        bool AddUsings(DbColumnInfo col, Dictionary<string, string> usings);

    }

}
