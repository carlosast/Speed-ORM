using Speed.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbTableInfo
    {

        #region Declarations

        [DataMember]
        public string TableName { get; set; }
        [DataMember]
        public string SchemaName { get; set; }
        [DataMember]
        public string SequenceColumn { get; set; }
        [DataMember]
        public string SequenceName { get; set; }
        [DataMember]
        public Dictionary<string, DbColumnInfo> Columns { get; set; }
        [DataMember]
        public Dictionary<string, DbColumnInfo> PrimaryKey { get; set; }
        [DataMember]
        public Dictionary<string, DbReferencialConstraintInfo> ParentRelations { get; set; }
        [DataMember]
        public Dictionary<string, DbReferencialConstraintInfo> ChildrenRelations { get; set; }
        [DataMember]
        public string FullName { get { return string.IsNullOrEmpty(SchemaName) ? TableName : SchemaName + "." + TableName; } }

        #endregion Declarations

        public DbTableInfo(Database cn, string tableName)
            : this(cn, tableName, false)
        {
        }

        public DbTableInfo(Database db, string tableName, bool isCommand = false)
            : this(db, null, tableName, isCommand, null, null)
        {
        }

        // TODO: implementar schemaName
        public DbTableInfo(Database db, string schemaName, string tableName, bool isCommand = false, string sequenceColumn = null, string sequenceName = null)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.SequenceColumn = sequenceColumn;
            this.SequenceName = sequenceName;

            Columns = new Dictionary<string, DbColumnInfo>(StringComparer.InvariantCultureIgnoreCase);
            PrimaryKey = new Dictionary<string, DbColumnInfo>(StringComparer.InvariantCultureIgnoreCase);
            ParentRelations = new Dictionary<string, DbReferencialConstraintInfo>(StringComparer.InvariantCultureIgnoreCase);
            ChildrenRelations = new Dictionary<string, DbReferencialConstraintInfo>();
            //DataTable tb = db.GetSchema("Columns", new string[] { null, null, tableName, null });
            DataTable tb = db.GetSchemaColumns(schemaName, tableName);

            DataTable tb2 = null;

            var tc = new TimerCount("GetCalculatedColumns");

            var calculatedColumns = db.Provider.GetCalculatedColumns(db.DatabaseName, schemaName, tableName);

            bool newMethod = false;

            string sql = db.Provider.SetTop("select * from " + db.Provider.GetObjectName(schemaName, tableName) + "", db.ProviderType == EnumDbProviderType.Access ? 1 : 0);

            if (!isCommand) //is object (table or view)
            {
                try
                {
                    //if (db.ProviderType == EnumDbProviderType.Oracle)
                    //{
                    //    newMethod = true;
                    //    sql = "select * from " + db.Provider.GetObjectName(schemaName, tableName) + " where rownum = 0";
                    //}

                    tc.Next("GetSchemaTable");
                    if (newMethod)
                    {
                        tb2 = db.GetSchemaTable(sql);
                    }
                    else
                        tb2 = db.ExecuteDataTable(sql, 0); // top 0 dá erro no access
                }
                catch (Exception ex)
                {
                    throw new Exception("Speed: Error generating data class for the table/view '" + tableName + "'", ex);
                }
            }
            else
            {
                tc.Next("GetSchemaTable2");
                if (newMethod)
                    tb2 = db.GetSchemaTable(sql);
                else
                    tb2 = db.ExecuteDataTable(sql);
            }

            DataRow[] rows = tb.Select("", "ORDINAL_POSITION");

            string tbSchemaName = null;

            if (rows.Length > 0)
            {
                tc.Next("Parse");
                Dictionary<string, DbDataType> dataTypes = db.Provider.DataTypes;
                foreach (DataRow row in rows)
                {
                    DbColumnInfo col = new DbColumnInfo(db, row);

                    if (tb2.Columns.Contains(col.ColumnName))
                    {
                        if (calculatedColumns != null && calculatedColumns.Count > 0)
                        {
                            bool isComputed = calculatedColumns.Exists(p => p.ToUpper() == col.ColumnName.ToUpper());
                            if (isComputed)
                                col.IsComputed = true;
                        }

                        // coloca algumas propridades adicionais
                        if (tb2 != null)
                        {
                            col.DataTypeDotNet = tb2.Columns[col.ColumnName].DataType.Name;
                        }
                        else
                        {
                            col.DataTypeDotNet = dataTypes[col.DataType].DataType;
                            if (col.DataTypeDotNet.StartsWith("System."))
                                col.DataTypeDotNet = col.DataTypeDotNet.Substring("System.".Length, col.DataTypeDotNet.Length - "System.".Length);
                        }
                        Columns.Add(col.ColumnName, col);

                        if (string.IsNullOrEmpty(schemaName))
                        {
                            if (Columns.Count == 1)
                            {
                                if (db.ProviderType == EnumDbProviderType.Oracle)
                                    tbSchemaName = tb.Columns.Contains("TABLE_CATALOG") ? Conv.ToString(row["TABLE_CATALOG"]) : Conv.ToString(row["TABLE_SCHEMA"]);
                                else
                                    tbSchemaName = Conv.ToString(row["TABLE_SCHEMA"]);
                                if (!string.IsNullOrEmpty(tbSchemaName))
                                {
                                    schemaName = tbSchemaName;
                                    this.SchemaName = schemaName;
                                }
                            }
                        }

                        if (db.ProviderType == EnumDbProviderType.PostgreSQL)
                        {
                            var seqs = db.Sequences;
                            if (!col.IsIdentity && (!string.IsNullOrEmpty(col.ColumnDefault) && col.ColumnDefault.ToLower().Contains("nextval(")))
                            {
                                this.SequenceColumn = col.ColumnName;
                                string key = Conv.GetKey(Conv.Unquote(schemaName), Conv.Unquote(tableName), col.ColumnName);
                                if (seqs.ContainsKey(key))
                                {
                                    this.SequenceName = seqs[key].SequenceName;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Usa DataTable
                tc.Next("DbColumnInfo");
                foreach (DataColumn col in tb2.Columns)
                {
                    DbColumnInfo item = new DbColumnInfo(db, tableName, col);
                    // coloca algumas propridades adicionais
                    item.DataTypeDotNet = col.DataType.Name;
                    Columns.Add(item.ColumnName, item);
                }
            }

            // Identity
            tc.Next("Identity");
            var idCol = (from c in Columns where c.Value.IsIdentity select c.Value.ColumnName).FirstOrDefault();
            string identity;

            if (idCol != null)
                identity = idCol;
            else
            {
                identity = db.Provider.GetIdentityColumn(db.DatabaseName, schemaName, tableName);

                if (!string.IsNullOrEmpty(identity))
                    if (Columns.ContainsKey(identity))
                        Columns[identity].IsIdentity = true;
            }

            // se o datatable retornar a pk é melhor, pq sp_keys não retorna nada para views
            tc.Next("Pk");
            if (tb2 != null && tb2.PrimaryKey != null && tb2.PrimaryKey.Length > 0)
            {
                foreach (DataColumn col in tb2.PrimaryKey)
                {
                    DbColumnInfo ci = Columns[col.ColumnName];
                    PrimaryKey.Add(ci.ColumnName, ci);
                }
            }
            else
            {
                string[] keys = db.Provider.GetPrimaryKeyColumns(db.DatabaseName, schemaName, tableName);
                if (keys != null && keys.Length > 0)
                {
                    foreach (var key in keys)
                    {
                        DbColumnInfo ci = Columns[key];
                        PrimaryKey.Add(ci.ColumnName, ci);
                    }
                }
            }

            //            // ParentRelations
            //            try
            //            {
            //#if DEBUG2
            //                try
            //                {
            //                    tc.Next("GetParentRelations");
            //                    var x = db.Provider.GetParentRelations(schemaName, tableName);

            //                }
            //                catch (Exception ex)
            //                {
            //                    ex.ToString();
            //                }
            //#endif
            //                    tc.Next("GetParentRelations");
            //                var fks = db.Provider.GetParentRelations(schemaName, tableName);
            //                if (fks != null && fks.Count > 0)
            //                {
            //                    foreach (var fk in fks)
            //                    {
            //                        if (!ParentRelations.ContainsKey(fk.ConstraintName))
            //                            ParentRelations.Add(fk.ConstraintName, fk);
            //                    }
            //                    ToString();
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                throw new Exception("Error in GetParentRelations", ex);
            //            }

            //            // ChildRelations
            //                    tc.Next("ChildRelations");
            //            try
            //            {
            //                foreach (var pair in db.ReferentialContraints)
            //                {
            //                    foreach (DbReferencialConstraintInfo constr in pair.Value)
            //                    {
            //                        if (constr.Columns.Count > 0)
            //                        {
            //                            var col = constr.Columns[0];
            //                            if (db.ProviderType == EnumDbProviderType.Access)
            //                            {
            //                                if (col.ReferencedTableSchema.EqualsICIC(schemaName) && col.ReferencedTableName.EqualsICIC(tableName))
            //                                {
            //                                    ChildrenRelations.Add(constr.ConstraintFullName, constr);
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if ((col.ReferencedTableSchema.EqualsICIC(schemaName) | schemaName == "sqlite_default_schema") && col.ReferencedTableName.EqualsICIC(tableName))
            //                                {
            //                                    ChildrenRelations.Add(constr.ConstraintFullName, constr);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                throw new Exception("Error in DbTableInfo", ex);
            //            }
            string time = tc.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", SchemaName, TableName);
        }

    }

}
