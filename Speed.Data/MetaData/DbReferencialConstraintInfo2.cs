using Speed.Common;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    [DataContract]
    public class DbReferencialConstraintInfo2
    {

        [DataMember]
        public string ConstraintCatalog { get; set; }
        [DataMember]
        public string ConstraintSchema { get; set; }
        [DataMember]
        public string ConstraintName { get; set; }
        [DataMember]
        public string ConstraintFullName { get { return Conv.GetKey(ConstraintCatalog, ConstraintSchema, ConstraintName); } }

        [DataMember]
        public string UniqueConstraintCatalog { get; set; }
        [DataMember]
        public string UniqueConstraintSchema { get; set; }
        [DataMember]
        public string UniqueConstraintName { get; set; }
        [DataMember]
        public string UniqueConstraintFullName { get { return Conv.GetKey(UniqueConstraintCatalog, UniqueConstraintSchema, UniqueConstraintName); } }

        [DataMember]
        public string TableCatalog { get; set; }
        [DataMember]
        public string TableSchema { get; set; }
        [DataMember]
        public string TableName { get; set; }
        [DataMember]
        public string TableFullName { get { return Conv.GetKey(TableSchema, TableName); } }

        [DataMember]
        public EnumConstraintType ConstraintType { get; set; }

        [DataMember]
        public List<DbConstraintColumnInfo> Columns { get; set; }

        public DbReferencialConstraintInfo2()
        {
            Columns = new List<DbConstraintColumnInfo>();
        }

        public DbReferencialConstraintInfo2(DataTable tb, DataRow row, EnumConstraintType constraintType)
            : this()
        {
            if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
                this.ConstraintCatalog = Conv.ToString(row["CONSTRAINT_CATALOG"]);

            if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
                this.ConstraintSchema = Conv.ToString(row["CONSTRAINT_SCHEMA"]);

            if (tb.Columns.Contains("CONSTRAINT_NAME"))
                this.ConstraintName = (string)row["CONSTRAINT_NAME"];

            if (tb.Columns.Contains("UNIQUE_CONSTRAINT_CATALOG"))
                this.UniqueConstraintCatalog = Conv.ToString(row["UNIQUE_CONSTRAINT_CATALOG"]);
            if (tb.Columns.Contains("FKEY_TO_CATALOG"))
                this.UniqueConstraintCatalog = Conv.ToString(row["FKEY_TO_CATALOG"]);
            if (tb.Columns.Contains("FK_TABLE_CATALOG"))
                this.UniqueConstraintCatalog = Conv.ToString(row["FK_TABLE_CATALOG"]);

            if (tb.Columns.Contains("UNIQUE_CONSTRAINT_SCHEMA"))
                this.UniqueConstraintSchema = Conv.ToString(row["UNIQUE_CONSTRAINT_SCHEMA"]);
            if (tb.Columns.Contains("FKEY_TO_SCHEMA"))
                this.UniqueConstraintSchema = Conv.ToString(row["FKEY_TO_SCHEMA"]);
            if (tb.Columns.Contains("FK_TABLE_SCHEMA"))
                this.UniqueConstraintSchema = Conv.ToString(row["FK_TABLE_SCHEMA"]);
            if (tb.Columns.Contains("FK_NAME"))
                this.ConstraintName = Conv.ToString(row["FK_NAME"]);
            

            if (tb.Columns.Contains("UNIQUE_CONSTRAINT_NAME"))
                this.UniqueConstraintName = (string)row["UNIQUE_CONSTRAINT_NAME"];
            if (tb.Columns.Contains("FKEY_TO_TABLE"))
                this.UniqueConstraintName = (string)row["FKEY_TO_TABLE"];
            if (tb.Columns.Contains("FK_TABLE_NAME"))
                this.UniqueConstraintName = (string)row["FK_TABLE_NAME"];

            if (tb.Columns.Contains("TABLE_CATALOG"))
                this.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);
            if (tb.Columns.Contains("TABLE_SCHEMA"))
                this.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
            if (tb.Columns.Contains("TABLE_NAME"))
                this.TableName = (string)row["TABLE_NAME"];

            if (tb.Columns.Contains("PK_TABLE_CATALOG"))
                this.TableCatalog = Conv.ToString(row["PK_TABLE_CATALOG"]);
            if (tb.Columns.Contains("PK_TABLE_SCHEMA"))
                this.TableSchema = Conv.ToString(row["PK_TABLE_SCHEMA"]);
            if (tb.Columns.Contains("PK_TABLE_NAME"))
                this.TableName = (string)row["PK_TABLE_NAME"];

            this.ConstraintType = constraintType;

        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", UniqueConstraintCatalog, UniqueConstraintSchema, UniqueConstraintName);
        }

    }

}
