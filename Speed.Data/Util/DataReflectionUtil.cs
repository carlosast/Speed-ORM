using Speed.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Speed.Data
{

    /// <summary>
    /// Classe estática de métodos de serialização usada neste projeto
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class DataReflectionUtil
    {

        public static string GetTableName(Type type)
        {
            var ca = type.GetCustomAttributesEx<DbTableAttribute>(false);
            if (ca.Length > 0 && ca[0].TableName == null)
                ca[0].TableName = type.Name;
            else if (ca.Length == 0)
                return type.Name;
            return ca[0].TableName.removeBrackets();
        }

        public static string GetSequenceName(Type type)
        {
            DbTableAttribute[] ca = (DbTableAttribute[])type.GetCustomAttributesEx<DbTableAttribute>(false);
            if (ca.Length > 0)
                return ca[0].SequenceName != null ? ca[0].SequenceName.removeBrackets() : null;
            else
                return null;
        }

        public static string GetSequenceColumn(Type type)
        {
            DbTableAttribute[] ca = type.GetCustomAttributesEx<DbTableAttribute>(false);
            if (ca.Length > 0)
                return ca[0].SequenceColumn != null ? ca[0].SequenceColumn.removeBrackets() : null;
            else
                return null;
        }

        public static string GetSchemaName(Type type)
        {
            DbTableAttribute[] ca = type.GetCustomAttributesEx<DbTableAttribute>(false);
            //if (ca[0].SchemaName  == null)
            //    ca[0].SchemaName = type.Name;
            if (ca.Length > 0)
                return ca[0].SchemaName != null ? ca[0].SchemaName.removeBrackets() : null;
            else
                return null;
        }

        //public static string GetViewName(Type type)
        //{
        //    DbViewAttribute[] ca = (DbViewAttribute[])type.GetCustomAttributesEx(typeof(DbViewAttribute), false);
        //    if (ca[0].ViewName == null)
        //        ca[0].ViewName = type.Name;
        //    return ca[0].ViewName;
        //}

        public static string GetSqlCommand(Type type)
        {
            DbSqlCommandAttribute[] ca = type.GetCustomAttributesEx<DbSqlCommandAttribute>(false);
            return ca[0].Sql;
        }

        /// <summary>
        /// retorna um Dictionary com os nomes das propriedades como chave
        /// e os objectos ColumnAttribute como valores
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, DbColumnAttribute> GetColumns(Type type)
        {
            Dictionary<string, DbColumnAttribute> columns = new Dictionary<string, DbColumnAttribute>(StringComparer.OrdinalIgnoreCase);
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                Type t = pi.PropertyType;
                string name = t.Name;

                var cas = pi.GetCustomAttributes(typeof(DbColumnAttribute), true).ToList();
                if (cas.Count == 1)
                {
                    DbColumnAttribute col = (DbColumnAttribute)cas[0];
                    if (col.ColumnName == null)
                        col.ColumnName = pi.Name;
                    columns.Add(pi.Name, col);
                }
                // Poco
                else
                {
                    columns.Add(pi.Name, new DbColumnAttribute(pi.Name));
                }
            }
            return columns;
        }

        /// <summary>
        /// Retorna um dictionary, indexado pelos nomes das colunas da tabela, com valores
        /// dos nomes das propriedades da classe
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> GetMapColumns(Type type)
        {
            Dictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            var pis = type.GetProperties().ToList<PropertyInfo>();

            //if (type.(typeof(Record)))
            //{
            var pir = typeof(Record).GetProperties().ToDictionary(p => p.Name);
            pis.RemoveAll(p => pir.ContainsKey(p.Name));
            //}

            foreach (PropertyInfo pi in pis)
            {
                Type t = pi.PropertyType;
                string name = t.Name;

                var cas = pi.GetCustomAttributes(typeof(DbColumnAttribute), true).ToList();
                if (cas.Count == 1)
                {
                    DbColumnAttribute col = (DbColumnAttribute)cas[0];
                    if (col.ColumnName == null)
                        col.ColumnName = pi.Name;
                    columns.Add(col.ColumnName, pi);
                }
                else
                    columns.Add(pi.Name, pi);
            }
            return columns;
        }

        /// <summary>
        /// retorna uma string com os nomes das colunas separadas por ',', para usar num sql
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetColumnsList(Type type)
        {
            Dictionary<string, DbColumnAttribute> columns = GetColumns(type);
            string result = "";
            foreach (KeyValuePair<string, DbColumnAttribute> pair in columns)
            {
                result += "[" + pair.Value.ColumnName + "], ";
            }
            if (result.Length == 0)
                return null;
            else
                return result.Substring(0, result.Length - 2);
        }

        public static Dictionary<string, object> GetValues(Type type)
        {
            Dictionary<string, object> columns = new Dictionary<string, object>();

            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                var cas = pi.GetCustomAttributes(typeof(DbColumnAttribute), true).ToList();
                if (cas.Count == 1)
                {
                    columns.Add(pi.Name, pi.GetValue(null, null));
                }
            }
            return columns;
        }

        private static string removeBrackets(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (value.IndexOf('[') > -1)
                value = value.Substring(1, value.Length - 1);
            if (value.IndexOf('[') > -1)
                value = value.Substring(0, value.Length - 1);
            return value;
        }


    }
}
