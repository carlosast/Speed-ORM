using System;
using System.Runtime.Serialization;
using System.Data;
using System.Data.Common;
using Speed.Common;

namespace Speed.Data.MetaData
{

    /// <summary>
    /// Classe com informações de uma coluna da base de dados
    /// </summary>

    // // [DataContract]
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbColumnInfo
    {

        #region Declarations

        // // [DataMember]
        public string TableCatalog { get; set; }
        // // [DataMember]
        public string TableSchema { get; set; }
        // // [DataMember]
        public string TableName { get; set; }
        // // [DataMember]
        public string ColumnName { get; set; }
        // // [DataMember]
        public int OrdinalPosition { get; set; }
        // // [DataMember]
        public string ColumnDefault { get; set; }
        // // [DataMember]
        public bool IsNullable { get; set; }
        // // [DataMember]
        public bool IsIdentity { get; set; }
        // // [DataMember]
        public bool IsComputed { get; set; }
        // // [DataMember]
        public string DataType { get; set; }

        private string dataTypeDotNet;

        // // [DataMember]
        public long CharacterMaximumLength { get; set; }
        // // [DataMember]
        public long CharacterOctetLength { get; set; }
        // // [DataMember]
        public byte NumericPrecision { get; set; }
        // // [DataMember]
        public short NumericPrecisionRadix { get; set; }
        // // [DataMember]
        public int NumericScale { get; set; }
        // // [DataMember]
        public short DatetimePrecision { get; set; }
        // // [DataMember]
        public string CharacterSetCatalog { get; set; }
        // // [DataMember]
        public string CharacterSetSchema { get; set; }
        // // [DataMember]
        public string CharacterSetName { get; set; }
        // // [DataMember]
        public string CollationCatalog { get; set; }

        // // [DataMember]
        public bool AddQuote = false;

        #endregion Declarations

        public DbColumnInfo()
        {
            IsComputed = false;
        }

        public DbColumnInfo(Database db, DataRow row)
            : this()
        {
            DataTable tb = row.Table;
            for (int i = 0; i < tb.Columns.Count; i++)
                if (tb.Columns[i].DataType == typeof(string))
                    row[i] = Conv.Trim(row[i]);

            // TODO: Mysql
            //+select 
            //+statistics.TABLE_NAME, statistics.COLUMN_NAME, statistics.TABLE_CATALOG,
            //statistics.TABLE_SCHEMA, statistics.NON_UNIQUE, statistics.INDEX_SCHEMA,
            //statistics.INDEX_NAME, statistics.SEQ_IN_INDEX, statistics.COLLATION,
            //statistics.CARDINALITY, statistics.SUB_PART, statistics.PACKED, statistics.NULLABLE,
            //statistics.INDEX_TYPE, statistics.COMMENT, 
            //+columns.TABLE_CATALOG, columns.TABLE_SCHEMA, columns.COLUMN_DEFAULT, columns.IS_NULLABLE,
            //columns.DATA_TYPE, columns.CHARACTER_MAXIMUM_LENGTH, columns.CHARACTER_OCTET_LENGTH,
            //columns.NUMERIC_PRECISION, columns.NUMERIC_SCALE, columns.CHARACTER_SET_NAME,
            //columns.COLLATION_NAME, columns.COLUMN_TYPE, columns.COLUMN_KEY, columns.EXTRA,
            //columns.COLUMN_COMMENT
            //+from information_schema.statistics join information_schema.columns
            //using(table_name,column_name) where table_name='user';


            //if (tb.Columns.Contains("COLUMN_KEY"))
            //    this.IsIdentity = Conv.ToString(row["COLUMN_KEY"]) == "PRI"; //--

            this.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
            this.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
            this.TableName = Conv.ToString(row["TABLE_NAME"]);
            this.ColumnName = Conv.ToString(row["COLUMN_NAME"]);
            this.OrdinalPosition = Conv.ToInt32(row["ORDINAL_POSITION"]);
            this.ColumnDefault = Conv.ToString(row["COLUMN_DEFAULT"]);
            this.IsNullable = Conv.ToBoolean(row["IS_NULLABLE"]);
            if (tb.Columns.Contains("DATA_TYPE"))
                this.DataType = Conv.ToString(row["DATA_TYPE"]);
            if (tb.Columns.Contains("COLUMN_DATA_TYPE"))
                this.DataType = Conv.ToString(row["COLUMN_DATA_TYPE"]);

            if (tb.Columns.Contains("CHARACTER_MAXIMUM_LENGTH"))
                this.CharacterMaximumLength = Conv.ToInt64(row["CHARACTER_MAXIMUM_LENGTH"]);
            if (tb.Columns.Contains("COLUMN_SIZE"))
                this.CharacterMaximumLength = Conv.ToInt64(row["COLUMN_SIZE"]);

            if (tb.Columns.Contains("CHARACTER_OCTET_LENGTH"))
                this.CharacterOctetLength = Conv.ToInt64(row["CHARACTER_OCTET_LENGTH"]); //--
            this.NumericPrecision = Conv.ToByte(row["NUMERIC_PRECISION"]);
            if (tb.Columns.Contains("NUMERIC_PRECISION_RADIX"))
                this.NumericPrecisionRadix = Conv.ToInt16(row["NUMERIC_PRECISION_RADIX"]); //--
            this.NumericScale = Conv.ToInt32(row["NUMERIC_SCALE"]);
            if (tb.Columns.Contains("DATETIME_PRECISION"))
                this.DatetimePrecision = Conv.ToInt16(row["DATETIME_PRECISION"]);  //--
            if (tb.Columns.Contains("CHARACTER_SET_CATALOG"))
                this.CharacterSetCatalog = Conv.ToString(row["CHARACTER_SET_CATALOG"]);  //--
            if (tb.Columns.Contains("CHARACTER_SET_SCHEMA"))
                this.CharacterSetSchema = Conv.ToString(row["CHARACTER_SET_SCHEMA"]); //--
            if (tb.Columns.Contains("CHARACTER_SET_NAME"))
                this.CharacterSetName = Conv.ToString(row["CHARACTER_SET_NAME"]);
            if (tb.Columns.Contains("COLLATION_CATALOG"))
                this.CollationCatalog = Convert.ToString(row["COLLATION_CATALOG"]); //--

            if (tb.Columns.Contains("AUTOINCREMENT"))
                this.IsIdentity = Conv.ToBoolean(row["AUTOINCREMENT"]); //--

            if (tb.Columns.Contains("IS_IDENTITY"))
                this.IsIdentity = Conv.ToBoolean(row["IS_IDENTITY"]); //--

            //if (db.ProviderType == EnumDbProviderType.PostgreSQL)
            //{
            //    if (!string.IsNullOrEmpty(this.ColumnDefault) && this.ColumnDefault.ToLower().Contains("nextval("))
            //        this.IsIdentity = true;
            //}

            //string sql = string.Format(
            //    "SELECT COLUMNPROPERTY( OBJECT_ID('{0}'),'{1}','IsIdentity')",
            //    this.TableName, this.ColumnName);
            //// TODO: para mysql dá erro
            //try
            //{
            //    this.IsIdentity = Convert.ToBoolean(cn.ExecuteScalar(sql));
            //}
            //catch { }
        }

        public DbColumnInfo(Database db, string tableName, DataColumn col)
        {
            // TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
            // TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
            TableName = tableName;
            ColumnName = col.ColumnName;
            OrdinalPosition = col.Ordinal;
            //ColumnDefault = col.DefaultValue.ToString();
            IsNullable = col.AllowDBNull;
            DataType = col.DataType.Name;

            // CharacterMaximumLength = Conv.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]);
            // CharacterOctetLength = Conv.ToInt32(row["CHARACTER_OCTET_LENGTH"]);
            // NumericPrecision = Conv.ToByte(row["NUMERIC_PRECISION"]);
            // NumericPrecisionRadix = Conv.ToInt16(row["NUMERIC_PRECISION_RADIX"]);
            // NumericScale = Conv.ToInt32(row["NUMERIC_SCALE"]);
            // DatetimePrecision = Conv.ToInt16(row["DATETIME_PRECISION"]);
            // CharacterSetCatalog = Conv.ToString(row["CHARACTER_SET_CATALOG"]);
            // CharacterSetSchema = Conv.ToString(row["CHARACTER_SET_SCHEMA"]);
            // CharacterSetName = Conv.ToString(row["CHARACTER_SET_NAME"]);
            // CollationCatalog = Convert.ToString(row["COLLATION_CATALOG"]);

            IsIdentity = col.AutoIncrement;
        }

        // // [DataMember]
        public string DataTypeDotNet
        {
            get { return dataTypeDotNet; }
            set
            {
                //if (value == "SqlHierarchyId")
                //{
                //    value = "Int64";
                //    AddQuote = true;
                //}
                dataTypeDotNet = value;
            }
        }

        public string GetDataTypeName()
        {
            if (IsNullable && DataTypeDotNet != "String" && DataTypeDotNet != "Object" && DataTypeDotNet != "Byte[]")
                return DataTypeDotNet + "?";
            //else if (DataTypeDotNet == "SqlHierarchyId")
            //{
            //    DataTypeDotNet = "Int64";
            //    return "Int64";
            //}
            else
                return DataTypeDotNet;
        }

        public string GetNullableDataTypeName()
        {
            if (DataTypeDotNet != "String" && DataTypeDotNet != "Object" && DataTypeDotNet != "Byte[]")
                return DataTypeDotNet + "?";
            else
                return DataTypeDotNet;
        }

        public string GetServerDataType()
        {
            string ret;
            string type = DataType;

            if (type.In("int", "integer", "money", "float", "real"))
            {
                ret = DataType;
            }
            else if (type.In("varchar", "nvarchar", "char", "nchar", "text", "ntext"))
            {
                if (CharacterMaximumLength != -1)
                    ret = string.Format("{0}({1})", DataType, CharacterMaximumLength);
                else
                    ret = string.Format("{0}(MAX)", DataType);
            }
            else
            {
                if (this.NumericPrecision > 0)
                    ret = string.Format("{0}({1},{2})", DataType, NumericPrecision, NumericScale);
                if (this.CharacterMaximumLength > 0)
                    ret = string.Format("{0}({1})", DataType, CharacterMaximumLength);
                else
                    ret = DataType;
            }
            return ret;
        }
        /*
        public string GetString()
        {
            string ret = string.Format(
                "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}",
            "TableCatalog=" + Aspas(TableCatalog),
            "TableSchema=" + Aspas(TableSchema),
            "TableName=" + Aspas(TableName),
            "ColumnName=" + Aspas(ColumnName),
            "OrdinalPosition=" + OrdinalPosition,
            "ColumnDefault=" + Aspas(ColumnDefault),
            "IsNullable=" + IsNullable.ToString().ToLower(),
            "IsIdentity=" + IsIdentity.ToString().ToLower(),
            "DataType=" + Aspas(DataType),
            "DataTypeDotNet=" + Aspas(DataTypeDotNet),
            "CharacterMaximumLength=" + CharacterMaximumLength,
            "CharacterOctetLength=" + CharacterOctetLength,
            "NumericPrecision=" + NumericPrecision,
            "NumericPrecisionRadix=" + NumericPrecisionRadix,
            "NumericScale=" + NumericScale,
            "DatetimePrecision=" + DatetimePrecision,
            "CharacterSetCatalog=" + Aspas(CharacterSetCatalog),
            "CharacterSetSchema=" + Aspas(CharacterSetSchema),
            "CharacterSetName=" + Aspas(CharacterSetName),
            "CollationCatalog=" + Aspas(CollationCatalog));
            return ret;
        }
        */

        //string Aspas(string value)
        //{
        //    return "\"" + value + "\"";
        //}

        public string GetQuote(string value)
        {
            if (this.DataTypeDotNet.ToUpper() == "STRING")
                return "'" + value + "'";
            else
                return value;
        }

        public string GetQuoteValue(string value)
        {
            if (this.DataTypeDotNet.ToUpper() == "STRING")
                return "Q(" + value + ")";
            else
                return value;
        }

        public override string ToString()
        {
            return string.Format("{0}", ColumnName);
        }

    }

}
