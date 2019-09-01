using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed.Data
{

    internal class TemplatesCommands
    {

        #region INSERT_COMMANDS

        #region INSERT

        internal const string INSERT =
@"
        [TypeName] value = ([TypeName])instance;
        string sql = 'insert into [TableName] ([InsertColumns]) values ([InsertParameters])';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            value.RecordStatus = RecordStatus.Existing;
            return db.ExecuteNonQuery(cmd);
        }
";
        #endregion INSERT

        /*
        #region INSERT_ACCESS

        internal const string INSERT_ACCESS =
@"
        [TypeName] value = ([TypeName])instance;

        string sql;
        if (db.ProviderType != EnumDbProviderType.Oracle && db.ProviderType != EnumDbProviderType.PostgreSQL)
        {
            sql = 'insert into [TableName] ([InsertColumns]) values ([InsertParameters])';
        }
        else if (db.ProviderType == EnumDbProviderType.Oracle)
        {
            string sequenceColumn = '[SequenceColumn]';
            string sequenceName =   '[SequenceName]';
            if (!string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceName))
            {
                sql  =  'declare p___sequence integer;\r\n';
                sql +=  'begin\r\n';
                sql +=  '    p___sequence := [SequenceValue];\r\n';
                sql +=  '    insert into [TableName] ([InsertColumns]) values ([InsertParameters]);\r\n';
                sql +=  'end;';
            }
            else
            {
                sql  =  'begin';
                sql += '  insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
//                sql += '  open :cur for [Sql] where [WhereRead];';
                sql +=  'end;';
            }
        }
        else // if (db.ProviderType == EnumDbProviderType.PostgreSQL)
        {
            string sequenceColumn = '[SequenceColumn]';
            string sequenceName =   '[SequenceName]';
            if (!string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceName))
            {
                sql =  'do\r\n';
                sql +=  '$$\r\n';
                sql +=  '	DECLARE p___sequence integer;\r\n';
                sql +=  'begin\r\n';
                sql +=  '    p___sequence = [SequenceValue];\r\n\r\n';
                sql +=  '    insert into [TableName] ([InsertColumns]) values ([InsertParameters]);\r\n';
                sql +=  'end;\r\n';
                sql +=  '$$';
            }
            else
            {
                sql = 'insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
            }
        }
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            int ret = db.ExecuteNonQuery(cmd);
            value.RecordStatus = RecordStatus.Existing;
            return ret;
        }
";
        #endregion INSERT_ACCESS
        */

        #endregion INSERT_COMMANDS

        #region INSERT_REQUERY_COMMANDS

        #region INSERT_REQUERY

        internal static string INSERT_REQUERY =

@"
        [TypeName] value = ([TypeName])instance;
        string sql;
        sql = 'insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
        sql += '[Sql] where [WhereRead];';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }";

        #endregion INSERT_REQUERY

        #region INSERT_REQUERY_ORACLE

        internal static string INSERT_REQUERY_ORACLE =

@"[TypeName] value = ([TypeName])instance;
        string sql;
        string sequenceColumn = '[SequenceColumn]';
        string sequenceName =   '[SequenceName]';

        if (!string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceName))
        {
            sql  =  'declare p___sequence integer;\r\n';
            sql +=  'begin\r\n';
            sql +=  '    p___sequence := [SequenceValue];\r\n';
            sql +=  '    insert into [TableName] ([InsertColumns]) values ([InsertRequeryParametersSequence]);\r\n';
            sql +=  '    open :cur for [Sql] where [WhereReadSequence];\r\n';
            sql +=  'end;';
        }
        else
        {
            sql  =  'begin';
            sql += '  insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
            sql += '  open :cur for [Sql] where [WhereRead];';
            sql +=  'end;';
        }

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            db.AddParameterRefCursor(cmd, 'cur', System.Data.ParameterDirection.Output);
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }";

        #endregion INSERT_REQUERY_ORACLE

        #region INSERT_REQUERY_POSTGRESQL

        // TODO: Descobrir como executar numa única chamada os códigos do PostgreSQL, em INSERT_REQUERY_POSTGRESQL
        internal static string INSERT_REQUERY_POSTGRESQL =

@"[TypeName] value = ([TypeName])instance;
        string sql;
        string sequenceColumn = '[SequenceColumn]';
        string sequenceName =   '[SequenceName]';

        if (!string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceName))
        {

            string seq = db.ExecuteString('select ' + '[SequenceValue]');

            sql =  'insert into [TableName] ([InsertColumns]) values ([InsertRequeryParametersSequence])'.Replace('p___sequence', seq);
            using (DbCommand cmd = db.NewCommand(sql))
            {
                [SetParameters]
                cmd.ExecuteNonQuery();
            }

            sql = '[Sql] where [WhereReadSequence]'.Replace('p___sequence', seq);

//            sql  =  'do returns table ([PgReturnsTable]) as\r\n';
//            sql +=  '$$\r\n';
//            sql +=  'declare p___sequence integer;\r\n';
//            sql +=  'declare p___sequence integer;\r\n';
//            sql +=  'begin\r\n';
//            sql +=  '    p___sequence := [SequenceValue];\r\n';
//            sql +=  '    insert into [TableName] ([InsertColumns]) values ([InsertRequeryParametersSequence]);\r\n';
//            sql +=  '    return query [Sql] where [WhereReadSequence];\r\n';
//            sql +=  'end;';
//            sql +=  '$$\r\n';
        }
        else
        {
            sql  =  'begin';
            sql += '  insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
            sql += '  open :cur for [Sql] where [WhereRead];';
            sql +=  'end;';
        }

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }";

        #endregion INSERT_REQUERY_POSTGRESQL

        #region INSERT_REQUERY_FIREBIRD

        internal static string INSERT_REQUERY_FIREBIRD =

@"[TypeName] value = ([TypeName])instance;
        string sql;
        string sequenceColumn = '[SequenceColumn]';
        string sequenceName =   '[SequenceName]';

        if (!string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceName))
        {
            [SequenceColumnGet]
            sql = 'insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
            using (DbCommand cmd = db.NewCommand(sql))
            {
    [SetParameters]
                cmd.ExecuteNonQuery();
            }

            sql = '[Sql] where [WhereRead];';
            using (DbCommand cmd = db.NewCommand(sql))
            {
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        readRecord(dr, ref value);
                        value.RecordStatus = RecordStatus.Existing;
    //                    if (value.RecordStatus == RecordStatus.New)
    //                    {
    //                        [TypeName] old = new [TypeName]();
    //                        readRecord(dr, ref old);
    //                        value.Original = old;
    //                    }
    //                    else
    //                        value.RecordStatus = RecordStatus.Existing;
                        return 1;
                    }
                    return 0;
                }
            }
        }
        else
        {
            sql  =  'begin';
            sql += '  insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
            sql += '  [Sql] where [WhereRead];';
            sql +=  'end;';
        }

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            db.AddParameterRefCursor(cmd, 'cur', System.Data.ParameterDirection.Output);
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }";

        #endregion INSERT_REQUERY_FIREBIRD

        #region INSERT_REQUERY_POSTGRESQL_OLD

        internal static string INSERT_REQUERY_POSTGRESQL_OLD =

@"[TypeName] value = ([TypeName])instance;
        string sql;
        string sequenceColumn = '[SequenceColumn]';
        string sequenceName =   '[SequenceName]';

        if (!string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceName))
        {
            sql  =  'do returns table ([PgReturnsTable]) as\r\n';
            sql +=  '$$\r\n';
            sql +=  'declare p___sequence integer;\r\n';
            sql +=  'declare p___sequence integer;\r\n';
            sql +=  'begin\r\n';
            sql +=  '    p___sequence := [SequenceValue];\r\n';
            sql +=  '    insert into [TableName] ([InsertColumns]) values ([InsertRequeryParametersSequence]);\r\n';
            sql +=  '    return query [Sql] where [WhereReadSequence];\r\n';
            sql +=  'end;';
            sql +=  '$$\r\n';
        }
        else
        {
            sql  =  'begin';
            sql += '  insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
            sql += '  open :cur for [Sql] where [WhereRead];';
            sql +=  'end;';
        }

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }";

        #endregion INSERT_REQUERY_POSTGRESQL_OLD

        #region INSERT_REQUERY_ACCESS

        internal static string INSERT_REQUERY_ACCESS =
@"if (![HasIdentity])
        {
[INSERT]
        }
        else
        {
            [TypeName] value = ([TypeName])instance;
            string sql = 'insert into [TableName] ([InsertColumns]) values ([InsertParameters])';
            using (DbCommand cmd = db.NewCommand(sql))
            {
    [SetParameters]
                cmd.ExecuteNonQuery();
            }

            long identity = db.ExecuteInt64('SELECT [GetSqlIdentityInsert]');
        
            sql = string.Format('[Sql] where [WhereReadNoBatch]', identity);
            using (DbCommand cmd = db.NewCommand(sql))
            {
    [SetParameters]
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        readRecord(dr, ref value);
                        value.RecordStatus = RecordStatus.Existing;
    //                    if (value.RecordStatus == RecordStatus.New)
    //                    {
    //                        [TypeName] old = new [TypeName]();
    //                        readRecord(dr, ref old);
    //                        value.Original = old;
    //                    }
    //                    else
    //                        value.RecordStatus = RecordStatus.Existing;
                        return 1;
                    }
                    return 0;
                }
            }
        }
";

        #endregion INSERT_REQUERY_ACCESS

        #endregion INSERT_REQUERY_COMMANDS

        #region UPDATE_REQUERY_COMMANDS

        #region UPDATE_REQUERY

        internal const string UPDATE_REQUERY =
@"
        [TypeName] value = ([TypeName])instance;
        string sql;
        sql = 'update [TableName] set [UpdateColumns] where [WhereOldParameters];';
        sql += '\r\n' + '[Sql] where [WhereParameters];';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
            [TypeName] old = value.Original != null ? ([TypeName])value.Original : value;
[SetOldParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old2 = new [TypeName]();
//                        readRecord(dr, ref old2);
//                        value.Original = old2;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }
";
        #endregion UPDATE_REQUERY

        #region UPDATE_REQUERY_ORACLE

        internal const string UPDATE_REQUERY_ORACLE =
@"
        [TypeName] value = ([TypeName])instance;
        string sql;
        
        sql  =  'begin';
        sql += '\r\n' + '  update [TableName] set [UpdateColumns] where [WhereOldParameters];';
        sql += '\r\n' + '  open :cur for [Sql] where [WhereOldParameters];';
        sql +=  '\r\n' + 'end;';

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
            [TypeName] old = value.Original != null ? ([TypeName])value.Original : value;
[SetOldParameters]
            db.AddParameterRefCursor(cmd, 'cur', System.Data.ParameterDirection.Output);
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old2 = new [TypeName]();
//                        readRecord(dr, ref old2);
//                        value.Original = old2;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }
";
        #endregion UPDATE_REQUERY_ORACLE

        #region UPDATE_REQUERY_POSTGRESQL

        // TODO: Descobrir como executar numa única chamada os códigos do PostgreSQL, em UPDATE_REQUERY_POSTGRESQL
        internal const string UPDATE_REQUERY_POSTGRESQL =
@"
        [TypeName] value = ([TypeName])instance;
        string sql;
        
        sql =  'update [TableName] set [UpdateColumns] where [WhereOldParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
        [TypeName] old = value.Original != null ? ([TypeName])value.Original : value;
[SetOldParameters]
            cmd.ExecuteNonQuery();
        }

        sql = '[Sql] where [WhereParameters]';

        using (DbCommand cmd = db.NewCommand(sql))
        {
            [TypeName] old = value.Original != null ? ([TypeName])value.Original : value;
[SetParametersWithIdentity]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    if (value.RecordStatus == RecordStatus.New)
                    {
                        [TypeName] old2 = new [TypeName]();
                        readRecord(dr, ref old2);
                        value.Original = old2;
                    }
                    else
                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }
";
        #endregion UPDATE_REQUERY_POSTGRESQL

        #region UPDATE_REQUERY_ACCESS

        internal const string UPDATE_REQUERY_ACCESS =
@"
        [TypeName] value = ([TypeName])instance;
        string sql = 'update [TableName] set [UpdateColumns] where [WhereOldParameters]';
        [TypeName] old = value.Original != null ? ([TypeName])value.Original : value;

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
[SetOldParameters]
            cmd.ExecuteNonQuery();
        }

        sql = '\r\n' + '[Sql] where [WhereOldParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetOldParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }
";
        #endregion UPDATE_REQUERY_ACCESS

        #region UPDATE_REQUERY_FIREBIRD

        internal const string UPDATE_REQUERY_FIREBIRD =
@"
        [TypeName] value = ([TypeName])instance;
        string sql = 'update [TableName] set [UpdateColumns] where [WhereOldParameters]';
        [TypeName] old = value.Original != null ? ([TypeName])value.Original : value;

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
[SetOldParameters]
            cmd.ExecuteNonQuery();
        }

        sql = '\r\n' + '[Sql] where [WhereOldParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetOldParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }
";
        #endregion UPDATE_REQUERY_FIREBIRD

        #endregion UPDATE_REQUERY_COMMANDS

        #region TEMP

        const string TEMP =
@"
    // Microsoft Access
    private int insertReadAllColumnsNoBatch(Database db, object instance, bool hasIdentity)
    {
        if (![HasIdentity])
            return insertRequery(db, instance);

        [TypeName] value = ([TypeName])instance;
        string sql = 'insert into [TableName] ([InsertColumns]) values ([InsertParameters])';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            cmd.ExecuteNonQuery();
        }

        long identity = db.ExecuteInt64('SELECT [GetSqlIdentityInsert]');
        
        sql = string.Format('[Sql] where [WhereReadNoBatch]', identity);
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
//                    if (value.RecordStatus == RecordStatus.New)
//                    {
//                        [TypeName] old = new [TypeName]();
//                        readRecord(dr, ref old);
//                        value.Original = old;
//                    }
//                    else
//                        value.RecordStatus = RecordStatus.Existing;
                    return 1;
                }
                return 0;
            }
        }
    }
";
        #endregion TEMP

    }

}
