using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Speed.Data
{

    [DataContract(Name = "Record", Namespace = "")]
   // [Serializable]
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public partial class Record
    {


        /// <summary>
        /// Valores originais do registro
        /// </summary>
        // // [DataMember]
        public Record RecordOriginal { get; set; }

        // // [DataMember]
        public RecordStatus RecordStatus { get; set; }

        /// <summary>
        /// A property for free use by the programmer
        /// </summary>
        // // [DataMember]
        public object RecordTag { get; set; }

        public Record()
        {
            this.RecordStatus = Speed.Data.RecordStatus.New;
        }


        public virtual object Clone()
        {
            throw new NotImplementedException();
        }
    }

    [DataContract(Name = "RecordStatus", Namespace = "")]
    public enum RecordStatus : byte
    {
        [EnumMember]
        New = 0,
        [EnumMember]
        Existing = 1,
        [EnumMember]
        Deleted = 2
    }

}
