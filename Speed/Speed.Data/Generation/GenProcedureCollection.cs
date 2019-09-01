using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Speed.Data.Generation
{


#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    [Serializable]
    [CollectionDataContract(ItemName = "Procedure")]
    public class GenProcedureCollection : List<GenProcedure>
    {

        public GenProcedureCollection()
        {
        }

        public GenProcedure Add(string procedureName, string methodName, string returnTypeName, EnumReturnType returnType)
        {
            var proc = new GenProcedure(procedureName, methodName, returnTypeName, returnType);
            return proc;
        }

        public GenProcedure Add(string schemaName, string procedureName, string methodName, string returnTypeName, EnumReturnType returnType)
        {
            var proc = new GenProcedure(schemaName, procedureName, methodName, returnTypeName, returnType);
            return proc;
        }

    }

}
