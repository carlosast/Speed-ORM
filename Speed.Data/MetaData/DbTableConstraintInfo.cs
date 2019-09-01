using Speed.Common;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class DbTableConstraintInfo
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
        public bool IsDeferrable { get; set; }
        // // [DataMember]
        public bool InitiallyDeferred { get; set; }

        // // [DataMember]
        public EnumConstraintType ConstraintType { get; set; }

        // // [DataMember]
        public List<DbConstraintColumnInfo> Columns { get; set; }

        #endregion Declarations

        public DbTableConstraintInfo()
        {
            Columns = new List<DbConstraintColumnInfo>();
        }

        public DbTableConstraintInfo(Database db, DataRow row, DataTable tbAux)
            : this()
        {
            DataTable tb = row.Table;
            for (int i = 0; i < tb.Columns.Count; i++)
                if (tb.Columns[i].DataType == typeof(string))
                    row[i] = Conv.Trim(row[i]);

            #region  Access

            if (db.ProviderType == EnumDbProviderType.Access)
            {
                // AccessOleDb
                //Table_Constraints
                //CONSTRAINT_CATALOG | CONSTRAINT_SCHEMA |                         CONSTRAINT_NAME | TABLE_CATALOG | TABLE_SCHEMA |                   TABLE_NAME | CONSTRAINT_TYPE | IS_DEFERRABLE | INITIALLY_DEFERRED | DESCRIPTION
                //              NULL |              NULL |                          'CustomerSale' |          NULL |         NULL |                       'Sale' |   'FOREIGN KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL | 'Data_3DBC83DA87584704B1D4B5488B06FE2A' |          NULL |         NULL |              'MSysResources' |        'UNIQUE' |             0 |                  0 |        NULL
                //              NULL |              NULL |                                    'Id' |          NULL |         NULL |          'MSysAccessStorage' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                                    'Id' |          NULL |         NULL | 'MSysNavPaneGroupCategories' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                                    'Id' |          NULL |         NULL |          'MSysNavPaneGroups' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                                    'Id' |          NULL |         NULL |  'MSysNavPaneGroupToObjects' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                                    'Id' |          NULL |         NULL |              'MSysResources' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                            'ParentIdId' |          NULL |         NULL |          'MSysAccessStorage' |        'UNIQUE' |             0 |                  0 |        NULL
                //              NULL |              NULL |                          'ParentIdName' |          NULL |         NULL |          'MSysAccessStorage' |        'UNIQUE' |             0 |                  0 |        NULL
                //              NULL |              NULL |                            'PrimaryKey' |          NULL |         NULL |                   'Customer' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                            'PrimaryKey' |          NULL |         NULL |                       'Sale' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                            'PrimaryKey' |          NULL |         NULL |                 'SaleDetail' |   'PRIMARY KEY' |             0 |                  0 |        NULL
                //              NULL |              NULL |                        'SaleSaleDetail' |          NULL |         NULL |                 'SaleDetail' |   'FOREIGN KEY' |             0 |                  0 |        NULL

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
                if (tb.Columns.Contains("IS_DEFERRABLE"))
                    this.IsDeferrable = Conv.ToBoolean(row["IS_DEFERRABLE"]);
                if (tb.Columns.Contains("INITIALLY_DEFERRED"))
                    this.InitiallyDeferred = Conv.ToBoolean(row["INITIALLY_DEFERRED"]);

                string type = ((string)row["CONSTRAINT_TYPE"]).ToUpper();

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

            #endregion  Access

            #region  SQLite

            else if (db.ProviderType == EnumDbProviderType.SQLite)
            {
            }

            #endregion  SQLite

            #region  Oracle

            else if (db.ProviderType == EnumDbProviderType.Oracle)
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
                if (tb.Columns.Contains("IS_DEFERRABLE"))
                    this.IsDeferrable = Conv.ToBoolean(row["IS_DEFERRABLE"]);
                if (tb.Columns.Contains("INITIALLY_DEFERRED"))
                    this.InitiallyDeferred = Conv.ToBoolean(row["INITIALLY_DEFERRED"]);

                string type = ((string)row["CONSTRAINT_TYPE"]).ToUpper();

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
                else
                    this.ConstraintType = EnumConstraintType.Other;


                if (string.IsNullOrEmpty(this.TableCatalog))
                    this.TableCatalog = this.ConstraintCatalog;
            }

            #endregion  SQLite

            #region  INFORMATION_SCHEMA

            else
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
                if (tb.Columns.Contains("IS_DEFERRABLE"))
                    this.IsDeferrable = Conv.ToBoolean(row["IS_DEFERRABLE"]);
                if (tb.Columns.Contains("INITIALLY_DEFERRED"))
                    this.InitiallyDeferred = Conv.ToBoolean(row["INITIALLY_DEFERRED"]);

                string type = ((string)row["CONSTRAINT_TYPE"]).ToUpper();

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
                else
                    this.ConstraintType = EnumConstraintType.Other;


                if (string.IsNullOrEmpty(this.TableCatalog))
                    this.TableCatalog = this.ConstraintCatalog;
            }

            #endregion  INFORMATION_SCHEMA

        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", TableCatalog, TableSchema, TableName);
        }

    }

}
