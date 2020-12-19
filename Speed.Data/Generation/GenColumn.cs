using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Speed.Data.Generation
{

    [Serializable]
    [DataContract]
    public class GenColumn
    {

        [DataMember]
        [XmlElement(Order = 0)]
        public string ColumnName { get; set; }
        //Speed.Data.MetaData.DbColumnInfo y;

        [DataMember]
        [XmlElement(Order = 1)]
        public string PropertyName { get; set; }

        [DataMember]
        [XmlElement(Order = 2)]
        public string Title { get; set; }

        [DataMember]
        [XmlElement(Order = 3)]
        public string Description { get; set; }

        [DataMember]
        [XmlElement(Order = 4)]
        public string DataType { get; set; }

        [DataMember]
        [XmlElement(Order = 5)]
        public bool IsRequired { get; set; }

        [DataMember]
        [XmlElement(Order = 6)]
        public string Attributes { get; set; }


        public override string ToString()
        {
            return string.Format("{0} - {1} - {2} - {3} - {4} - {5}", ColumnName, PropertyName, DataType, IsRequired, Title, Description);
        }

    }

}
