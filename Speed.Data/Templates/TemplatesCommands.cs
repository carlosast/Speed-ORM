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
            [!POCO]value.RecordStatus = RecordStatus.Existing;
            return db.ExecuteNonQuery(cmd);
        }
";
        #endregion INSERT

        #region SELECT_PAGE

        #region Sql Server

        internal static string SELECT_PAGE_SQL =
@"
WITH Ordered AS
(
    SELECT [Columns],
    ROW_NUMBER() OVER (ORDER BY {2}) AS Row____Number
    FROM [TableName] {3}
) 
SELECT [Columns]  
FROM Ordered
WHERE Row____Number BETWEEN {0} AND {1};
";
        #endregion Sql Server

        internal static string SELECT_PAGE_MY_SQL = @"SELECT [Columns] FROM [TableName] {2} {3} LIMIT {0},{1}";

        internal static string SELECT_PAGE_ORACLE =
@"
select *
 from (
select /*+ first_rows({0}) */
  [Columns],
  row_number() 
  over (order by {2}) rn
 from [TableName] {3})
where rn between {0} and {1}
";
        // order by {2} 

        #endregion SELECT_PAGE

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
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        }";

        #endregion INSERT_REQUERY_ORACLE

        #region INSERT_REQUERY_POSTGRESQL

        // TODO: Descobrir como executar numa única chamada os códigos do PostgreSQL, em INSERT_REQUERY_POSTGRESQL
        internal static string INSERT_REQUERY_POSTGRESQL(string identityColumn)
        {
            var template = @"[TypeName] value = ([TypeName])instance;
        string sql;
        string sequenceColumn = '[SequenceColumn]';
        string sequenceName =   '[SequenceName]';

        if ('[identityColumn]' != string.Empty)
        {
            sql = '  insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
            sql += 'select * from [TableName] where [identityColumn] = lastval();';
        }
        else if (!string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceName))
        {

            string seq = db.ExecuteString('select ' + '[SequenceValue]');

            sql =  'insert into [TableName] ([InsertColumns]) values ([InsertRequeryParametersSequence])'.Replace('p___sequence', seq);
            using (DbCommand cmd = db.NewCommand(sql))
            {
                [SetParameters]
                cmd.ExecuteNonQuery();
            }

            sql = '[Sql] where [WhereReadSequence]'.Replace('p___sequence', seq);
        }
        else
        {
            sql = '  insert into [TableName] ([InsertColumns]) values ([InsertParameters]);';
        }

        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        }";

            template = template.Replace("[identityColumn]", identityColumn);
            return template;
        }

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
                        readRecord(db, dr, ref value);
                        [!POCO]value.RecordStatus = RecordStatus.Existing;
                        [!POCO]if (concurrency)
                        [!POCO]    value.RecordOriginal = value.CloneT();
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
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
                        readRecord(db, dr, ref value);
                        [!POCO]value.RecordStatus = RecordStatus.Existing;
                        [!POCO]if (concurrency)
                        [!POCO]    value.RecordOriginal = value.CloneT();
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
        [!POCO]if (value.RecordOriginal as [TypeName] != null) CheckConcurrency(db, value);
        string sql;
        sql = 'update [TableName] set [UpdateColumns] where [WhereOldParameters];';
        sql += '\r\n' + '[Sql] where [WhereParameters];';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
            [!POCO][TypeName] old = value.RecordOriginal as [TypeName] != null ? ([TypeName])value.RecordOriginal : value;
            [POCO][TypeName] old = value;
[SetOldParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
        [!POCO]if (value.RecordOriginal as [TypeName] != null) CheckConcurrency(db, value);
        string sql;
        sql  =  'begin';
        sql += '\r\n' + '  update [TableName] set [UpdateColumns] where [WhereOldParameters];';
        sql += '\r\n' + '  open :cur for [Sql] where [WhereOldParameters];';
        sql +=  '\r\n' + 'end;';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
            [!POCO][TypeName] old = value.RecordOriginal as [TypeName] != null ? ([TypeName])value.RecordOriginal : value;
            [POCO][TypeName] old = value;
[SetOldParameters]
            db.AddParameterRefCursor(cmd, 'cur', System.Data.ParameterDirection.Output);
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
        [!POCO]if (value.RecordOriginal as [TypeName] != null) CheckConcurrency(db, value);
        string sql;
        
        sql =  'update [TableName] set [UpdateColumns] where [WhereOldParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
        [!POCO][TypeName] old = value.RecordOriginal as [TypeName] != null ? ([TypeName])value.RecordOriginal : value;
        [POCO][TypeName] old = value;
[SetOldParameters]
            cmd.ExecuteNonQuery();
        }

        sql = '[Sql] where [WhereParameters]';

        using (DbCommand cmd = db.NewCommand(sql))
        {
            [!POCO][TypeName] old = value.RecordOriginal as [TypeName] != null ? ([TypeName])value.RecordOriginal : value;
            [POCO][TypeName] old = value;
[SetParametersWithIdentity]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(db, dr, ref value);
                    [!POCO]if (value.RecordStatus == RecordStatus.New)
                    [!POCO]{
                    [!POCO]    [TypeName] old2 = new [TypeName]();
                    [!POCO]    readRecord(db, dr, ref old2);
                    [!POCO]    value.RecordOriginal = old2;
                    [!POCO]}
                    [!POCO]else
                    [!POCO]    value.RecordStatus = RecordStatus.Existing;
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
        [!POCO]if (value.RecordOriginal as [TypeName] != null) CheckConcurrency(db, value);
        string sql = 'update [TableName] set [UpdateColumns] where [WhereOldParameters]';
        [!POCO][TypeName] old = value.RecordOriginal as [TypeName] != null ? ([TypeName])value.RecordOriginal : value;
        [POCO][TypeName] old = value;

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
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
        [!POCO]if (value.RecordOriginal as [TypeName] != null) CheckConcurrency(db, value);
        string sql = 'update [TableName] set [UpdateColumns] where [WhereOldParameters]';
        [!POCO][TypeName] old = value.RecordOriginal as [TypeName] != null ? ([TypeName])value.RecordOriginal : value;
        [POCO][TypeName] old = value;

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
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
                    readRecord(db, dr, ref value);
                    [!POCO]value.RecordStatus = RecordStatus.Existing;
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
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
