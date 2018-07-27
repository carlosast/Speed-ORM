using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        public new GenTable Add(GenTable tb)
        {
            base.Add(tb);
            return tb;
        }

        public GenTable Add(string tableName, string dataClasssName)
        {
            return this.Add(new GenTable(tableName, dataClasssName));
        }

        public GenTable Add(string tableName, string businessClassName, string dataClasssName, string enumColumnName = null)
        {
            return this.Add(new GenTable(null, tableName, businessClassName, dataClasssName, enumColumnName));
        }

        public GenTable Add(string schemaName, string tableName, string businessClassName, string dataClasssName, string enumColumnName = null)
        {
            return this.Add(new GenTable(schemaName, tableName, businessClassName, dataClasssName, enumColumnName));
        }

        public GenTable Add(string schemaName, string tableName, string businessClassName, string dataClasssName, string enumColumnName = null, string sequenceName = null)
        {
            return this.Add(new GenTable(schemaName, tableName, businessClassName, dataClasssName, enumColumnName, sequenceName));
        }

    }

}
