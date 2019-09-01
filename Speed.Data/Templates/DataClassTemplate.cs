namespace Speed.Data
{

    public static class DataClassTemplate
    {

        #region DATACLASSTEMPLATE

        internal const string DATACLASSTEMPLATE_USING =

    @"using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Speed;
using Speed.Data;
";

        public const string DATACLASSTEMPLATE_CODE =

    @"
[Serializable]
public partial class [ClassName] : DataClass
{

//    public Record CreateInstance()
//    {
//        return new [TypeName]();
//    }

    private void readRecord(DbDataReader dr, ref [TypeName] value)
    {
        [!POCO]value.RecordStatus = RecordStatus.Existing;
[SetColumns]
    }

    private void readRecordSql(DbDataReader dr, List<string> names, ref [TypeName] value)
    {
        [!POCO]value.RecordStatus = RecordStatus.Existing;
[SetColumnsSql]
    }

    private List<[TypeName]> readReader(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<[TypeName]> list = new List<[TypeName]>();
            while (dr.Read())
            {
                [TypeName] value = new [TypeName]();
                readRecord(dr, ref value);
                [!POCO]if (concurrency)
                [!POCO]    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<[TypeName]> readReader(Database db, string sql, bool concurrency, params Parameter[] parameters)
    {
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<[TypeName]> list = new List<[TypeName]>();
            while (dr.Read())
            {
                [TypeName] value = new [TypeName]();
                readRecord(dr, ref value);
                [!POCO]if (concurrency)
                [!POCO]    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private [TypeName] readReaderSingle(Database db, string sql, bool concurrency)
    {
        [TypeName] value = null;
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<[TypeName]> list = new List<[TypeName]>();
            if (dr.Read())
            {
                value = new [TypeName]();
                readRecord(dr, ref value);
                [!POCO]if (concurrency)
                [!POCO]    value.RecordOriginal = value.CloneT();
            }
            return value;
        }
    }

    private [TypeName] readReaderSingle(Database db, string sql, Parameter[] parameters)
    {
        [TypeName] value = null;
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<[TypeName]> list = new List<[TypeName]>();
            if (dr.Read())
            {
                value = new [TypeName]();
                readRecord(dr, ref value);
            }
            return value;
        }
    }

    private List<[TypeName]> readReaderSql(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<[TypeName]> list = new List<[TypeName]>();
            // pega os nomes das colunas continas no sql
            List<string> names = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
                names.Add(dr.GetName(i));
            while (dr.Read())
            {
                [TypeName] value = new [TypeName]();
                readRecordSql(dr, names, ref value);
                [!POCO]if (concurrency)
                [!POCO]    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<[TypeName]> select(Database db, bool concurrency)
    {
        string sql = '[Sql]';
        return readReader(db, sql, concurrency);
    }

    private List<[TypeName]> select(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = '[Sql]';
        else
            sql = '[Sql] where ' + where;
        return readReader(db, sql, concurrency);
    }

    private List<[TypeName]> select(Database db, string where, params Parameter[] parameters)
    {
        string sql = '[Sql] where ' + where;
        return readReader(db, sql, false, parameters);
    }

    private List<[TypeName]> select(Database db, string where, bool concurrency, params Parameter[] parameters)
    {
        string sql = '[Sql] where ' + where;
        return readReader(db, sql, concurrency, parameters);
    }

    private List<[TypeName]> selectTop(Database db, int top, bool concurrency)
    {
        // string sql = string.Format('[SqlTop]', top);
        string sql = '[Sql]';
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private List<[TypeName]> selectTop(Database db, int top, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = '[Sql]';
        else
            sql = '[Sql] where ' + where;
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private long count(Database db, string where)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = '[SqlCount]';
        else
            sql = '[SqlCount] where ' + where;
        return db.ExecuteInt64(sql);
    }

    private long max(Database db, string columnName, string where)
    {
        string sql = string.Format('[SqlCount]', columnName);
        if (!string.IsNullOrWhiteSpace(where))
            sql += ' where ' + where;
        return db.ExecuteInt64(sql);
    }

    private List<[TypeName]> selectColumns(Database db, bool concurrency, params string[] columns)
    {
        string sql = 'select ' + Concat(', ', columns) + ' from [TableName]';
        return readReaderSql(db, sql, concurrency);
    }

    private List<[TypeName]> selectColumns(Database db, string where, bool concurrency, params string[] columns)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = 'select ' + Concat(', ', columns) + ' from [TableName] ';
        else
            sql = 'select ' + Concat(', ', columns) + ' from [TableName] where ' + where;
        return readReaderSql(db, sql, concurrency);
    }

    private [TypeName] selectSingle(Database db, bool concurrency)
    {
        string sql = '[Sql]';
        return readReaderSingle(db, sql, concurrency);
    }

    private [TypeName] selectSingle(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = '[Sql]';
        else
            sql = '[Sql] where ' + where;
        return readReaderSingle(db, sql, concurrency);
    }

    private object selectByPk(Database db, object instance, bool concurrency)
    {
        [TypeName] value = ([TypeName])instance;
        [TypeName] value2 = new [TypeName]();
        string sql = '[Sql] where [WhereParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetCurParameters]
            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value2);
                    [!POCO]if (concurrency)
                    [!POCO]    value2.RecordOriginal = value2.CloneT();
                    return value2;
                }
                else
                    return null;
            }
        }
    }

    private int insert(Database db, object instance)
    {
        [INSERT]
    }

    private int insertRequery(Database db, object instance, bool concurrency)
    {
        [INSERT_REQUERY];
    }

    public override int Update(Database db, object instance)
    {
        return update(db, instance);
    }

    public override int Update(Database db, object instance, EnumSaveMode saveMode, bool concurrency = false)
    {
        if (saveMode == EnumSaveMode.None)
            return update(db, instance);
        else
            return updateRequery(db, instance, concurrency);
    }

    private int update(Database db, object instance)
    {
        [TypeName] value = ([TypeName])instance;
        string sql = 'update [TableName] set [UpdateColumns] where [WhereOldParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetParametersWithIdentity]
            [!POCO][TypeName] old = (value.RecordOriginal as [TypeName]) != null ? ([TypeName])value.RecordOriginal : value;
            [POCO][TypeName] old = value;
[SetOldParameters]
            return db.ExecuteNonQuery(cmd);
        }
    }

    private int updateRequery(Database db, object instance, bool concurrency = false)
    {
        [UPDATE_REQUERY]
    }

    public override int Truncate(Database db)
    {
        string sql = 'truncate table [TableName]';
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db)
    {
        string sql = 'delete from [TableName]';
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, object instance)
    {
        [TypeName] value = ([TypeName])instance;
        string sql = 'delete from [TableName] where [WhereOldParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
            [!POCO][TypeName] old = (value.RecordOriginal as [TypeName]) != null ? ([TypeName])value.RecordOriginal : value;
            [POCO][TypeName] old = value;
[SetOldParameters]
            return db.ExecuteNonQuery(cmd);
        }
    }

    public override int Delete(Database db, string where)
    {
        string sql = 'delete from [TableName] where ' + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int DeleteByPk(Database db, object instance)
    {
        [TypeName] value = ([TypeName])instance;
        string sql = 'DELETE FROM [TableName] where [WhereParameters]';
        using (DbCommand cmd = db.NewCommand(sql))
        {
[SetCurParameters]
            return cmd.ExecuteNonQuery();
        }
    }

    public override int Delete(Database db, string where, params Parameter[] parameters)
    {
        string sql = 'delete from [TableName] where ' + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Delete(Database db, string where, int commandTimeout, params Parameter[] parameters)
    {
        string sql = 'delete from [TableName] where ' + where;
        using (DbCommand cmd = db.NewCommand(sql, commandTimeout))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Save(Database db, object instance)
    {
        [!POCO][TypeName] value = ([TypeName])instance;
        [!POCO]if (value.RecordStatus == RecordStatus.Existing)
        [!POCO]    return update(db, value);
        [!POCO]else if (value.RecordStatus == RecordStatus.New)
        [!POCO]    return insert(db, value);
        [!POCO]else // if (value.RecordStatus == RecordStatus.Deleted)
        [!POCO]    return Delete(db, value);
        [POCO]throw new Exception('To use the Save method, the class must inherit from \'Record\'. Use INSERT, UPDATE, and DELETE directly');
    }

    public override int Save(Database db, object instance, EnumSaveMode saveMode)
    {
        [!POCO][TypeName] value = ([TypeName])instance;
        [!POCO]if (value.RecordStatus == RecordStatus.Existing)
        [!POCO]    return Update(db, value, saveMode);
        [!POCO]else if (value.RecordStatus == RecordStatus.New)
        [!POCO]    return Insert(db, value, saveMode);
        [!POCO]else // if (value.RecordStatus == RecordStatus.Deleted)
        [!POCO]    return Delete(db, value);
        [POCO]throw new Exception('To use the Save method, the class must inherit from \'Record\'. Use INSERT, UPDATE, and DELETE directly');
    }

    public override void SaveList(Database db, object instance, EnumSaveMode saveMode, bool continueOnError)
    {
        List<[TypeName]> values = (List<[TypeName]>)instance;
        bool isTran = db.UsingTransaction;

        if (!isTran)
            db.BeginTransaction();

        foreach ([TypeName] value in values)
        {
            try
            {
                Save(db, value, saveMode);
            }
            catch (Exception ex)
            {
                if (!continueOnError)
                {
                    db.Rollback();
                    throw ex;
                 }
            }
        }

        if (!isTran)
            db.Commit();
    }

    public override int DeleteAll(Database db, object[] instance)
    {
        return 1;
    }

    public override object Select(Database db)
    {
        return select(db, false);
    }

    public override object Select(Database db, bool concurrency)
    {
        return select(db, concurrency);
    }

    public override object Select(Database db, string where)
    {
        return select(db, where, false);
    }

    public override object Select(Database db, string where, bool concurrency)
    {
        return select(db, where, concurrency);
    }

    public override object Select(Database db, string where, params Parameter[] parameters)
    {
        return select(db, where, false, parameters);
    }

    public override object Select(Database db, object _filter, EnumDbFilter mode = EnumDbFilter.AndEqual, bool concurrency = false, int commandTimeout = 30)
    {
        [TypeName] filter = ([TypeName])_filter;
        string sql = '[Sql] {0} {1} {2}';

        StringBuilder where = new StringBuilder();
        string op = (mode == EnumDbFilter.AndLike || mode == EnumDbFilter.AndEqual) ? ' AND ' : ' OR ';

        List<[TypeName]> list = new List<[TypeName]>();

        using (var cmd = db.NewCommand('', commandTimeout, CommandType.Text))
        {
            int index = 0;
[ColumnsFilter]

            cmd.CommandText = string.Format(sql, (where.Length > 0 ? ' WHERE ' + where.ToString() : ''), '', '');

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    [TypeName] value = new [TypeName]();
                    readRecord(dr, ref value);
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
                    list.Add(value);
                }
            }
        }
        return list;
    }

    public override long Count(Database db, object _filter, EnumDbFilter mode = EnumDbFilter.AndEqual, int commandTimeout = 30)
    {
        [TypeName] filter = ([TypeName])_filter;
        string sql = 'select count(*) from [TableName] {0} ';

        StringBuilder where = new StringBuilder();
        string op = (mode == EnumDbFilter.AndLike || mode == EnumDbFilter.AndEqual) ? ' AND ' : ' OR ';

        List<[TypeName]> list = new List<[TypeName]>();

        using (var cmd = db.NewCommand('', commandTimeout, CommandType.Text))
        {
            int index = 0;

[ColumnsFilter]

            cmd.CommandText = string.Format(sql, (where.Length > 0 ? ' WHERE ' + where.ToString() : ''), '', '');

            return Convert.ToInt64(cmd.ExecuteScalar());
        }
    }

    //public Record CreateInstance()
    //{
    //    throw new NotImplementedException();
    //}

    public override object SelectPage(Database db, int start, int pageSize = 20, string sort = null)
    {
        return selectPage(db, null, start, pageSize, sort, false, null);
    }

    public override object SelectPage(Database db, int start, int pageSize = 20, string sort = null, bool concurrency = false)
    {
        return selectPage(db, null, start, pageSize, sort, concurrency, null);
    }

    public override object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null)
    {
        return selectPage(db, where, start, pageSize, sort, false, null);
    }

    public override object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, bool concurrency = false)
    {
        return selectPage(db, where, start, pageSize, sort, concurrency, null);
    }

    public override object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, params Parameter[] parameters)
    {
        return selectPage(db, where, start, pageSize, sort, false, parameters);
    }

    public override object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, bool concurrency = false, params Parameter[] parameters)
    {
        return selectPage(db, where, start, pageSize, sort, concurrency, parameters);
    }

    private object selectPage(Database db, string where, int start, int pageSize, string sort, bool concurrency, params Parameter[] parameters)
    {
        sort = sort ?? '[SortDefault]';
        string sql =
@'[SqlPage]';
        sql = string.Format(sql, (start + 1), (start + pageSize), sort, (where ?? ''));
        return readReader(db, sql, concurrency, parameters);
    }

    public override object SelectPage(Database db, object _filter, int start, int pageSize = 20, string sort = null, EnumDbFilter mode = EnumDbFilter.AndEqual, int commandTimeout = 30, bool concurrency = false, params Parameter[] parameters)
    {
        [TypeName] filter = ([TypeName])_filter;
        sort = sort ?? '[SortDefault]';
        StringBuilder where = new StringBuilder();
        string op = (mode == EnumDbFilter.AndLike || mode == EnumDbFilter.AndEqual) ? ' AND ' : ' OR ';

        List<[TypeName]> list = new List<[TypeName]>();

        using (var cmd = db.NewCommand('', commandTimeout, CommandType.Text))
        {
            int index = 0;
[ColumnsFilter]

        string w = where.ToString();
        string sql =
@'[SqlPage]';
        sql = string.Format(sql, (start + 1), (start + pageSize), sort, (where.Length > 0 ?  ' WHERE ' + w : ''));

            cmd.CommandText = sql;

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    [TypeName] value = new [TypeName]();
                    readRecord(dr, ref value);
                    [!POCO]if (concurrency)
                    [!POCO]    value.RecordOriginal = value.CloneT();
                    list.Add(value);
                }
            }
        }
        return list;
    }

    public override object SelectTop(Database db, int top)
    {
        return selectTop(db, top, false);
    }

    public override object SelectTop(Database db, int top, bool concurrency)
    {
        return selectTop(db, top, concurrency);
    }

    public override object SelectTop(Database db, int top, string where)
    {
        return selectTop(db, top, where, false);
    }

    public override object SelectTop(Database db, int top, string where, bool concurrency)
    {
        return selectTop(db, top, where, concurrency);
    }

    public override long Count(Database db)
    {
        return count(db, null);
    }

    public override long Count(Database db, string where)
    {
        return count(db, where);
    }

    public override long Max(Database db, string columnName)
    {
        return max(db, columnName, null);
    }

    public override long Max(Database db, string columnName, string where)
    {
        return max(db, columnName, where);
    }

    public override object SelectColumns(Database db, bool concurrency, params string[] columns)
    {
        return selectColumns(db, concurrency, columns);
    }

    public override object SelectColumns(Database db, string where, bool concurrency, params string[] columns)
    {
        return selectColumns(db, where, concurrency, columns);
    }

    public override object SelectSingle(Database db)
    {
        return selectSingle(db, false);
    }

    public override object SelectSingle(Database db, bool concurrency)
    {
        return selectSingle(db, concurrency);
    }

    public override object SelectSingle(Database db, string where)
    {
        return selectSingle(db, where, false);
    }

    public override object SelectSingle(Database db, string where, bool concurrency)
    {
        return selectSingle(db, where, concurrency);
    }

    public override object SelectSingle(Database db, string where, params Parameter[] parameters)
    {
        string sql = '[Sql] where ' + where;
        return readReaderSingle(db, sql, parameters);
    }

    public override object SelectSingle(Database db, object _filter, EnumDbFilter mode = EnumDbFilter.AndEqual, bool concurrency = false, int commandTimeout = 30)
    {
        [TypeName] filter = ([TypeName])_filter;
        string sql = '[Sql] {0} {1} {2}';

        StringBuilder where = new StringBuilder();
        string op = (mode == EnumDbFilter.AndLike || mode == EnumDbFilter.AndEqual) ? ' AND ' : ' OR ';

        [TypeName] ret = null;

        using (var cmd = db.NewCommand('', commandTimeout, CommandType.Text))
        {
            int index = 0;
[ColumnsFilter]

            sql = string.Format(sql, (where.Length > 0 ? ' WHERE ' + where.ToString() : ''), '', '');
            sql = db.Provider.SetTop(sql, 1);
            cmd.CommandText = sql;

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    ret = new [TypeName]();
                    readRecord(dr, ref ret);
                    [!POCO]if (concurrency)
                    [!POCO]    ret.RecordOriginal = ret.CloneT();
                }
            }
        }
        return ret;
    }

    public override object SelectArray(Database db)
    {
        return select(db, false);
    }

    public override object SelectArray(Database db, bool concurrency)
    {
        return select(db, concurrency).ToArray();
    }

    public override object SelectArray(Database db, string where)
    {
        return select(db, where, false);
    }

    public override object SelectArray(Database db, string where, bool concurrency)
    {
        return select(db, where, concurrency).ToArray();
    }

    public override string SelectToJson(Database db)
    {
        return SelectToJson(db, false);
    }

    public override string SelectToJson(Database db, bool concurrency)
    {
        return SerializeToJson<[TypeName]>(select(db, concurrency));
    }

    public override string SelectToJson(Database db, string where)
    {
        return SelectToJson(db, where, false);
    }

    public override string SelectToJson(Database db, string where, bool concurrency)
    {
        return SerializeToJson<[TypeName]>(select(db, where, concurrency));
    }

    public override object Query(Database db, string sql)
    {
        return readReaderSql(db, sql, false);
    }

    public override object Query(Database db, string sql, bool concurrency)
    {
        return readReaderSql(db, sql, concurrency);
    }

    public override object SelectByPk(Database db, object instance)
    {
        return selectByPk(db, instance, true);
    }

    public override object SelectByPk(Database db, object instance, bool concurrency)
    {
        return selectByPk(db, instance, concurrency);
    }

    public override string UpdateFromJson(Database db, string json, EnumSaveMode saveMode)
    {
        [TypeName] rec = DeserializeFromJson<[TypeName]>(json);
        Update(db, rec, saveMode);
        return SerializeToJson<[TypeName]>(rec);
    }

    public override string SaveFromJson(Database db, string json, EnumSaveMode saveMode)
    {
        [TypeName] rec = DeserializeFromJson<[TypeName]>(json);
        Save(db, rec, saveMode);
        return SerializeToJson<[TypeName]>(rec);
    }

    public override string SaveListFromJson(Database db, string json, EnumSaveMode saveMode, bool continueOnError)
    {
        List<[TypeName]> rec = DeserializeFromJson<List<[TypeName]>>(json);
        SaveList(db, rec, saveMode, continueOnError);
        return SerializeToJson<List<[TypeName]>>(rec);
    }

    public override int Insert(Database db, object instance)
    {
        return insert(db, instance);
    }

    public override int Insert(Database db, object instance, EnumSaveMode saveMode, bool concurrency = false)
    {
        if (saveMode == EnumSaveMode.None)
            return insert(db, instance);
        else
            return insertRequery(db, instance, concurrency);
    }

    public static string Concat(string endSeparator, params object[] values)
    {
        return Concat(null, endSeparator, values);
    }

    public static string Concat(string beforeSeparator, string endSeparator, params object[] values)
    {
        System.Text.StringBuilder text = new System.Text.StringBuilder();
        for (int i = 0; i < values.Length; i++)
        {
            if (beforeSeparator != null)
                text.Append(beforeSeparator);
            text.Append(values[i].ToString());
            if (i < values.Length - 1 && endSeparator != null)
                text.Append(endSeparator);
        }
        return text.ToString();
    }

    public override int InsertXml(Database db, object instance, EnumSaveMode saveMode = EnumSaveMode.None)
    {
[InsertXml]
    }

    public void CheckConcurrency(Database db, [TypeName] value)
    {
        [!POCO]var original = value.RecordOriginal as [TypeName];
        [!POCO]if (original == null)
        [!POCO]    return;
        [!POCO]var current = ([TypeName])SelectByPk(db, original);
        [!POCO]if (current == null)
        [!POCO]    throw new Exception('Original record has been deleted');
        [!POCO]else if (!original.IsEqual(current))
        [!POCO]    throw new DBConcurrencyException();
    }

    public override DataClass CloneT()
    {
        return new [ClassName]();
    }

}   
";

        #endregion DATACLASSTEMPLATE


    }

}
