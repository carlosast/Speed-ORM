using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Speed.Data
{

    [Serializable]
    [DataContract(Name = "Spd", Namespace = "Speed")]
    public class ConnectionInfo
    {

        [DataMember]
        [XmlElement(Order = 0)]
        public string Name { get; set; }

        [DataMember]
        [XmlElement(Order = 1)]
        public string Server { get; set; }

        [DataMember]
        [XmlElement(Order = 2)]
        public string Database { get; set; }

        [DataMember]
        [XmlElement(Order = 3)]
        public bool IntegratedSecurity{ get; set; }

        [DataMember]
        [XmlElement(Order = 4)]
        public string UserId { get; set; }

        [DataMember]
        [XmlElement(Order = 5)]
        public string Password { get; set; }

        [DataMember]
        [XmlElement(Order = 6)]
        public int Port { get; set; }

        [DataMember]
        [XmlElement(Order = 7)]
        public string ConnectionString { get; set; }

        private EnumDbProviderType provider;
        [XmlElement(Order = 8)]
        [DataMember]
        public EnumDbProviderType Provider
        {
            get { return provider; }
            set { provider = value; }
        }

        [XmlElement(Order = 9)]
        [DataMember]
        public int CommandTimeout { get; set; }

        [DataMember]
        [XmlElement(Order = 10)]
        public bool Embedded { get; set; }

        /// <summary>
        /// Name of OleDbProvider
        /// </summary>
        [DataMember]
        [XmlElement(Order = 11)]
        public string ProviderName { get; set; }

        [DataMember]
        [XmlElement(Order = 12)]
        public bool BuildConnectionString { get; set; }

        public ConnectionInfo()
        {
            CommandTimeout = 30;
            Provider = EnumDbProviderType.SqlServer;
            BuildConnectionString = true;
        }
        public ConnectionInfo(EnumDbProviderType provider, string server, string database, string userId, string password, int port = 0, int commandTimeout = 30, bool embedded = false, string providerName = null)
        {
            this.Provider = provider;
            this.Server = server;
            this.Database = database;
            this.UserId = userId;
            this.Password = password;
            this.Port = port;
            this.CommandTimeout = commandTimeout;
            this.Embedded = embedded;
            this.ProviderName = providerName;
            BuildConnectionString = true;
        }

        public string Text
        {
            get
            {
                string text = "";
                if (!string.IsNullOrWhiteSpace(this.Server))
                    text += this.Server;
                if (!string.IsNullOrWhiteSpace(this.Database))
                    text += !string.IsNullOrWhiteSpace(text) ? " - " + this.Database : this.Database;
                if (!string.IsNullOrWhiteSpace(this.UserId))
                    text += !string.IsNullOrWhiteSpace(text) ? " - " + this.UserId : this.UserId;
                return text.ToUpper();
            }
        }

    }

}
