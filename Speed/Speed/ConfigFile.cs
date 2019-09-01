using Speed.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Speed
{

    public class ConfigFile
    {

        [DataMember]
        [XmlElement(Order = 1)]
        public List<string> RecentFiles { get; set; }

        [DataMember]
        [XmlElement(Order = 2)]
        public List<ConnectionInfo> Connections { get; set; }

        public ConfigFile()
        {
            RecentFiles = new List<string>();
            Connections = new List<ConnectionInfo>();
        }

        public void SetRecentFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            this.RecentFiles.RemoveAll(p => string.IsNullOrWhiteSpace(p));
            int index = this.RecentFiles.FindIndex(p => p.EqualsICIC(fileName));
            if (index > -1)
                this.RecentFiles.RemoveAt(index);
            this.RecentFiles.Insert(0, fileName);

            int max = 25;

            while (this.RecentFiles.Count > max)
                this.RecentFiles.RemoveAt(this.RecentFiles.Count - 1);
        }

    }

}
