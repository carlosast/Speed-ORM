using Speed.Common;
using Speed.Data.Generation;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class DbRoutineInfo
    {

        // // [DataMember]
        public string SpecificCatalog { get; set; }
        // // [DataMember]
        public string SpecificSchema { get; set; }
        // // [DataMember]
        public string SpecificName { get; set; }
        // // [DataMember]
        public string RoutineCatalog { get; set; }
        // // [DataMember]
        public string RoutineSchema { get; set; }
        // // [DataMember]
        public string RoutineName { get; set; }
        // // [DataMember]
        public string RoutineTypeStr { get; set; }
        // // [DataMember]
        public EnumRoutineType RoutineType { get; set; }
        // // [DataMember]
        public string ModuleCatalog { get; set; }
        // // [DataMember]
        public string ModuleSchema { get; set; }
        // // [DataMember]
        public string ModuleName { get; set; }
        // // [DataMember]
        public string UdtCatalog { get; set; }
        // // [DataMember]
        public string UdtSchema { get; set; }
        // // [DataMember]
        public string UdtName { get; set; }
        // // [DataMember]
        public string DataType { get; set; }
        // // [DataMember]
        public string CharacterMaximumLength { get; set; }
        // // [DataMember]
        public string CharacterOctetLength { get; set; }
        // // [DataMember]
        public string CollationCatalog { get; set; }
        // // [DataMember]
        public string CollationSchema { get; set; }
        // // [DataMember]
        public string CollationName { get; set; }
        // // [DataMember]
        public string CharacterSetCatalog { get; set; }
        // // [DataMember]
        public string CharacterSetSchema { get; set; }
        // // [DataMember]
        public string CharacterSetName { get; set; }
        // // [DataMember]
        public string NumericPrecision { get; set; }
        // // [DataMember]
        public string NumericPrecisionRadix { get; set; }
        // // [DataMember]
        public string NumericScale { get; set; }
        // // [DataMember]
        public string DatetimePrecision { get; set; }
        // // [DataMember]
        public string IntervalType { get; set; }
        // // [DataMember]
        public string IntervalPrecision { get; set; }
        // // [DataMember]
        public string TypeUdtCatalog { get; set; }
        // // [DataMember]
        public string TypeUdtSchema { get; set; }
        // // [DataMember]
        public string TypeUdtName { get; set; }
        // // [DataMember]
        public string ScopeCatalog { get; set; }
        // // [DataMember]
        public string ScopeSchema { get; set; }
        // // [DataMember]
        public string ScopeName { get; set; }
        // // [DataMember]
        public string MaximumCardinality { get; set; }
        // // [DataMember]
        public string DtdIdentifier { get; set; }
        // // [DataMember]
        public string RoutineBody { get; set; }
        // // [DataMember]
        public string RoutineDefinition { get; set; }
        // // [DataMember]
        public string ExternalName { get; set; }
        // // [DataMember]
        public string ExternalLanguage { get; set; }
        // // [DataMember]
        public string ParameterStyle { get; set; }
        // // [DataMember]
        public string IsDeterministic { get; set; }
        // // [DataMember]
        public string SqlDataAccess { get; set; }
        // // [DataMember]
        public string IsNullCall { get; set; }
        // // [DataMember]
        public string SqlPath { get; set; }
        // // [DataMember]
        public string SchemaLevelRoutine { get; set; }
        // // [DataMember]
        public string MaxDynamicResultSets { get; set; }
        // // [DataMember]
        public string IsUserDefinedCast { get; set; }
        // // [DataMember]
        public string IsImplicitlyInvocable { get; set; }
        // // [DataMember]
        public string Created { get; set; }
        // // [DataMember]
        public string LastAltered { get; set; }

        // // [DataMember]
        public string FullName { get { return string.IsNullOrEmpty(RoutineSchema) ? RoutineName : RoutineSchema + "." + RoutineName; } }

        // // [DataMember]
        public List<DbParameterInfo> Parameters { get; set; }

        public DbRoutineInfo()
        {
            Parameters = new List<DbParameterInfo>();
        }

        public DbRoutineInfo(DataTable tb, DataRow row)
            : this()
        {
            for (int i = 0; i < tb.Columns.Count; i++)
                if (tb.Columns[i].DataType == typeof(string))
                    row[i] = Conv.Trim(row[i]);

            if (tb.Columns.Contains("SPECIFIC_CATALOG"))
                this.SpecificCatalog = Conv.ToString(row["SPECIFIC_CATALOG"]);
            if (tb.Columns.Contains("SPECIFIC_SCHEMA"))
                this.SpecificSchema = Conv.ToString(row["SPECIFIC_SCHEMA"]);
            if (tb.Columns.Contains("SPECIFIC_NAME"))
                this.SpecificName = Conv.ToString(row["SPECIFIC_NAME"]);
            if (tb.Columns.Contains("ROUTINE_CATALOG"))
                this.RoutineCatalog = Conv.ToString(row["ROUTINE_CATALOG"]);
            if (tb.Columns.Contains("ROUTINE_SCHEMA"))
                this.RoutineSchema = Conv.ToString(row["ROUTINE_SCHEMA"]);
            if (tb.Columns.Contains("ROUTINE_NAME"))
                this.RoutineName = Conv.ToString(row["ROUTINE_NAME"]);
            if (tb.Columns.Contains("ROUTINE_TYPE"))
                this.RoutineTypeStr = Conv.ToString(row["ROUTINE_TYPE"]).ToUpper();

            if (RoutineTypeStr == "PROCEDURE")
                RoutineType = EnumRoutineType.Procedure;
            else if (RoutineTypeStr == "FUNCTION")
                RoutineType = EnumRoutineType.Function;
            else
                RoutineType = EnumRoutineType.Undefined;

            if (tb.Columns.Contains("MODULE_CATALOG"))
                this.ModuleCatalog = Conv.ToString(row["MODULE_CATALOG"]);
            if (tb.Columns.Contains("MODULE_SCHEMA"))
                this.ModuleSchema = Conv.ToString(row["MODULE_SCHEMA"]);
            if (tb.Columns.Contains("MODULE_NAME"))
                this.ModuleName = Conv.ToString(row["MODULE_NAME"]);
            if (tb.Columns.Contains("UDT_CATALOG"))
                this.UdtCatalog = Conv.ToString(row["UDT_CATALOG"]);
            if (tb.Columns.Contains("UDT_SCHEMA"))
                this.UdtSchema = Conv.ToString(row["UDT_SCHEMA"]);
            if (tb.Columns.Contains("UDT_NAME"))
                this.UdtName = Conv.ToString(row["UDT_NAME"]);
            if (tb.Columns.Contains("DATA_TYPE"))
                this.DataType = Conv.ToString(row["DATA_TYPE"]);
            if (tb.Columns.Contains("CHARACTER_MAXIMUM_LENGTH"))
                this.CharacterMaximumLength = Conv.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
            if (tb.Columns.Contains("CHARACTER_OCTET_LENGTH"))
                this.CharacterOctetLength = Conv.ToString(row["CHARACTER_OCTET_LENGTH"]);
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
                this.NumericPrecision = Conv.ToString(row["NUMERIC_PRECISION"]);
            if (tb.Columns.Contains("NUMERIC_PRECISION_RADIX"))
                this.NumericPrecisionRadix = Conv.ToString(row["NUMERIC_PRECISION_RADIX"]);
            if (tb.Columns.Contains("NUMERIC_SCALE"))
                this.NumericScale = Conv.ToString(row["NUMERIC_SCALE"]);
            if (tb.Columns.Contains("DATETIME_PRECISION"))
                this.DatetimePrecision = Conv.ToString(row["DATETIME_PRECISION"]);
            if (tb.Columns.Contains("INTERVAL_TYPE"))
                this.IntervalType = Conv.ToString(row["INTERVAL_TYPE"]);
            if (tb.Columns.Contains("INTERVAL_PRECISION"))
                this.IntervalPrecision = Conv.ToString(row["INTERVAL_PRECISION"]);
            if (tb.Columns.Contains("TYPE_UDT_CATALOG"))
                this.TypeUdtCatalog = Conv.ToString(row["TYPE_UDT_CATALOG"]);
            if (tb.Columns.Contains("TYPE_UDT_SCHEMA"))
                this.TypeUdtSchema = Conv.ToString(row["TYPE_UDT_SCHEMA"]);
            if (tb.Columns.Contains("TYPE_UDT_NAME"))
                this.TypeUdtName = Conv.ToString(row["TYPE_UDT_NAME"]);
            if (tb.Columns.Contains("SCOPE_CATALOG"))
                this.ScopeCatalog = Conv.ToString(row["SCOPE_CATALOG"]);
            if (tb.Columns.Contains("SCOPE_SCHEMA"))
                this.ScopeSchema = Conv.ToString(row["SCOPE_SCHEMA"]);
            if (tb.Columns.Contains("SCOPE_NAME"))
                this.ScopeName = Conv.ToString(row["SCOPE_NAME"]);
            if (tb.Columns.Contains("MAXIMUM_CARDINALITY"))
                this.MaximumCardinality = Conv.ToString(row["MAXIMUM_CARDINALITY"]);
            if (tb.Columns.Contains("DTD_IDENTIFIER"))
                this.DtdIdentifier = Conv.ToString(row["DTD_IDENTIFIER"]);
            if (tb.Columns.Contains("ROUTINE_BODY"))
                this.RoutineBody = Conv.ToString(row["ROUTINE_BODY"]);
            if (tb.Columns.Contains("ROUTINE_DEFINITION"))
                this.RoutineDefinition = Conv.ToString(row["ROUTINE_DEFINITION"]);
            if (tb.Columns.Contains("EXTERNAL_NAME"))
                this.ExternalName = Conv.ToString(row["EXTERNAL_NAME"]);
            if (tb.Columns.Contains("EXTERNAL_LANGUAGE"))
                this.ExternalLanguage = Conv.ToString(row["EXTERNAL_LANGUAGE"]);
            if (tb.Columns.Contains("PARAMETER_STYLE"))
                this.ParameterStyle = Conv.ToString(row["PARAMETER_STYLE"]);
            if (tb.Columns.Contains("IS_DETERMINISTIC"))
                this.IsDeterministic = Conv.ToString(row["IS_DETERMINISTIC"]);
            if (tb.Columns.Contains("SQL_DATA_ACCESS"))
                this.SqlDataAccess = Conv.ToString(row["SQL_DATA_ACCESS"]);
            if (tb.Columns.Contains("IS_NULL_CALL"))
                this.IsNullCall = Conv.ToString(row["IS_NULL_CALL"]);
            if (tb.Columns.Contains("SQL_PATH"))
                this.SqlPath = Conv.ToString(row["SQL_PATH"]);
            if (tb.Columns.Contains("SCHEMA_LEVEL_ROUTINE"))
                this.SchemaLevelRoutine = Conv.ToString(row["SCHEMA_LEVEL_ROUTINE"]);
            if (tb.Columns.Contains("MAX_DYNAMIC_RESULT_SETS"))
                this.MaxDynamicResultSets = Conv.ToString(row["MAX_DYNAMIC_RESULT_SETS"]);
            if (tb.Columns.Contains("IS_USER_DEFINED_CAST"))
                this.IsUserDefinedCast = Conv.ToString(row["IS_USER_DEFINED_CAST"]);
            if (tb.Columns.Contains("IS_IMPLICITLY_INVOCABLE"))
                this.IsImplicitlyInvocable = Conv.ToString(row["IS_IMPLICITLY_INVOCABLE"]);
            if (tb.Columns.Contains("CREATED"))
                this.Created = Conv.ToString(row["CREATED"]);
            if (tb.Columns.Contains("LAST_ALTERED"))
                this.LastAltered = Conv.ToString(row["LAST_ALTERED"]);
        }

        public GenProcedure ToGenProcedure()
        {
            GenProcedure table = new GenProcedure();
            table.SchemaName = this.RoutineSchema;
            table.ProcedureName = this.RoutineName;
            return table;
        }

    }

}
