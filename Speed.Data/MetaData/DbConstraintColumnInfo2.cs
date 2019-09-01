using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using Speed.Common;

namespace Speed.Data.MetaData
{

    public class DbConstraintColumnInfo2
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
        public string ColumnName { get; set; }
        // // [DataMember]
        public int OrdinalPosition { get; set; }

        // // [DataMember]
        public string ReferencedTableSchema { get; set; }
        // // [DataMember]
        public string ReferencedTableName { get; set; }
        // // [DataMember]
        public string ReferencedColumnName { get; set; }
        // // [DataMember]
        public string ReferencedTableFullName { get { return Conv.GetKey(ReferencedTableSchema, ReferencedTableName); } }

        public DbConstraintColumnInfo2()
        {
        }

        public DbConstraintColumnInfo2(Database db, DataRow row)
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
            if (tb.Columns.Contains("UNIQUE_CONSTRAINT_CATALOG"))
                this.TableCatalog = Conv.ToString(row["UNIQUE_CONSTRAINT_CATALOG"]);

            if (tb.Columns.Contains("TABLE_SCHEMA"))
                this.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);
            if (tb.Columns.Contains("UNIQUE_CONSTRAINT_SCHEMA"))
                this.TableSchema = Conv.ToString(row["UNIQUE_CONSTRAINT_SCHEMA"]);

            if (tb.Columns.Contains("TABLE_NAME"))
                this.TableName = Conv.ToString(row["TABLE_NAME"]);
            if (tb.Columns.Contains("UNIQUE_CONSTRAINT_NAME"))
                this.TableName = Conv.ToString(row["UNIQUE_CONSTRAINT_NAME"]);

            // Access
            if (tb.Columns.Contains("FK_TABLE_CATALOG"))
                this.TableCatalog = Conv.ToString(row["FK_TABLE_CATALOG"]);
            if (tb.Columns.Contains("FK_TABLE_SCHEMA"))
                this.TableSchema = Conv.ToString(row["FK_TABLE_SCHEMA"]);
            if (tb.Columns.Contains("FK_TABLE_NAME"))
                this.TableName = (string)row["FK_TABLE_NAME"];


            if (tb.Columns.Contains("COLUMN_NAME"))
                this.ColumnName = (string)row["COLUMN_NAME"];
            // SQLite
            if (tb.Columns.Contains("FKEY_FROM_COLUMN"))
                this.ColumnName = (string)row["FKEY_FROM_COLUMN"];
            // Access
            if (tb.Columns.Contains("FK_COLUMN_NAME"))
                this.ColumnName = (string)row["FK_COLUMN_NAME"];

            // SQLite
            if (tb.Columns.Contains("FKEY_TO_SCHEMA"))
                this.ReferencedTableSchema = Conv.ToString(row["FKEY_TO_SCHEMA"]);
            if (tb.Columns.Contains("FKEY_TO_TABLE"))
                this.ReferencedTableName = (string)row["FKEY_TO_TABLE"];
            if (tb.Columns.Contains("FKEY_TO_COLUMN"))
                this.ReferencedColumnName = (string)row["FKEY_TO_COLUMN"];

            // Access
            if (tb.Columns.Contains("FK_TABLE_SCHEMA"))
                this.ReferencedTableSchema = Conv.ToString(row["FK_TABLE_SCHEMA"]);
            if (tb.Columns.Contains("FK_TABLE_NAME"))
                this.ReferencedTableName = (string)row["FK_TABLE_NAME"];
            // Access
            if (tb.Columns.Contains("PK_COLUMN_NAME"))
                this.ReferencedColumnName = (string)row["PK_COLUMN_NAME"];

            if (tb.Columns.Contains("ORDINAL_POSITION"))
                this.OrdinalPosition = Convert.ToInt32(row["ORDINAL_POSITION"]);
            if (tb.Columns.Contains("FKEY_FROM_ORDINAL_POSITION"))
                this.OrdinalPosition = Convert.ToInt32(row["FKEY_FROM_ORDINAL_POSITION"]);
            if (tb.Columns.Contains("ORDINAL"))
                this.OrdinalPosition = Convert.ToInt32(row["ORDINAL"]);
        }

    }

}
