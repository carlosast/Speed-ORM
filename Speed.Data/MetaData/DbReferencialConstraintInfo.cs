using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class DbReferencialConstraintInfo
    {

        #region Delcarations

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

        #endregion Delcarations

        public DbReferencialConstraintInfo()
        {
            Columns = new List<DbConstraintColumnInfo>();
        }

        public DbReferencialConstraintInfo(Database db, DataRow row, DataTable tbAux, EnumConstraintType constraintType)
            : this()
        {
            DataTable tb = row.Table;
            for (int i = 0; i < tb.Columns.Count; i++)
                if (tb.Columns[i].DataType == typeof(string))
                    row[i] = Conv.Trim(row[i]);

            #region Access

            if (db.ProviderType == EnumDbProviderType.Access)
            {

                // tbAux
                //PK_TABLE_CATALOG | PK_TABLE_SCHEMA | PK_TABLE_NAME | PK_COLUMN_NAME | PK_COLUMN_GUID | PK_COLUMN_PROPID | FK_TABLE_CATALOG | FK_TABLE_SCHEMA | FK_TABLE_NAME |   FK_COLUMN_NAME | FK_COLUMN_GUID | FK_COLUMN_PROPID | ORDINAL | UPDATE_RULE | DELETE_RULE |      PK_NAME |          FK_NAME | DEFERRABILITY
                //            NULL |            NULL |    'Customer' |   'CustomerId' |           NULL |                0 |             NULL |            NULL |        'Sale' | 'SaleCustomerId' |           NULL |                0 |       1 |   'CASCADE' |   'CASCADE' | 'PrimaryKey' |   'CustomerSale' |             0
                //            NULL |            NULL |        'Sale' |       'SaleId' |           NULL |                0 |             NULL |            NULL |  'SaleDetail' |      'DetSaleId' |           NULL |                0 |       1 |   'CASCADE' |   'CASCADE' | 'PrimaryKey' | 'SaleSaleDetail' |             0

                if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
                    this.ConstraintCatalog = Conv.ToString(row["CONSTRAINT_CATALOG"]);

                if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
                    this.ConstraintSchema = Conv.ToString(row["CONSTRAINT_SCHEMA"]);

                if (tb.Columns.Contains("CONSTRAINT_NAME"))
                    this.ConstraintName = (string)row["CONSTRAINT_NAME"];

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_CATALOG"))
                    this.UniqueConstraintCatalog = Conv.ToString(row["UNIQUE_CONSTRAINT_CATALOG"]);

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_SCHEMA"))
                    this.UniqueConstraintSchema = Conv.ToString(row["UNIQUE_CONSTRAINT_SCHEMA"]);

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_NAME"))
                    this.UniqueConstraintName = Conv.ToString(row["UNIQUE_CONSTRAINT_NAME"]);

                // Access retorna errado UNIQUE_CONSTRAINT_NAME
                var rowsAux = tbAux.Select("FK_NAME  =" + Conv.ToSqlTextA(ConstraintName));
                if (rowsAux.Length > 0)
                {
                    if (tbAux.Columns.Contains("PK_NAME"))
                        this.UniqueConstraintName = Conv.ToString(rowsAux[0]["PK_NAME"]);
                }

                this.ConstraintType = constraintType;
            }

            #endregion Access

            #region Oracle

            else if (db.ProviderType == EnumDbProviderType.Oracle)
            {
                //                 CONSTRAINT_NAME | CONSTRAINT_TYPE |                       TABLE_NAME | CONSTRAINT_SCHEMA | UNIQUE_CONSTRAINT_SCHEMA | UNIQUE_CONSTRAINT_NAME
                //                  'SYS_C0011078' |   'FOREIGN_KEY' |                    'SALE_DETAIL' |           'SPEED' |                  'SPEED' |         'SYS_C0011069'
                //                  'SYS_C0011077' |   'PRIMARY_KEY' |                    'SALE_DETAIL' |           'SPEED' |                     NULL |                   NULL
                //                  'SYS_C0011066' |   'PRIMARY_KEY' |                       'CUSTOMER' |           'SPEED' |                     NULL |                   NULL
                //                  'SYS_C0011069' |   'PRIMARY_KEY' |                           'SALE' |           'SPEED' |                     NULL |                   NULL
                //'BIN$An6tgfdlQhaydQgNTyb3Vg==$0' |   'PRIMARY_KEY' | 'BIN$yfZqiay1RIKcqxxE0zCV0Q==$0' |           'SPEED' |                     NULL |                   NULL
                //                  'SYS_C0011070' |   'FOREIGN_KEY' |                           'SALE' |           'SPEED' |                  'SPEED' |         'SYS_C0011066'
                //'BIN$OxzcZdG5S127UtLJfkkfhg==$0' |   'PRIMARY_KEY' | 'BIN$Wc1XCCf4RymsVLcBHyS+UQ==$0' |           'SPEED' |                     NULL |                   NULL
                //'BIN$WgWH4ZIlRxuG9dSDt0Doow==$0' |   'PRIMARY_KEY' | 'BIN$+lf/qZHMSSmAWDtgE0PcmQ==$0' |           'SPEED' |                     NULL |                   NULL
                //'BIN$yrXCZtcVRp6ycrT36aV6dQ==$0' |   'PRIMARY_KEY' | 'BIN$8e4C91sPQCyqrHfJAkD7Lg==$0' |           'SPEED' |                     NULL |                   NULL


                if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
                    this.ConstraintCatalog = Conv.ToString(row["CONSTRAINT_CATALOG"]);

                if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
                    this.ConstraintSchema = Conv.ToString(row["CONSTRAINT_SCHEMA"]);

                if (tb.Columns.Contains("CONSTRAINT_NAME"))
                    this.ConstraintName = (string)row["CONSTRAINT_NAME"];

                if (tb.Columns.Contains("TABLE_CATALOG"))
                    this.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);

                if (tb.Columns.Contains("TABLE_SCHEMA"))
                    this.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);

                if (tb.Columns.Contains("TABLE_NAME"))
                    this.TableName = Conv.ToString(row["TABLE_NAME"]);

                //if (tb.Columns.Contains("FKEY_TO_CATALOG"))
                //    this.UniqueConstraintCatalog = Conv.ToString(row["FKEY_TO_CATALOG"]);

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_SCHEMA"))
                    this.UniqueConstraintSchema = Conv.ToString(row["UNIQUE_CONSTRAINT_SCHEMA"]);

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_NAME"))
                    this.UniqueConstraintName = Conv.ToString(row["UNIQUE_CONSTRAINT_NAME"]);
            }

            #endregion Oracle

            #region SQLLite

            else if (db.ProviderType == EnumDbProviderType.SQLite)
            {
                if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
                    this.ConstraintCatalog = Conv.ToString(row["CONSTRAINT_CATALOG"]);

                if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
                    this.ConstraintSchema = Conv.ToString(row["CONSTRAINT_SCHEMA"]);

                if (tb.Columns.Contains("CONSTRAINT_NAME"))
                    this.ConstraintName = (string)row["CONSTRAINT_NAME"];

                if (tb.Columns.Contains("TABLE_CATALOG"))
                    this.TableCatalog = Conv.ToString(row["TABLE_CATALOG"]);

                if (tb.Columns.Contains("TABLE_SCHEMA"))
                    this.TableSchema = Conv.ToString(row["TABLE_SCHEMA"]);

                if (tb.Columns.Contains("TABLE_NAME"))
                    this.TableName = Conv.ToString(row["TABLE_NAME"]);

                if (tb.Columns.Contains("FKEY_TO_CATALOG"))
                    this.UniqueConstraintCatalog = Conv.ToString(row["FKEY_TO_CATALOG"]);

                if (tb.Columns.Contains("FKEY_TO_SCHEMA"))
                    this.UniqueConstraintSchema = Conv.ToString(row["FKEY_TO_SCHEMA"]);

                if (tb.Columns.Contains("FKEY_TO_TABLE")) // ?
                    this.UniqueConstraintName = Conv.ToString(row["FKEY_TO_TABLE"]);
            }

            #endregion SQLLite

            #region  INFORMATION_SCHEMA

            else
            {
                if (tb.Columns.Contains("CONSTRAINT_CATALOG"))
                    this.ConstraintCatalog = Conv.Trim(row["CONSTRAINT_CATALOG"]);

                if (tb.Columns.Contains("CONSTRAINT_SCHEMA"))
                    this.ConstraintSchema = Conv.Trim(row["CONSTRAINT_SCHEMA"]);

                if (tb.Columns.Contains("CONSTRAINT_NAME"))
                    this.ConstraintName = (string)row["CONSTRAINT_NAME"];

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_CATALOG"))
                    this.UniqueConstraintCatalog = Conv.Trim(row["UNIQUE_CONSTRAINT_CATALOG"]);
                if (tb.Columns.Contains("REFERENCED_TABLE_CATALOG"))
                    this.UniqueConstraintCatalog = Conv.Trim(row["REFERENCED_TABLE_CATALOG"]);

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_SCHEMA"))
                    this.UniqueConstraintSchema = Conv.Trim(row["UNIQUE_CONSTRAINT_SCHEMA"]);
                if (tb.Columns.Contains("R_OWNER"))
                    this.UniqueConstraintSchema = Conv.Trim(row["R_OWNER"]);
                if (tb.Columns.Contains("REFERENCED_TABLE_SCHEMA"))
                    this.UniqueConstraintSchema = Conv.Trim(row["REFERENCED_TABLE_SCHEMA"]);

                if (tb.Columns.Contains("UNIQUE_CONSTRAINT_NAME"))
                    this.UniqueConstraintName = Conv.Trim(row["UNIQUE_CONSTRAINT_NAME"]);
                if (tb.Columns.Contains("R_CONSTRAINT_NAME"))
                    this.UniqueConstraintName = Conv.Trim(row["R_CONSTRAINT_NAME"]);
                if (tb.Columns.Contains("INDEX_NAME"))
                    this.UniqueConstraintName = Conv.Trim(row["INDEX_NAME"]);

                //CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |    CONSTRAINT_NAME | TABLE_CATALOG | TABLE_SCHEMA |    TABLE_NAME | REFERENCED_TABLE_CATALOG | REFERENCED_TABLE_SCHEMA | REFERENCED_TABLE_NAME | IS_DEFERRABLE | INITIALLY_DEFERRED | MATCH_OPTION | UPDATE_RULE | DELETE_RULE |         INDEX_NAME
                //              NULL |              NULL |        'FK_SALE_1' |          NULL |         NULL |        'SALE' |                     NULL |                    NULL |            'CUSTOMER' |          'NO' |               'NO' |       'FULL' |   'CASCADE' |   'CASCADE' |        'FK_SALE_1'
                //              NULL |              NULL | 'FK_SALE_DETAIL_1' |          NULL |         NULL | 'SALE_DETAIL' |                     NULL |                    NULL |                'SALE' |          'NO' |               'NO' |       'FULL' |   'CASCADE' |   'CASCADE' | 'FK_SALE_DETAIL_1'



                this.ConstraintType = constraintType;
            }

            #endregion  INFORMATION_SCHEMA

        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3} - {4}", TableFullName, ConstraintFullName, UniqueConstraintCatalog, UniqueConstraintSchema, UniqueConstraintName);
        }

    }

}
