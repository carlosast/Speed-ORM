using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Speed.Data.MetaData;
using System.Xml.Serialization;

namespace Speed.Data.Generation
{

    /// <summary>
    /// Classe de configuração de Tables ou Views
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    [Serializable]
    [DataContract(Name = "Table")]
    [XmlType(TypeName = "Table")]
    public class GenTable
    {

        /// <summary>
        /// Nome do Schema do banco de dados que a tabela pertence
        /// </summary>
        [DataMember]
        [XmlElement(Order = 0)]
        public string SchemaName { get; set; }

        /// <summary>
        /// Nome da tabela
        /// </summary>
        [DataMember]
        [XmlElement(Order = 1)]
        public string TableName { get; set; }

        /// <summary>
        /// Nome da classe de dados
        /// </summary>
        [DataMember]
        [XmlElement(Order = 2)]
        public string DataClassName { get; set; }

        /// <summary>
        /// Nome da classe de negócios
        /// </summary>
        [DataMember]
        [XmlElement(Order = 3)]
        public string BusinessClassName { get; set; }

        /// <summary>
        /// Nome da coluna que possui os valores texto do Enum
        /// </summary>
        [DataMember]
        [XmlElement(Order = 4)]
        public string EnumColumnName { get; set; }

        /// <summary>
        /// Nome da coluna que possui os valores numéricos do Enum
        /// </summary>
        [DataMember]
        [XmlElement(Order = 5)]
        public string EnumColumnId { get; set; }

        /// <summary>
        /// Enum name
        /// </summary>
        [DataMember]
        [XmlElement(Order = 6)]
        public string EnumName { get; set; }

        /// <summary>
        /// Sequence Name
        /// </summary>
        [DataMember]
        [XmlElement(Order = 7)]
        public string SequenceName { get; set; }

        /// <summary>
        /// Column of sequence
        /// </summary>
        [DataMember]
        [XmlElement(Order = 8)]
        public string SequenceColumn { get; set; }

        /// <summary>
        /// Id will be processed
        /// </summary>
        [DataMember]
        [XmlElement(Order = 9)]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Nome da tabela
        /// </summary>
        [DataMember]
        [XmlElement(Order = 10)]
        public string SubDirectory { get; set; }

        [DataMember]
        [XmlArray(Order = 11, ElementName = "Columns")]
        public List<GenColumn> Columns { get; set; }

        [DataMember]
        [XmlElement(Order = 12)]
        public string DataAnnotation { get; set; }



        [XmlIgnore]
        public object Tag { get; set; }

        /// <summary>
        /// Nome completo da Table
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SchemaName))
                    return TableName;
                else
                    return string.Format("{0}.{1}", SchemaName, TableName);
            }
        }

        public GenTable()
        {
            IsSelected = true;
            Columns = new List<GenColumn>();
        }

        public GenTable(string tableName, string dataClasssName)
            : this(null, tableName, tableName, dataClasssName, null)
        {
        }

        public GenTable(string tableName, string businessClassName, string dataClasssName, string enumColumnName)
            : this(null, tableName, businessClassName, dataClasssName, enumColumnName)
        {
        }

        public GenTable(string schemaName, string tableName, string businessClassName, string dataClasssName, string enumColumnName)
            : this()
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.BusinessClassName = businessClassName;
            this.DataClassName = dataClasssName;
            this.EnumColumnName = enumColumnName;
        }

        public GenTable(string schemaName, string tableName, string businessClassName, string dataClasssName, string enumColumnName, string sequenceName = null)
            : this()
        {
            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.BusinessClassName = businessClassName;
            this.DataClassName = dataClasssName;
            this.EnumColumnName = enumColumnName;
            this.SequenceName = sequenceName;
        }


        public GenTable(TableInfo table)
            : this()
        {
            this.SchemaName = table.TableSchema;
            this.TableName = table.TableName;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8}", SchemaName, TableName, DataClassName, BusinessClassName, SequenceColumn, SequenceName, EnumColumnId, EnumColumnName, EnumName);
        }

        public void CheckEnum()
        {
            if (string.IsNullOrEmpty(EnumColumnName))
                EnumName = EnumColumnName = EnumColumnId = null;
        }

    }

}
