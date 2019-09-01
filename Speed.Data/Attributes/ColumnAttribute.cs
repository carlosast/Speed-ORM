using System;
using System.Runtime.Serialization;

namespace Speed.Data
{

    /// <summary>
    /// Classe de atributo de uma coluna do banco de dados
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbColumnAttribute : Attribute
    {

        // // [DataMember]
        public string TableCatalog { get; set; }
        // // [DataMember]
        public string TableSchema { get; set; }
        // // [DataMember]
        public string TableName { get; set; }
        // // [DataMember]
        public string ColumnName { get; set; }
        // // [DataMember]
        public int OrdinalPosition { get; set; }
        // // [DataMember]
        public string ColumnDefault { get; set; }
        // // [DataMember]
        public bool IsNullable { get; set; }
        // // [DataMember]
        public bool IsIdentity { get; set; }
        // // [DataMember]
        public string DataType { get; set; }
        // // [DataMember]
        public string DataTypeDotNet { get; set; }
        // // [DataMember]
        public int CharacterMaximumLength { get; set; }
        // // [DataMember]
        public int CharacterOctetLength { get; set; }
        // // [DataMember]
        public byte NumericPrecision { get; set; }
        // // [DataMember]
        public short NumericPrecisionRadix { get; set; }
        // // [DataMember]
        public int NumericScale { get; set; }
        // // [DataMember]
        public short DatetimePrecision { get; set; }
        // // [DataMember]
        public string CharacterSetCatalog { get; set; }
        // // [DataMember]
        public string CharacterSetSchema { get; set; }
        // // [DataMember]
        public string CharacterSetName { get; set; }
        // // [DataMember]
        public string CollationCatalog { get; set; }

        public DbColumnAttribute()
        {
            // TODO: tratar quando não passar nada
        }

        public DbColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }

    }

}
