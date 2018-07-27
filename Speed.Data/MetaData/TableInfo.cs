using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class TableInfo
    {
        [DataMember]
        public string TableCatalog { get; set; }
        [DataMember]
        public string TableSchema { get; set; }
        [DataMember]
        public string TableName { get; set; }
        [DataMember]
        public EnumTableType TableType { get; set; }
        [DataMember]
        public string FullName
        {
            get
            {
                if (TableSchema == null)
                    return TableName;
                else
                    return !string.IsNullOrWhiteSpace(TableSchema) ?  string.Format("{0}.{1}", TableSchema, TableName) : TableName;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ", TableType, FullName);
        }
    }

    public enum EnumTableType
    {
        Table,
        View
    }

}
