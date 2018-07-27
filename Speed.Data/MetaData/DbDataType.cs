using System.Data;

namespace Speed.Data.MetaData
{

    public class DbDataType
    {
        public string TypeName { get; set; }
        public int ProviderDbType { get; set; }
        public int ColumnSize { get; set; }
        public string CreateFormat { get; set; }
        public string CreateParameters { get; set; }
        public string DataType { get; set; }
        public bool IsAutoIncrementable { get; set; }
        public bool IsBestMatch { get; set; }
        public bool IsCaseSensitive { get; set; }
        public bool IsFixedLength { get; set; }
        public bool IsFixedPrecisionScale { get; set; }
        public bool IsLong { get; set; }
        /// <summary>
        /// Indica se o tipo permite nulos, e não se uma propriedade específica é nula ou não
        /// </summary>
        public bool IsNullable { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsSearchableWithLike { get; set; }
        public bool IsUnsigned { get; set; }
        public int MaximumScale { get; set; }
        public int MinimumScale { get; set; }
        public bool IsConcurrencyType { get; set; }
        public bool IsLiteralSupported { get; set; }
        public string LiteralPrefix { get; set; }
        public string LiteralSuffix { get; set; }

        public DbDataType()
        {
        }

        public DbDataType(DataRow row)
        {
            var tb = row.Table;
            if (tb.Columns.Contains("TypeName"))
                this.TypeName = (string)row["TypeName"];
            
            if (tb.Columns.Contains("ProviderDbType"))
                this.ProviderDbType = Conv.ToInt32(row["ProviderDbType"]);
            
            if (tb.Columns.Contains("ColumnSize"))
                this.ColumnSize = Conv.ToInt32(row["ColumnSize"]);
            
            if (tb.Columns.Contains("CreateFormat"))
                this.CreateFormat = Conv.ToString(row["CreateFormat"]);
            
            if (tb.Columns.Contains("CreateParameters"))
                this.CreateParameters = Conv.ToString(row["CreateParameters"]);
            
            if (tb.Columns.Contains("DataType"))
                this.DataType = Conv.ToString(row["DataType"]);
            
            if (tb.Columns.Contains("IsAutoIncrementable"))
                this.IsAutoIncrementable = Conv.ToBoolean(row["IsAutoIncrementable"]);
            
            if (tb.Columns.Contains("IsBestMatch"))
                this.IsBestMatch = Conv.ToBoolean(row["IsBestMatch"]);
            
            if (tb.Columns.Contains("IsCaseSensitive"))
                this.IsCaseSensitive = Conv.ToBoolean(row["IsCaseSensitive"]);
            
            if (tb.Columns.Contains("IsFixedLength"))
                this.IsFixedLength = Conv.ToBoolean(row["IsFixedLength"]);
            
            if (tb.Columns.Contains("IsFixedPrecisionScale"))
                this.IsFixedPrecisionScale = Conv.ToBoolean(row["IsFixedPrecisionScale"]);
            
            if (tb.Columns.Contains("IsLong"))
                this.IsLong = Conv.ToBoolean(row["IsLong"]);
            
            if (tb.Columns.Contains("IsNullable"))
                this.IsNullable = Conv.ToBoolean(row["IsNullable"]);
            
            if (tb.Columns.Contains("IsSearchable"))
                this.IsSearchable = Conv.ToBoolean(row["IsSearchable"]);
            
            if (tb.Columns.Contains("IsSearchableWithLike"))
                this.IsSearchableWithLike = Conv.ToBoolean(row["IsSearchableWithLike"]);
            
            if (tb.Columns.Contains("IsUnsigned"))
                this.IsUnsigned = Conv.ToBoolean(row["IsUnsigned"]);
            
            if (tb.Columns.Contains("MaximumScale"))
                this.MaximumScale = Conv.ToInt32(row["MaximumScale"]);
            
            if (tb.Columns.Contains("MinimumScale"))
                this.MinimumScale = Conv.ToInt32(row["MinimumScale"]);
            
            if (tb.Columns.Contains("IsConcurrencyType"))
                this.IsConcurrencyType = Conv.ToBoolean(row["IsConcurrencyType"]);
            
            if (tb.Columns.Contains("IsLiteralSupported"))
                this.IsLiteralSupported = Conv.ToBoolean(row["IsLiteralSupported"]);
            
            if (tb.Columns.Contains("LiteralPrefix"))
                this.LiteralPrefix = Conv.ToString(row["LiteralPrefix"]);
            
            if (tb.Columns.Contains("LiteralSuffix"))
                this.LiteralSuffix = Conv.ToString(row["LiteralSuffix"]);

        }

        private string dataTypeDotNet;
        public string GetDataTypeDotNet(bool allowNull)
        {
            if (dataTypeDotNet == null)
                dataTypeDotNet = DbUtil.GetDataTypeDotNet(DataType, allowNull);
            return dataTypeDotNet;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", TypeName, DataType, ColumnSize);
        }

    }

}
