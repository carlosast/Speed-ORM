using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Speed.Data.MetaData;
using System.Diagnostics;
using System.IO;

namespace Speed.Data.Generation
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class CodeGenerator
    {

        /// <summary>
        /// Usada apenas para pegar o assembly
        /// </summary>
        private static List<string> assemblies;

        static CodeGenerator()
        {
            assemblies = new List<string>();
            assemblies.Add("System.dll");
            assemblies.Add("System.Data.dll");
            assemblies.Add("System.Xml.dll");
            assemblies.Add((new TimerCount()).GetType().Assembly.Location); // "Speed.Common.dll"
            assemblies.Add((new Database()).GetType().Assembly.Location); // "Speed.Data.dll"
        }

        public static string GenerateDataClassCode(Database db, Type type, out string className, out string fullTableName, out DbTableInfo table, out Dictionary<string, DbColumnInfo> infos, Dictionary<string, string> otherUsings)
        {
            className = "DataClass" + type.Name;
            string fullTypeName = type.Namespace + "." + type.Name;

            bool isTable = ((DbTableAttribute[])type.GetCustomAttributes(typeof(DbTableAttribute), false)).Length > 0;

            // Poco?
            bool isPoco = false;
            if (!type.IsInstanceOfType(typeof(Record)))
            {
                isPoco = true;
                isTable = true;
            }

            bool isView = false;  // ((DbViewAttribute[])type.GetCustomAttributes(typeof(DbViewAttribute), false)).Length > 0;
            bool isSqlCommand = ((DbSqlCommandAttribute[])type.GetCustomAttributes(typeof(DbSqlCommandAttribute), false)).Length > 0;
            string schemaName = "";
            string tableName = "";
            string tableNameReal = "";
            string sequenceColumn = "";
            string sequenceColumnGet = "";
            string sequenceName = "";

            if (isTable)
            {
                tableNameReal = DataReflectionUtil.GetTableName(type);
                tableName = db.Provider.GetObjectName(tableNameReal);
                schemaName = db.Provider.GetObjectName(DataReflectionUtil.GetSchemaName(type));
                sequenceColumn = DataReflectionUtil.GetSequenceColumn(type);
                sequenceName = DataReflectionUtil.GetSequenceName(type);
                fullTableName = db.Provider.GetObjectName(schemaName, tableNameReal);
            }
            //else if (isView)
            //{
            //    tableNameReal = DataReflectionUtil.GetViewName(type);
            //    tableName = db.Provider.GetObjectName(tableNameReal);
            //    schemaName = db.Provider.GetObjectName(DataReflectionUtil.GetSchemaName(type));
            //    sequenceColumn = DataReflectionUtil.GetSequenceColumn(type);
            //    sequenceName = DataReflectionUtil.GetSequenceName(type);
            //    fullTableName = db.Provider.GetObjectName(schemaName, tableNameReal);
            //}
            else // if (isSqlCommand)
            {
                tableNameReal = tableName = DataReflectionUtil.GetSqlCommand(type);
                schemaName = null;
                sequenceColumn = null;
                sequenceName = null;
                fullTableName = tableName;
            }

            // gera código pra ler os valores do datareader, mas de todas
            string setColumns = "";
            // gera código pra ler os valores do datareader, mas não de todas
            string setColumnsSql = "";
            // guarda os valores antigos, pra ser usado num update
            string oldValues = "";
            // lê informações da classe
            Dictionary<string, PropertyInfo> classColumns = DataReflectionUtil.GetMapColumns(type);

            // lê informações da tabela
            try
            {
                table = new DbTableInfo(db, schemaName, tableNameReal, isSqlCommand, sequenceColumn, sequenceName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + db.Provider.GetObjectName(schemaName, tableNameReal) + "\r\n\r\n" + ex.Message + "\r\n\r\n" + ex.StackTrace);
                infos = null;
                table = null;
                return null;
            }
            //table = new DbTableInfo(db, schemaName, tableName, isSqlCommand);

            string insertParameters = "";
            string insertRequeryParametersSequence = "";
            string sequenceValue = "";
            string selectColumns = "";
            string selectFilterColumns = "";
            string insertColumns = "";
            string insertDataTableAddColumns = "";
            string insertDataTableSetColumns = "";
            string insertDataTableWithColumns = "";
            string setParameters = "";
            string setParametersWithIdentity = "";
            // PostgreSQL
            string pgReturnsTable = "";
            //string setParametersWithIdentity2 = "";
            string updateColumns = "";
            DbColumnInfo identityColumn = null;
            bool hasIdentity = false;
            int ordinal = 0;
            string pkColumns = "";

            // update sequenceColumn, sequenceName from table
            if (!string.IsNullOrEmpty(table.SequenceColumn))
                sequenceColumn = table.SequenceColumn;
            if (!string.IsNullOrEmpty(table.SequenceName))
                sequenceName = table.SequenceName;

            bool hasSequence = !string.IsNullOrWhiteSpace(sequenceColumn) && !string.IsNullOrWhiteSpace(sequenceColumn);

            // se não foi setada, checa se existe alguma automática
            if (!hasSequence && db.ProviderType == EnumDbProviderType.PostgreSQL)
            {
            }

            // Dictionary<string, string> mapColumns = DataReflectionUtil.GetColumnsList

            infos = table.Columns;

            foreach (KeyValuePair<string, DbColumnInfo> pair in table.Columns)
            {
                DbColumnInfo ci = pair.Value;
                bool isIdentity = ci.IsIdentity;
                bool isUpdated = ci.DataType.ToLower() != "timestamp" & !ci.IsComputed;

                // baseado no tipo de dados, usado o método correto do datareader
                string methodName = "";
                if (classColumns.ContainsKey(pair.Key))
                {
                    PropertyInfo pi = classColumns[pair.Key];
                    string propertyName = pi.Name;

                    // a checagem de null deve ser feita pela propriedade da classe e não pela coluna da base 
                    // de dados, pq o valor será escrito na propriedade
                    bool isNullable = pi.PropertyType.Name == "String" || pi.PropertyType.Name.Contains("Nullable");
                    string propType = pi.PropertyType.FullName; // .ReflectedType.Name;

                    // TODO: procurar outro método melhor de tirar o type
                    //System.Nullable`1[[System.Int16, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
                    bool propIsNullable = propType.Contains("System.Nullable");
                    string propTypeName = propType.Replace("System.Nullable`1", ""); //.Replace("[", "");
                    if (propTypeName.Contains(","))
                        propTypeName = propTypeName.Substring(0, propTypeName.IndexOf(","));
                    propTypeName = propTypeName.Replace("System.", "");

                    int pos1 = propType.IndexOf("[[");
                    int pos2 = propType.Substring(pos1 + 1, propType.Length - (pos1 + 1)).IndexOf(" ");
                    string propFullTypeName = isNullable && propTypeName != "String"
                        ? propTypeName + "?"
                        : propTypeName;
                    if (propTypeName.StartsWith("[["))
                        propTypeName = propTypeName.Right(propTypeName.Length - 2);
                    if (propFullTypeName.StartsWith("[["))
                        propFullTypeName = propFullTypeName.Right(propFullTypeName.Length - 2);
                    string setColumn = "";

                    methodName = "";
                    if (propTypeName == "Byte[]")
                        methodName = "GetBytes(dr, [ColumnName])";
                    else if (propTypeName == "TimeSpan")
                        methodName = "GetTimeSpan(dr, [ColumnName])";
                    else if (propTypeName == "Char[]")
                        methodName = "GetChars(dr, [ColumnName])";
                    else if (propTypeName == "Single")
                        methodName = "dr.GetFloat([ColumnName])";
                    else if (propTypeName == "UInt32")
                        methodName = "(UInt32)dr.GetInt32([ColumnName])";
                    else if (propTypeName == "UInt32")
                        methodName = "(UInt32)dr.GetInt32([ColumnName])";
                    else if (propTypeName == "SByte")
                        methodName = "(SByte)dr.GetByte([ColumnName])";
                    else if (propTypeName.Contains("SqlHierarchyId"))
                    {
                        /****
                        if (!otherUsings.ContainsKey("using Microsoft.SqlServer.Types;"))
                        {
                            var _type = typeof(Microsoft.SqlServer.Types.SqlHierarchyId);
                            otherUsings.Add("using Microsoft.SqlServer.Types;", typeof(Microsoft.SqlServer.Types.SqlHierarchyId).Assembly.Location);
                        }
                        methodName = "(SqlHierarchyId)dr[[ColumnName]]";
                        ci.IsNullable = false;
                        propIsNullable = false;
                        */
                    }
                    else
                    {
                        propTypeName = propTypeName.Replace("[", "");
                        methodName = "dr.Get" + propTypeName + "([ColumnName])";
                    }
                    //System.Data.SqlClient.DbDataReader r;
                    //r.GetFloat

                    if (ci.DataTypeDotNet == "SqlHierarchyId")
                    {
                        setColumn += string.Format(
                            "[Check]value.{0} = {1};\r\n", propertyName, methodName);
                    }
                    else if (ci.DataTypeDotNet == propTypeName)
                    {
                        if (!ci.IsNullable)
                        {
                            // este é de longe o método mais rápido de todos, pq não tem que tratar nulos e
                            // nem conversões
                            setColumn += string.Format(
                                "[Check]value.{0} = {1};\r\n", propertyName, methodName);
                        }
                        else
                        {
                            if (!propIsNullable)
                            {
                                setColumn += string.Format(
                                    "[Check]value.{0} = dr.IsDBNull([ColumnName]) ? ({3}){2} : {1};\r\n", propertyName, methodName, Conv.DefaultsTexts[propTypeName], ci.DataTypeDotNet);
                            }
                            else
                            {
                                setColumn += string.Format(
                                    "[Check]value.{0} = dr.IsDBNull([ColumnName]) ? ({2})null : {1};\r\n", propertyName, methodName, ci.GetDataTypeName());
                            }
                        }
                    }
                    else if (propIsNullable) //(ci.IsNullable)
                        setColumn += string.Format(
                            "[Check]value.{0} = Conv.IsNullOrEmpty(dr.GetValue([ColumnName])) ? ({3})null : Convert.To{2}(dr.GetValue([ColumnName]));\r\n", propertyName, pair.Key, propTypeName, propFullTypeName);
                    else
                    {
                        if (!ci.IsNullable)
                            setColumn += string.Format(
                                "[Check]value.{0} = Convert.To{1}(dr.GetValue([ColumnName]));\r\n", propertyName, propTypeName);
                        else
                            setColumn += string.Format(
                                "[Check]value.{0} = Conv.To{1}(dr.GetValue([ColumnName]));\r\n", propertyName, propTypeName);
                    }

                    // pra setColumnsSql precisa checar pra cada coluna da tabela se este existe na query passada em Query
                    setColumnsSql += setColumn
                        .Replace("[Check]", "        if (names.Contains(\"" + ci.ColumnName + "\")) ")
                        .Replace("[ColumnName]", ci.OrdinalPosition.ToString());
                    //.Replace("[ColumnName]", "dr.GetOrdinal((\"" + ci.ColumnName + "\"))");

                    // pra setColumns usar o ordinal ao invés do nome da coluna, que fica mais rápido
                    setColumns += setColumn
                        .Replace("[Check]", "        ")
                        .Replace("[ColumnName]", ordinal.ToString());
                    // .Replace("[ColumnName]", ordinal.ToString());

                    if (ci.ColumnName == "TimeStamp")
                        ci.ColumnName.ToString();

                    if (table.TableName == "SalesOrderDetail")
                        table.ToString();

                    if (!isIdentity && isUpdated)
                    {
                        updateColumns += string.Format("{0} = {1}",
                            db.Provider.GetObjectName(ci.ColumnName), db.Provider.ParameterSymbolVar + GetParameterName(ci.ColumnName));
                        if (ordinal < classColumns.Count - 1)
                            updateColumns += ", ";
                    }

                    oldValues += string.Format(
                        "                old.{0} = value.{0};\r\n", pair.Key);

                    // 
                    if (!isIdentity && isUpdated)
                    {
                        insertColumns += db.Provider.GetObjectName(ci.ColumnName) + ", ";

                        if (!hasSequence | db.ProviderType == EnumDbProviderType.Firebird)
                            insertParameters += db.Provider.ParameterSymbolVar + GetParameterName(ci.ColumnName) + ", ";
                        else
                        {
                            if (ci.ColumnName.EqualsICIC(sequenceColumn))
                                insertParameters += "[SequenceValue], ";
                            else
                                insertParameters += db.Provider.ParameterSymbolVar + GetParameterName(ci.ColumnName) + ", ";
                        }
                        insertDataTableAddColumns += string.Format("tb.Columns.Add('{0}', typeof({1}));\r\n", ci.ColumnName, ci.DataTypeDotNet);
                        insertDataTableSetColumns += string.Format("row['{0}'] = value.{1};\r\n", ci.ColumnName, propertyName);
                        insertDataTableWithColumns += string.Format("{0} {1} '{2}',\r\n", ci.ColumnName, ci.GetServerDataType(), ci.ColumnName);
                    }
                    else
                    {
                        identityColumn = ci;
                        hasIdentity = true;
                    }

                    // Sequence
                    if (hasSequence && ci.ColumnName.EqualsICIC(sequenceColumn))
                    {
                        sequenceColumnGet = string.Format("value.{0} = ({1})db.ExecuteSequenceInt64(\"{2}\");", propertyName, ci.DataTypeDotNet, sequenceName);
                        insertRequeryParametersSequence += "p___sequence, ";
                        if (db.ProviderType == EnumDbProviderType.Oracle)
                        {
                            sequenceValue = sequenceName + ".nextval";
                        }
                        else if (db.ProviderType == EnumDbProviderType.PostgreSQL)
                        {
                            var vals = sequenceName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                            sequenceName = string.Join(".", vals.Select(p => Conv.Quote(p)));
                            sequenceValue = string.Format("nextval('{0}')", sequenceName);
                        }
                        else if (db.ProviderType == EnumDbProviderType.Firebird)
                        {
                            sequenceValue = string.Format("gen_id({0},1)", sequenceName);
                        }
                    }
                    else
                    {
                        insertRequeryParametersSequence += db.Provider.ParameterSymbolVar + GetParameterName(ci.ColumnName) + ", ";
                    }

                    selectColumns += db.Provider.GetObjectName(ci.ColumnName) + ", ";

                    if (db.ProviderType == EnumDbProviderType.PostgreSQL)
                        pgReturnsTable += string.Format("{0} {1}, ", db.Provider.GetObjectName(ci.ColumnName), ci.GetServerDataType());

                    // 
                    if (!isIdentity && isUpdated)
                    {
                        if (db.ProviderType != EnumDbProviderType.Firebird && (hasSequence && ci.ColumnName.EqualsICIC(sequenceColumn)))
                        {
                        }
                        else
                        {
                            setParameters += string.Format("            db.AddWithValue(cmd, \"{0}\", \"{1}\", GetValue(value.{2}));\r\n",
                            db.Provider.ParameterSymbol + GetParameterName(ci.ColumnName), propTypeName, propertyName);
                        }
                    }
                    if (isUpdated)
                    {
                        setParametersWithIdentity += string.Format("            db.AddWithValue(cmd, \"{0}\", \"{1}\", GetValue(value.{2}));\r\n",
                            db.Provider.ParameterSymbol + GetParameterName(ci.ColumnName), propTypeName, propertyName);
                    }

                    selectFilterColumns += (ci.DataTypeDotNet.ToLower() == "string" ? Templates.SELECT_FILTER_TEMPLATE_STRING : Templates.SELECT_FILTER_TEMPLATE)
                        .Replace("[ColumnName]", ci.ColumnName)
                        .Replace("[PropertyName]", propertyName)
                        .Replace("[ParameterName]", db.Provider.ParameterSymbol + GetParameterName(ci.ColumnName)).Replace("'", "\"");

                    //System.Data.SqlClient.SqlParameter par;
                    //par = new System.Data.SqlClient.SqlParameter(GetParameterName(ci.ColumnName, true),);
                    ordinal++;
                }
            }

            if (insertColumns.Length > 0)
                insertColumns = insertColumns.Left(insertColumns.Length - 2);
            if (insertParameters.Length > 0)
                insertParameters = insertParameters.Left(insertParameters.Length - 2);
            if (insertRequeryParametersSequence.Length > 0)
                insertRequeryParametersSequence = insertRequeryParametersSequence.Left(insertRequeryParametersSequence.Length - 2);
            if (insertDataTableAddColumns.Length > 0)
                insertDataTableAddColumns = insertDataTableAddColumns.Left(insertDataTableAddColumns.Length - 2);
            if (insertDataTableSetColumns.Length > 0)
                insertDataTableSetColumns = insertDataTableSetColumns.Left(insertDataTableSetColumns.Length - 2);
            if (insertDataTableWithColumns.Length > 0)
                insertDataTableWithColumns = insertDataTableWithColumns.Left(insertDataTableWithColumns.Length - 3);
            if (selectColumns.Length > 0)
                selectColumns = selectColumns.Left(selectColumns.Length - 2);
            if (selectFilterColumns.Length > 0)
                selectFilterColumns = selectFilterColumns.Left(selectFilterColumns.Length - 2);
            if (pgReturnsTable.Length > 0)
                pgReturnsTable = pgReturnsTable.Left(pgReturnsTable.Length - 2);

            // gera códido do where do select, baseado nas informações da pk da tabela, lidas da base de dados
            string where = "";
            string whereRead = "";
            string whereReadSequence = "";
            string whereReadNoBatch = "";
            string whereParameters = "";
            string whereOldParameters = "";
            string setOldParameters = "";
            string setCurParameters = "";
            ordinal = 0;
            foreach (KeyValuePair<string, DbColumnInfo> pair in table.Columns)
            {
                DbColumnInfo ci = table.Columns[pair.Key];
                string columnName = ci.ColumnName;

                if (classColumns.ContainsKey(pair.Key))
                {
                    PropertyInfo pi = classColumns[pair.Key];
                    string propertyName = pi.Name;
                    if (table.PrimaryKey.Count == 0 || table.PrimaryKey.ContainsKey(columnName))
                    {
                        // "codProd = '" + c.CodProd + "'";
                        where += string.Format("{0} = '\" + value.{1} + \"'", db.Provider.GetObjectName(columnName), propertyName, ordinal);
                        if (ordinal < table.PrimaryKey.Count - 1)
                            where += " and ";

                        pkColumns += ci.IsIdentity ? db.Provider.GetObjectName(columnName) + "," : "";

                        whereRead += ci.IsIdentity
                            ? string.Format("{0} = {1}", db.Provider.GetObjectName(columnName), db.Provider.GetSqlIdentityInsert())
                            : string.Format("{0} = '\" + value.{1} + \"'", db.Provider.GetObjectName(columnName), propertyName, ordinal);
                        if (ordinal < table.PrimaryKey.Count - 1)
                            whereRead += " and ";

                        whereReadSequence += ci.ColumnName.EqualsICIC(sequenceColumn)
                            ? string.Format("{0} = {1}", db.Provider.GetObjectName(columnName), "p___sequence")
                            : string.Format("{0} = '\" + value.{1} + \"'", db.Provider.GetObjectName(columnName), propertyName, ordinal);
                        if (ordinal < table.PrimaryKey.Count - 1)
                            whereReadSequence += " and ";

                        whereReadNoBatch += ci.IsIdentity
                            ? string.Format("{0} = {1};", db.Provider.GetObjectName(columnName), "{0}")
                            : string.Format("{0} = '\" + value.{1} + \"'", db.Provider.GetObjectName(columnName), propertyName, ordinal);
                        if (ordinal < table.PrimaryKey.Count - 1)
                            whereReadNoBatch += " and ";

                        // Name = @Name
                        whereParameters += string.Format("{0} = {1}",
                            db.Provider.GetObjectName(columnName), db.Provider.ParameterSymbolVar + GetParameterName(columnName));
                        if (ordinal < table.PrimaryKey.Count - 1)
                            whereParameters += " and ";

                        whereOldParameters += string.Format("{0} = {1}",
                            db.Provider.GetObjectName(columnName), db.Provider.ParameterSymbolVar + GetParameterName("Old" + columnName));
                        if (ordinal < table.PrimaryKey.Count - 1)
                            whereOldParameters += " and ";

                        setOldParameters += string.Format("            db.AddWithValue(cmd, \"{0}\", GetValue(old.{1}));\r\n",
                            db.Provider.ParameterSymbol + GetParameterName("Old" + columnName), propertyName);
                        setCurParameters += string.Format("            db.AddWithValue(cmd, \"{0}\", GetValue(value.{1}));\r\n",
                            db.Provider.ParameterSymbol + GetParameterName(columnName), propertyName);
                    }
                    ordinal++;
                }
            }

            string cmdInsert;
            string cmdInsertRequery;
            string cmdUpdateRequery;
            string cmdSelectPage = "";

            if (db.ProviderType == EnumDbProviderType.Access)
            {
                cmdInsert = TemplatesCommands.INSERT;
                cmdInsertRequery = TemplatesCommands.INSERT_REQUERY_ACCESS;
                cmdUpdateRequery = TemplatesCommands.UPDATE_REQUERY_ACCESS;
            }
            else if (db.ProviderType == EnumDbProviderType.Oracle)
            {
                cmdInsert = TemplatesCommands.INSERT;
                cmdInsertRequery = TemplatesCommands.INSERT_REQUERY_ORACLE;
                cmdUpdateRequery = TemplatesCommands.UPDATE_REQUERY_ORACLE;
            }
            else if (db.ProviderType == EnumDbProviderType.PostgreSQL)
            {
                cmdInsert = TemplatesCommands.INSERT;
                cmdInsertRequery = TemplatesCommands.INSERT_REQUERY_POSTGRESQL;
                cmdUpdateRequery = TemplatesCommands.UPDATE_REQUERY_POSTGRESQL;
            }
            else if (db.ProviderType == EnumDbProviderType.Firebird)
            {
                cmdInsert = TemplatesCommands.INSERT;
                cmdInsertRequery = TemplatesCommands.INSERT_REQUERY_FIREBIRD;
                cmdUpdateRequery = TemplatesCommands.UPDATE_REQUERY_FIREBIRD;
            }
            else if (db.ProviderType == EnumDbProviderType.SqlServer)
            {
                cmdInsert = TemplatesCommands.INSERT;
                cmdInsertRequery = TemplatesCommands.INSERT_REQUERY;
                cmdUpdateRequery = TemplatesCommands.UPDATE_REQUERY;
                cmdSelectPage = TemplatesCommands.SELECT_PAGE_SQL;
            }
            else
            {
                cmdInsert = TemplatesCommands.INSERT;
                cmdInsertRequery = TemplatesCommands.INSERT_REQUERY;
                cmdUpdateRequery = TemplatesCommands.UPDATE_REQUERY;
                cmdSelectPage = TemplatesCommands.SELECT_PAGE_MY_SQL;
            }

            //if (db.ProviderType == EnumDbProviderType.Oracle)
            //{
            //    setParameters2 = setParameters +
            //        "\t\t\t" + "db.AddParameterRefCursor(cmd, \"cur\", System.Data.ParameterDirection.Output);\r\n";
            //    setParametersWithIdentity2 = setParametersWithIdentity +
            //        "\t\t\t" + "db.AddParameterRefCursor(cmd, \"cur\", System.Data.ParameterDirection.Output);\r\n";
            //}
            //else
            //{
            //    setParameters2 = setParameters;
            //    setParametersWithIdentity2 = setParametersWithIdentity;
            //}

            string sql, sqlTop, sqlCount, sqlMax;
            if (isTable || isView)
            {
                sql = "select [Columns] from [TableName]";
                sqlTop = "db.Provider.SetTop(\\\"select [Columns] from [TableName]\\\", {0})";
                sqlCount = "select count(*) from [TableName]";
                sqlMax = "select MAX({0}) from [TableName]";
            }
            else
            {
                sql = tableName;
                sqlTop = tableName;
                sqlCount = tableName;
                sqlMax = tableName;
            }

            updateColumns = updateColumns.Trim();
            if (updateColumns.EndsWith(","))
                updateColumns = updateColumns.Substring(0, updateColumns.Length - 1);

            if (pkColumns.EndsWith(","))
                pkColumns = pkColumns.Substring(0, pkColumns.Length - 1);

            cmdSelectPage = cmdSelectPage.Replace("[Columns]", selectColumns);

            string text = "";

            text += DataClassTemplate.DATACLASSTEMPLATE_CODE;

            text = text
                // aqui a ordem é importante
                .Replace("[INSERT_REQUERY]", cmdInsertRequery)
                .Replace("[UPDATE_REQUERY]", cmdUpdateRequery)
                .Replace("[INSERT]", cmdInsert)
                .Replace("[SqlPage]", cmdSelectPage);




            text = text.Replace("[InsertXml]", db.Provider.GetInsertXml());

            text = text.Replace("'", "\"");

            text = text.Replace("[DocumentElement]", "'/DocumentElement/*'");
            text = text.Replace("[InsertDataTableAddColumns]", insertDataTableAddColumns.Replace("'", "\""));
            text = text.Replace("[InsertDataTableSetColumns]", insertDataTableSetColumns.Replace("'", "\""));
            text = text.Replace("[InsertDataTableWithColumns]", insertDataTableWithColumns);

            text = text.Replace("[ClassName]", className);
            text = text.Replace("[TypeName]", fullTypeName);
            text = text.Replace("[Sql]", Q(db, sql));
            text = text.Replace("[SqlTop]", Q(db, sqlTop));
            text = text.Replace("[SqlCount]", Q(db, sqlCount));
            text = text.Replace("[SqlMax", Q(db, sqlMax));
            text = text.Replace("[TableName]", Q(db, fullTableName));
            text = text.Replace("[Columns]", Q(db, selectColumns));
            text = text.Replace("[ColumnsFilter]", Q(db, selectFilterColumns));
            text = text.Replace("[SetColumns]", Q(db, setColumns));
            text = text.Replace("[SetColumnsSql]", setColumnsSql);
            text = text.Replace("[Where]", Q(db, where));
            text = text.Replace("[WhereRead]", Q(db, whereRead));
            text = text.Replace("[WhereReadSequence]", Q(db, whereReadSequence));
            text = text.Replace("[SequenceColumn]", Q(db, sequenceColumn));
            text = text.Replace("[SequenceColumnGet]", Q(db, sequenceColumnGet));
            text = text.Replace("[SequenceName]", Q(db, sequenceName));

            text = text.Replace("[WhereReadNoBatch]", Q(db, whereReadNoBatch));
            text = text.Replace("[GetSqlIdentityInsert]", db.Provider.GetSqlIdentityInsert());
            text = text.Replace("[HasIdentity]", hasIdentity.ToString().ToLower());

            text = text.Replace("[InsertColumns]", Q(db, insertColumns));
            text = text.Replace("[InsertParameters]", Q(db, insertParameters));
            text = text.Replace("[InsertRequeryParametersSequence]", Q(db, insertRequeryParametersSequence));
            text = text.Replace("[PgReturnsTable]", Q(db, pgReturnsTable));

            text = text.Replace("[SequenceValue]", Q(db, sequenceValue));

            text = text.Replace("[SetParameters]", setParameters);
            text = text.Replace("[SetParametersWithIdentity]", setParametersWithIdentity);
            text = text.Replace("[SetOldParameters]", setOldParameters);
            text = text.Replace("[SetCurParameters]", setCurParameters);
            text = text.Replace("[WhereParameters]", Q(db, whereParameters));
            text = text.Replace("[WhereOldParameters]", Q(db, whereOldParameters));
            text = text.Replace("[Original]", Q(db, oldValues));
            text = text.Replace("[UpdateColumns]", Q(db, updateColumns));
            text = text.Replace("[UpdateColumns]", Q(db, updateColumns));
            text = text.Replace("[SortDefault]", Q(db, pkColumns));

#if DEBUG2
            // Descomentar somente se for necessário. Impacta muito a performance
            try
            {
                if (!System.IO.Directory.Exists(@"..\..\DataClass"))
                    System.IO.Directory.CreateDirectory(@"..\..\DataClass");
                using (System.IO.StreamWriter w = new System.IO.StreamWriter(@"..\..\DataClass\" + type.Name + ".cs", false))
                    w.Write(text);
            }
            catch { }
#endif
            if (isPoco)
            {
                text = text
                    .Replace("[POCO]", "")
                    .Replace("[!POCO]", "//");
            }
            else
            {
                text = text
                    .Replace("[POCO]", "//")
                    .Replace("[!POCO]", "");
            }
            return text;
        }

        static string Q(Database db, string text)
        {
            if (db.ProviderType == EnumDbProviderType.PostgreSQL)
                return text.Replace("\"", "\\\"");
            else
                return text;
        }

        public static DataClass GenerateDataClass(Database db, Type type, out DbTableInfo table, out Dictionary<string, DbColumnInfo> infos)
        {
            string className;
            string tableName;

            // other usings
            Dictionary<string, string> otherUsings = new Dictionary<string, string>();

            string code = GenerateDataClassCode(db, type, out className, out tableName, out table, out infos, otherUsings);

            StringBuilder b = new StringBuilder();
            b.AppendLine(DataClassTemplate.DATACLASSTEMPLATE_USING);
            foreach (var pair in otherUsings)
                b.AppendLine(pair.Key);
            b.AppendLine(code);
            string text = b.ToString();

            Assembly ass = Compile(db, type, text, tableName);
            Type objType = ass.GetType(className);
            DataClass dc = (DataClass)Activator.CreateInstance(objType);
            dc.Code = text;
            return (DataClass)Activator.CreateInstance(objType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfOneClass"></param>
        /// <param name="text"></param>
        /// <param name="id"></param>
        /// <param name="fileDll">Caminho completo onde será salva a dll. se não for fornecido, a dll será gravada na memória</param>
        /// <returns></returns>
        public static Assembly Compile(Database db, Type typeOfOneClass, string text, string id, string fileDll = null, Dictionary<string, string> otherUsings = null)
        {
            CompilerResults cr;
            CodeDomProvider provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;

            if (fileDll == null)
                cp.GenerateInMemory = true;
            else
            {
                cp.GenerateInMemory = false;
                cp.OutputAssembly = fileDll;
            }

#if DEBUG
            cp.IncludeDebugInformation = true;
#else
            cp.IncludeDebugInformation = false;
#endif

            // assemblies default
            foreach (var asm in assemblies)
                cp.ReferencedAssemblies.Add(asm);

            // assemblies de uma classe
            cp.ReferencedAssemblies.Add(typeOfOneClass.Assembly.Location);

            //foreach (var asm in Assembly.GetEntryAssembly().GetReferencedAssemblies())
            //    cp.ReferencedAssemblies.Add(asm.Name + ".dll");

            //cp.ReferencedAssemblies.Add("Microsoft.SqlServer.Types.dll");


            var location = db.Connection.GetType().Assembly.Location;
            if (cp.ReferencedAssemblies.ToList<string>().FirstOrDefault(p => Path.GetFileName(p).EqualsICIC(Path.GetFileName(location))) == null)
                cp.ReferencedAssemblies.Add(location);

            //if (db.ProviderType == EnumDbProviderType.MySql || db.ProviderType == EnumDbProviderType.MariaDB)
            //    cp.ReferencedAssemblies.Add("MySql.Data.dll");
            //else if (db.ProviderType == EnumDbProviderType.Firebird)
            //    cp.ReferencedAssemblies.Add("FirebirdSql.Data.FirebirdClient.dll");
            //else if (db.ProviderType == EnumDbProviderType.PostgreSQL)
            //    cp.ReferencedAssemblies.Add("Npgsql.dll");
            //else if (db.ProviderType == EnumDbProviderType.Oracle)
            //    cp.ReferencedAssemblies.Add("Oracle.ManagedDataAccess.dll");

            // otherUsings
            if (otherUsings != null)
            {
                foreach (var pair in otherUsings)
                    if (!cp.ReferencedAssemblies.Contains(pair.Key))
                        cp.ReferencedAssemblies.Add(pair.Value);
            }

            cr = provider.CompileAssemblyFromSource(cp, text);
            StringBuilder errText = new StringBuilder();
            foreach (CompilerError error in cr.Errors)
                errText.AppendLine(string.Format("Error: {0}\r\nFileName: {1}\r\nErrorNumber: {2}\r\nLine: {3}\r\nColumn: {4}\r\n\r\n", error.ErrorText, error.FileName, error.ErrorNumber, error.Line, error.Column));

            string err = errText.ToString();
            if (!string.IsNullOrEmpty(err))
            {
                throw new Exception("Errros ao gerar a classe de dados do objeto '" + id + "\r\n" + err);
            }
            else
            {
#if DEBUG
                Database.Code.Add(text);
#endif
            }
            return cr.CompiledAssembly;
        }

        /// <summary>
        /// Substitui ' ' por '_'
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetParameterName(string name)
        {
            return "P_" + name.Replace(' ', '_');
        }

    }

}