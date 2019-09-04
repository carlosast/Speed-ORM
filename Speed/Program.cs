using System;
using System.Windows.Forms;
using System.IO;
using Speed.Common;
using Speed.Data;

namespace Speed
{

    class Program : Speed.UI.Program
    {

        public static ConfigFile Config;
        public static string configFileName;


        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Program.ProviderType = Speed.Data.EnumDbProviderType.SqlServer;
            Title = AppName = "Speed.ORM";
#if !DEBUG
            Program.IsDebug = false;
#endif

#if DEBUG2

            Sys.ConnectionString = "Data Source=.;Initial Catalog=GFT;Integrated Security=True;MultipleActiveResultSets=True";
            using (var db = Sys.NewDb())
            {
                var pr = new Data.DbSqlServerProvider(db);
            }
#endif

            LoadConfig();
            Application.ApplicationExit += Application_ApplicationExit;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Oracle", StringComparison.OrdinalIgnoreCase))
            {
                var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (IntPtr.Size > 4)
                {
                    var dll = System.IO.Path.Combine(path, @"MySubDir\MyDLL_x64.dll");
                    return System.Reflection.Assembly.LoadFile(dll);
                }
                else
                {
                    var dll = System.IO.Path.Combine(path, @"MySubDir\MyDLL.dll");
                    return System.Reflection.Assembly.LoadFile(dll);
                }
            }
            return null;
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
