using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Speed.Data;
using Speed.Data.Generation;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;

namespace Speed.UI
{

    [Serializable]
    [DataContract]
    public class SpeedFile
    {

        [DataMember]
        public ConnectionInfo Connection { get; set; }
        [DataMember]
        public bool UseSchemaName { get; set; }
        
        [DataMember]
        public GenParameters Parameters { get; set; }
        
        public SpeedFile()
        {
            this.Connection = new ConnectionInfo();
            this.Parameters = new GenParameters();
            this.UseSchemaName = true;
        }

        public static SpeedFile Load(string filename)
        {
            SpeedFile file = new SpeedFile();
            var s = new System.Xml.Serialization.XmlSerializer(typeof(SpeedFile));
            var ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
            using (System.IO.StreamReader reader = new StreamReader(filename))
            {
               file =  (SpeedFile)s.Deserialize(reader);
            }
            return file;
        }

        public void Save(string filename)
        {
            var s = new System.Xml.Serialization.XmlSerializer(this.GetType());
            var ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(filename))
            {
                s.Serialize(writer, this, ns);
                writer.Close();
            }
        }

        public void TrimExcess()
        {
            this.Parameters.TrimExcess();
        }

        public static void Teste()
        {
            SpeedFile file = new SpeedFile();
            file.UseSchemaName = false;
            file.Connection = new ConnectionInfo { Provider = EnumDbProviderType.SqlServer, Server = ".", Database = "testes", UserId = "sa", Password = "****", Port = 0 };
            file.Parameters = new GenParameters();
            file.Parameters.Tables.Add("TableTeste", "ClassTableTeste");
            file.Parameters.Procedures = new  GenProcedureCollection   ();
            //file.Parameters.Procedures.Add { ProcedureName = "ProcedureTeste" };
            file.TrimExcess();
            file.Save("./SpeedFile.xml");
        }
    }


}
