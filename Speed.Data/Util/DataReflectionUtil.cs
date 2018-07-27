using System;
using System.Collections.Generic;
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
            DbTableAttribute[] ca = (DbTableAttribute[])type.GetCustomAttributes(typeof(DbTableAttribute), false);
            if (ca.Length > 0 && ca[0].TableName == null)
                ca[0].TableName = type.Name;
            else if (ca.Length == 0)
                return type.Name;
            return ca[0].TableName;
        }

        public static string GetSequenceName(Type type)
        {
            DbTableAttribute[] ca = (DbTableAttribute[])type.GetCustomAttributes(typeof(DbTableAttribute), false);
            if (ca.Length > 0)
                return ca[0].SequenceName != null ? ca[0].SequenceName : null;
            else
                return null;
        }

        public static string GetSequenceColumn(Type type)
        {
            DbTableAttribute[] ca = (DbTableAttribute[])type.GetCustomAttributes(typeof(DbTableAttribute), false);
            if (ca.Length > 0)
                return ca[0].SequenceColumn != null ? ca[0].SequenceColumn : null;
            else
                return null;
        }

        public static string GetSchemaName(Type type)
        {
            DbTableAttribute[] ca = (DbTableAttribute[])type.GetCustomAttributes(typeof(DbTableAttribute), false);
            //if (ca[0].SchemaName  == null)
            //    ca[0].SchemaName = type.Name;
            if (ca.Length > 0)
                return ca[0].SchemaName != null ? ca[0].SchemaName : null;
            else
                return null;
        }

        //public static string GetViewName(Type type)
        //{
        //    DbViewAttribute[] ca = (DbViewAttribute[])type.GetCustomAttributes(typeof(DbViewAttribute), false);
        //    if (ca[0].ViewName == null)
        //        ca[0].ViewName = type.Name;
        //    return ca[0].ViewName;
        //}

        public static string GetSqlCommand(Type type)
        {
            DbSqlCommandAttribute[] ca = (DbSqlCommandAttribute[])type.GetCustomAttributes(typeof(DbSqlCommandAttribute), false);
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
            Dictionary<string, DbColumnAttribute> columns = new Dictionary<string, DbColumnAttribute>(StringComparer.InvariantCultureIgnoreCase);
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                Type t = pi.PropertyType;
                string name = t.Name;

                object[] cas = pi.GetCustomAttributes(typeof(DbColumnAttribute), true);
                if (cas.Length == 1)
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
            Dictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                Type t = pi.PropertyType;
                string name = t.Name;

                object[] cas = pi.GetCustomAttributes(typeof(DbColumnAttribute), true);
                if (cas.Length == 1)
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
                object[] cas = pi.GetCustomAttributes(typeof(DbColumnAttribute), true);
                if (cas.Length == 1)
                {
                    columns.Add(pi.Name, pi.GetValue(null, null));
                }
            }
            return columns;
        }


    }
}
