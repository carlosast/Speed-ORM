using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Speed.Windows;
using Speed.Data;
using System.Data;
using System.Text;
using Speed.UI;
using System.IO;

namespace Speed
{

    class Program : Speed.UI.Program
    {

        public static ConfigFile Config;
        public static string configFileName;


        [STAThread]
        static void Main()
        {
            Program.ProviderType = Speed.Data.EnumDbProviderType.SqlServer;
            Title = AppName = "Speed.ORM";

#if !DEBUG
            Program.IsDebug = false;
#endif

            LoadConfig();
            Application.ApplicationExit += Application_ApplicationExit;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            SaveConfig();
        }

        #region Config

        static void LoadConfig()
        {
            configFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Speed.xml");
            if (File.Exists(configFileName))
            {
                var s = new System.Xml.Serialization.XmlSerializer(typeof(ConfigFile));
                var ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                ns.Add("", "");
                using (System.IO.StreamReader reader = new StreamReader(configFileName))
                    Config = (ConfigFile)s.Deserialize(reader);
            }
            else
                Config = new ConfigFile();
        }

        static void SaveConfig()
        {
            if (File.Exists(configFileName))
                File.SetAttributes(configFileName, FileAttributes.Normal);

            var s = new System.Xml.Serialization.XmlSerializer(typeof(ConfigFile));
            var ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(configFileName))
            {
                s.Serialize(writer, Config, ns);
                writer.Close();
            }
        }

        #endregion Config

    }

}
