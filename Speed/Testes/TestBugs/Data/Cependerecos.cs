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

    [DbTable("dbo", "CepEnderecos", "", "")]
    [Serializable]
    [DataContract(Name = "CepEnderecos", Namespace = "")]
//    [System.Diagnostics.DebuggerStepThrough]
    public partial class CepEnderecos : Record, ICloneable, INotifyPropertyChanged
    {

        private String z_Cep;
        [DbColumn("Cep")]
        [DataMember]
        public String Cep
        {
            get { return z_Cep; }
            set
            {
                if (z_Cep != value)
                {
                    z_Cep = value;
                    this.RaisePropertyChanged("Cep");
                }
            }
        }

        private Int32 z_Seq;
        [DbColumn("Seq")]
        [DataMember]
        public Int32 Seq
        {
            get { return z_Seq; }
            set
            {
                if (z_Seq != value)
                {
                    z_Seq = value;
                    this.RaisePropertyChanged("Seq");
                }
            }
        }

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

        private String z_Localidade;
        [DbColumn("Localidade")]
        [DataMember]
        public String Localidade
        {
            get { return z_Localidade; }
            set
            {
                if (z_Localidade != value)
                {
                    z_Localidade = value;
                    this.RaisePropertyChanged("Localidade");
                }
            }
        }

        private String z_NomeCidade;
        [DbColumn("NomeCidade")]
        [DataMember]
        public String NomeCidade
        {
            get { return z_NomeCidade; }
            set
            {
                if (z_NomeCidade != value)
                {
                    z_NomeCidade = value;
                    this.RaisePropertyChanged("NomeCidade");
                }
            }
        }

        private String z_BaiIni;
        [DbColumn("BaiIni")]
        [DataMember]
        public String BaiIni
        {
            get { return z_BaiIni; }
            set
            {
                if (z_BaiIni != value)
                {
                    z_BaiIni = value;
                    this.RaisePropertyChanged("BaiIni");
                }
            }
        }

        private String z_BaiFim;
        [DbColumn("BaiFim")]
        [DataMember]
        public String BaiFim
        {
            get { return z_BaiFim; }
            set
            {
                if (z_BaiFim != value)
                {
                    z_BaiFim = value;
                    this.RaisePropertyChanged("BaiFim");
                }
            }
        }

        private String z_Complemento;
        [DbColumn("Complemento")]
        [DataMember]
        public String Complemento
        {
            get { return z_Complemento; }
            set
            {
                if (z_Complemento != value)
                {
                    z_Complemento = value;
                    this.RaisePropertyChanged("Complemento");
                }
            }
        }

        private String z_Logradouro;
        [DbColumn("Logradouro")]
        [DataMember]
        public String Logradouro
        {
            get { return z_Logradouro; }
            set
            {
                if (z_Logradouro != value)
                {
                    z_Logradouro = value;
                    this.RaisePropertyChanged("Logradouro");
                }
            }
        }

        private String z_SemAcento;
        [DbColumn("SemAcento")]
        [DataMember]
        public String SemAcento
        {
            get { return z_SemAcento; }
            set
            {
                if (z_SemAcento != value)
                {
                    z_SemAcento = value;
                    this.RaisePropertyChanged("SemAcento");
                }
            }
        }

        private String z_LocalidadeSemAcento;
        [DbColumn("LocalidadeSemAcento")]
        [DataMember]
        public String LocalidadeSemAcento
        {
            get { return z_LocalidadeSemAcento; }
            set
            {
                if (z_LocalidadeSemAcento != value)
                {
                    z_LocalidadeSemAcento = value;
                    this.RaisePropertyChanged("LocalidadeSemAcento");
                }
            }
        }

        private String z_BaiIniSemAcento;
        [DbColumn("BaiIniSemAcento")]
        [DataMember]
        public String BaiIniSemAcento
        {
            get { return z_BaiIniSemAcento; }
            set
            {
                if (z_BaiIniSemAcento != value)
                {
                    z_BaiIniSemAcento = value;
                    this.RaisePropertyChanged("BaiIniSemAcento");
                }
            }
        }

        private String z_BaiFimSemAcento;
        [DbColumn("BaiFimSemAcento")]
        [DataMember]
        public String BaiFimSemAcento
        {
            get { return z_BaiFimSemAcento; }
            set
            {
                if (z_BaiFimSemAcento != value)
                {
                    z_BaiFimSemAcento = value;
                    this.RaisePropertyChanged("BaiFimSemAcento");
                }
            }
        }

        private Int32? z_IdCidade;
        [DbColumn("IdCidade")]
        [DataMember]
        public Int32? IdCidade
        {
            get { return z_IdCidade; }
            set
            {
                if (z_IdCidade != value)
                {
                    z_IdCidade = value;
                    this.RaisePropertyChanged("IdCidade");
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

        public bool IsEqual(CepEnderecos value)
        {
            if (value == null)
                return false;
            return
				this.Cep == value.Cep &&
				this.Seq == value.Seq &&
				this.UF == value.UF &&
				this.Localidade == value.Localidade &&
				this.NomeCidade == value.NomeCidade &&
				this.BaiIni == value.BaiIni &&
				this.BaiFim == value.BaiFim &&
				this.Complemento == value.Complemento &&
				this.Logradouro == value.Logradouro &&
				this.SemAcento == value.SemAcento &&
				this.LocalidadeSemAcento == value.LocalidadeSemAcento &&
				this.BaiIniSemAcento == value.BaiIniSemAcento &&
				this.BaiFimSemAcento == value.BaiFimSemAcento &&
				this.IdCidade == value.IdCidade;

        }

        #endregion IsEqual

        #region Clone

        public override object Clone()
        {
            return CloneT();
        }

        public CepEnderecos CloneT()
        {
            CepEnderecos value = new CepEnderecos();
            value.RecordStatus = this.RecordStatus;
            value.RecordTag = this.RecordTag;

			value.Cep = this.Cep;
			value.Seq = this.Seq;
			value.UF = this.UF;
			value.Localidade = this.Localidade;
			value.NomeCidade = this.NomeCidade;
			value.BaiIni = this.BaiIni;
			value.BaiFim = this.BaiFim;
			value.Complemento = this.Complemento;
			value.Logradouro = this.Logradouro;
			value.SemAcento = this.SemAcento;
			value.LocalidadeSemAcento = this.LocalidadeSemAcento;
			value.BaiIniSemAcento = this.BaiIniSemAcento;
			value.BaiFimSemAcento = this.BaiFimSemAcento;
			value.IdCidade = this.IdCidade;

            return value;
        }

        #endregion Clone

        #region Create

        public static CepEnderecos Create(String _Cep, Int32 _Seq, String _UF, String _Localidade, String _NomeCidade, String _BaiIni, String _BaiFim, String _Complemento, String _Logradouro, String _SemAcento, String _LocalidadeSemAcento, String _BaiIniSemAcento, String _BaiFimSemAcento, Int32? _IdCidade)
        {
            CepEnderecos __value = new CepEnderecos();

			__value.Cep = _Cep;
			__value.Seq = _Seq;
			__value.UF = _UF;
			__value.Localidade = _Localidade;
			__value.NomeCidade = _NomeCidade;
			__value.BaiIni = _BaiIni;
			__value.BaiFim = _BaiFim;
			__value.Complemento = _Complemento;
			__value.Logradouro = _Logradouro;
			__value.SemAcento = _SemAcento;
			__value.LocalidadeSemAcento = _LocalidadeSemAcento;
			__value.BaiIniSemAcento = _BaiIniSemAcento;
			__value.BaiFimSemAcento = _BaiFimSemAcento;
			__value.IdCidade = _IdCidade;

            return __value;
        }

        #endregion Create

   }

}