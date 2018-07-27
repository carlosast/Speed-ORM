using System.Data;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class DbParameterInfo
    {

        [DataMember]
        public string SpecificCatalog { get; set; }
        [DataMember]
        public string SpecificSchema { get; set; }
        [DataMember]
        public string SpecificName { get; set; }
        [DataMember]
        public int OrdinalPosition { get; set; }
        [DataMember]
        public ParameterDirection ParameterMode { get; set; }
        [DataMember]
        public bool IsResult { get; set; }
        [DataMember]
        public bool AsLocator { get; set; }
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public string DataType { get; set; }
        [DataMember]
        public int CharacterMaximumLength { get; set; }
        [DataMember]
        public int CharacterOctetLength { get; set; }
        [DataMember]
        public string CollationCatalog { get; set; }
        [DataMember]
        public string CollationSchema { get; set; }
        [DataMember]
        public string CollationName { get; set; }
        [DataMember]
        public string CharacterSetCatalog { get; set; }
        [DataMember]
        public string CharacterSetSchema { get; set; }
        [DataMember]
        public string CharacterSetName { get; set; }
        [DataMember]
        public int NumericPrecision { get; set; }
        [DataMember]
        public int NumericPrecisionRadix { get; set; }
        [DataMember]
        public int NumericScale { get; set; }
        [DataMember]
        public string DatetimePrecision { get; set; }
        [DataMember]
        public string IntervalType { get; set; }
        [DataMember]
        public string IntervalPrecision { get; set; }
        [DataMember]
        public string UserDefinedTypeCatalog { get; set; }
        [DataMember]
        public string UserDefinedTypeSchema { get; set; }
        [DataMember]
        public string UserDefinedTypeName { get; set; }
        [DataMember]
        public string ScopeCatalog { get; set; }
        [DataMember]
        public string ScopeSchema { get; set; }
        [DataMember]
        public string ScopeName { get; set; }

        [DataMember]
        public string RoutineFullName { get { return string.IsNullOrEmpty(SpecificSchema) ? SpecificName : SpecificSchema + "." + SpecificName; } }


        public DbParameterInfo()
        {
        }

        public DbParameterInfo(DataTable tb, DataRow row)  :this()
        {
            if (tb.Columns.Contains("PARAMETER_NAME"))
                this.ParameterName = Conv.ToString(row["PARAMETER_NAME"]);

            if (tb.Columns.Contains("SPECIFIC_CATALOG"))
                this.SpecificCatalog = Conv.ToString(row["SPECIFIC_CATALOG"]);

            if (tb.Columns.Contains("SPECIFIC_SCHEMA"))
                this.SpecificSchema = Conv.ToString(row["SPECIFIC_SCHEMA"]);
            
            if (tb.Columns.Contains("SPECIFIC_NAME"))
                this.SpecificName = Conv.ToString(row["SPECIFIC_NAME"]);
            
            if (tb.Columns.Contains("ORDINAL_POSITION"))
                this.OrdinalPosition = Conv.ToInt32(row["ORDINAL_POSITION"]);

            if (tb.Columns.Contains("PARAMETER_MODE"))
            {
                if (ParameterName == "@ParString")
                    ToString();

                string parameterMode = Conv.ToString(row["PARAMETER_MODE"]);
                ParameterDirection mode = ParameterDirection.Input;
                if (parameterMode == "IN")
                    mode = ParameterDirection.Input;
                else if (parameterMode == "OUT")
                    mode = ParameterDirection.Output;
                else if (parameterMode == "INOUT" | parameterMode == "IN_OUT" | parameterMode == "IN OUT")
                    mode = ParameterDirection.InputOutput;
                this.ParameterMode = mode;
            }

            // TODO: checar se está correto setar 'ParameterMode = ParameterDirection.ReturnValue' aqui ou no bloco acima
            if (this.IsResult)
                this.ParameterMode = ParameterDirection.ReturnValue;
            
            if (tb.Columns.Contains("IS_RESULT"))
                this.IsResult = Conv.ToBoolean(row["IS_RESULT"]);
            
            if (tb.Columns.Contains("AS_LOCATOR"))
                this.AsLocator = Conv.ToBoolean(row["AS_LOCATOR"]);
            
            if (tb.Columns.Contains("DATA_TYPE"))
                this.DataType = Conv.ToString(row["DATA_TYPE"]);
            
            if (tb.Columns.Contains("CHARACTER_MAXIMUM_LENGTH"))
                this.CharacterMaximumLength = Conv.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]);
            
            if (tb.Columns.Contains("CHARACTER_OCTET_LENGTH"))
                this.CharacterOctetLength = Conv.ToInt32(row["CHARACTER_OCTET_LENGTH"]);
            
            if (tb.Columns.Contains("COLLATION_CATALOG"))
                this.CollationCatalog = Conv.ToString(row["COLLATION_CATALOG"]);
            
            if (tb.Columns.Contains("COLLATION_SCHEMA"))
                this.CollationSchema = Conv.ToString(row["COLLATION_SCHEMA"]);
            
            if (tb.Columns.Contains("COLLATION_NAME"))
                this.CollationName = Conv.ToString(row["COLLATION_NAME"]);
            
            if (tb.Columns.Contains("CHARACTER_SET_CATALOG"))
                this.CharacterSetCatalog = Conv.ToString(row["CHARACTER_SET_CATALOG"]);
            
            if (tb.Columns.Contains("CHARACTER_SET_SCHEMA"))
                this.CharacterSetSchema = Conv.ToString(row["CHARACTER_SET_SCHEMA"]);
            
            if (tb.Columns.Contains("CHARACTER_SET_NAME"))
                this.CharacterSetName = Conv.ToString(row["CHARACTER_SET_NAME"]);
            
            if (tb.Columns.Contains("NUMERIC_PRECISION"))
                this.NumericPrecision = Conv.ToInt32(row["NUMERIC_PRECISION"]);
            
            if (tb.Columns.Contains("NUMERIC_PRECISION_RADIX"))
                this.NumericPrecisionRadix = Conv.ToInt32(row["NUMERIC_PRECISION_RADIX"]);
            
            if (tb.Columns.Contains("NUMERIC_SCALE"))
                this.NumericScale = Conv.ToInt32(row["NUMERIC_SCALE"]);
            
            if (tb.Columns.Contains("DATETIME_PRECISION"))
                this.DatetimePrecision = Conv.ToString(row["DATETIME_PRECISION"]);
            
            if (tb.Columns.Contains("INTERVAL_TYPE"))
                this.IntervalType = Conv.ToString(row["INTERVAL_TYPE"]);
            
            if (tb.Columns.Contains("INTERVAL_PRECISION"))
                this.IntervalPrecision = Conv.ToString(row["INTERVAL_PRECISION"]);
            
            if (tb.Columns.Contains("USER_DEFINED_TYPE_CATALOG"))
                this.UserDefinedTypeCatalog = Conv.ToString(row["USER_DEFINED_TYPE_CATALOG"]);
            
            if (tb.Columns.Contains("USER_DEFINED_TYPE_SCHEMA"))
                this.UserDefinedTypeSchema = Conv.ToString(row["USER_DEFINED_TYPE_SCHEMA"]);
            
            if (tb.Columns.Contains("USER_DEFINED_TYPE_NAME"))
                this.UserDefinedTypeName = Conv.ToString(row["USER_DEFINED_TYPE_NAME"]);
            
            if (tb.Columns.Contains("SCOPE_CATALOG"))
                this.ScopeCatalog = Conv.ToString(row["SCOPE_CATALOG"]);
            
            if (tb.Columns.Contains("SCOPE_SCHEMA"))
                this.ScopeSchema = Conv.ToString(row["SCOPE_SCHEMA"]);
            
            if (tb.Columns.Contains("SCOPE_NAME"))
                this.ScopeName = Conv.ToString(row["SCOPE_NAME"]);
        
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
            return string.Format("{0} - {1} - {2}", this.ParameterName, ParameterMode, CharacterMaximumLength);
        }


    }

}
