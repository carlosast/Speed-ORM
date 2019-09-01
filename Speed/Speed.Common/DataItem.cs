using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Speed
{

    /// <summary>
    /// Classe útil de Texto/Valor, para várias finalidades
    /// </summary>
    [Serializable]
    [DataContract]
    public class DataItem
    {

        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public object Tag { get; set; }

        public DataItem()
        {
        }

        public DataItem(string text)
        {
            Text = text;
        }
        public DataItem(string text, object value)
        {
            Text = text;
            Value = value;
        }
        public DataItem(string text, object value, string tag)
        {
            Text = text;
            Value = value;
            Tag = tag;
        }

        public override string ToString()
        {
            return Text;
        }

    }

}
