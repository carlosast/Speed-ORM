using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Speed.Data
{

    [DataContract(Name = "Record", Namespace = "")]
    [Serializable]
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class Record : ICloneable
    {

        /// <summary>
        /// Valores originais do registro
        /// </summary>
        [DataMember]
        [Bindable(false)]
        public Record Original { get; set; }

        [DataMember]
        [Bindable(false)]
        public RecordStatus RecordStatus { get; set; }

        [DataMember]
        public object RecordTag { get; set; }

        public Record()
        {
            this.RecordStatus = RecordStatus.New;
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
