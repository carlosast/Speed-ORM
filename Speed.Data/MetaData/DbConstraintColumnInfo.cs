using Speed.Common;
using System;
using System.Data;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class DbConstraintColumnInfo
    {

        #region Declarations

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

        #endregion Declarations

        public DbConstraintColumnInfo()
        {
        }

        public DbConstraintColumnInfo(Database db, DataRow row, DataTable tbAux)
            : this()
        {
            throw new NotImplementedException();

            //DbTable tb = row.Table;
            //for (int i = 0; i < tb.Columns.Count; i++)
            //    if (tb.Columns[i].DataType == typeof(string))
            //        row[i] = Conv.Trim(row[i]);

            //#region Access

            //if (db.ProviderType == EnumDbProviderType.Access)
            //{
            //    //Constraint_Column_Usage
            //    //TABLE_CATALOG | TABLE_SCHEMA |                   TABLE_NAME |      COLUMN_NAME | COLUMN_GUID | COLUMN_PROPID | CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |                         CONSTRAINT_NAME

            //    if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
            //        this.ConstraintCatalog = Conv.Trim(row["CONSTRAINT_CATALOG"]);

            //    if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
            //        this.ConstraintSchema = Conv.Trim(row["CONSTRAINT_SCHEMA"]);

            //    if (tb.Columns.Contains("CONSTRAINT_NAME"))
            //        this.ConstraintName = (string)row["CONSTRAINT_NAME"];

            //    if (tb.Columns.Contains("TABLE_CATALOG"))
            //        this.TableCatalog = Conv.Trim(row["TABLE_CATALOG"]);

            //    if (tb.Columns.Contains("TABLE_SCHEMA"))
            //        this.TableSchema = Conv.Trim(row["TABLE_SCHEMA"]);

            //    if (tb.Columns.Contains("TABLE_NAME"))
            //        this.TableName = Conv.Trim(row["TABLE_NAME"]);

            //    if (tb.Columns.Contains("COLUMN_NAME"))
            //        this.ColumnName = (string)row["COLUMN_NAME"];

            //    if (tb.Columns.Contains("COLUMN_PROPID"))
            //        this.OrdinalPosition = Conv.ToInt32(row["COLUMN_PROPID"]);

            //    // tbAux
            //    //PK_TABLE_CATALOG | PK_TABLE_SCHEMA | PK_TABLE_NAME | PK_COLUMN_NAME | PK_COLUMN_GUID | PK_COLUMN_PROPID | FK_TABLE_CATALOG | FK_TABLE_SCHEMA | FK_TABLE_NAME |   FK_COLUMN_NAME | FK_COLUMN_GUID | FK_COLUMN_PROPID | ORDINAL | UPDATE_RULE | DELETE_RULE |      PK_NAME |          FK_NAME | DEFERRABILITY
            //    //            NULL |            NULL |    'Customer' |   'CustomerId' |           NULL |                0 |             NULL |            NULL |        'Sale' | 'SaleCustomerId' |           NULL |                0 |       1 |   'CASCADE' |   'CASCADE' | 'PrimaryKey' |   'CustomerSale' |             0
            //    //            NULL |            NULL |        'Sale' |       'SaleId' |           NULL |                0 |             NULL |            NULL |  'SaleDetail' |      'DetSaleId' |           NULL |                0 |       1 |   'CASCADE' |   'CASCADE' | 'PrimaryKey' | 'SaleSaleDetail' |             0
            //    var rowsAux = tbAux.Select("FK_NAME  =" + Conv.ToSqlTextA(ConstraintName));
            //    if (rowsAux.Length > 0)
            //    {
            //        if (tbAux.Columns.Contains("PK_TABLE_SCHEMA"))
            //            this.ReferencedTableSchema = Conv.Trim(rowsAux[0]["PK_TABLE_SCHEMA"]);
            //        if (tbAux.Columns.Contains("PK_TABLE_NAME"))
            //            this.ReferencedTableName = Conv.Trim(rowsAux[0]["PK_TABLE_NAME"]);
            //        if (tbAux.Columns.Contains("PK_COLUMN_NAME"))
            //            this.ReferencedColumnName = Conv.Trim(rowsAux[0]["PK_COLUMN_NAME"]);
            //    }
            //}

            //#endregion Access

            //#region Oracle

            //else if (db.ProviderType == EnumDbProviderType.Oracle)
            //{
            //    // ForeignKeys
            //    //  OWNER |                  CONSTRAINT_NAME | CONSTRAINT_TYPE |                       TABLE_NAME | R_OWNER | R_CONSTRAINT_NAME | INDEX_OWNER |                       INDEX_NAME |        COLUMN_NAME | POSITION
            //    //'SPEED' |                   'SYS_C0011070' |   'FOREIGN_KEY' |                           'SALE' | 'SPEED' |    'SYS_C0011066' |        NULL |                             NULL | 'SALE_CUSTOMER_ID' |        1
            //    //'SPEED' |                   'SYS_C0011078' |   'FOREIGN_KEY' |                    'SALE_DETAIL' | 'SPEED' |    'SYS_C0011069' |        NULL |                             NULL |      'DET_SALE_ID' |        1
            //    //'SPEED' |                   'SYS_C0011077' |   'PRIMARY_KEY' |                    'SALE_DETAIL' |    NULL |              NULL |     'SPEED' |                   'SYS_C0011077' |        'DETAIL_ID' |        1
            //    //'SPEED' | 'BIN$OxzcZdG5S127UtLJfkkfhg==$0' |   'PRIMARY_KEY' | 'BIN$Wc1XCCf4RymsVLcBHyS+UQ==$0' |    NULL |              NULL |     'SPEED' | 'BIN$KaS2w/QBQ62yDHceFQelSg==$0' |        'DETAIL_ID' |        1
            //    //'SPEED' |                   'SYS_C0011069' |   'PRIMARY_KEY' |                           'SALE' |    NULL |              NULL |     'SPEED' |                   'SYS_C0011069' |          'SALE_ID' |        1
            //    //'SPEED' |                   'SYS_C0011066' |   'PRIMARY_KEY' |                       'CUSTOMER' |    NULL |              NULL |     'SPEED' |                   'SYS_C0011066' |      'CUSTOMER_ID' |        1
            //    //'SPEED' | 'BIN$An6tgfdlQhaydQgNTyb3Vg==$0' |   'PRIMARY_KEY' | 'BIN$yfZqiay1RIKcqxxE0zCV0Q==$0' |    NULL |              NULL |     'SPEED' | 'BIN$wEobQjQ7TQCRjb+FYjRydg==$0' |         'DETAILID' |        1
            //    //'SPEED' | 'BIN$WgWH4ZIlRxuG9dSDt0Doow==$0' |   'PRIMARY_KEY' | 'BIN$+lf/qZHMSSmAWDtgE0PcmQ==$0' |    NULL |              NULL |     'SPEED' | 'BIN$d6RG9gpLRhSghpmNTzGGRQ==$0' |           'SALEID' |        1
            //    //'SPEED' | 'BIN$yrXCZtcVRp6ycrT36aV6dQ==$0' |   'PRIMARY_KEY' | 'BIN$8e4C91sPQCyqrHfJAkD7Lg==$0' |    NULL |              NULL |     'SPEED' | 'BIN$logiB/SLTA+NIAYNgzDZfw==$0' |       'CUSTOMERID' |        1

            //    //if (tb.Columns.Contains("TABLE_CATALOG"))
            //    //    this.TableCatalog = Conv.Trim(row["TABLE_CATALOG"]);

            //    // Constraint
            //    if (tb.Columns.Contains("OWNER"))
            //        this.ConstraintSchema = Conv.Trim(row["OWNER"]);

            //    if (tb.Columns.Contains("CONSTRAINT_NAME"))
            //        this.ConstraintName = (string)row["CONSTRAINT_NAME"];

            //    // Table
            //    if (tb.Columns.Contains("R_OWNER"))
            //        this.TableSchema = Conv.Trim(row["R_OWNER"]);
            //    if (string.IsNullOrEmpty(this.TableSchema))
            //        this.TableSchema = Conv.Trim(row["OWNER"]);

            //    if (tb.Columns.Contains("TABLE_NAME"))
            //        this.TableName = (string)row["TABLE_NAME"];

            //    if (tb.Columns.Contains("COLUMN_NAME"))
            //        this.ColumnName = (string)row["COLUMN_NAME"];

            //    if (tb.Columns.Contains("POSITION"))
            //        this.OrdinalPosition = Conv.ToInt32(row["POSITION"]);

            //    if (tb.Columns.Contains("R_OWNER"))
            //        this.ReferencedTableSchema = Conv.Trim(row["R_OWNER"]);

            //    if (string.IsNullOrEmpty(this.ReferencedTableSchema))
            //        this.ReferencedTableSchema = Conv.Trim(row["OWNER"]);

            //    if (string.IsNullOrEmpty(this.ReferencedTableSchema))
            //        ToString();


            //    //if (tb.Columns.Contains("FKEY_TO_TABLE"))
            //    //    this.ReferencedTableName = Conv.Trim(row["FKEY_TO_TABLE"]);
            //    //if (tb.Columns.Contains("FKEY_TO_COLUMN"))
            //    //    this.ReferencedColumnName = Conv.Trim(row["FKEY_TO_COLUMN"]);
            //}

            //#endregion Oracle

            //#region SQLite

            //else if (db.ProviderType == EnumDbProviderType.SQLite)
            //{
            //    // ForeignKeys
            //    // CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |     CONSTRAINT_NAME | TABLE_CATALOG | TABLE_SCHEMA |   TABLE_NAME | CONSTRAINT_TYPE | IS_DEFERRABLE | INITIALLY_DEFERRED | FKEY_ID | FKEY_FROM_COLUMN | FKEY_FROM_ORDINAL_POSITION | FKEY_TO_CATALOG | FKEY_TO_SCHEMA | FKEY_TO_TABLE | FKEY_TO_COLUMN | FKEY_ON_UPDATE | FKEY_ON_DELETE | FKEY_MATCH
            //    if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
            //        this.ConstraintCatalog = Conv.Trim(row["CONSTRAINT_CATALOG"]);

            //    if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
            //        this.ConstraintSchema = Conv.Trim(row["CONSTRAINT_SCHEMA"]);

            //    if (tb.Columns.Contains("CONSTRAINT_NAME"))
            //        this.ConstraintName = (string)row["CONSTRAINT_NAME"];

            //    if (tb.Columns.Contains("TABLE_CATALOG"))
            //        this.TableCatalog = Conv.Trim(row["TABLE_CATALOG"]);

            //    if (tb.Columns.Contains("TABLE_SCHEMA"))
            //        this.TableSchema = Conv.Trim(row["TABLE_SCHEMA"]);

            //    if (tb.Columns.Contains("TABLE_NAME"))
            //        this.TableName = (string)row["TABLE_NAME"];

            //    if (tb.Columns.Contains("FKEY_FROM_COLUMN"))
            //        this.ColumnName = (string)row["FKEY_FROM_COLUMN"];

            //    if (tb.Columns.Contains("FKEY_FROM_ORDINAL_POSITION"))
            //        this.OrdinalPosition = Conv.ToInt32(row["FKEY_FROM_ORDINAL_POSITION"]);

            //    // FKEY_TO_SCHEMA | FKEY_TO_TABLE | FKEY_TO_COLUMN 
            //    if (tb.Columns.Contains("FKEY_TO_SCHEMA"))
            //        this.ReferencedTableSchema = Conv.Trim(row["FKEY_TO_SCHEMA"]);
            //    if (tb.Columns.Contains("FKEY_TO_TABLE"))
            //        this.ReferencedTableName = Conv.Trim(row["FKEY_TO_TABLE"]);
            //    if (tb.Columns.Contains("FKEY_TO_COLUMN"))
            //        this.ReferencedColumnName = Conv.Trim(row["FKEY_TO_COLUMN"]);

            //    /*
            //    if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
            //        this.ConstraintCatalog = Conv.Trim(row["CONSTRAINT_CATALOG"]);

            //    if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
            //        this.ConstraintSchema = Conv.Trim(row["CONSTRAINT_SCHEMA"]);

            //    if (tb.Columns.Contains("CONSTRAINT_NAME"))
            //        this.ConstraintName = (string)row["CONSTRAINT_NAME"];

            //    if (tb.Columns.Contains("TABLE_CATALOG"))
            //        this.TableCatalog = Conv.Trim(row["TABLE_CATALOG"]);

            //    if (tb.Columns.Contains("TABLE_SCHEMA"))
            //        this.TableSchema = Conv.Trim(row["TABLE_SCHEMA"]);

            //    if (tb.Columns.Contains("TABLE_NAME"))
            //        this.TableName = (string)row["TABLE_NAME"];

            //    if (tb.Columns.Contains("COLUMN_NAME"))
            //        this.ColumnName = Conv.Trim(row["COLUMN_NAME"]);

            //    if (tb.Columns.Contains("ORDINAL_POSITION"))
            //        this.OrdinalPosition = Conv.ToInt32(row["ORDINAL_POSITION"]);
            //    */
            //}

            //#endregion SQLite

            //#region INFORMATION_SCHEMA

            //else
            //{
            //    if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
            //        this.ConstraintCatalog = Conv.Trim(row["CONSTRAINT_CATALOG"]);

            //    if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
            //        this.ConstraintSchema = Conv.Trim(row["CONSTRAINT_SCHEMA"]);

            //    if (tb.Columns.Contains("CONSTRAINT_NAME"))
            //        this.ConstraintName = (string)row["CONSTRAINT_NAME"];

            //    if (tb.Columns.Contains("TABLE_CATALOG"))
            //        this.TableCatalog = Conv.Trim(row["TABLE_CATALOG"]);
            //    if (tb.Columns.Contains("UNIQUE_CONSTRAINT_CATALOG"))
            //        this.TableCatalog = Conv.Trim(row["UNIQUE_CONSTRAINT_CATALOG"]);

            //    if (tb.Columns.Contains("TABLE_SCHEMA"))
            //        this.TableSchema = Conv.Trim(row["TABLE_SCHEMA"]);
            //    if (tb.Columns.Contains("UNIQUE_CONSTRAINT_SCHEMA"))
            //        this.TableSchema = Conv.Trim(row["UNIQUE_CONSTRAINT_SCHEMA"]);

            //    if (tb.Columns.Contains("TABLE_NAME"))
            //        this.TableName = (string)row["TABLE_NAME"];
            //    if (tb.Columns.Contains("UNIQUE_CONSTRAINT_NAME"))
            //        this.TableName = Conv.Trim(row["UNIQUE_CONSTRAINT_NAME"]);

            //    if (tb.Columns.Contains("COLUMN_NAME"))
            //        this.ColumnName = (string)row["COLUMN_NAME"];

            //    if (tb.Columns.Contains("ORDINAL_POSITION"))
            //        this.OrdinalPosition = Convert.ToInt32(row["ORDINAL_POSITION"]);


            //    if (tb.Columns.Contains("REFERENCED_TABLE_SCHEMA"))
            //        this.ReferencedTableSchema = Conv.Trim(row["REFERENCED_TABLE_SCHEMA"]);
            //    if (tb.Columns.Contains("REFERENCED_TABLE_NAME"))
            //        this.ReferencedTableName = Conv.Trim(row["REFERENCED_TABLE_NAME"]);
            //    if (tb.Columns.Contains("REFERENCED_COLUMN_NAME"))
            //        this.ReferencedColumnName = Conv.Trim(row["REFERENCED_COLUMN_NAME"]);

            //}
            //#endregion INFORMATION_SCHEMA

        }

    }

}
