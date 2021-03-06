﻿using System;
using System.Collections.Generic;

namespace Speed.Data
{

    public static class DbUtil
    {

        /// <summary>
        /// Retorna os enums dos types, indexado pelos valores numéricos
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<int, Enum> GetTypes<T>()
        {
            Type type = typeof(T);
            var enums = Enum.GetValues(type);
            Dictionary<int, Enum> types = new Dictionary<int, Enum>();

            foreach (var _enum in enums)
                types.Add(Convert.ToInt32(_enum), (Enum)Enum.Parse(type, _enum.ToString()));
            return types;
        }

        /// <summary>
        /// Retorna o DataType no formato .NET
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="allowNull"></param>
        /// <returns></returns>
        public static string GetDataTypeDotNet(string dataType, bool allowNull)
        {
            if (dataType == null)
                return null;

            if (dataType.StartsWith("System."))
                dataType = dataType.Substring("System.".Length, dataType.Length - "System.".Length);

            if (allowNull)
            {
                if (dataType != "String")
                    dataType += "?";
            }
            return dataType;
        }

        /*
        public static string GetSqlOperation(EnumDbSqlOperation operation)
        {
            switch (operation)
            {
                case EnumDbSqlOperation.Different:
                    return "<>";
                case EnumDbSqlOperation.Equal:
                    return "=";
                case EnumDbSqlOperation.GreaterThan:
                    return ">";
                case EnumDbSqlOperation.GreaterThanOrEqual:
                    return ">=";
                case EnumDbSqlOperation.LessThan:
                    return "<";
                case EnumDbSqlOperation.LessThanOrEqual:
                    return "<=";
                default: // case EnumDbSqlOperation.Like:
                    return "LIKE";
            }
        }
        */

        public static string ParsePoco(string text, bool isPoco)
        {
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

    }

}
