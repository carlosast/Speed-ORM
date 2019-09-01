//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Web;

//namespace Speed.Data
//{
    
//    /// <summary>
//    /// Classe útil para guardar informações da base de dados e criar classes Databases
//    /// TODO: Ainda está usando usuário e senha fixos. Parametrizar
//    /// </summary>
//#if !DEBUG
//    [System.Diagnostics.DebuggerStepThrough]
//#endif
//    public class DataConfig
//    {

//        internal readonly string DbServerName;
//        internal readonly string DbDatabaseName;
//        internal readonly string ConnectionString;


//        // Lê as configurações de web.Config, das propriedades DbServerName e DbDatabaseName
//        public DataConfig()
//            : this(System.Configuration.ConfigurationSettings.AppSettings["DbServerName"],
//            System.Configuration.ConfigurationSettings.AppSettings["DbDatabaseName"])
//        {
//        }

//        public DataConfig(string dbServerName, string dbDatabaseName)
//        {
//            // Lê as configurações de web.Config
//            DbServerName = dbServerName;
//            DbDatabaseName = dbDatabaseName;
//            ConnectionString = string.Format(
//                "Data Source={0};Initial Catalog={1};User ID=ccmsys;Password=ccmsysadmin;Persist Security Info=False;",
//                DbServerName, DbDatabaseName);
//        }

//        /// <summary>
//        /// Cria uma instância de Database, já com a string de conexão correta e
//        /// abre a base de dados
//        /// </summary>
//        /// <returns></returns>
//        public Database NewDatabase()
//        {
//            Database db = new Database(ConnectionString, 300);
//            db.Open();
//            return db;
//        }

//        /// <summary>
//        /// Cria uma instância de Database, já com a string de conexão correta e
//        /// abre a base de dados
//        /// </summary>
//        /// <param name="commandTimeout"></param>
//        /// <returns></returns>
//        public Database NewDatabase(int commandTimeout)
//        {
//            Database db = new Database(
//                DbServerName, DbDatabaseName,
//                "ccmsys", "ccmsysadmin", commandTimeout);
//            db.Open();
//            return db;
//        }


//    }

//}
