using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class DataTableUtil
    {

        /// <summary>
        /// Formato de data usado
        /// </summary>
        public static string DateFormat = "MM/dd/yyyy HH:mm:ss";

        public static void Save(Database cn, DataTable tb, string tableName, DataColumn[] pk, DataColumn identity)
        {
            foreach (DataRow row in tb.Rows)
            {
                string sql = null;
                if (row.RowState == DataRowState.Modified)
                    sql = Update(tb, tableName, pk, identity, row);
                else if (row.RowState == DataRowState.Added)
                    sql = Insert(tb, tableName, identity, row);
                else if (row.RowState == DataRowState.Deleted)
                    sql = Delete(tb, tableName, pk, row);
                if (sql != null)
                    cn.ExecuteNonQuery(sql);

            }
        }

        public static string Update(DataTable tb, string tableName, DataColumn[] pk, DataColumn identity, DataRow row)
        {
            string sql = "update " + ObjectName(tableName) + " set ";
            int count = 0;
            var cols = GetValidColumns(tb);
            for (int i = 0; i < cols.Count; i++)
            {
                DataColumn col = cols[i];
                if (col != identity)
                {
                    if (count > 0)
                        sql += ", ";
                    count++;
                    sql += ObjectName(col.ColumnName) + " = " + GetValue(col, row[col.ColumnName, DataRowVersion.Current]);
                }
            }
            sql += " where " + GetWhere(tb, pk, row, DataRowVersion.Current);
            return sql + ";";
        }

        public static string Insert(DataTable tb, string tableName, DataColumn identity, DataRow row)
        {
            string columns = "";
            string values = "";
            int count = 0;
            var cols = GetValidColumns(tb);
            for (int i = 0; i < cols.Count; i++)
            {
                DataColumn col = cols[i];
                if (col != identity)
                {
                    if (count > 0)
                    {
                        columns += ", ";
                        values += ", ";
                    }
                    count++;
                    columns += ObjectName(col.ColumnName);
                    values += GetValue(col, row[col.ColumnName]);
                }
            }

            string sql = string.Format("insert into {0}({1}) values({2})",
            ObjectName(tableName), columns, values);
            return sql + ";";
        }

        public static string Delete(DataTable tb, string tableName, DataColumn[] pk, DataRow row)
        {
            string sql = "delete from " + ObjectName(tableName) + " where " + GetWhere(tb, pk, row, DataRowVersion.Original);
            return sql + ";";
        }

        /// <summary>
        /// Retorna todas as colunas do DataTablew que não possuem a propriedade extendida "Fake".
        /// Fake não serão salvas
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static List<DataColumn> GetValidColumns(DataTable tb)
        {
            List<DataColumn> cols = new List<DataColumn>();
            foreach (DataColumn col in tb.Columns)
                if (!col.ExtendedProperties.ContainsKey("Fake"))
                    cols.Add(col);
            return cols;

        }

        public static string Exists(bool status, DataTable tb, string tableName, DataColumn[] pk, DataRow row)
        {
            string sql = "IF " + (status ? "" : "NOT") + " EXISTS(SELECT * from " + ObjectName(tableName) + " where " + GetWhere(tb, pk, row, DataRowVersion.Original) + ")";
            return sql;
        }

        public static string ObjectName(string name)
        {
            if (name.Contains(" ") && !name.Contains("["))
                return "[" + name + "]";
            else
                return name;
            //else
            //return name;
        }

        public static string GetValue(DataColumn col, object value)
        {
            return GetValue(col.DataType, value);
        }

        public static string GetValue(Type dataType, object value)
        {
            string type = dataType.ToString().Replace("System.", "").ToLower();
            string val = Conv.ToString(value);
            string ret = "";

            if (Conv.IsNull(value))
            {
                switch (type)
                {
                    case "string":
                    case "guid":
                    case "byte[]":
                        ret = "NULL";
                        break;
                    case "smalldatetime":
                    case "datetime":
                        ret = "NULL";
                        break;
                    case "boolean":
                        ret = "0";
                        break;
                    default:
                        ret = "0";
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case "guid":
                    case "string":
                        ret = SqlTextA(val);
                        break;
                    case "datetime":
                        ret = SqlTextA(((DateTime)value).ToString(DateFormat));
                        break;
                    case "smalldatetime":
                        ret = SqlTextA(((DateTime)value).ToString(DateFormat));
                        break;
                    case "boolean":
                        ret = Conv.ToSqlBit(value).ToString();
                        break;
                    case "int32":
                    case "int64":
                    case "byte":
                        if (val == "")
                            ret = "0";
                        else
                            ret = Convert.ToInt64(value).ToString();
                        break;
                    case "decimal":
                    case "float":
                    case "double":
                    case "single":
                        if (val == "")
                            ret = "0";
                        else
                            ret = val.Replace(',', '.');
                        break;
                    default:
                        ret = val;
                        break;
                    case "byte[]":
                        byte[] by = (byte[])value;
                        ret = "0x" + HexEncoding.ToString(by);
                        break;
                }
            }
            return ret;
        }

        private static string GetWhere(DataTable tb, DataColumn[] pk, DataRow row, DataRowVersion rowVersion)
        {
            if (pk == null)
                return null;

            string sql = "";
            for (int i = 0; i < pk.Length; i++)
            {
                DataColumn col = pk[i];
                if (i > 0 && i < tb.Columns.Count)
                    sql += " and ";
                sql += ObjectName(col.ColumnName) + " = " + GetValue(col, row[col.ColumnName, rowVersion]);
            }
            return sql;
        }

        private static string SqlText(object value)
        {
            return Conv.ToString(value).Replace("'", "''");
        }

        private static string SqlTextA(object value)
        {
            return "'" + SqlText(value) + "'";
        }

        public static DataTable ConvertDomainListToDataTable<T>(
            this IList<T> list, Func<T, T> transformCallback, Func<string, string> headerCallback)
        {
            var entityType = typeof(T);
            var table = entityType.BuildTableFromType(headerCallback);
            var properties = TypeDescriptor.GetProperties(entityType);
            foreach (var item in list)
            {
                var tableRow = table.NewRow();
                var item2 = transformCallback == null ? item : transformCallback(item);
                if (transformCallback != null) item2 = transformCallback(item);
                var i = 0;
                foreach (PropertyDescriptor property in properties) { tableRow[i++] = property.GetValue(item2); }

                table.Rows.Add(tableRow);
            }

            return table;
        }

        public static DataTable BuildTableFromType<T>(this T entityType, Func<string, string> headerCallback)
            where T : Type
        {
            var table = new DataTable(entityType.Name);
            var properties = TypeDescriptor.GetProperties(entityType);
            foreach (PropertyDescriptor property in properties)
            {
                var headerItem = (headerCallback == null) ? property.Name : headerCallback(property.Name);
                table.Columns.Add(headerItem, property.PropertyType);
            }

            return table;
        }

        /// <summary>
        /// Atualiza o Row com os valores do objeto newValues, com as propriedades com os mesmos nomes das colunas do DataTable
        /// </summary>
        /// <param name="row"></param>
        /// <param name="newValues"></param>
        public static void UpdateDataRow(DataRow row, object newValues)
        {
            var props = newValues.GetType().GetProperties().ToDictionary(p => p.Name);

            foreach (DataColumn col in row.Table.Columns)
            {
                if (props.ContainsKey(col.ColumnName))
                    row[col.ColumnName] = props[col.ColumnName].GetValue(newValues, null);
            }
        }

        /// <summary>
        /// Atualiza o Row com os valores do objeto newValues, com as propriedades com os mesmos nomes das colunas do DataTable
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="row"></param>
        /// <param name="newValues"></param>
        public static void UpdateDataRow(DataTable tb, DataRowView row, object newValues)
        {
            var props = newValues.GetType().GetProperties().ToDictionary(p => p.Name);

            foreach (DataColumn col in tb.Columns)
            {
                if (props.ContainsKey(col.ColumnName))
                    row[col.ColumnName] = props[col.ColumnName].GetValue(newValues, null) ?? DBNull.Value;
            }
        }

        /// <summary>
        /// Retorna a lista de valores para uma única coluna do DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tb"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static List<T> GetValues<T>(this DataTable tb, string columnName)
        {
            List<T> values = new List<T>();
            foreach (DataRow row in tb.Rows)
                values.Add((T)row[columnName]);
            return values;
        }

        /// <summary>
        /// Retorna a lista de valores para uma única coluna do DataTable
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static List<object> GetValues(this DataTable tb, string columnName)
        {
            return GetValues<object>(tb, columnName);
        }

        /// <summary>
        /// Retorna o resultado de das rows do DataTable formatado pra visualização
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="rowStart">Row inicial</param>
        /// <param name="rowEnd">Row final. Deixe -1 pra todas rows (tb.Rows.Count)</param>
        /// <returns></returns>
        public static string GetText(this DataTable tb, int rowStart = 0, int rowEnd = -1)
        {
            if (rowEnd == -1)
                rowEnd = tb.Rows.Count - 1;

            rowEnd = Math.Min(rowEnd, tb.Rows.Count - 1);
            int rowCount = rowEnd - rowStart;

            string[,] values = new string[rowCount + 2, tb.Columns.Count]; // 1 linha pro Header
            int[] widths = new int[tb.Columns.Count];

            // Header
            for (int c = 0; c < tb.Columns.Count; c++)
            {
                values[0, c] = tb.Columns[c].ColumnName;
                widths[c] = Math.Max(0, (values[0, c] ?? "").Length);
            }

            // Rows
            for (int r = rowStart; r <= rowEnd; r++)
            {
                for (int c = 0; c < tb.Columns.Count; c++)
                {
                    values[r - rowStart + 1, c] = GetValue(tb.Columns[c], tb.Rows[r][c]);
                    widths[c] = Math.Max(widths[c], (values[r - rowStart + 1, c] ?? "").Length);
                }
            }

            var b = new StringBuilder();

            for (int r = 0; r <= values.GetUpperBound(0); r++)
            {
                for (int c = 0; c < tb.Columns.Count; c++)
                {
                    b.Append(values[r - rowStart, c].PadLeft(widths[c]));
                    if (c < tb.Columns.Count - 1)
                        b.Append(" | ");
                }
                b.AppendLine();
            }
            return b.ToString();
        }

        public static string GenerateClass(this DataTable tb, bool check = false)
        {
            StringBuilder b = new StringBuilder();

            foreach (DataColumn col in tb.Columns)
            {
                string value = col.ColumnName;
                b.AppendLine("\t\tpublic string " + value.ToPascalCase() + " {get;set;}");
            }

            b.AppendLine();

            b.AppendLine(
@"        List<CLASS> list = new List<CLASS>();
            foreach (DataRow row in tb.Rows)
            {
                CLASS rec = new CLASS();
            ");

            foreach (DataColumn col in tb.Columns)
            {
                string value = col.ColumnName;
                if (check)
                    b.AppendLine("\t\tif (tb.Columns.Contains(\"" + value + "\"))");
                b.AppendLine((check ? "\t\t\t" : "\t\t") + "rec." + value.ToPascalCase() + " = (string)row[\"" + value + "\"];");
            }

            b.AppendLine("\t\tlist.Add(rec);");
            b.AppendLine("\t\t}");

            return b.ToString();
        }

    }

    public enum DataAction
    {
        Insert, Update, Delete
    }

    public enum CheckMode
    {
        None, IfExists, IfNotExists
    }



}
