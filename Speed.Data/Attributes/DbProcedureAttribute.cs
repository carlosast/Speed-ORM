using System;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class DbProcedureAttribute : Attribute
    {

        public string ProcedureName { get; set; }
        public ProcedureTypeEnum ProcedureType;

        public DbProcedureAttribute(string procedureName, ProcedureTypeEnum procedureType)
        {
            this.ProcedureName = procedureName;
            this.ProcedureType = procedureType;
        }

    }

    public enum ProcedureTypeEnum { ReturnValues, NoReturnValues }

}
