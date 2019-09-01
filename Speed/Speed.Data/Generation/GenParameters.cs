using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Speed.Data.Generation
{

    /// <summary>
    /// Parâmetros de geração de código
    /// </summary>
    [Serializable]
    [DataContract( Name="Spd", Namespace = "Speed")]
    public class GenParameters
    {

        /// <summary>
        /// Namespace das classes de dados
        /// </summary>
        [DataMember]
        [XmlElement(Order = 0)]
        public GenClassParameters DataClass { get; set; }

        /// <summary>
        /// Namespace das classes de dados
        /// </summary>
        [DataMember]
        [XmlElement(Order = 1)]
        public GenClassParameters BusinessClass { get; set; }

        [DataMember]
        [XmlElement(Order = 2)]
        public bool ArrangeDirectoriesBySchema { get; set; }



        /// <summary>
        /// Tables e Views do banco de dados que serão geradas classes
        /// </summary>
        [DataMember]
        [XmlArray(Order=3, ElementName="Tables")]
        public GenTableCollection Tables { get; set; }

        /// <summary>
        /// Tables e Views do banco de dados que serão geradas classes
        /// </summary>
        [DataMember]
        [XmlArray(Order = 4, ElementName = "Views")]
        public GenTableCollection Views { get; set; }

        /// <summary>
        /// Procedures do banco de dados que serão geradas classes
        /// </summary>
        [DataMember]
        [XmlArray(Order = 5, ElementName="Procedures")]
        public GenProcedureCollection Procedures { get; set; }
        
        public GenParameters()
        {
            Tables = new GenTableCollection();
            Views = new GenTableCollection();
            Procedures = new GenProcedureCollection();
            DataClass = new GenClassParameters();
            BusinessClass = new GenClassParameters();
        }

        public void Save(string fileName)
        {
            SerializationUtil.WriteToXml(fileName, this);
        }

        public void TrimExcess()
        {
            this.Tables.TrimExcess();
            this.Views.TrimExcess();
            this.Procedures.TrimExcess();
        }

        internal string GetDirectory(string schemaName, string classDirectory)
        {
            if (this.ArrangeDirectoriesBySchema && !string.IsNullOrEmpty(schemaName))
                return schemaName + "\\" + classDirectory;
            else
                return classDirectory;
        }

    }


    [Serializable]
    [DataContract(Name = "Spd", Namespace = "Speed")]
    public class GenClassParameters
    {

        /// <summary>
        /// Namespace das classes de dados
        /// </summary>
        [DataMember]
        [XmlElement(Order = 0)]
        public string NameSpace { get; set; }

        /// <summary>
        /// Diretório onde serão geradas as classes de dados
        /// </summary>
        [DataMember]
        [XmlElement(Order = 1)]
        public string Directory { get; set; }

        /// <summary>
        /// Diretório onde serão geradas as classes de extensão de dados
        /// </summary>
        [DataMember]
        [XmlElement(Order = 2)]
        public string DirectoryExt { get; set; }

        /// <summary>
        /// Se inclui o Schema da base de dados, no início dos nomes das classes de dados e negócios
        /// </summary>
        [DataMember]
        [XmlElement(Order = 3)]
        public bool StartWithSchema { get; set; }

        /// <summary>
        /// Modo de conversão dos nomes dos objetos da base de dados ao gerar as classes
        /// </summary>
        [DataMember]
        [XmlElement(Order = 4)]
        public EnumNameCase NameCase { get; set; }

        /// <summary>
        /// Modo de conversão dos nomes dos objetos da base de dados ao gerar as classes
        /// </summary>
        [DataMember]
        [XmlElement(Order = 5)]
        public string Prefix { get; set; }

        /// <summary>
        /// Modo de conversão dos nomes dos objetos da base de dados ao gerar as classes
        /// </summary>
        [DataMember]
        [XmlElement(Order = 6)]
        public string Remove { get; set; }

    }
    
    /// <summary>
    /// Modo de case do texto.
    /// </summary>
    public enum EnumNameCase
    {

        /// <summary>
        /// Não aplica case. Usa os nomes que existem no banco de dados. EX: NOME_EMPRESA fica NOME_EMPRESA
        /// </summary>
        None,
        /// <summary>
        /// Pascal Case. Convere a primeira letra para maiúscula. EX: NOME_EMPRESA fica NomeEmpresa
        /// </summary>
        Pascal,
        /// <summary>
        /// Camel Case. Convere a primeira letra para minúscula. EX: NOME_EMPRESA fica nomeepresa
        /// </summary>
        Camel,
        /// <summary>
        /// Converte para maiúsculas
        /// </summary>
        Upper,
        /// <summary>
        /// Converte para minúsculas
        /// </summary>
        Lower

    }

}
