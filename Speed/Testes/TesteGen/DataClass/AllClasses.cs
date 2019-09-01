using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Speed;
using Speed.Data;


[Serializable]
public partial class DataClassCustomer : DataClass
{

//    public Record CreateInstance()
//    {
//        return new TestGen.SqlServer.Data.Customer();
//    }

    private void readRecord(DbDataReader dr, ref TestGen.SqlServer.Data.Customer value)
    {
        value.RecordStatus = RecordStatus.Existing;
        value.CustomerId = dr.GetInt32(0);
        value.CustomerName = dr.GetString(1);

    }

    private void readRecordSql(DbDataReader dr, List<string> names, ref TestGen.SqlServer.Data.Customer value)
    {
        value.RecordStatus = RecordStatus.Existing;
        if (names.Contains("CustomerId")) value.CustomerId = dr.GetInt32(dr.GetOrdinal(("CustomerId")));
        if (names.Contains("CustomerName")) value.CustomerName = dr.GetString(dr.GetOrdinal(("CustomerName")));

    }

    private List<TestGen.SqlServer.Data.Customer> readReader(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Customer> list = new List<TestGen.SqlServer.Data.Customer>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Customer value = new TestGen.SqlServer.Data.Customer();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.Customer> readReader(Database db, string sql, bool concurrency, params Parameter[] parameters)
    {
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.Customer> list = new List<TestGen.SqlServer.Data.Customer>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Customer value = new TestGen.SqlServer.Data.Customer();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private TestGen.SqlServer.Data.Customer readReaderSingle(Database db, string sql, bool concurrency)
    {
        TestGen.SqlServer.Data.Customer value = null;
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Customer> list = new List<TestGen.SqlServer.Data.Customer>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.Customer();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
            }
            return value;
        }
    }

    private TestGen.SqlServer.Data.Customer readReaderSingle(Database db, string sql, Parameter[] parameters)
    {
        TestGen.SqlServer.Data.Customer value = null;
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.Customer> list = new List<TestGen.SqlServer.Data.Customer>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.Customer();
                readRecord(dr, ref value);
            }
            return value;
        }
    }

    private List<TestGen.SqlServer.Data.Customer> readReaderSql(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Customer> list = new List<TestGen.SqlServer.Data.Customer>();
            // pega os nomes das colunas continas no sql
            List<string> names = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
                names.Add(dr.GetName(i));
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Customer value = new TestGen.SqlServer.Data.Customer();
                readRecordSql(dr, names, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.Customer> select(Database db, bool concurrency)
    {
        string sql = "select CustomerId, CustomerName from dbo.Customer";
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Customer> select(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select CustomerId, CustomerName from dbo.Customer";
        else
            sql = "select CustomerId, CustomerName from dbo.Customer where " + where;
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Customer> select(Database db, string where, params Parameter[] parameters)
    {
        string sql = "select CustomerId, CustomerName from dbo.Customer where " + where;
        return readReader(db, sql, false, parameters);
    }

    private List<TestGen.SqlServer.Data.Customer> select(Database db, string where, bool concurrency, params Parameter[] parameters)
    {
        string sql = "select CustomerId, CustomerName from dbo.Customer where " + where;
        return readReader(db, sql, concurrency, parameters);
    }

    private List<TestGen.SqlServer.Data.Customer> selectTop(Database db, int top, bool concurrency)
    {
        // string sql = string.Format("db.Provider.SetTop(\"select CustomerId, CustomerName from dbo.Customer\", {0})", top);
        string sql = "select CustomerId, CustomerName from dbo.Customer";
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Customer> selectTop(Database db, int top, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select CustomerId, CustomerName from dbo.Customer";
        else
            sql = "select CustomerId, CustomerName from dbo.Customer where " + where;
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private long count(Database db, string where)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select count(*) from dbo.Customer";
        else
            sql = "select count(*) from dbo.Customer where " + where;
        return db.ExecuteInt64(sql);
    }

    private List<TestGen.SqlServer.Data.Customer> selectColumns(Database db, bool concurrency, params string[] columns)
    {
        string sql = "select " + Concat(", ", columns) + " from dbo.Customer";
        return readReaderSql(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Customer> selectColumns(Database db, string where, bool concurrency, params string[] columns)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select " + Concat(", ", columns) + " from dbo.Customer ";
        else
            sql = "select " + Concat(", ", columns) + " from dbo.Customer where " + where;
        return readReaderSql(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.Customer selectSingle(Database db, bool concurrency)
    {
        string sql = "select CustomerId, CustomerName from dbo.Customer";
        return readReaderSingle(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.Customer selectSingle(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select CustomerId, CustomerName from dbo.Customer";
        else
            sql = "select CustomerId, CustomerName from dbo.Customer where " + where;
        return readReaderSingle(db, sql, concurrency);
    }

    private object selectByPk(Database db, object instance, bool concurrency)
    {
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        TestGen.SqlServer.Data.Customer value2 = new TestGen.SqlServer.Data.Customer();
        string sql = "select CustomerId, CustomerName from dbo.Customer where CustomerId = @P_CustomerId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_CustomerId", GetValue(value.CustomerId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value2);
                    if (concurrency)
                        value2.RecordOriginal = value2.CloneT();
                    return value2;
                }
                else
                    return null;
            }
        }
    }

    private int insert(Database db, object instance)
    {
        
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        string sql = "insert into dbo.Customer (CustomerName) values (@P_CustomerName)";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_CustomerName", "String", GetValue(value.CustomerName));

            value.RecordStatus = RecordStatus.Existing;
            return db.ExecuteNonQuery(cmd);
        }

    }

    private int insertRequery(Database db, object instance, bool concurrency)
    {
        
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        string sql;
        sql = "insert into dbo.Customer (CustomerName) values (@P_CustomerName);";
        sql += "select CustomerId, CustomerName from dbo.Customer where CustomerId = SCOPE_IDENTITY();";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_CustomerName", "String", GetValue(value.CustomerName));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        };
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
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql = "update dbo.Customer set CustomerName = @P_CustomerName where CustomerId = @P_OldCustomerId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_CustomerId", "Int32", GetValue(value.CustomerId));
            db.AddWithValue(cmd, "@P_CustomerName", "String", GetValue(value.CustomerName));

            TestGen.SqlServer.Data.Customer old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Customer)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldCustomerId", GetValue(old.CustomerId));

            return db.ExecuteNonQuery(cmd);
        }
    }

    private int updateRequery(Database db, object instance, bool concurrency = false)
    {
        
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql;
        sql = "update dbo.Customer set CustomerName = @P_CustomerName where CustomerId = @P_OldCustomerId;";
        sql += "\r\n" + "select CustomerId, CustomerName from dbo.Customer where CustomerId = @P_CustomerId;";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_CustomerId", "Int32", GetValue(value.CustomerId));
            db.AddWithValue(cmd, "@P_CustomerName", "String", GetValue(value.CustomerName));

            TestGen.SqlServer.Data.Customer old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Customer)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldCustomerId", GetValue(old.CustomerId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        }

    }

    public override int Truncate(Database db)
    {
        string sql = "truncate table dbo.Customer";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db)
    {
        string sql = "delete from dbo.Customer";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, object instance)
    {
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        string sql = "delete from dbo.Customer where CustomerId = @P_OldCustomerId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            TestGen.SqlServer.Data.Customer old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Customer)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldCustomerId", GetValue(old.CustomerId));

            return db.ExecuteNonQuery(cmd);
        }
    }

    public override int Delete(Database db, string where)
    {
        string sql = "delete from dbo.Customer where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, string where, params Parameter[] parameters)
    {
        string sql = "delete from dbo.Customer where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Delete(Database db, string where, int commandTimeout, params Parameter[] parameters)
    {
        string sql = "delete from dbo.Customer where " + where;
        using (DbCommand cmd = db.NewCommand(sql, commandTimeout))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Save(Database db, object instance)
    {
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return update(db, value);
        else if (value.RecordStatus == RecordStatus.New)
            return insert(db, value);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override int Save(Database db, object instance, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.Customer value = (TestGen.SqlServer.Data.Customer)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return Update(db, value, saveMode);
        else if (value.RecordStatus == RecordStatus.New)
            return Insert(db, value, saveMode);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override void SaveList(Database db, object instance, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.Customer> values = (List<TestGen.SqlServer.Data.Customer>)instance;
        bool isTran = db.UsingTransaction;

        if (!isTran)
            db.BeginTransaction();

        foreach (TestGen.SqlServer.Data.Customer value in values)
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
        string sql = "select CustomerId, CustomerName from dbo.Customer where " + where;
        return readReaderSingle(db, sql, parameters);
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
        return SerializeToJson<TestGen.SqlServer.Data.Customer>(select(db, concurrency));
    }

    public override string SelectToJson(Database db, string where)
    {
        return SelectToJson(db, where, false);
    }

    public override string SelectToJson(Database db, string where, bool concurrency)
    {
        return SerializeToJson<TestGen.SqlServer.Data.Customer>(select(db, where, concurrency));
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
        TestGen.SqlServer.Data.Customer rec = DeserializeFromJson<TestGen.SqlServer.Data.Customer>(json);
        Update(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.Customer>(rec);
    }

    public override string SaveFromJson(Database db, string json, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.Customer rec = DeserializeFromJson<TestGen.SqlServer.Data.Customer>(json);
        Save(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.Customer>(rec);
    }

    public override string SaveListFromJson(Database db, string json, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.Customer> rec = DeserializeFromJson<List<TestGen.SqlServer.Data.Customer>>(json);
        SaveList(db, rec, saveMode, continueOnError);
        return SerializeToJson<List<TestGen.SqlServer.Data.Customer>>(rec);
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

        List<TestGen.SqlServer.Data.Customer> values = (List<TestGen.SqlServer.Data.Customer>)instance;
        
        // create DataTable
        DataTable tb = new DataTable("Table");
tb.Columns.Add("CustomerName", typeof(String));
        foreach (var value in values)
        {
            DataRow row = tb.NewRow();
row["CustomerName"] = value.CustomerName;
            tb.Rows.Add(row);
        }

string sql = 
@"
declare @xmlData xml = @Xml
BEGIN
    DECLARE @idInt int;

    EXEC sp_xml_preparedocument @idInt OUTPUT, @xmlData;

    INSERT INTO dbo.Customer(CustomerName)
    SELECT CustomerName
    FROM OPENXML (@idInt, '/DocumentElement/*')
    WITH 
    (
        CustomerName varchar(100) 'CustomerName'
    )

    select CAST( SCOPE_IDENTITY() - @@ROWCOUNT + 1 AS INT);
    exec sp_xml_removedocument @idInt;
END;";

        using (System.IO.StringWriter swStringWriter = new System.IO.StringWriter())
        {
            // Datatable as XML format 
            tb.WriteXml(swStringWriter);
            // Datatable as XML string 
            string xml = swStringWriter.ToString();
            using (var cmd = db.NewCommand(sql, 0))
            {
                db.AddParameter(cmd, "@xml", DbType.Xml, xml, ParameterDirection.Input);
                int firstId = Convert.ToInt32(cmd.ExecuteScalar());
                return firstId;
            }
        }

    }

    public void CheckConcurrency(Database db, TestGen.SqlServer.Data.Customer value)
    {
        var original = (TestGen.SqlServer.Data.Customer)value.RecordOriginal;
        var current = (TestGen.SqlServer.Data.Customer)SelectByPk(db, original);
        if (current == null)
            throw new Exception("Original record has been deleted");
        else if (!original.IsEqual(current))
            throw new DBConcurrencyException("Concurrency exception");
    }

}


[Serializable]
public partial class DataClassSale : DataClass
{

//    public Record CreateInstance()
//    {
//        return new TestGen.SqlServer.Data.Sale();
//    }

    private void readRecord(DbDataReader dr, ref TestGen.SqlServer.Data.Sale value)
    {
        value.RecordStatus = RecordStatus.Existing;
        value.SaleId = dr.GetInt32(0);
        value.SaleCustomerId = dr.GetInt32(1);

    }

    private void readRecordSql(DbDataReader dr, List<string> names, ref TestGen.SqlServer.Data.Sale value)
    {
        value.RecordStatus = RecordStatus.Existing;
        if (names.Contains("SaleId")) value.SaleId = dr.GetInt32(dr.GetOrdinal(("SaleId")));
        if (names.Contains("SaleCustomerId")) value.SaleCustomerId = dr.GetInt32(dr.GetOrdinal(("SaleCustomerId")));

    }

    private List<TestGen.SqlServer.Data.Sale> readReader(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Sale> list = new List<TestGen.SqlServer.Data.Sale>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Sale value = new TestGen.SqlServer.Data.Sale();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.Sale> readReader(Database db, string sql, bool concurrency, params Parameter[] parameters)
    {
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.Sale> list = new List<TestGen.SqlServer.Data.Sale>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Sale value = new TestGen.SqlServer.Data.Sale();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private TestGen.SqlServer.Data.Sale readReaderSingle(Database db, string sql, bool concurrency)
    {
        TestGen.SqlServer.Data.Sale value = null;
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Sale> list = new List<TestGen.SqlServer.Data.Sale>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.Sale();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
            }
            return value;
        }
    }

    private TestGen.SqlServer.Data.Sale readReaderSingle(Database db, string sql, Parameter[] parameters)
    {
        TestGen.SqlServer.Data.Sale value = null;
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.Sale> list = new List<TestGen.SqlServer.Data.Sale>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.Sale();
                readRecord(dr, ref value);
            }
            return value;
        }
    }

    private List<TestGen.SqlServer.Data.Sale> readReaderSql(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Sale> list = new List<TestGen.SqlServer.Data.Sale>();
            // pega os nomes das colunas continas no sql
            List<string> names = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
                names.Add(dr.GetName(i));
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Sale value = new TestGen.SqlServer.Data.Sale();
                readRecordSql(dr, names, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.Sale> select(Database db, bool concurrency)
    {
        string sql = "select SaleId, SaleCustomerId from dbo.Sale";
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sale> select(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select SaleId, SaleCustomerId from dbo.Sale";
        else
            sql = "select SaleId, SaleCustomerId from dbo.Sale where " + where;
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sale> select(Database db, string where, params Parameter[] parameters)
    {
        string sql = "select SaleId, SaleCustomerId from dbo.Sale where " + where;
        return readReader(db, sql, false, parameters);
    }

    private List<TestGen.SqlServer.Data.Sale> select(Database db, string where, bool concurrency, params Parameter[] parameters)
    {
        string sql = "select SaleId, SaleCustomerId from dbo.Sale where " + where;
        return readReader(db, sql, concurrency, parameters);
    }

    private List<TestGen.SqlServer.Data.Sale> selectTop(Database db, int top, bool concurrency)
    {
        // string sql = string.Format("db.Provider.SetTop(\"select SaleId, SaleCustomerId from dbo.Sale\", {0})", top);
        string sql = "select SaleId, SaleCustomerId from dbo.Sale";
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sale> selectTop(Database db, int top, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select SaleId, SaleCustomerId from dbo.Sale";
        else
            sql = "select SaleId, SaleCustomerId from dbo.Sale where " + where;
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private long count(Database db, string where)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select count(*) from dbo.Sale";
        else
            sql = "select count(*) from dbo.Sale where " + where;
        return db.ExecuteInt64(sql);
    }

    private List<TestGen.SqlServer.Data.Sale> selectColumns(Database db, bool concurrency, params string[] columns)
    {
        string sql = "select " + Concat(", ", columns) + " from dbo.Sale";
        return readReaderSql(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sale> selectColumns(Database db, string where, bool concurrency, params string[] columns)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select " + Concat(", ", columns) + " from dbo.Sale ";
        else
            sql = "select " + Concat(", ", columns) + " from dbo.Sale where " + where;
        return readReaderSql(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.Sale selectSingle(Database db, bool concurrency)
    {
        string sql = "select SaleId, SaleCustomerId from dbo.Sale";
        return readReaderSingle(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.Sale selectSingle(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select SaleId, SaleCustomerId from dbo.Sale";
        else
            sql = "select SaleId, SaleCustomerId from dbo.Sale where " + where;
        return readReaderSingle(db, sql, concurrency);
    }

    private object selectByPk(Database db, object instance, bool concurrency)
    {
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        TestGen.SqlServer.Data.Sale value2 = new TestGen.SqlServer.Data.Sale();
        string sql = "select SaleId, SaleCustomerId from dbo.Sale where SaleId = @P_SaleId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_SaleId", GetValue(value.SaleId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value2);
                    if (concurrency)
                        value2.RecordOriginal = value2.CloneT();
                    return value2;
                }
                else
                    return null;
            }
        }
    }

    private int insert(Database db, object instance)
    {
        
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        string sql = "insert into dbo.Sale (SaleCustomerId) values (@P_SaleCustomerId)";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_SaleCustomerId", "Int32", GetValue(value.SaleCustomerId));

            value.RecordStatus = RecordStatus.Existing;
            return db.ExecuteNonQuery(cmd);
        }

    }

    private int insertRequery(Database db, object instance, bool concurrency)
    {
        
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        string sql;
        sql = "insert into dbo.Sale (SaleCustomerId) values (@P_SaleCustomerId);";
        sql += "select SaleId, SaleCustomerId from dbo.Sale where SaleId = SCOPE_IDENTITY();";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_SaleCustomerId", "Int32", GetValue(value.SaleCustomerId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        };
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
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql = "update dbo.Sale set SaleCustomerId = @P_SaleCustomerId where SaleId = @P_OldSaleId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_SaleId", "Int32", GetValue(value.SaleId));
            db.AddWithValue(cmd, "@P_SaleCustomerId", "Int32", GetValue(value.SaleCustomerId));

            TestGen.SqlServer.Data.Sale old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Sale)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldSaleId", GetValue(old.SaleId));

            return db.ExecuteNonQuery(cmd);
        }
    }

    private int updateRequery(Database db, object instance, bool concurrency = false)
    {
        
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql;
        sql = "update dbo.Sale set SaleCustomerId = @P_SaleCustomerId where SaleId = @P_OldSaleId;";
        sql += "\r\n" + "select SaleId, SaleCustomerId from dbo.Sale where SaleId = @P_SaleId;";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_SaleId", "Int32", GetValue(value.SaleId));
            db.AddWithValue(cmd, "@P_SaleCustomerId", "Int32", GetValue(value.SaleCustomerId));

            TestGen.SqlServer.Data.Sale old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Sale)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldSaleId", GetValue(old.SaleId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        }

    }

    public override int Truncate(Database db)
    {
        string sql = "truncate table dbo.Sale";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db)
    {
        string sql = "delete from dbo.Sale";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, object instance)
    {
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        string sql = "delete from dbo.Sale where SaleId = @P_OldSaleId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            TestGen.SqlServer.Data.Sale old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Sale)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldSaleId", GetValue(old.SaleId));

            return db.ExecuteNonQuery(cmd);
        }
    }

    public override int Delete(Database db, string where)
    {
        string sql = "delete from dbo.Sale where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, string where, params Parameter[] parameters)
    {
        string sql = "delete from dbo.Sale where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Delete(Database db, string where, int commandTimeout, params Parameter[] parameters)
    {
        string sql = "delete from dbo.Sale where " + where;
        using (DbCommand cmd = db.NewCommand(sql, commandTimeout))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Save(Database db, object instance)
    {
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return update(db, value);
        else if (value.RecordStatus == RecordStatus.New)
            return insert(db, value);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override int Save(Database db, object instance, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.Sale value = (TestGen.SqlServer.Data.Sale)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return Update(db, value, saveMode);
        else if (value.RecordStatus == RecordStatus.New)
            return Insert(db, value, saveMode);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override void SaveList(Database db, object instance, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.Sale> values = (List<TestGen.SqlServer.Data.Sale>)instance;
        bool isTran = db.UsingTransaction;

        if (!isTran)
            db.BeginTransaction();

        foreach (TestGen.SqlServer.Data.Sale value in values)
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
        string sql = "select SaleId, SaleCustomerId from dbo.Sale where " + where;
        return readReaderSingle(db, sql, parameters);
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
        return SerializeToJson<TestGen.SqlServer.Data.Sale>(select(db, concurrency));
    }

    public override string SelectToJson(Database db, string where)
    {
        return SelectToJson(db, where, false);
    }

    public override string SelectToJson(Database db, string where, bool concurrency)
    {
        return SerializeToJson<TestGen.SqlServer.Data.Sale>(select(db, where, concurrency));
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
        TestGen.SqlServer.Data.Sale rec = DeserializeFromJson<TestGen.SqlServer.Data.Sale>(json);
        Update(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.Sale>(rec);
    }

    public override string SaveFromJson(Database db, string json, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.Sale rec = DeserializeFromJson<TestGen.SqlServer.Data.Sale>(json);
        Save(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.Sale>(rec);
    }

    public override string SaveListFromJson(Database db, string json, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.Sale> rec = DeserializeFromJson<List<TestGen.SqlServer.Data.Sale>>(json);
        SaveList(db, rec, saveMode, continueOnError);
        return SerializeToJson<List<TestGen.SqlServer.Data.Sale>>(rec);
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

        List<TestGen.SqlServer.Data.Sale> values = (List<TestGen.SqlServer.Data.Sale>)instance;
        
        // create DataTable
        DataTable tb = new DataTable("Table");
tb.Columns.Add("SaleCustomerId", typeof(Int32));
        foreach (var value in values)
        {
            DataRow row = tb.NewRow();
row["SaleCustomerId"] = value.SaleCustomerId;
            tb.Rows.Add(row);
        }

string sql = 
@"
declare @xmlData xml = @Xml
BEGIN
    DECLARE @idInt int;

    EXEC sp_xml_preparedocument @idInt OUTPUT, @xmlData;

    INSERT INTO dbo.Sale(SaleCustomerId)
    SELECT SaleCustomerId
    FROM OPENXML (@idInt, '/DocumentElement/*')
    WITH 
    (
        SaleCustomerId int 'SaleCustomerId'
    )

    select CAST( SCOPE_IDENTITY() - @@ROWCOUNT + 1 AS INT);
    exec sp_xml_removedocument @idInt;
END;";

        using (System.IO.StringWriter swStringWriter = new System.IO.StringWriter())
        {
            // Datatable as XML format 
            tb.WriteXml(swStringWriter);
            // Datatable as XML string 
            string xml = swStringWriter.ToString();
            using (var cmd = db.NewCommand(sql, 0))
            {
                db.AddParameter(cmd, "@xml", DbType.Xml, xml, ParameterDirection.Input);
                int firstId = Convert.ToInt32(cmd.ExecuteScalar());
                return firstId;
            }
        }

    }

    public void CheckConcurrency(Database db, TestGen.SqlServer.Data.Sale value)
    {
        var original = (TestGen.SqlServer.Data.Sale)value.RecordOriginal;
        var current = (TestGen.SqlServer.Data.Sale)SelectByPk(db, original);
        if (current == null)
            throw new Exception("Original record has been deleted");
        else if (!original.IsEqual(current))
            throw new DBConcurrencyException("Concurrency exception");
    }

}


[Serializable]
public partial class DataClassSaleDetail : DataClass
{

//    public Record CreateInstance()
//    {
//        return new TestGen.SqlServer.Data.SaleDetail();
//    }

    private void readRecord(DbDataReader dr, ref TestGen.SqlServer.Data.SaleDetail value)
    {
        value.RecordStatus = RecordStatus.Existing;
        value.DetailId = dr.GetInt32(0);
        value.DetSaleId = dr.GetInt32(1);

    }

    private void readRecordSql(DbDataReader dr, List<string> names, ref TestGen.SqlServer.Data.SaleDetail value)
    {
        value.RecordStatus = RecordStatus.Existing;
        if (names.Contains("DetailId")) value.DetailId = dr.GetInt32(dr.GetOrdinal(("DetailId")));
        if (names.Contains("DetSaleId")) value.DetSaleId = dr.GetInt32(dr.GetOrdinal(("DetSaleId")));

    }

    private List<TestGen.SqlServer.Data.SaleDetail> readReader(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.SaleDetail> list = new List<TestGen.SqlServer.Data.SaleDetail>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.SaleDetail value = new TestGen.SqlServer.Data.SaleDetail();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.SaleDetail> readReader(Database db, string sql, bool concurrency, params Parameter[] parameters)
    {
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.SaleDetail> list = new List<TestGen.SqlServer.Data.SaleDetail>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.SaleDetail value = new TestGen.SqlServer.Data.SaleDetail();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private TestGen.SqlServer.Data.SaleDetail readReaderSingle(Database db, string sql, bool concurrency)
    {
        TestGen.SqlServer.Data.SaleDetail value = null;
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.SaleDetail> list = new List<TestGen.SqlServer.Data.SaleDetail>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.SaleDetail();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
            }
            return value;
        }
    }

    private TestGen.SqlServer.Data.SaleDetail readReaderSingle(Database db, string sql, Parameter[] parameters)
    {
        TestGen.SqlServer.Data.SaleDetail value = null;
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.SaleDetail> list = new List<TestGen.SqlServer.Data.SaleDetail>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.SaleDetail();
                readRecord(dr, ref value);
            }
            return value;
        }
    }

    private List<TestGen.SqlServer.Data.SaleDetail> readReaderSql(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.SaleDetail> list = new List<TestGen.SqlServer.Data.SaleDetail>();
            // pega os nomes das colunas continas no sql
            List<string> names = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
                names.Add(dr.GetName(i));
            while (dr.Read())
            {
                TestGen.SqlServer.Data.SaleDetail value = new TestGen.SqlServer.Data.SaleDetail();
                readRecordSql(dr, names, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.SaleDetail> select(Database db, bool concurrency)
    {
        string sql = "select DetailId, DetSaleId from dbo.SaleDetail";
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.SaleDetail> select(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select DetailId, DetSaleId from dbo.SaleDetail";
        else
            sql = "select DetailId, DetSaleId from dbo.SaleDetail where " + where;
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.SaleDetail> select(Database db, string where, params Parameter[] parameters)
    {
        string sql = "select DetailId, DetSaleId from dbo.SaleDetail where " + where;
        return readReader(db, sql, false, parameters);
    }

    private List<TestGen.SqlServer.Data.SaleDetail> select(Database db, string where, bool concurrency, params Parameter[] parameters)
    {
        string sql = "select DetailId, DetSaleId from dbo.SaleDetail where " + where;
        return readReader(db, sql, concurrency, parameters);
    }

    private List<TestGen.SqlServer.Data.SaleDetail> selectTop(Database db, int top, bool concurrency)
    {
        // string sql = string.Format("db.Provider.SetTop(\"select DetailId, DetSaleId from dbo.SaleDetail\", {0})", top);
        string sql = "select DetailId, DetSaleId from dbo.SaleDetail";
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.SaleDetail> selectTop(Database db, int top, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select DetailId, DetSaleId from dbo.SaleDetail";
        else
            sql = "select DetailId, DetSaleId from dbo.SaleDetail where " + where;
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private long count(Database db, string where)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select count(*) from dbo.SaleDetail";
        else
            sql = "select count(*) from dbo.SaleDetail where " + where;
        return db.ExecuteInt64(sql);
    }

    private List<TestGen.SqlServer.Data.SaleDetail> selectColumns(Database db, bool concurrency, params string[] columns)
    {
        string sql = "select " + Concat(", ", columns) + " from dbo.SaleDetail";
        return readReaderSql(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.SaleDetail> selectColumns(Database db, string where, bool concurrency, params string[] columns)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select " + Concat(", ", columns) + " from dbo.SaleDetail ";
        else
            sql = "select " + Concat(", ", columns) + " from dbo.SaleDetail where " + where;
        return readReaderSql(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.SaleDetail selectSingle(Database db, bool concurrency)
    {
        string sql = "select DetailId, DetSaleId from dbo.SaleDetail";
        return readReaderSingle(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.SaleDetail selectSingle(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select DetailId, DetSaleId from dbo.SaleDetail";
        else
            sql = "select DetailId, DetSaleId from dbo.SaleDetail where " + where;
        return readReaderSingle(db, sql, concurrency);
    }

    private object selectByPk(Database db, object instance, bool concurrency)
    {
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        TestGen.SqlServer.Data.SaleDetail value2 = new TestGen.SqlServer.Data.SaleDetail();
        string sql = "select DetailId, DetSaleId from dbo.SaleDetail where DetailId = @P_DetailId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_DetailId", GetValue(value.DetailId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value2);
                    if (concurrency)
                        value2.RecordOriginal = value2.CloneT();
                    return value2;
                }
                else
                    return null;
            }
        }
    }

    private int insert(Database db, object instance)
    {
        
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        string sql = "insert into dbo.SaleDetail (DetSaleId) values (@P_DetSaleId)";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_DetSaleId", "Int32", GetValue(value.DetSaleId));

            value.RecordStatus = RecordStatus.Existing;
            return db.ExecuteNonQuery(cmd);
        }

    }

    private int insertRequery(Database db, object instance, bool concurrency)
    {
        
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        string sql;
        sql = "insert into dbo.SaleDetail (DetSaleId) values (@P_DetSaleId);";
        sql += "select DetailId, DetSaleId from dbo.SaleDetail where DetailId = SCOPE_IDENTITY();";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_DetSaleId", "Int32", GetValue(value.DetSaleId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        };
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
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql = "update dbo.SaleDetail set DetSaleId = @P_DetSaleId where DetailId = @P_OldDetailId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_DetailId", "Int32", GetValue(value.DetailId));
            db.AddWithValue(cmd, "@P_DetSaleId", "Int32", GetValue(value.DetSaleId));

            TestGen.SqlServer.Data.SaleDetail old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.SaleDetail)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldDetailId", GetValue(old.DetailId));

            return db.ExecuteNonQuery(cmd);
        }
    }

    private int updateRequery(Database db, object instance, bool concurrency = false)
    {
        
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql;
        sql = "update dbo.SaleDetail set DetSaleId = @P_DetSaleId where DetailId = @P_OldDetailId;";
        sql += "\r\n" + "select DetailId, DetSaleId from dbo.SaleDetail where DetailId = @P_DetailId;";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_DetailId", "Int32", GetValue(value.DetailId));
            db.AddWithValue(cmd, "@P_DetSaleId", "Int32", GetValue(value.DetSaleId));

            TestGen.SqlServer.Data.SaleDetail old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.SaleDetail)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldDetailId", GetValue(old.DetailId));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        }

    }

    public override int Truncate(Database db)
    {
        string sql = "truncate table dbo.SaleDetail";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db)
    {
        string sql = "delete from dbo.SaleDetail";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, object instance)
    {
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        string sql = "delete from dbo.SaleDetail where DetailId = @P_OldDetailId";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            TestGen.SqlServer.Data.SaleDetail old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.SaleDetail)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldDetailId", GetValue(old.DetailId));

            return db.ExecuteNonQuery(cmd);
        }
    }

    public override int Delete(Database db, string where)
    {
        string sql = "delete from dbo.SaleDetail where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, string where, params Parameter[] parameters)
    {
        string sql = "delete from dbo.SaleDetail where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Delete(Database db, string where, int commandTimeout, params Parameter[] parameters)
    {
        string sql = "delete from dbo.SaleDetail where " + where;
        using (DbCommand cmd = db.NewCommand(sql, commandTimeout))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Save(Database db, object instance)
    {
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return update(db, value);
        else if (value.RecordStatus == RecordStatus.New)
            return insert(db, value);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override int Save(Database db, object instance, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.SaleDetail value = (TestGen.SqlServer.Data.SaleDetail)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return Update(db, value, saveMode);
        else if (value.RecordStatus == RecordStatus.New)
            return Insert(db, value, saveMode);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override void SaveList(Database db, object instance, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.SaleDetail> values = (List<TestGen.SqlServer.Data.SaleDetail>)instance;
        bool isTran = db.UsingTransaction;

        if (!isTran)
            db.BeginTransaction();

        foreach (TestGen.SqlServer.Data.SaleDetail value in values)
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
        string sql = "select DetailId, DetSaleId from dbo.SaleDetail where " + where;
        return readReaderSingle(db, sql, parameters);
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
        return SerializeToJson<TestGen.SqlServer.Data.SaleDetail>(select(db, concurrency));
    }

    public override string SelectToJson(Database db, string where)
    {
        return SelectToJson(db, where, false);
    }

    public override string SelectToJson(Database db, string where, bool concurrency)
    {
        return SerializeToJson<TestGen.SqlServer.Data.SaleDetail>(select(db, where, concurrency));
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
        TestGen.SqlServer.Data.SaleDetail rec = DeserializeFromJson<TestGen.SqlServer.Data.SaleDetail>(json);
        Update(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.SaleDetail>(rec);
    }

    public override string SaveFromJson(Database db, string json, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.SaleDetail rec = DeserializeFromJson<TestGen.SqlServer.Data.SaleDetail>(json);
        Save(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.SaleDetail>(rec);
    }

    public override string SaveListFromJson(Database db, string json, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.SaleDetail> rec = DeserializeFromJson<List<TestGen.SqlServer.Data.SaleDetail>>(json);
        SaveList(db, rec, saveMode, continueOnError);
        return SerializeToJson<List<TestGen.SqlServer.Data.SaleDetail>>(rec);
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

        List<TestGen.SqlServer.Data.SaleDetail> values = (List<TestGen.SqlServer.Data.SaleDetail>)instance;
        
        // create DataTable
        DataTable tb = new DataTable("Table");
tb.Columns.Add("DetSaleId", typeof(Int32));
        foreach (var value in values)
        {
            DataRow row = tb.NewRow();
row["DetSaleId"] = value.DetSaleId;
            tb.Rows.Add(row);
        }

string sql = 
@"
declare @xmlData xml = @Xml
BEGIN
    DECLARE @idInt int;

    EXEC sp_xml_preparedocument @idInt OUTPUT, @xmlData;

    INSERT INTO dbo.SaleDetail(DetSaleId)
    SELECT DetSaleId
    FROM OPENXML (@idInt, '/DocumentElement/*')
    WITH 
    (
        DetSaleId int 'DetSaleId'
    )

    select CAST( SCOPE_IDENTITY() - @@ROWCOUNT + 1 AS INT);
    exec sp_xml_removedocument @idInt;
END;";

        using (System.IO.StringWriter swStringWriter = new System.IO.StringWriter())
        {
            // Datatable as XML format 
            tb.WriteXml(swStringWriter);
            // Datatable as XML string 
            string xml = swStringWriter.ToString();
            using (var cmd = db.NewCommand(sql, 0))
            {
                db.AddParameter(cmd, "@xml", DbType.Xml, xml, ParameterDirection.Input);
                int firstId = Convert.ToInt32(cmd.ExecuteScalar());
                return firstId;
            }
        }

    }

    public void CheckConcurrency(Database db, TestGen.SqlServer.Data.SaleDetail value)
    {
        var original = (TestGen.SqlServer.Data.SaleDetail)value.RecordOriginal;
        var current = (TestGen.SqlServer.Data.SaleDetail)SelectByPk(db, original);
        if (current == null)
            throw new Exception("Original record has been deleted");
        else if (!original.IsEqual(current))
            throw new DBConcurrencyException("Concurrency exception");
    }

}


[Serializable]
public partial class DataClassSales : DataClass
{

//    public Record CreateInstance()
//    {
//        return new TestGen.SqlServer.Data.Sales();
//    }

    private void readRecord(DbDataReader dr, ref TestGen.SqlServer.Data.Sales value)
    {
        value.RecordStatus = RecordStatus.Existing;
        value.TestPerfID = dr.GetInt32(0);
        value.CarrierTrackingNumber = dr.GetString(1);
        value.OrderQty = dr.GetInt32(2);
        value.ProductID = dr.GetInt32(3);
        value.SpecialOfferID = dr.GetInt32(4);
        value.UnitPrice = dr.GetDecimal(5);
        value.UnitPriceDiscount = dr.GetDecimal(6);
        value.LineTotal = dr.GetDecimal(7);
        value.LongText = dr.GetString(8);
        value.rowguid = dr.GetGuid(9);
        value.ModifiedDate = dr.GetDateTime(10);

    }

    private void readRecordSql(DbDataReader dr, List<string> names, ref TestGen.SqlServer.Data.Sales value)
    {
        value.RecordStatus = RecordStatus.Existing;
        if (names.Contains("TestPerfID")) value.TestPerfID = dr.GetInt32(dr.GetOrdinal(("TestPerfID")));
        if (names.Contains("CarrierTrackingNumber")) value.CarrierTrackingNumber = dr.GetString(dr.GetOrdinal(("CarrierTrackingNumber")));
        if (names.Contains("OrderQty")) value.OrderQty = dr.GetInt32(dr.GetOrdinal(("OrderQty")));
        if (names.Contains("ProductID")) value.ProductID = dr.GetInt32(dr.GetOrdinal(("ProductID")));
        if (names.Contains("SpecialOfferID")) value.SpecialOfferID = dr.GetInt32(dr.GetOrdinal(("SpecialOfferID")));
        if (names.Contains("UnitPrice")) value.UnitPrice = dr.GetDecimal(dr.GetOrdinal(("UnitPrice")));
        if (names.Contains("UnitPriceDiscount")) value.UnitPriceDiscount = dr.GetDecimal(dr.GetOrdinal(("UnitPriceDiscount")));
        if (names.Contains("LineTotal")) value.LineTotal = dr.GetDecimal(dr.GetOrdinal(("LineTotal")));
        if (names.Contains("LongText")) value.LongText = dr.GetString(dr.GetOrdinal(("LongText")));
        if (names.Contains("rowguid")) value.rowguid = dr.GetGuid(dr.GetOrdinal(("rowguid")));
        if (names.Contains("ModifiedDate")) value.ModifiedDate = dr.GetDateTime(dr.GetOrdinal(("ModifiedDate")));

    }

    private List<TestGen.SqlServer.Data.Sales> readReader(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Sales> list = new List<TestGen.SqlServer.Data.Sales>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Sales value = new TestGen.SqlServer.Data.Sales();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.Sales> readReader(Database db, string sql, bool concurrency, params Parameter[] parameters)
    {
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.Sales> list = new List<TestGen.SqlServer.Data.Sales>();
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Sales value = new TestGen.SqlServer.Data.Sales();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private TestGen.SqlServer.Data.Sales readReaderSingle(Database db, string sql, bool concurrency)
    {
        TestGen.SqlServer.Data.Sales value = null;
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Sales> list = new List<TestGen.SqlServer.Data.Sales>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.Sales();
                readRecord(dr, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
            }
            return value;
        }
    }

    private TestGen.SqlServer.Data.Sales readReaderSingle(Database db, string sql, Parameter[] parameters)
    {
        TestGen.SqlServer.Data.Sales value = null;
        using (DbDataReader dr = db.ExecuteReader(sql, parameters))
        {
            List<TestGen.SqlServer.Data.Sales> list = new List<TestGen.SqlServer.Data.Sales>();
            if (dr.Read())
            {
                value = new TestGen.SqlServer.Data.Sales();
                readRecord(dr, ref value);
            }
            return value;
        }
    }

    private List<TestGen.SqlServer.Data.Sales> readReaderSql(Database db, string sql, bool concurrency)
    {
        using (DbDataReader dr = db.ExecuteReader(sql))
        {
            List<TestGen.SqlServer.Data.Sales> list = new List<TestGen.SqlServer.Data.Sales>();
            // pega os nomes das colunas continas no sql
            List<string> names = new List<string>();
            for (int i = 0; i < dr.FieldCount; i++)
                names.Add(dr.GetName(i));
            while (dr.Read())
            {
                TestGen.SqlServer.Data.Sales value = new TestGen.SqlServer.Data.Sales();
                readRecordSql(dr, names, ref value);
                if (concurrency)
                    value.RecordOriginal = value.CloneT();
                list.Add(value);
            }
            return list;
        }
    }

    private List<TestGen.SqlServer.Data.Sales> select(Database db, bool concurrency)
    {
        string sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales";
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sales> select(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales";
        else
            sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where " + where;
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sales> select(Database db, string where, params Parameter[] parameters)
    {
        string sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where " + where;
        return readReader(db, sql, false, parameters);
    }

    private List<TestGen.SqlServer.Data.Sales> select(Database db, string where, bool concurrency, params Parameter[] parameters)
    {
        string sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where " + where;
        return readReader(db, sql, concurrency, parameters);
    }

    private List<TestGen.SqlServer.Data.Sales> selectTop(Database db, int top, bool concurrency)
    {
        // string sql = string.Format("db.Provider.SetTop(\"select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales\", {0})", top);
        string sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales";
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sales> selectTop(Database db, int top, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales";
        else
            sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where " + where;
        sql = db.Provider.SetTop(sql, top);
        return readReader(db, sql, concurrency);
    }

    private long count(Database db, string where)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select count(*) from dbo.Sales";
        else
            sql = "select count(*) from dbo.Sales where " + where;
        return db.ExecuteInt64(sql);
    }

    private List<TestGen.SqlServer.Data.Sales> selectColumns(Database db, bool concurrency, params string[] columns)
    {
        string sql = "select " + Concat(", ", columns) + " from dbo.Sales";
        return readReaderSql(db, sql, concurrency);
    }

    private List<TestGen.SqlServer.Data.Sales> selectColumns(Database db, string where, bool concurrency, params string[] columns)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select " + Concat(", ", columns) + " from dbo.Sales ";
        else
            sql = "select " + Concat(", ", columns) + " from dbo.Sales where " + where;
        return readReaderSql(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.Sales selectSingle(Database db, bool concurrency)
    {
        string sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales";
        return readReaderSingle(db, sql, concurrency);
    }

    private TestGen.SqlServer.Data.Sales selectSingle(Database db, string where, bool concurrency)
    {
        string sql;
        if (string.IsNullOrWhiteSpace(where))
            sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales";
        else
            sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where " + where;
        return readReaderSingle(db, sql, concurrency);
    }

    private object selectByPk(Database db, object instance, bool concurrency)
    {
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        TestGen.SqlServer.Data.Sales value2 = new TestGen.SqlServer.Data.Sales();
        string sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where TestPerfID = @P_TestPerfID";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_TestPerfID", GetValue(value.TestPerfID));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value2);
                    if (concurrency)
                        value2.RecordOriginal = value2.CloneT();
                    return value2;
                }
                else
                    return null;
            }
        }
    }

    private int insert(Database db, object instance)
    {
        
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        string sql = "insert into dbo.Sales (CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LongText, rowguid, ModifiedDate) values (@P_CarrierTrackingNumber, @P_OrderQty, @P_ProductID, @P_SpecialOfferID, @P_UnitPrice, @P_UnitPriceDiscount, @P_LongText, @P_rowguid, @P_ModifiedDate)";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_CarrierTrackingNumber", "String", GetValue(value.CarrierTrackingNumber));
            db.AddWithValue(cmd, "@P_OrderQty", "Int32", GetValue(value.OrderQty));
            db.AddWithValue(cmd, "@P_ProductID", "Int32", GetValue(value.ProductID));
            db.AddWithValue(cmd, "@P_SpecialOfferID", "Int32", GetValue(value.SpecialOfferID));
            db.AddWithValue(cmd, "@P_UnitPrice", "Decimal", GetValue(value.UnitPrice));
            db.AddWithValue(cmd, "@P_UnitPriceDiscount", "Decimal", GetValue(value.UnitPriceDiscount));
            db.AddWithValue(cmd, "@P_LongText", "String", GetValue(value.LongText));
            db.AddWithValue(cmd, "@P_rowguid", "Guid", GetValue(value.rowguid));
            db.AddWithValue(cmd, "@P_ModifiedDate", "DateTime", GetValue(value.ModifiedDate));

            value.RecordStatus = RecordStatus.Existing;
            return db.ExecuteNonQuery(cmd);
        }

    }

    private int insertRequery(Database db, object instance, bool concurrency)
    {
        
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        string sql;
        sql = "insert into dbo.Sales (CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LongText, rowguid, ModifiedDate) values (@P_CarrierTrackingNumber, @P_OrderQty, @P_ProductID, @P_SpecialOfferID, @P_UnitPrice, @P_UnitPriceDiscount, @P_LongText, @P_rowguid, @P_ModifiedDate);";
        sql += "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where TestPerfID = SCOPE_IDENTITY();";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_CarrierTrackingNumber", "String", GetValue(value.CarrierTrackingNumber));
            db.AddWithValue(cmd, "@P_OrderQty", "Int32", GetValue(value.OrderQty));
            db.AddWithValue(cmd, "@P_ProductID", "Int32", GetValue(value.ProductID));
            db.AddWithValue(cmd, "@P_SpecialOfferID", "Int32", GetValue(value.SpecialOfferID));
            db.AddWithValue(cmd, "@P_UnitPrice", "Decimal", GetValue(value.UnitPrice));
            db.AddWithValue(cmd, "@P_UnitPriceDiscount", "Decimal", GetValue(value.UnitPriceDiscount));
            db.AddWithValue(cmd, "@P_LongText", "String", GetValue(value.LongText));
            db.AddWithValue(cmd, "@P_rowguid", "Guid", GetValue(value.rowguid));
            db.AddWithValue(cmd, "@P_ModifiedDate", "DateTime", GetValue(value.ModifiedDate));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        };
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
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql = "update dbo.Sales set CarrierTrackingNumber = @P_CarrierTrackingNumber, OrderQty = @P_OrderQty, ProductID = @P_ProductID, SpecialOfferID = @P_SpecialOfferID, UnitPrice = @P_UnitPrice, UnitPriceDiscount = @P_UnitPriceDiscount, LongText = @P_LongText, rowguid = @P_rowguid, ModifiedDate = @P_ModifiedDate where TestPerfID = @P_OldTestPerfID";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_TestPerfID", "Int32", GetValue(value.TestPerfID));
            db.AddWithValue(cmd, "@P_CarrierTrackingNumber", "String", GetValue(value.CarrierTrackingNumber));
            db.AddWithValue(cmd, "@P_OrderQty", "Int32", GetValue(value.OrderQty));
            db.AddWithValue(cmd, "@P_ProductID", "Int32", GetValue(value.ProductID));
            db.AddWithValue(cmd, "@P_SpecialOfferID", "Int32", GetValue(value.SpecialOfferID));
            db.AddWithValue(cmd, "@P_UnitPrice", "Decimal", GetValue(value.UnitPrice));
            db.AddWithValue(cmd, "@P_UnitPriceDiscount", "Decimal", GetValue(value.UnitPriceDiscount));
            db.AddWithValue(cmd, "@P_LongText", "String", GetValue(value.LongText));
            db.AddWithValue(cmd, "@P_rowguid", "Guid", GetValue(value.rowguid));
            db.AddWithValue(cmd, "@P_ModifiedDate", "DateTime", GetValue(value.ModifiedDate));

            TestGen.SqlServer.Data.Sales old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Sales)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldTestPerfID", GetValue(old.TestPerfID));

            return db.ExecuteNonQuery(cmd);
        }
    }

    private int updateRequery(Database db, object instance, bool concurrency = false)
    {
        
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        if (value.RecordOriginal != null) CheckConcurrency(db, value);
        string sql;
        sql = "update dbo.Sales set CarrierTrackingNumber = @P_CarrierTrackingNumber, OrderQty = @P_OrderQty, ProductID = @P_ProductID, SpecialOfferID = @P_SpecialOfferID, UnitPrice = @P_UnitPrice, UnitPriceDiscount = @P_UnitPriceDiscount, LongText = @P_LongText, rowguid = @P_rowguid, ModifiedDate = @P_ModifiedDate where TestPerfID = @P_OldTestPerfID;";
        sql += "\r\n" + "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where TestPerfID = @P_TestPerfID;";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            db.AddWithValue(cmd, "@P_TestPerfID", "Int32", GetValue(value.TestPerfID));
            db.AddWithValue(cmd, "@P_CarrierTrackingNumber", "String", GetValue(value.CarrierTrackingNumber));
            db.AddWithValue(cmd, "@P_OrderQty", "Int32", GetValue(value.OrderQty));
            db.AddWithValue(cmd, "@P_ProductID", "Int32", GetValue(value.ProductID));
            db.AddWithValue(cmd, "@P_SpecialOfferID", "Int32", GetValue(value.SpecialOfferID));
            db.AddWithValue(cmd, "@P_UnitPrice", "Decimal", GetValue(value.UnitPrice));
            db.AddWithValue(cmd, "@P_UnitPriceDiscount", "Decimal", GetValue(value.UnitPriceDiscount));
            db.AddWithValue(cmd, "@P_LongText", "String", GetValue(value.LongText));
            db.AddWithValue(cmd, "@P_rowguid", "Guid", GetValue(value.rowguid));
            db.AddWithValue(cmd, "@P_ModifiedDate", "DateTime", GetValue(value.ModifiedDate));

            TestGen.SqlServer.Data.Sales old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Sales)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldTestPerfID", GetValue(old.TestPerfID));

            using (DbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    readRecord(dr, ref value);
                    value.RecordStatus = RecordStatus.Existing;
                    if (concurrency)
                        value.RecordOriginal = value.CloneT();
                    return 1;
                }
                return 0;
            }
        }

    }

    public override int Truncate(Database db)
    {
        string sql = "truncate table dbo.Sales";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db)
    {
        string sql = "delete from dbo.Sales";
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, object instance)
    {
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        string sql = "delete from dbo.Sales where TestPerfID = @P_OldTestPerfID";
        using (DbCommand cmd = db.NewCommand(sql))
        {
            TestGen.SqlServer.Data.Sales old = value.RecordOriginal != null ? (TestGen.SqlServer.Data.Sales)value.RecordOriginal : value;
            db.AddWithValue(cmd, "@P_OldTestPerfID", GetValue(old.TestPerfID));

            return db.ExecuteNonQuery(cmd);
        }
    }

    public override int Delete(Database db, string where)
    {
        string sql = "delete from dbo.Sales where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd);
    }

    public override int Delete(Database db, string where, params Parameter[] parameters)
    {
        string sql = "delete from dbo.Sales where " + where;
        using (DbCommand cmd = db.NewCommand(sql))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Delete(Database db, string where, int commandTimeout, params Parameter[] parameters)
    {
        string sql = "delete from dbo.Sales where " + where;
        using (DbCommand cmd = db.NewCommand(sql, commandTimeout))
            return db.ExecuteNonQuery(cmd, parameters);
    }

    public override int Save(Database db, object instance)
    {
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return update(db, value);
        else if (value.RecordStatus == RecordStatus.New)
            return insert(db, value);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override int Save(Database db, object instance, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.Sales value = (TestGen.SqlServer.Data.Sales)instance;
        if (value.RecordStatus == RecordStatus.Existing)
            return Update(db, value, saveMode);
        else if (value.RecordStatus == RecordStatus.New)
            return Insert(db, value, saveMode);
        else // if (value.RecordStatus == RecordStatus.Deleted)
            return Delete(db, value);
    }

    public override void SaveList(Database db, object instance, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.Sales> values = (List<TestGen.SqlServer.Data.Sales>)instance;
        bool isTran = db.UsingTransaction;

        if (!isTran)
            db.BeginTransaction();

        foreach (TestGen.SqlServer.Data.Sales value in values)
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
        string sql = "select TestPerfID, CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LineTotal, LongText, rowguid, ModifiedDate from dbo.Sales where " + where;
        return readReaderSingle(db, sql, parameters);
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
        return SerializeToJson<TestGen.SqlServer.Data.Sales>(select(db, concurrency));
    }

    public override string SelectToJson(Database db, string where)
    {
        return SelectToJson(db, where, false);
    }

    public override string SelectToJson(Database db, string where, bool concurrency)
    {
        return SerializeToJson<TestGen.SqlServer.Data.Sales>(select(db, where, concurrency));
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
        TestGen.SqlServer.Data.Sales rec = DeserializeFromJson<TestGen.SqlServer.Data.Sales>(json);
        Update(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.Sales>(rec);
    }

    public override string SaveFromJson(Database db, string json, EnumSaveMode saveMode)
    {
        TestGen.SqlServer.Data.Sales rec = DeserializeFromJson<TestGen.SqlServer.Data.Sales>(json);
        Save(db, rec, saveMode);
        return SerializeToJson<TestGen.SqlServer.Data.Sales>(rec);
    }

    public override string SaveListFromJson(Database db, string json, EnumSaveMode saveMode, bool continueOnError)
    {
        List<TestGen.SqlServer.Data.Sales> rec = DeserializeFromJson<List<TestGen.SqlServer.Data.Sales>>(json);
        SaveList(db, rec, saveMode, continueOnError);
        return SerializeToJson<List<TestGen.SqlServer.Data.Sales>>(rec);
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

        List<TestGen.SqlServer.Data.Sales> values = (List<TestGen.SqlServer.Data.Sales>)instance;
        
        // create DataTable
        DataTable tb = new DataTable("Table");
tb.Columns.Add("CarrierTrackingNumber", typeof(String));
tb.Columns.Add("OrderQty", typeof(Int32));
tb.Columns.Add("ProductID", typeof(Int32));
tb.Columns.Add("SpecialOfferID", typeof(Int32));
tb.Columns.Add("UnitPrice", typeof(Decimal));
tb.Columns.Add("UnitPriceDiscount", typeof(Decimal));
tb.Columns.Add("LongText", typeof(String));
tb.Columns.Add("rowguid", typeof(Guid));
tb.Columns.Add("ModifiedDate", typeof(DateTime));
        foreach (var value in values)
        {
            DataRow row = tb.NewRow();
row["CarrierTrackingNumber"] = value.CarrierTrackingNumber;
row["OrderQty"] = value.OrderQty;
row["ProductID"] = value.ProductID;
row["SpecialOfferID"] = value.SpecialOfferID;
row["UnitPrice"] = value.UnitPrice;
row["UnitPriceDiscount"] = value.UnitPriceDiscount;
row["LongText"] = value.LongText;
row["rowguid"] = value.rowguid;
row["ModifiedDate"] = value.ModifiedDate;
            tb.Rows.Add(row);
        }

string sql = 
@"
declare @xmlData xml = @Xml
BEGIN
    DECLARE @idInt int;

    EXEC sp_xml_preparedocument @idInt OUTPUT, @xmlData;

    INSERT INTO dbo.Sales(CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LongText, rowguid, ModifiedDate)
    SELECT CarrierTrackingNumber, OrderQty, ProductID, SpecialOfferID, UnitPrice, UnitPriceDiscount, LongText, rowguid, ModifiedDate
    FROM OPENXML (@idInt, '/DocumentElement/*')
    WITH 
    (
        CarrierTrackingNumber nvarchar(50) 'CarrierTrackingNumber',
OrderQty int 'OrderQty',
ProductID int 'ProductID',
SpecialOfferID int 'SpecialOfferID',
UnitPrice money 'UnitPrice',
UnitPriceDiscount money 'UnitPriceDiscount',
LongText varchar(1000) 'LongText',
rowguid uniqueidentifier 'rowguid',
ModifiedDate datetime 'ModifiedDate'
    )

    select CAST( SCOPE_IDENTITY() - @@ROWCOUNT + 1 AS INT);
    exec sp_xml_removedocument @idInt;
END;";

        using (System.IO.StringWriter swStringWriter = new System.IO.StringWriter())
        {
            // Datatable as XML format 
            tb.WriteXml(swStringWriter);
            // Datatable as XML string 
            string xml = swStringWriter.ToString();
            using (var cmd = db.NewCommand(sql, 0))
            {
                db.AddParameter(cmd, "@xml", DbType.Xml, xml, ParameterDirection.Input);
                int firstId = Convert.ToInt32(cmd.ExecuteScalar());
                return firstId;
            }
        }

    }

    public void CheckConcurrency(Database db, TestGen.SqlServer.Data.Sales value)
    {
        var original = (TestGen.SqlServer.Data.Sales)value.RecordOriginal;
        var current = (TestGen.SqlServer.Data.Sales)SelectByPk(db, original);
        if (current == null)
            throw new Exception("Original record has been deleted");
        else if (!original.IsEqual(current))
            throw new DBConcurrencyException("Concurrency exception");
    }

}

