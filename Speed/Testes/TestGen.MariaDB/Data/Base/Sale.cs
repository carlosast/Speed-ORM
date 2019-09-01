// ****** SPEED ******
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using Speed;
using Speed.Data;


namespace TestGen.MariaDB.Data
{

    [DbTable("Speed", "sale", "", "")]
    [Serializable]
    [DataContract(Name = "Sale", Namespace = "")]
//    [System.Diagnostics.DebuggerStepThrough]
    public partial class Sale : Record, ICloneable, INotifyPropertyChanged
    {

        private Int32 z_SaleId;
        [DbColumn("Sale_Id")]
        [DataMember]
        public Int32 SaleId
        {
            get { return z_SaleId; }
            set
            {
                if (z_SaleId != value)
                {
                    z_SaleId = value;
                    this.RaisePropertyChanged("SaleId");
                }
            }
        }

        private Int32 z_SaleCustomerId;
        [DbColumn("Sale_Customer_Id")]
        [DataMember]
        public Int32 SaleCustomerId
        {
            get { return z_SaleCustomerId; }
            set
            {
                if (z_SaleCustomerId != value)
                {
                    z_SaleCustomerId = value;
                    this.RaisePropertyChanged("SaleCustomerId");
                }
            }
        }

        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IsEqual

        public bool IsEqual(Sale value)
        {
            if (value == null)
                return false;
            return
				this.SaleId == value.SaleId &&
				this.SaleCustomerId == value.SaleCustomerId;

        }

        #endregion IsEqual

        #region Clone

        public override object Clone()
        {
            return CloneT();
        }

        public Sale CloneT()
        {
            Sale value = new Sale();
            value.RecordStatus = this.RecordStatus;
            value.RecordTag = this.RecordTag;

			value.SaleId = this.SaleId;
			value.SaleCustomerId = this.SaleCustomerId;

            return value;
        }

        #endregion Clone

        #region Create

        public static Sale Create(Int32 _SaleCustomerId)
        {
            Sale __value = new Sale();

			__value.SaleCustomerId = _SaleCustomerId;

            return __value;
        }

        #endregion Create

   }

}