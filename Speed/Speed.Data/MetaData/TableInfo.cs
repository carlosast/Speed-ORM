using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Speed.Data.MetaData
{

    public class TableInfo
    {
        [DataMember]
        public string TableCatalog;
        [DataMember]
        public string TableSchema;
        [DataMember]
        public string TableName;
        [DataMember]
        public EnumTableType TableType;
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
