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


namespace TesteRaizen.Data
{

    [DbTable("dbo", "CepEstados", "", "")]
    [Serializable]
    [DataContract(Name = "CepEstados", Namespace = "")]
//    [System.Diagnostics.DebuggerStepThrough]
    public partial class CepEstados : Record, ICloneable, INotifyPropertyChanged
    {

        private String z_UF;
        [DbColumn("UF")]
        [DataMember]
        public String UF
        {
            get { return z_UF; }
            set
            {
                if (z_UF != value)
                {
                    z_UF = value;
                    this.RaisePropertyChanged("UF");
                }
            }
        }

        private String z_NomeEstado;
        [DbColumn("NomeEstado")]
        [DataMember]
        public String NomeEstado
        {
            get { return z_NomeEstado; }
            set
            {
                if (z_NomeEstado != value)
                {
                    z_NomeEstado = value;
                    this.RaisePropertyChanged("NomeEstado");
                }
            }
        }

        private String z_FaixaCep1Ini;
        [DbColumn("FaixaCep1Ini")]
        [DataMember]
        public String FaixaCep1Ini
        {
            get { return z_FaixaCep1Ini; }
            set
            {
                if (z_FaixaCep1Ini != value)
                {
                    z_FaixaCep1Ini = value;
                    this.RaisePropertyChanged("FaixaCep1Ini");
                }
            }
        }

        private String z_FaixaCep1Fim;
        [DbColumn("FaixaCep1Fim")]
        [DataMember]
        public String FaixaCep1Fim
        {
            get { return z_FaixaCep1Fim; }
            set
            {
                if (z_FaixaCep1Fim != value)
                {
                    z_FaixaCep1Fim = value;
                    this.RaisePropertyChanged("FaixaCep1Fim");
                }
            }
        }

        private String z_FaixaCep2Ini;
        [DbColumn("FaixaCep2Ini")]
        [DataMember]
        public String FaixaCep2Ini
        {
            get { return z_FaixaCep2Ini; }
            set
            {
                if (z_FaixaCep2Ini != value)
                {
                    z_FaixaCep2Ini = value;
                    this.RaisePropertyChanged("FaixaCep2Ini");
                }
            }
        }

        private String z_FaixaCep2Fim;
        [DbColumn("FaixaCep2Fim")]
        [DataMember]
        public String FaixaCep2Fim
        {
            get { return z_FaixaCep2Fim; }
            set
            {
                if (z_FaixaCep2Fim != value)
                {
                    z_FaixaCep2Fim = value;
                    this.RaisePropertyChanged("FaixaCep2Fim");
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

        public bool IsEqual(CepEstados value)
        {
            if (value == null)
                return false;
            return
				this.UF == value.UF &&
				this.NomeEstado == value.NomeEstado &&
				this.FaixaCep1Ini == value.FaixaCep1Ini &&
				this.FaixaCep1Fim == value.FaixaCep1Fim &&
				this.FaixaCep2Ini == value.FaixaCep2Ini &&
				this.FaixaCep2Fim == value.FaixaCep2Fim;

        }

        #endregion IsEqual

        #region Clone

        public override object Clone()
        {
            return CloneT();
        }

        public CepEstados CloneT()
        {
            CepEstados value = new CepEstados();
            value.RecordStatus = this.RecordStatus;
            value.RecordTag = this.RecordTag;

			value.UF = this.UF;
			value.NomeEstado = this.NomeEstado;
			value.FaixaCep1Ini = this.FaixaCep1Ini;
			value.FaixaCep1Fim = this.FaixaCep1Fim;
			value.FaixaCep2Ini = this.FaixaCep2Ini;
			value.FaixaCep2Fim = this.FaixaCep2Fim;

            return value;
        }

        #endregion Clone

        #region Create

        public static CepEstados Create(String _UF, String _NomeEstado, String _FaixaCep1Ini, String _FaixaCep1Fim, String _FaixaCep2Ini, String _FaixaCep2Fim)
        {
            CepEstados __value = new CepEstados();

			__value.UF = _UF;
			__value.NomeEstado = _NomeEstado;
			__value.FaixaCep1Ini = _FaixaCep1Ini;
			__value.FaixaCep1Fim = _FaixaCep1Fim;
			__value.FaixaCep2Ini = _FaixaCep2Ini;
			__value.FaixaCep2Fim = _FaixaCep2Fim;

            return __value;
        }

        #endregion Create

   }

}