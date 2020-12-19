using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Speed.Data.Generation
{

    // [Serializable]
    [DataContract(Name = "Procedure")]
    public class GenProcedure
    {

        /// <summary>
        /// Nome do Schema do banco de dados que a tabela pertence
        /// </summary>
        [DataMember]
        public string SchemaName { get; set; }
        [DataMember]
        public string ProcedureName { get; set; }
        [DataMember]
        public string MethodName { get; set; }
        [DataMember]
        public string ReturnTypeName { get; set; }
        [DataMember]
        public EnumReturnType ReturnType { get; set; }
        /// <summary>
        /// Nome completo da Table
        /// </summary>
        //[DataMember]
        // // // //[XmlIgnore]
        public string FullName
        {
            get
            {
                if (SchemaName == null)
                    return ProcedureName;
                else
                    return string.Format("{0}.{1}", SchemaName, ProcedureName);
            }
        }
        /// <summary>
        /// Se a prccedure será processada
        /// </summary>
        [DataMember]
        public bool IsSelected = true;

        public GenProcedure()
        {
        }

        public GenProcedure(string schemaName, string procedureName, string methodName, string returnTypeName, EnumReturnType returnType)
        {
            this.SchemaName = schemaName;
            this.ProcedureName = procedureName;
            this.MethodName = methodName;
            this.ReturnTypeName = returnTypeName;
            this.ReturnType = returnType;
        }

        public GenProcedure(string procedureName, string methodName, string returnTypeName, EnumReturnType returnType) :
            this(null, procedureName, methodName, returnTypeName, returnType)
        {
        }

    }

    public enum EnumReturnType
    {
        None,
        Scalar,
        SingleEntity,
        ListEntity
    }

}
