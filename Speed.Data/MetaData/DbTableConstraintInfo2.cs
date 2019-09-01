using Speed.Common;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class DbTableConstraintInfo2
    {

        // // [DataMember]
        public string ConstraintCatalog { get; set; }
        // // [DataMember]
        public string ConstraintSchema { get; set; }
        // // [DataMember]
        public string ConstraintName { get; set; }
        // // [DataMember]
        public string ConstraintFullName { get { return Conv.GetKey(ConstraintCatalog, ConstraintSchema, ConstraintName); } }

        // // [DataMember]
        public string TableCatalog { get; set; }
        // // [DataMember]
        public string TableSchema { get; set; }
        // // [DataMember]
        public string TableName { get; set; }
        // // [DataMember]
        public string TableFullName { get { return Conv.GetKey(TableCatalog, TableSchema, TableName); } }

        // // [DataMember]
        public bool IsDeferrable { get; set; }
        // // [DataMember]
        public bool InitiallyDeferred { get; set; }

        // // [DataMember]
        public EnumConstraintType ConstraintType { get; set; }

        // // [DataMember]
        public List<DbConstraintColumnInfo> Columns { get; set; }

        public DbTableConstraintInfo2()
        {
            Columns = new List<DbConstraintColumnInfo>();
        }

        public DbTableConstraintInfo2(Database db, DataRow row)
            : this()
        {
            DataTable tb = row.Table;

            if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
                this.ConstraintCatalog = Conv.ToString(row["CONSTRAINT_CATALOG"]);
            if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
                this.ConstraintSchema = Conv.ToString(row["CONSTRAINT_SCHEMA"]);
            
            if (tb.Columns.Contains("CONSTRAINT_NAME"))
                this.ConstraintName = (string)row["CONSTRAINT_NAME"];
            if (tb.Columns.Contains("FK_NAME"))
                this.ConstraintName = (string)row["FK_NAME"];
            
            if (tb.Columns.Contains("TABLE_CATALOG"))
                this.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
            if (tb.Columns.Contains("TABLE_SCHEMA"))
                this.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
            if (tb.Columns.Contains("TABLE_NAME"))
                this.TableName = Conv.ToString(row["TABLE_NAME"]);
            if (tb.Columns.Contains("IS_DEFERRABLE"))
                this.IsDeferrable = Conv.ToBoolean(row["IS_DEFERRABLE"]);
            if (tb.Columns.Contains("INITIALLY_DEFERRED"))
                this.InitiallyDeferred = Conv.ToBoolean(row["INITIALLY_DEFERRED"]);

            // Access
            if (tb.Columns.Contains("FK_TABLE_CATALOG"))
                this.TableCatalog = Conv.ToString(row["FK_TABLE_CATALOG"]);
            if (tb.Columns.Contains("FK_TABLE_SCHEMA"))
                this.TableSchema = Conv.ToString(row["FK_TABLE_SCHEMA"]);
            if (tb.Columns.Contains("FK_TABLE_NAME"))
                this.TableName = (string)row["FK_TABLE_NAME"];

            string type = "FOREIGN";
            if (tb.Columns.Contains("CONSTRAINT_TYPE"))
                type = ((string)row["CONSTRAINT_TYPE"]).ToUpper();

            if (type.StartsWith("UNIQUE"))
                this.ConstraintType = EnumConstraintType.Unique;
            else if (type.StartsWith("PRIMARY"))
                this.ConstraintType = EnumConstraintType.PrimaryKey;
            else if (type.StartsWith("FOREIGN"))
                this.ConstraintType = EnumConstraintType.ForeignKey;

            else if (type.Contains("UNIQUE"))
                this.ConstraintType = EnumConstraintType.Unique;
            else if (type.Contains("PRIMARY"))
                this.ConstraintType = EnumConstraintType.PrimaryKey;
            else if (type.Contains("FOREIGN"))
                this.ConstraintType = EnumConstraintType.ForeignKey;

            if (string.IsNullOrEmpty(this.TableCatalog))
                this.TableCatalog = this.ConstraintCatalog;

        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", TableCatalog, TableSchema, TableName);
        }

    }

}
