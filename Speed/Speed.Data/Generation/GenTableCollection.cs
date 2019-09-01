using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Speed.Data.Generation
{


#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    [Serializable]
    [CollectionDataContract(ItemName = "Table")]
    public class GenTableCollection : List<GenTable>
    {

        public GenTableCollection()
        {
        }

        public void Add(string tableName, string dataClasssName)
        {
            this.Add(new GenTable(tableName, dataClasssName));
        }

        public void Add(string tableName, string businessClassName, string dataClasssName, string enumColumnName = null)
        {
            this.Add(new GenTable(null, tableName, businessClassName, dataClasssName, enumColumnName));
        }

        public void Add(string schemaName, string tableName, string businessClassName, string dataClasssName, string enumColumnName = null)
        {
            this.Add(new GenTable(schemaName, tableName, businessClassName, dataClasssName, enumColumnName));
        }

        public void Add(string schemaName, string tableName, string businessClassName, string dataClasssName, string enumColumnName = null, string sequenceName = null)
        {
            this.Add(new GenTable(schemaName, tableName, businessClassName, dataClasssName, enumColumnName, sequenceName));
        }

    }

}
