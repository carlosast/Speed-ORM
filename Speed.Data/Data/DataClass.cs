using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Xml;
using Speed.Data.MetaData;
using System.Linq;
using System.Reflection;
using System.Text;
using Speed.Common;

namespace Speed.Data
{

    [Serializable]
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DataClass
    {

        public DbTableInfo TableInfo;
        public Dictionary<string, DbColumnInfo> ColumnInfos;
        /// <summary>
        /// Guarda o código C# da classe. Apenas pra proposta de debug
        /// </summary>
        public string Code;
        //Dictionary<string, PropertyInfo> properties;
        Dictionary<string, PropertyInfo> columns;
        //Dictionary<string, DbColumnAttribute> columnsByColName;

        #region Select

        //public Record CreateInstance()
        //{
        //    throw new NotImplementedException();
        //}

        public virtual object Select(Database db)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, int commandTimeout)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, string where)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, string where, int commandTimeout)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, string where, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, string where, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, string where, bool concurrency, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object Select(Database db, object filter, EnumDbFilter mode = EnumDbFilter.AndEqual, bool concurrency = false, int commandTimeout = 30)
        {
            throw new NotImplementedException();
        }

        #endregion Select

        //public Record CreateInstance()
        //{
        //    throw new NotImplementedException();
        //}

        #region SelectPage

        public virtual object SelectPage(Database db, int start, int pageSize = 20, string sort = null, int commandTimeout = 30)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectPage(Database db, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, bool concurrency = false)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, bool concurrency = false)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectPage(Database db, string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, bool concurrency = false, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectPage(Database db, object filter, int start, int pageSize = 20, string sort = null, EnumDbFilter mode = EnumDbFilter.AndEqual, int commandTimeout = 30, bool concurrency = false, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        #endregion SelectPage

        #region SelectTop

        public virtual object SelectTop(Database db, int top)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectTop(Database db, int top, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectTop(Database db, int top, string where)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectTop(Database db, int top, string where, bool concurrency)
        {
            throw new NotImplementedException();
        }

        #endregion SelectTop

        #region SelectColumns

        public virtual object SelectColumns(Database db, bool concurrency, params string[] columns)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectColumns(Database db, string where, bool concurrency, params string[] columns)
        {
            throw new NotImplementedException();
        }

        #endregion SelectColumns

        #region SelectSingle

        public virtual object SelectSingle(Database db)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectSingle(Database db, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectSingle(Database db, string where)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectSingle(Database db, string where, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectSingle(Database db, string where, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectSingle(Database db, object filter, EnumDbFilter mode = EnumDbFilter.AndEqual, bool concurrency = false, int commandTimeout = 30)
        {
            throw new NotImplementedException();
        }

        #endregion SelectSingle

        #region SelectArray

        public virtual object SelectArray(Database db)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectArray(Database db, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectArray(Database db, string where)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectArray(Database db, string where, bool concurrency)
        {
            throw new NotImplementedException();
        }

        #endregion SelectArray

        #region SelectToJson

        public virtual string SelectToJson(Database db)
        {
            throw new NotImplementedException();
        }

        public virtual string SelectToJson(Database db, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual string SelectToJson(Database db, string where)
        {
            throw new NotImplementedException();
        }

        public virtual string SelectToJson(Database db, string where, bool concurrency)
        {
            throw new NotImplementedException();
        }

        public virtual string UpdateFromJson(Database db, string json, EnumSaveMode saveMode)
        {
            throw new NotImplementedException();
        }

        public virtual string SaveFromJson(Database db, string json, EnumSaveMode saveMode)
        {
            throw new NotImplementedException();
        }

        public virtual string SaveListFromJson(Database db, string json, EnumSaveMode saveMode, bool continueOnError)
        {
            throw new NotImplementedException();
        }

        #endregion SelectToJson

        #region SelectSql

        public virtual object SelectSql(Database db, string sql)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectSql(Database db, string sql, int commandTimeout)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectSql(Database db, string sql, int commandTimeout, bool concurrency)
        {
            throw new NotImplementedException();
        }

        #endregion SelectSql

        #region SelectByPk

        public virtual object SelectByPk(Database db, object instance)
        {
            throw new NotImplementedException();
        }

        public virtual object SelectByPk(Database db, object instance, bool concurrency)
        {
            throw new NotImplementedException();
        }

        #endregion SelectByPk

        #region Count

        public virtual long Count(Database db)
        {
            throw new NotImplementedException();
        }

        public virtual long Count(Database db, string where)
        {
            throw new NotImplementedException();
        }

        public virtual long Count(Database db, object _filter, EnumDbFilter mode = EnumDbFilter.AndEqual, int commandTimeout = 30)
        {
            throw new NotImplementedException();
        }

        #endregion Count

        #region Max

        public virtual long Max(Database db, string columnName)
        {
            throw new NotImplementedException();
        }

        public virtual long Max(Database db, string columnName, string where)
        {
            throw new NotImplementedException();
        }

        #endregion Max

        #region CRUD

        public virtual int Insert(Database db, object instance)
        {
            throw new NotImplementedException();
        }

        public virtual int Insert(Database db, object instance, EnumSaveMode mode, bool concurrency = false)
        {
            throw new NotImplementedException();
        }

        public virtual int InsertXml(Database db, object instance)
        {
            throw new NotImplementedException();
        }

        public virtual int InsertXml(Database db, object instance, EnumSaveMode mode = EnumSaveMode.None)
        {
            throw new NotImplementedException();
        }

        public virtual int Update(Database db, object instance)
        {
            throw new NotImplementedException();
        }

        public virtual int Update(Database db, object instance, EnumSaveMode mode, bool concurrency = false)
        {
            throw new NotImplementedException();
        }

        public virtual int Save(Database db, object instance)
        {
            throw new NotImplementedException();
        }

        public virtual int Save(Database db, object instance, EnumSaveMode mode)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveList(Database db, object instance, EnumSaveMode mode, bool continueOnError)
        {
            throw new NotImplementedException();
        }

        public virtual int Truncate(Database db)
        {
            throw new NotImplementedException();
        }

        public virtual int Delete(Database db)
        {
            throw new NotImplementedException();
        }

        public virtual int Delete(Database db, object instance)
        {
            throw new NotImplementedException();
        }

        public virtual int Delete(Database db, string where)
        {
            throw new NotImplementedException();
        }

        public virtual int Delete(Database db, string where, int commandTimeout)
        {
            throw new NotImplementedException();
        }

        public virtual int Delete(Database db, string where, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual int Delete(Database db, string where, int commandTimeout, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual int DeleteAll(Database db, object[] instance)
        {
            throw new NotImplementedException();
        }

        public virtual int DeleteByPk(Database db, object instance)
        {
            throw new NotImplementedException();
        }

        public object GetValue(object value)
        {
#if DEBUG2
            object val = value == null ? DBNull.Value : value;
            return val;
#else
            return value == null ? DBNull.Value : value;
#endif
        }

        public byte[] GetBytes(DbDataReader reader, int ordinal)
        {
            return (byte[])reader.GetValue(ordinal);
        }

        public bool? GetBoolean(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (bool?)null : reader.GetBoolean(ordinal);
        }

        public DateTime? GetDateTime(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        public Decimal? GetDecimal(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }

        public float? GetFloat(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (float?)null : reader.GetFloat(ordinal);
        }

        public object ParseDbNull(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public string GetString(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public Guid? GetGuid(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (Guid?)null : reader.GetGuid(ordinal);
        }

        public Int16? GetInt16(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (Int16?)null : reader.GetInt16(ordinal);
        }

        public Int32? GetInt32(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (Int32?)null : reader.GetInt32(ordinal);
        }

        public Int64? GetInt64(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (Int64?)null : reader.GetInt64(ordinal);
        }

        public Double? GetDouble(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (Double?)null : reader.GetDouble(ordinal);
        }

        public Byte? GetByte(DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (Byte?)null : reader.GetByte(ordinal);
        }

        public TimeSpan? GetTimeSpan(Database db, DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (TimeSpan?)null : db.Provider.GetTimeSpan(reader, ordinal);
        }

        public char[] GetChars(DbDataReader reader, int ordinal)
        {
            /*
            using (MemoryStream ms = new MemoryStream())
            {
                char[] data = new char[1024 * 1000];
                int index = 0;
                while (true)
                {
                    long count = reader.GetChars(ordinal, index, data, 0, data.Length);
                    if (count == 0)
                    {
                        break;
                    }
                    else
                    {
                        index = index + (int)count;
                        byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(data, 0, (int)count);
                        ms.Write(bytes, 0, bytes.Length);
                    }
                }
                return ms.ToArray();
            */
            throw new NotImplementedException();
        }

        #endregion CRUD

        public static string Serialize<T>(object value)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T), GetTypes<T>());
            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw))
                {
                    dcs.WriteObject(xw, value);
                }
                return sw.ToString();
            }
        }

        public static T Deserialize<T>(string xml)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T), GetTypes<T>());
            using (StringReader sr = new StringReader(xml))
                return (T)dcs.ReadObject(XmlReader.Create(sr));
        }

        public static string SerializeToJson<T>(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(T), GetTypes<T>());
                serializer.WriteObject(ms, value);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    var ret = reader.ReadToEnd();
                    return ret;
                }
            }
        }

        public static T DeserializeFromJson<T>(string value)
        {
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(value)))
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(T), GetTypes<T>());

                return (T)serializer.ReadObject(ms);
            }
        }

        public static Type[] GetTypes<T>()
        {
            return new Type[] { typeof(T), typeof(List<T>), typeof(Record), typeof(List<Record>) };
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public virtual DataClass CloneT()
        {
            throw new NotImplementedException();
        }

        public string Validate<T>(T record, bool thowException = true)
        {
            // TODO: colocar essa lógica na geração de código pra melhorar a performance
            var b = new StringBuilder();

            if (columns == null)
            {
                columns = DataReflectionUtil.GetMapColumns(typeof(T));
            }

            foreach (var _col in columns)
            {
                string propertyName = _col.Value.Name;
                var col = ColumnInfos[_col.Key];
                string columnName = col.ColumnName;
                PropertyInfo prop = _col.Value;

                var value = prop.GetValue(record, null);
                var valueStr = value as string;

                if (!col.IsIdentity
                    && !columnName.EqualsICIC(TableInfo.SequenceColumn)
                    && value == null && !col.IsNullable)
                {
                    b.AppendFormat($"Property {propertyName} / Column {col.TableName}.{col.ColumnName} does not allow null\r\n");
                }

                if (!string.IsNullOrEmpty(valueStr))
                {
                    if (col.DataType != "CLOB" && col.DataType != "BLOB" && col.DataType != "NCLOB")
                    {
                        if (col.CharacterMaximumLength != 0 &&
                            valueStr.Length > col.CharacterMaximumLength)
                            b.AppendFormat($"Property {propertyName} / Column {col.TableName}.{col.ColumnName} Exceeded maximum allowed length {col.CharacterMaximumLength}. Field is {valueStr.Length} characters long.\r\n");
                    }
                }
            }

            if (thowException && !b.IsEmpty())
                throw new Exception(b.ToString());

            return b.Length == 0 ? null : b.ToString();
        }


    }

}
