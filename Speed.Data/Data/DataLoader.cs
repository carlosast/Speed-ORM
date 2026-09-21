namespace Speed.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Classe de carregar dados usando internamente um datatable
    /// </summary>
    public static class DataLoader
    {
        // Cache de propriedades por tipo (evita reflection repetida)
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Executa uma query e converte o resultado em <see cref="List{T}"/> de forma genérica e performática.
        /// T pode ser qualquer classe com propriedades públicas graváveis que correspondam aos nomes das colunas
        /// retornadas pelo DbDataReader. A comparação de nomes é case-insensitive.
        /// </summary>
        /// <typeparam name="T">Tipo de destino do mapeamento.</typeparam>
        /// <param name="db">Instância do banco.</param>
        /// <param name="commandText">SQL ou nome do comando.</param>
        /// <param name="parameters">Parâmetros do comando.</param>
        /// <param name="commandTimeout">Timeout em segundos. Padrão: 30.</param>
        /// <param name="commandType">Tipo do comando. Padrão: Text.</param>
        /// <param name="transaction">Transação opcional a ser associada ao comando.</param>
        /// <param name="columnMappings">
        /// Mapeamento opcional coluna → propriedade, usado quando o nome da coluna não coincide
        /// com a propriedade (nem após remover _, $ e #).
        /// Chave = nome da coluna no reader; Valor = nome da propriedade em T.
        /// Exemplo:
        /// <code>
        /// var mappings = new Dictionary&lt;string, string&gt;(StringComparer.OrdinalIgnoreCase)
        /// {
        ///     { "ID_FER", "IdFerrovia" },
        ///     { "COD_VG", "CodigoVagao" },
        ///     { "DT_REF", "DataReferencia" }
        /// };
        /// </code>
        /// </param>
        /// <returns>Lista de <typeparamref name="T"/> mapeada a partir do result set.</returns>
        public static List<T> Query<T>(
            Database db,
            string commandText,
            IEnumerable<DbParameter> parameters = null,
            int commandTimeout = 30,
            CommandType commandType = CommandType.Text,
            DbTransaction transaction = null,
            Dictionary<string, string> columnMappings = null) where T : class, new()
        {
            using (var cmd = db.NewCommand(commandText, commandTimeout, commandType))
            {
                if (parameters != null && parameters.Any())
                    cmd.Parameters.AddRange(parameters.ToArray());

                if (transaction != null)
                    cmd.Transaction = transaction;

                using (var reader = cmd.ExecuteReader())
                {
                    return ToList<T>(reader, columnMappings);
                }
            }
        }

        /// <summary>
        /// Converte um DbDataReader em List&lt;T&gt; de forma genérica e performática.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columnMappings">
        /// Mapeamento opcional coluna → propriedade, usado quando o nome da coluna não coincide
        /// com a propriedade (nem após remover _, $ e #).
        /// Chave = nome da coluna no reader; Valor = nome da propriedade em T.
        /// Exemplo:
        /// <code>
        /// var mappings = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "ID_FER", "IdFerrovia" },
        ///     { "COD_VG", "CodigoVagao" },
        ///     { "DT_REF", "DataReferencia" }
        /// };
        /// </code>
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static List<T> ToList<T>(
            DbDataReader reader,
            Dictionary<string, string> columnMappings = null) where T : class, new()
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var result = new List<T>();

            var type = typeof(T);
            var properties = _propertyCache.GetOrAdd(type, t =>
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(p => p.CanWrite)
                 .ToArray());

            var columnMap = new List<KeyValuePair<int, PropertyInfo>>();

            if (columnMappings != null && columnMappings.Count > 0)
            {
                foreach (var mapping in columnMappings)
                {
                    var prop = FindProperty(properties, mapping.Value);
                    if (prop != null)
                    {
                        int ordinal = reader.GetOrdinal(mapping.Key);
                        columnMap.Add(new KeyValuePair<int, PropertyInfo>(ordinal, prop));
                    }
                }
            }
            else
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var prop = FindProperty(properties, columnName);

                    if (prop != null)
                        columnMap.Add(new KeyValuePair<int, PropertyInfo>(i, prop));
                }
            }

            if (columnMap.Count == 0)
                throw new InvalidOperationException(
                    string.Format(
                        "Nenhuma coluna do DbDataReader corresponde a propriedades graváveis de {0}.",
                        type.Name));

            int rowIndex = 0;

            while (reader.Read())
            {
                rowIndex++;
                try
                {
                    var item = new T();

                    for (int c = 0; c < columnMap.Count; c++)
                    {
                        int ordinal = columnMap[c].Key;
                        PropertyInfo prop = columnMap[c].Value;

                        if (reader.IsDBNull(ordinal))
                            continue;

                        object value = reader.GetValue(ordinal);

                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        object convertedValue;

                        if (targetType.IsEnum)
                        {
                            convertedValue = Enum.Parse(targetType, value.ToString());
                        }
                        else if (targetType == typeof(Guid))
                        {
                            convertedValue = (value is Guid) ? value : Guid.Parse(value.ToString());
                        }
                        else if (targetType == typeof(DateTime) && value is DateTime)
                        {
                            convertedValue = value;
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(value, targetType);
                        }

                        prop.SetValue(item, convertedValue, null);
                    }

                    result.Add(item);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Erro ao mapear a linha {0} para {1}. Coluna(s) problemáticas ou tipo incompatível. Detalhes: {2}",
                            rowIndex,
                            type.Name,
                            ex.Message),
                        ex);
                }
            }

            return result;
        }

        private static PropertyInfo FindProperty(PropertyInfo[] properties, string columnName)
        {
            var prop = Array.Find(properties, p =>
                string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

            if (prop != null)
                return prop;

            if (string.IsNullOrEmpty(columnName))
                return null;

            if (columnName.IndexOf('_') >= 0 ||
                columnName.IndexOf('$') >= 0 ||
                columnName.IndexOf('#') >= 0)
            {
                var normalized = columnName
                    .Replace("_", string.Empty)
                    .Replace("$", string.Empty)
                    .Replace("#", string.Empty);

                prop = Array.Find(properties, p =>
                    string.Equals(p.Name, normalized, StringComparison.OrdinalIgnoreCase));
            }

            return prop;
        }
    }

}