#define USELINQ2

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Common;
//using System.Data.SqlServerCe;
using System.IO;
using Speed;
using Speed.Data.MetaData;
using Speed.Data.Generation;
using System.Runtime.InteropServices;
using Speed.Common;

#if USELINQ
using IQ;
using IQ.Data;
using System.IO;
#endif

namespace Speed.Data
{

    /// <summary>
    /// Classe de conexão e métodos úteis de acesso à base de dados
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class Database : Object, IDisposable
    {

        // [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        // private static extern bool SetDllDirectory(string path);
#if DEBUG
        public static List<string> Code = new List<string>();
#endif

        #region Declarations

        public static bool DllCache = false;
        public static int DllCacheMinutes = 60;

        internal static List<string> Errors = new List<string>();

        /// <summary>
        /// Get compilation errors
        /// </summary>
        /// <returns></returns>
        public static List<string> GetErrors()
        {
            return Errors;
        }

        IDbProvider provider;
        EnumDbProviderType providerType;
        private bool usingTransaction; // indica que uma transação está pendente
        private DbConnection cn;  // variável de conexão
        private DbTransaction tr; // variável de transação
        private string databaseName;
        /// <summary>
        /// Cache estático (usado por todas as instâncias)
        /// </summary>
        private static DataClassCache cache = new DataClassCache();
        // private event EventHandler<DbSqlInfoMessage> infoMessage;

        private int commandTimeout;
        private string connectionString;
        private DbConnectionStringBuilder connectionStringBuilder;
        internal static Dictionary<string, MethodInfo> refMethods = new Dictionary<string, MethodInfo>();

        #endregion Declarations

        #region Properties

        public IDbProvider Provider
        {
            get { return provider; }
        }

        public EnumDbProviderType ProviderType
        {
            get { return providerType; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Propriedade que indica que uma transação está em uso
        /// </summary>
        public bool UsingTransaction
        {
            get { return usingTransaction; }
        }

        /*
        public EventHandler<DbSqlInfoMessage> InfoMessage
        {
            get { return infoMessage; }
            set
            {
                infoMessage += value;
                switch (providerType)
                {
                    case EnumDbProviderType.SqlServer:
                        ((SqlConnection)cn).InfoMessage += (o, ev) =>
                        {
                            var evret = new DbSqlInfoMessage(ev);
                            if (infoMessage != null)
                                infoMessage(this, evret);
                        };
                        break;
                    //case EnumDbProviderType.SqlServerCe:
                    //    ((SqlCeConnection)cn).InfoMessage += (o, ev) =>
                    //    {
                    //        //var evret = new DbSqlInfoMessage(ev);
                    //        //if (infoMessage != null)
                    //        //    infoMessage(this, evret);
                    //    };
                    //    break;
                    case EnumDbProviderType.MySql:
                        ((MySqlConnection)cn).InfoMessage += (o, ev) =>
                        {
                            var evret = new DbSqlInfoMessage(ev);
                            if (infoMessage != null)
                                infoMessage(this, evret);
                        };
                        break;
                    case EnumDbProviderType.MariaDB:
                        ((MySqlConnection)cn).InfoMessage += (o, ev) =>
                        {
                            var evret = new DbSqlInfoMessage(ev);
                            if (infoMessage != null)
                                infoMessage(this, evret);
                        };
                        break;
                    case EnumDbProviderType.OleDb:
                        ((OleDbConnection)cn).InfoMessage += (o, ev) =>
                        {
                            var evret = new DbSqlInfoMessage(ev);
                            if (infoMessage != null)
                                infoMessage(this, evret);
                        };
                        break;
                    case EnumDbProviderType.PostgreSQL:
                        ((NpgsqlConnection)cn).Notice += (o, ev) =>
                        {
                            var evret = new DbSqlInfoMessage(ev);
                            if (infoMessage != null)
                                infoMessage(this, evret);
                        };
                        break;
                }

            }
        }
        */

        #endregion Properties

        #region Constructors

        static Database()
        {
            //string path = Sys.AppDirectory;
            //path = Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
            //bool ok = SetDllDirectory(path);
            //if (!ok) throw new System.ComponentModel.Win32Exception();

            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        //static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    string[] tokens = args.Name.Split(",".ToCharArray());
        //    System.Diagnostics.Debug.WriteLine("Resolving : " + args.Name);
        //    var asm = Assembly.Load(args.Name);
        //    //return Assembly.LoadFile(Path.Combine(new string[] { assemblyDirectory, tokens[0] + ".dll" }));
        //    return asm;
        //}

        internal Database()
        {
        }

        //public Database(string server, string database)
        //    : this(EnumDbProviderType.SqlServer, server, database)
        //{
        //}

        public Database(EnumDbProviderType providerType, string server, string database)
            : this()
        {
            this.providerType = providerType;
            provider = ProviderFactory.CreateProvider(this, providerType);
            this.databaseName = database;
            this.connectionStringBuilder = provider.CreateConnectionStringBuilder(server, database);
            this.connectionString = connectionStringBuilder.ToString();
        }

        //public Database(string server, string database, string userId, string password)
        //    : this(EnumDbProviderType.SqlServer, server, database, userId, password)
        //{
        //}

        public Database(EnumDbProviderType providerType, string server, string database, string userId, string password)
            : this(providerType, server, database, userId, password, 30)
        {
        }

        public Database(ConnectionInfo cn)
            : this(cn.Provider, cn.Server, cn.Database, cn.UserId, cn.Password, cn.CommandTimeout, cn.IntegratedSecurity, cn.Port, cn.Embedded)
        {
        }

        //public Database(string server, string database, string userId, string password, int commandTimeout, bool integratedSecurity = false, bool embedded = false)
        //    : this(EnumDbProviderType.SqlServer, server, database, userId, password, commandTimeout, integratedSecurity, embedded)
        //{
        //}

        public Database(EnumDbProviderType providerType, string server, string database, string userId, string password, int commandTimeout, bool integratedSecurity = false, int port = 0, bool embedded = false)
            : this()
        {
            this.databaseName = database;
            provider = ProviderFactory.CreateProvider(this, providerType);
            this.connectionStringBuilder = provider.CreateConnectionStringBuilder(server, database, userId, password, integratedSecurity, port, embedded);
            this.connectionString = connectionStringBuilder.ToString();
            this.commandTimeout = commandTimeout;
            // InitializeLinq();
        }

        public Database(string connectionString)
            : this(EnumDbProviderType.SqlServer, connectionString)
        {
        }

        public Database(EnumDbProviderType providerType, string connectionString)
            : this(providerType, connectionString, 30)
        {
        }

        public Database(string connectionString, int commandTimeout)
            : this(EnumDbProviderType.SqlServer, connectionString, commandTimeout)
        {
        }

        public Database(EnumDbProviderType providerType, string connectionString, int commandTimeout)
            : this()
        {
            this.providerType = providerType;
            provider = ProviderFactory.CreateProvider(this, providerType);
            this.connectionString = connectionString;

            try
            {
                this.connectionStringBuilder = provider.CreateConnectionStringBuilder(connectionString);
                if (connectionStringBuilder != null && connectionStringBuilder.ContainsKey("Database"))
                    this.DatabaseName = (string)connectionStringBuilder["Database"];
            }
            catch { }

            this.commandTimeout = commandTimeout;
#if USELINQ
            InitializeLinq();
#endif
        }

        public void Dispose()
        {
            Close();
            // TerminateLinq();
        }

        #endregion Constructors

        #region Properties

        public DbConnection Connection
        {
            get { return cn; }
        }

        #endregion Properties

        #region Methods

        public void Open()
        {
            cn = NewConnection();
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados.
        /// Caso esteja em transação, dá rollback automático
        /// </summary>
        public void Close()
        {
            if (tr != null)
            {
                // se o desenvolvedor não deu commit, dá rollback automático
                if (usingTransaction)
                    tr.Rollback();
                tr.Dispose();
                tr = null;
            }
            if (cn != null)
            {
                cn.Close();
                cn.Dispose();
                cn = null;
            }
        }

        public DbCommand NewCommand(string commandText, CommandType commandType = CommandType.Text)
        {
            return NewCommand(commandText, this.commandTimeout, commandType);
        }

        public DbCommand NewCommand(string commandText, int commandTimeout, CommandType commandType = CommandType.Text)
        {
            DbCommand cmd = provider.NewCommand(commandText);
            cmd.Connection = cn;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = commandTimeout;
            cmd.CommandType = commandType;
            if (tr != null)
                cmd.Transaction = tr;
            //else if (Transaction.Current != null)
            //    cmd.Transaction = tr;

            return cmd;
        }

        /// <summary>
        /// inicia uma transação
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// inicia uma transação
        /// </summary>
        /// <param name="iso">Isolation Level</param>
        public void BeginTransaction(System.Data.IsolationLevel iso)
        {
            usingTransaction = true;
            // se não carregou ainda, carrega os data types
            var d = Provider.DataTypes;
            tr = cn.BeginTransaction(iso);
        }

        public void Commit()
        {
            usingTransaction = false;
            tr.Commit();
            tr.Dispose();
            tr = null;
        }

        public void Rollback()
        {
            usingTransaction = false;
            if (tr != null)
            {
                tr.Rollback();
                tr.Dispose();
                tr = null;
            }
        }

        #endregion Methods

        #region Table Methods

        #region Count

        public long Count<T>()
        {
            DataClass dc = getCache<T>();
            return dc.Count(this);
        }

        public long Count<T>(string where)
        {
            DataClass dc = getCache<T>();
            return dc.Count(this, where);
        }

        public long Count<T>(T filter, EnumDbFilter mode = EnumDbFilter.AndEqual, int commandTimeout = 30)
        {
            DataClass dc = getCache<T>();
            return dc.Count(this, filter, mode, commandTimeout);
        }

        #endregion Count

        #region Max

        public long Max<T>(string columnName)
        {
            DataClass dc = getCache<T>();
            return dc.Max(this, columnName);
        }

        public long Max<T>(string columnName, string where)
        {
            DataClass dc = getCache<T>();
            return dc.Max(this, columnName, where);
        }

        #endregion Max

        #region Select

        /// <summary>
        /// Retorna todos os registros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Select<T>()
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(this);
        }

        /// <summary>
        /// Return all records
        /// de dados, use concurrency = true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrency">Concurrency</param>
        /// <returns></returns>
        public List<T> Select<T>(bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(this, concurrency);
        }

        public List<T> Select<T>(string where)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(this, where);
        }

        public List<T> Select<T>(string where, params object[] args)
        {
            return Select<T>(string.Format(where, args), false);
        }

        /// <summary>
        /// Select Format. Usa string.Format internamente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> SelectF<T>(string where, params object[] args)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(this, string.Format(where, args));
        }

        public List<T> Select<T>(string where, bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(this, where, concurrency);
        }

        public List<T> Select<T>(string where, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(this, where, parameters);
        }
        public List<T> Select<T>(Database db, object filter, EnumDbFilter mode = EnumDbFilter.AndEqual, bool concurrency = false, int commandTimeout = 30)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(db, filter, mode, concurrency, commandTimeout);
        }

        public List<T> Select<T>(string where, bool concurrency, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Select(this, where, concurrency, parameters);
        }

        #endregion Select

        #region SelectPage

        /// <summary>
        /// Retorna todos os registros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> SelectPage<T>(int start, int pageSize = 20, string sort = null, int commandTimeout = 30)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, start, pageSize, sort, commandTimeout);
        }

        /// <summary>
        /// Return all records
        /// de dados, use concurrency = true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrency">Concurrency</param>
        /// <returns></returns>
        public List<T> SelectPage<T>(int start, int pageSize = 20, string sort = null, int commandTimeout = 30, bool concurrency = false)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, start, pageSize, sort, commandTimeout, concurrency);
        }

        public List<T> SelectPage<T>(string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, where, start, pageSize, sort, commandTimeout);
        }

        public List<T> SelectPage<T>(string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, params object[] args)
        {
            return SelectPage<T>(string.Format(where, args), start, pageSize, sort, commandTimeout, false);
        }

        /// <summary>
        /// SelectPage Format. Usa string.Format internamente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> SelectPageF<T>(string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, params object[] args)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, string.Format(where, args), start, pageSize, sort, commandTimeout);
        }

        public List<T> SelectPage<T>(string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, bool concurrency = false)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, where, start, pageSize, sort, commandTimeout, concurrency);
        }

        public List<T> SelectPage<T>(string where, int start, int pageSize, string sort = null, int commandTimeout = 30, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, where, start, pageSize, sort, commandTimeout, parameters);
        }

        public List<T> SelectPage<T>(string where, int start, int pageSize = 20, string sort = null, int commandTimeout = 30, bool concurrency = false, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, where, start, pageSize, sort, commandTimeout, concurrency, parameters);
        }

        public List<T> SelectPage<T>(T filter, int start, int pageSize = 20, string sort = null, EnumDbFilter mode = EnumDbFilter.AndEqual, int commandTimeout = 30, bool concurrency = false, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectPage(this, filter, start, pageSize, sort, mode, commandTimeout, concurrency, parameters);
        }

        #endregion SelectPage

        #region SelectTop

        /// <summary>
        /// Retorna todos os registros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> SelectTop<T>(int top)
        {
            return SelectTop<T>(top, false);
        }

        /// <summary>
        /// Retorna todos os registros. Se desejar que estes possam ser usados para atualização no banco
        /// de dados, use concurrency = true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrency"></param>
        /// <returns></returns>
        public List<T> SelectTop<T>(int top, bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectTop(this, top, concurrency);
        }

        public List<T> SelectTop<T>(int top, string where)
        {
            return SelectTop<T>(top, where, false);
        }

        public List<T> SelectTop<T>(int top, string where, bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectTop(this, top, where, concurrency);
        }

        #endregion SelectTop

        #region SelectColumns

        /// <summary>
        /// Retorna todos os registros. Se desejar que estes possam ser usados para atualização no banco
        /// de dados, use concurrency = true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrency"></param>
        /// <returns></returns>
        public List<T> SelectColumns<T>(bool concurrency, params string[] columns)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectColumns(this, concurrency, columns);
        }

        public List<T> SelectColumns<T>(string where, bool concurrency, params string[] columns)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.SelectColumns(this, where, concurrency, columns);
        }

        #endregion SelectColumns

        #region SelectSingle

        /// <summary>
        /// Retorna todos os registros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SelectSingle<T>()
        {
            return SelectSingle<T>(false);
        }

        /// <summary>
        /// Retorn 1 record. Se desejar que estes possam ser usados para atualização no banco
        /// de dados, use concurrency = true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrency"></param>
        /// <returns></returns>
        public T SelectSingle<T>(bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (T)dc.SelectSingle(this, concurrency);
        }

        public T SelectSingle<T>(string where)
        {
            return SelectSingle<T>(where, false);
        }

        public T SelectSingle<T>(string where, params object[] args)
        {
            return SelectSingle<T>(string.Format(where, args), false);
        }

        public T SelectSingle<T>(string where, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return (T)dc.SelectSingle(this, where, parameters);
        }

        /// <summary>
        /// SelectSingle Format. Usa string.Format internamente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T SelectSingleF<T>(string where, params object[] args)
        {
            return SelectSingle<T>(string.Format(where, args), false);
        }

        public T SelectSingle<T>(string where, bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (T)dc.SelectSingle(this, where, concurrency);
        }

        public T SelectSingle<T>(Database db, object filter, EnumDbFilter mode = EnumDbFilter.AndEqual, bool concurrency = false, int commandTimeout = 30)
        {
            DataClass dc = getCache<T>();
            return (T)dc.SelectSingle(db, filter, mode, concurrency, commandTimeout);
        }

        #endregion SelectSingle

        public List<T> Query<T>(string sql)
        {
            return Query<T>(sql, false);
        }

        public List<T> Query<T>(string sql, bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (List<T>)dc.Query(this, sql, concurrency);
        }

        public T SelectByPK<T>(T instance)
        {
            DataClass dc = getCache<T>();
            return (T)dc.SelectByPk(this, instance);
        }

        public T SelectByPK<T>(T instance, bool concurrency)
        {
            DataClass dc = getCache<T>();
            return (T)dc.SelectByPk(this, instance, concurrency);
        }

        public int Insert<T>(Record instance)
        {
            DataClass dc = getCache<T>();
            return dc.Insert(this, instance);
        }

        public int Insert<T>(Record instance, EnumSaveMode mode = EnumSaveMode.None)
        {
            DataClass dc = getCache<T>();
            return dc.Insert(this, instance, mode);
        }

        public int InsertXml<T>(List<T> list, EnumSaveMode mode = EnumSaveMode.None)
        {
            DataClass dc = getCache<T>();
            return dc.InsertXml(this, list, mode);
        }

        public int Update<T>(Record instance)
        {
            DataClass dc = getCache<T>();
            return dc.Update(this, instance, EnumSaveMode.None);
        }

        public int Update<T>(Record instance, EnumSaveMode mode)
        {
            DataClass dc = getCache<T>();
            return dc.Update(this, instance, mode);
        }

        /// <summary>
        /// Salva na base de dados
        /// Se RecordStatus.New - executa Insert, se RecordStatus.Modified - executa Update e se RecordStatus.Deleted - executa Delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public int Save<T>(Record instance)
        {
            DataClass dc = getCache<T>();
            return dc.Save(this, instance, EnumSaveMode.None);
        }

        /// <summary>
        /// Salva na base de dados
        /// Se RecordStatus.New - executa Insert, se RecordStatus.Modified - executa Update e se RecordStatus.Deleted - executa Delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int Save<T>(Record instance, EnumSaveMode mode)
        {
            DataClass dc = getCache<T>();
            return dc.Save(this, instance, mode);
        }

        /// <summary>
        /// Salva na base de dados
        /// Se RecordStatus.New - executa Insert, se RecordStatus.Modified - executa Update e se RecordStatus.Deleted - executa Delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="continueOnError"></param>
        public void SaveList<T>(List<T> instance, bool continueOnError)
        {
            DataClass dc = getCache<T>();
            dc.SaveList(this, instance, EnumSaveMode.None, continueOnError);
        }

        /// <summary>
        /// Salva na base de dados
        /// Se RecordStatus.New - executa Insert, se RecordStatus.Modified - executa Update e se RecordStatus.Deleted - executa Delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="mode"></param>
        /// <param name="continueOnError"></param>
        public void SaveList<T>(List<T> instance, EnumSaveMode mode, bool continueOnError)
        {
            DataClass dc = getCache<T>();
            dc.SaveList(this, instance, mode, continueOnError);
        }

        public int Truncate<T>()
        {
            DataClass dc = getCache<T>();
            return dc.Truncate(this);
        }

        /// <summary>
        /// Exclui TODOS registros da tabela
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Delete<T>()
        {
            DataClass dc = getCache<T>();
            return dc.Delete(this);
        }

        /// <summary>
        /// Exclui 1 registro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public int Delete<T>(Record instance)
        {
            DataClass dc = getCache<T>();
            return dc.Delete(this, instance);
        }

        /// <summary>
        /// Exclui registros filtrados pela cláusula where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<T>(string where)
        {
            DataClass dc = getCache<T>();
            return dc.Delete(this, where);
        }

        /// <summary>
        /// Exclui registros filtrados pela cláusula where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>

        public int Delete<T>(string where, params object[] args)
        {
            DataClass dc = getCache<T>();
            return dc.Delete(this, string.Format(where, args));
        }

        /// <summary>
        /// Exclui registros filtrados pela cláusula where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Delete<T>(string where, int commandTimeout, params object[] args)
        {
            DataClass dc = getCache<T>();
            return dc.Delete(this, string.Format(where, args), commandTimeout);
        }

        /// <summary>
        /// Exclui registros filtrados pela cláusula where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Delete<T>(string where, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return dc.Delete(this, where, parameters);
        }

        /// <summary>
        /// Exclui registros filtrados pela cláusula where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Delete<T>(string where, int commandTimeout, params Parameter[] parameters)
        {
            DataClass dc = getCache<T>();
            return dc.Delete(this, where, commandTimeout, parameters);
        }


        /*
        public int DeleteAll<T>(Record[] instance)
        {
            DataClass dc = getCache<T>();
            return dc.DeleteAll(this, instance);
        }
        */

        public int DeleteByPk<T>(T instance)
        {
            DataClass dc = getCache<T>();
            return dc.DeleteByPk(this, instance);
        }

        #endregion Table Methods

        #region Cache


        Dictionary<Type, DataClass> _cacheType = new Dictionary<Type, DataClass>();
        private DataClass getCache<T>()
        {
            //return cache.GetDataClass(this, typeof(T));
            DataClass dc;
            Type type = typeof(T);
            if (_cacheType.TryGetValue(type, out dc))
                return dc;
            else
            {
                dc = cache.GetDataClass(this, type);
                _cacheType.Add(type, dc);
                return dc;
            }
        }

        Dictionary<string, DataClass> _cacheName = new Dictionary<string, DataClass>();
        private DataClass getCache(string fullTypeName)
        {
            DataClass dc;
            if (_cacheName.TryGetValue(fullTypeName, out dc))
                return dc;
            else
            {
                dc = cache.GetDataClass(this, fullTypeName);
                if (dc != null)
                    _cacheName.Add(fullTypeName, dc);
                return dc;
            }
        }

        public DataClass GetDataClass<T>()
        {
            return getCache<T>();
        }

        public DataClass GetDataClass(string fullTypeName)
        {
            return getCache(fullTypeName);
        }

        /// <summary>
        /// Reset all compiled class
        /// </summary>
        public static void ClearCache()
        {
            cache.Clear();
        }

        public Dictionary<string, DbColumnInfo> GetColumnInfos<T>()
        {
            return cache.GetDataClass(this, typeof(T)).ColumnInfos;
        }

        public Dictionary<string, DbColumnInfo> GetColumnInfos(string fullTypeName)
        {
            return cache.GetDataClass(this, fullTypeName).ColumnInfos;
        }

        #endregion Cache

        #region ExecuteNonQuery

        public int ExecuteNonQuery(string commandText, params object[] args)
        {
            return ExecuteNonQuery(string.Format(commandText, args));
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            using (DbCommand cmd = NewCommand(commandText, commandTimeout, commandType))
                return cmd.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string commandText, params Parameter[] parameters)
        {
            using (DbCommand cmd = NewCommand(commandText, CommandType.Text))
            {
                cmd.Parameters.AddRange(ConvertParameters(parameters));
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, params Parameter[] parameters)
        {
            using (DbCommand cmd = NewCommand(commandText, commandType))
            {
                cmd.Parameters.AddRange(ConvertParameters(parameters));
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, int commandTimeout, params Parameter[] parameters)
        {
            using (DbCommand cmd = NewCommand(commandText, commandTimeout, commandType))
            {
                cmd.Parameters.AddRange(ConvertParameters(parameters));
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(DbCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(DbCommand cmd, params Parameter[] parameters)
        {
            cmd.Parameters.AddRange(ConvertParameters(parameters));
            return cmd.ExecuteNonQuery();
        }

        #endregion ExecuteNonQuery

        #region ExecuteScalar

        public object ExecuteScalar(string commandText)
        {
            using (DbCommand cmd = NewCommand(commandText))
                return cmd.ExecuteScalar();
        }

        public object ExecuteScalar(string commandText, params object[] args)
        {
            return ExecuteScalar(string.Format(commandText, args));
        }

        public int ExecuteInt32(string commandText)
        {
            using (DbCommand cmd = NewCommand(commandText))
                return Conv.ToInt32(cmd.ExecuteScalar());
        }

        public int ExecuteInt32(string commandText, params object[] args)
        {
            return ExecuteInt32(string.Format(commandText, args));
        }

        public long ExecuteInt64(string commandText)
        {
            using (DbCommand cmd = NewCommand(commandText))
                return Conv.ToInt64(cmd.ExecuteScalar());
        }

        public long ExecuteInt64(string commandText, params object[] args)
        {
            return ExecuteInt64(string.Format(commandText, args));
        }

        public string ExecuteString(string commandText)
        {
            using (DbCommand cmd = NewCommand(commandText))
                return Convert.ToString(cmd.ExecuteScalar());
        }

        public string ExecuteString(string commandText, params object[] args)
        {
            return ExecuteString(string.Format(commandText, args));
        }

        #endregion ExecuteScalar

        #region ExecuteReader

        public DbDataReader ExecuteReader(string commandText)
        {
            DbCommand cmd = NewCommand(commandText);
            return cmd.ExecuteReader();
        }

        public DbDataReader ExecuteReader(string commandText, int commandTimeout)
        {
            DbCommand cmd = NewCommand(commandText, commandTimeout);
            return cmd.ExecuteReader();
        }

        public DbDataReader ExecuteReader(string commandText, params Parameter[] parameters)
        {
            DbCommand cmd = NewCommand(commandText);
            if (parameters != null)
            {
                foreach (var par in parameters)
                    cmd.Parameters.Add(provider.Convert(par));
            }
            return cmd.ExecuteReader();
        }

        public DbDataReader ExecuteReader(string commandText, int commandTimeout, params Parameter[] parameters)
        {
            DbCommand cmd = NewCommand(commandText, commandTimeout);
            if (parameters != null)
            {
                foreach (var par in parameters)
                    cmd.Parameters.Add(provider.Convert(par));
            }
            return cmd.ExecuteReader();
        }

        #endregion ExecuteReader

        /*
        #region Pk

        /// <summary>
        /// Retorna as colunas da pk da tabela tableName
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<string> GetPkColums(string tableName)
        {
            List<string> pkColumns = new List<string>();
            using (DbDataReader dr = ExecuteReader("sp_pkeys '" + GetObjectName(tableName) + "'"))
            {
                while (dr.Read())
                    pkColumns.Add((string)dr["column_name"]);
                return pkColumns;
            }
        }

        #endregion Pk
        */
        #region Metadata

        /// <summary>
        /// Retorna informações de todas as colunas da tabela tableName
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DbTableInfo GetTableInfo(string tableSchema, string tableName)
        {
            return new DbTableInfo(this, tableSchema, tableName);
        }

        public List<DbColumnInfo> GetColumnsInfo(string schemaName, string tableName)
        {
            var tb = Provider.GetSchemaColumns(schemaName, tableName);
            var cols = new List<DbColumnInfo>();
            foreach (DataRow row in tb.Rows)
                cols.Add(new DbColumnInfo(this, row));

            foreach (var col in cols)
            {
                var type = col.DataType;
                if (type.Contains("("))
                    type = type.Left(type.IndexOf("("));
                var dataType = Provider.DataTypes.GetValue(type);
                if (dataType != null)
                    col.DataTypeDotNet = dataType.GetDataTypeDotNet(true);
                else
                    return null; // objeto inválido
            }
            return cols;
        }

        /// <summary>
        /// Retorna um dictionary com a key sendo o tipo de dados do banco de dados e
        /// o value o tipo correspondente do .NET
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public Dictionary<string, DbDataType> GetDataTypesGeneric()
        {
            Dictionary<string, DbDataType> types = new Dictionary<string, DbDataType>(StringComparer.InvariantCultureIgnoreCase);
            try
            {
                System.Data.DataTable tb = this.GetSchema("DataTypes");
                foreach (DataRow row in tb.Rows)
                {
                    DbDataType type = new DbDataType(row);
                    // TODO: Mysql retorna typeNames repetidos, pq tem, int, uint, etc.
                    if (!types.ContainsKey(type.TypeName))
                        types.Add(type.TypeName, type);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return types;
        }

        #endregion Metadata

        public string ExecuteConcat(string sql, string columnName, string separator)
        {
            DataTable tb = ExecuteDataTable(sql);
            string ret = "";
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                ret += Conv.ToString(tb.Rows[i][columnName]);
                if (i < tb.Rows.Count - 1)
                    ret += separator;
            }
            return ret;
        }

        public string ExecuteConcat(string sql, string separator)
        {
            DataTable tb = ExecuteDataTable(sql);
            string ret = "";
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                ret += Conv.ToString(tb.Rows[i][0]);
                if (i < tb.Rows.Count - 1)
                    ret += separator;
            }
            return ret;
        }

        public DataTable ExecuteDataTable(string commandText)
        {
            using (DbDataAdapter da = provider.CreateDataAdapter(NewCommand(commandText)))
            {
                DataTable tb = new DataTable();
                tb.ExtendedProperties.Add("Command", commandText);
                da.Fill(tb);
                return tb;
            }
        }

        public DataTable ExecuteDataTable(string commandText, int commandTimeout = 30)
        {
            using (var cmd = NewCommand(commandText))
            {
                cmd.CommandTimeout = commandTimeout;
                using (DbDataAdapter da = provider.CreateDataAdapter(cmd))
                {
                    DataTable tb = new DataTable();
                    tb.ExtendedProperties.Add("Command", commandText);
                    da.Fill(tb);
                    return tb;
                }
            }
        }

        public DataTable ExecuteDataTable(DataSet ds, string tableName, string commandText)
        {
            DataTable tb = ExecuteDataTable(commandText);
            tb.TableName = tableName;
            ds.Tables.Add(tb);
            return tb;
        }

        public DataTable ExecuteDataTable(DataTable tb, string commandText)
        {
            using (DbDataAdapter da = provider.CreateDataAdapter(NewCommand(commandText)))
            {
                tb.ExtendedProperties.Add("Command", commandText);
                da.Fill(tb);
                return tb;
            }
        }

        public DataTable ExecuteDataTable(DbCommand cmd)
        {
            using (DbDataAdapter da = provider.CreateDataAdapter(cmd))
            {
                DataTable tb = new DataTable();
                tb.ExtendedProperties.Add("Command", cmd.CommandText);
                da.Fill(tb);
                return tb;
            }
        }

        /// <summary>
        /// Método rápido e simples de retornar o Schema
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataTable GetSchemaTable(string sql, CommandType commandType = CommandType.Text, int commandtimeout = 30)
        {
            using (var cmd = NewCommand(sql, commandtimeout, commandType))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    DataTable dtSchema = reader.GetSchemaTable();
                    DataTable tb = new DataTable();

                    List<DataColumn> listCols = new List<DataColumn>();
                    if (dtSchema != null)
                    {
                        foreach (DataRow drow in dtSchema.Rows)
                        {
                            string columnName = System.Convert.ToString(drow["ColumnName"]);
                            DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                            column.Unique = Conv.ToBoolean(drow["IsUnique"]);
                            column.AllowDBNull = Conv.ToBoolean(drow["AllowDBNull"]);
                            if (dtSchema.Columns.Contains("IsAutoIncrement"))
                                column.AutoIncrement = Conv.ToBoolean(drow["IsAutoIncrement"]);
                            listCols.Add(column);
                            tb.Columns.Add(column);
                        }
                    }
                    return tb;
                }
            }
        }

        /// <summary>
        /// Método rápido e simples de retornar o Schema
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataTable GetSchemaTable(DbCommand cmd)
        {
            using (var reader = cmd.ExecuteReader())
            {
                DataTable dtSchema = reader.GetSchemaTable();
                DataTable tb = new DataTable();

                List<DataColumn> listCols = new List<DataColumn>();
                if (dtSchema != null)
                {
                    foreach (DataRow drow in dtSchema.Rows)
                    {
                        string columnName = System.Convert.ToString(drow["ColumnName"]);
                        DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                        column.Unique = (bool)drow["IsUnique"];
                        column.AllowDBNull = (bool)drow["AllowDBNull"];
                        column.AutoIncrement = (bool)drow["IsAutoIncrement"];
                        listCols.Add(column);
                        tb.Columns.Add(column);
                    }
                }
                return tb;
            }
        }

        public DataTable GetSchema()
        {
            return cn.GetSchema();
        }
        public DataTable GetSchema(string collectionName)
        {
            return cn.GetSchema(collectionName);
        }
        public DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return cn.GetSchema(collectionName, restrictionValues);
        }

        public DataTable GetSchemaColumns(string schemaName, string tableName)
        {
            return provider.GetSchemaColumns(schemaName, tableName);
        }

        DictionarySchemaTable<DbSequenceInfo> sequences;
        public DictionarySchemaTable<DbSequenceInfo> Sequences
        {
            get
            {
                if (sequences == null)
                {
                    sequences = new DictionarySchemaTable<DbSequenceInfo>();
                    Provider.GetSequences().ForEach(p => sequences.Add(p.FullName, p));
                }
                return sequences;
            }
        }

        internal List<string> GetSequencesGeneric()
        {
            List<string> ret = new List<string>();
            try
            {
                DataTable tb = GetSchema("Sequences");
                foreach (DataRow row in tb.Rows)
                    ret.Add((string)row["sequence_name"]);
            }
            catch { }

            return ret;
        }

        DictionarySchemaTable<DataTable> bufferSchemaColumns;
        public DataTable GetSchemaColumnsGeneric(string schemaName, string tableName)
        {
            schemaName = Conv.Unquote(schemaName);
            tableName = Conv.Unquote(tableName);

            if (bufferSchemaColumns == null)
            {
                string sql = "select * from INFORMATION_SCHEMA.COLUMNS order by TABLE_SCHEMA, TABLE_NAME";
                bufferSchemaColumns = new DictionarySchemaTable<DataTable>(StringComparer.InvariantCultureIgnoreCase);
                string lastName = null;
                DataTable tbCur = null;

                DataTable tb = ExecuteDataTable(sql);
                foreach (DataRow row in tb.Rows)
                {
                    string tbName = Provider.GetObjectName((string)row["TABLE_SCHEMA"], (string)row["TABLE_NAME"]);
                    if (tbName != lastName)
                    {
                        tbCur = tb.Clone();
                        bufferSchemaColumns.Add(tbName, tbCur);
                    }
                    tbCur.Rows.Add(row.ItemArray);
                    lastName = tbName;
                }
                ToString();
            }

            return bufferSchemaColumns.Find(this, schemaName, tableName);
        }

        public DataTable GetSchemaColumnsGeneric2(string schemaName, string tableName)
        {
            if (bufferSchemaColumns == null)
            {
                bufferSchemaColumns = new DictionarySchemaTable<DataTable>(StringComparer.InvariantCultureIgnoreCase);
                string lastName = null;
                DataTable tbCur = null;

                //DataTable tb = Connection.GetSchema("Columns", new string[] { null, null, tableName, null });
                DataTable tb = Connection.GetSchema("Columns", new string[] { null, null, null, null });
                foreach (DataRow row in tb.Rows)
                {
                    string tbName = Provider.GetObjectName(Conv.ToString(row["TABLE_SCHEMA"]), (string)row["TABLE_NAME"]);
                    if (tbName != lastName)
                    {
                        tbCur = tb.Clone();
                        bufferSchemaColumns.Add(tbName, tbCur);
                    }
                    tbCur.Rows.Add(row.ItemArray);
                    lastName = tbName;
                }
                ToString();
            }

            return bufferSchemaColumns.Find(this, schemaName, tableName);
        }

        /*
        DictionarySchemaTable<List<DbReferencialConstraintInfo>> referentialConstraints;
        public DictionarySchemaTable<List<DbReferencialConstraintInfo>> ReferentialContraints
        {
            get
            {
                if (referentialConstraints == null)
                {
                    referentialConstraints = new DictionarySchemaTable<List<DbReferencialConstraintInfo>>(StringComparer.InvariantCultureIgnoreCase);

                    DataTable tb, tbAux;

                    // TODO: retirar essa lógica daqui e mover para os providers
                    // TODO: ReferentialContraints e outras variáveis estáticas deve ser dependente da base de dados. Se trabalhar com mais uma base de dados não funcionará. usar DataTimer e indexar por (ProviderType+ConnectionString)

                    if (provider.SupportInformationSchema)
                    {
                        tb = MetaDataInfo.GetReferentialConstrainsByInformationSchema(this);
                        tbAux = null;
                    }
                    else if (providerType == EnumDbProviderType.SQLite)
                    {
                        tb = MetaDataInfo.GetReferentialConstrainsSQLite(this);
                        tbAux = null;
                    }
                    else if (providerType == EnumDbProviderType.Oracle)
                    {
                        tb = MetaDataInfo.GetReferentialConstrainsOracle(this);
                        tbAux = null;
                    }
                    else if (providerType == EnumDbProviderType.Firebird)
                    {
                        tb = MetaDataInfo.GetReferentialConstrainsFirebird(this);
                        tbAux = null;
                    }
                    else if (Connection is OleDbConnection)
                    {
                        tb = MetaDataInfo.GetReferentialConstrainsOleDb(this);
                        tbAux = MetaDataInfo.GetForeign_Keys(this);

                        //referentialConstraints = new DictionarySchemaTable<List<DbReferencialConstraintInfo>>();
                        //return referentialConstraints;

                        //var cno = (OleDbConnection)Connection;
                        ////var tb4 = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new string[] { null, null, null });
                        ////tb = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Constraint_Table_Usage, new object[] { null, null, null, null });
                        ////var tb2 = ((OleDbConnection)Connection).GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, null, null });
                        ////var tb3 = ((OleDbConnection)Connection).GetOleDbSchemaTable(OleDbSchemaGuid.Table_Constraints, new object[] { null, null, null, null });
                        ////tb = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, null, null });
                        //tb = cno.GetOleDbSchemaTable(OleDbSchemaGuid.Referential_Constraints, new object[] { null, null, null });
                    }
                    else
                    {
                        tb = GetSchema("ForeignKeys"); //  GetSchema("TableConstraints");
                        tbAux = null; //  GetSchema("ForeignKeys");
                    }
                    //else
                    //    return referentialConstraints;

                    //using (tb = ExecuteDataTable(sql))
                    using (tb)
                    {
                        // ForeignKeys
                        List<DbReferencialConstraintInfo> fks2 = new List<DbReferencialConstraintInfo>();
                        foreach (DataRow row in tb.Rows)
                        {
                            DbReferencialConstraintInfo fk = new DbReferencialConstraintInfo(this, row, tbAux, EnumConstraintType.ForeignKey);
                            var cc = ColumnConstraints.GetValue(fk.ConstraintFullName);
                            if (cc != null)
                            {
                                //if (fk.ConstraintType == EnumConstraintType.ForeignKey)
                                //{
                                fk.Columns.AddRange(cc);
                                fks2.Add(fk);
                                //}
                            }
                        }

                        foreach (var fk in fks2)
                        {
                            var constr = TableContraints.GetValue(fk.ConstraintFullName);

                            //// Access
                            //if (constr == null)
                            //{
                            //    constr = (from c in TableContraints where c.Value.TableName == fk.TableName select c.Value).FirstOrDefault();
                            //}

                            if (constr != null)
                            {
                                fk.TableCatalog = constr.TableCatalog;
                                fk.TableSchema = constr.TableSchema;
                                fk.TableName = constr.TableName;

                                var unique = TableContraints.GetValue(fk.UniqueConstraintFullName);
                                // SQLite
                                if (unique == null)
                                {
                                    unique = (from c in TableContraints where c.Value.TableName == constr.TableName select c.Value).FirstOrDefault();
#if DEBUG2
                                    var unique2 = (from c in TableContraints where c.Value.TableName == constr.TableName select c.Value).ToList();
                                    unique2.ToString();
#endif
                                }

                                if (unique != null)
                                {
                                    var cols = fk.Columns.Where(p => p.TableName == fk.TableName).ToList();
                                    //for (int i = 0; i < cols.Count; i++)
                                    for (int i = 0; i < fk.Columns.Count; i++)
                                    {
                                        if (providerType != EnumDbProviderType.Access)
                                        {
                                            // seta somente se não veio do metadata
                                            DbConstraintColumnInfo col = fk.Columns[i];
                                            if (col.ReferencedTableName == null)
                                            {
                                                col.ReferencedTableSchema = unique.Columns[i].TableSchema;
                                                col.ReferencedTableName = unique.Columns[i].TableName;
                                                col.ReferencedColumnName = unique.Columns[i].ColumnName;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var fkGroups = fks2.GroupByToDictionary(p => p.TableFullName);
                        //var fkGroups = fks2.GroupByToDictionary(p => provider.GetObjectName(p.UniqueConstraintSchema, p.UniqueConstraintName));
                        foreach (var group in fkGroups)
                        {
                            string key = group.Key;
                            referentialConstraints.Add(key, group.Value);
                        }
                    }
                    ToString();
                }

                return referentialConstraints;
            }
        }

        DictionarySchemaTable<DbSequenceInfo> sequences;
        public DictionarySchemaTable<DbSequenceInfo> Sequences
        {
            get
            {
                if (sequences == null)
                {
                    sequences = new DictionarySchemaTable<DbSequenceInfo>();
                    Provider.GetSequences().ForEach(p => sequences.Add(p.FullName, p));
                }
                return sequences;
            }
        }

        DictionarySchemaTable<DbTableConstraintInfo> tableContraints;
        public DictionarySchemaTable<DbTableConstraintInfo> TableContraints
        {
            get
            {
                if (tableContraints == null)
                {
                    tableContraints = new DictionarySchemaTable<DbTableConstraintInfo>(StringComparer.InvariantCultureIgnoreCase);

                    DataTable tb = null, tbAux = null;

                    if (provider.SupportInformationSchema)
                    {
                        tb = MetaDataInfo.GetTableContraintsByInformationSchema(this);
                    }
                    else if (providerType == EnumDbProviderType.Oracle)
                    {
                        tb = MetaDataInfo.GetTableContraintsOracle(this);
                    }
                    else if (providerType == EnumDbProviderType.SQLite)
                    {
                        tb = MetaDataInfo.GetTableContraintsSQLite(this);
                    }
                    else if (providerType == EnumDbProviderType.Firebird)
                    {
                        tb = MetaDataInfo.GetTableContraintsFirebird(this);
                    }
                    else if (Connection is OleDbConnection)
                    {
                        tb = MetaDataInfo.GetTableContraintsOleDb(this);
                        tbAux = MetaDataInfo.GetForeign_Keys(this);
                    }
                    else
                    {
                        tb = GetSchema("TableConstraints");
                    }

                    using (tb)
                    {
                        // ForeignKeys
                        List<DbTableConstraintInfo> fks2 = new List<DbTableConstraintInfo>();
                        foreach (DataRow row in tb.Rows)
                        {
                            DbTableConstraintInfo fk = new DbTableConstraintInfo(this, row, tbAux);

                            var cols = ColumnConstraints.GetValue(fk.ConstraintFullName);
                            if (cols != null)
                            {
                                cols = cols.Where(p => p.TableName == fk.TableName).ToList();
                                fk.Columns.AddRange(cols);
                            }

                            fks2.Add(fk);
                        }

                        //var fkGroups = fks2.GroupByToDictionary(p => p.ConstraintFullName);
                        //foreach (var group in fkGroups)
                        //{
                        //    string key = group.Key;
                        //    _tableConstCache.Add(key, group.Value);
                        //}

                        //tableContraints = fks2.ToDictionary(p => p.ConstraintFullName);

                        tableContraints = new DictionarySchemaTable<DbTableConstraintInfo>();
                        foreach (var fk in fks2)
                            if (!tableContraints.ContainsKey(fk.ConstraintFullName))
                                tableContraints.Add(fk.ConstraintFullName, fk);
                    }
                    ToString();
                }

                return tableContraints;
            }
        }

        DictionarySchemaTable<List<DbConstraintColumnInfo>> columnConstraints;
        public DictionarySchemaTable<List<DbConstraintColumnInfo>> ColumnConstraints
        {
            get
            {
                if (columnConstraints == null)
                {
                    columnConstraints = new DictionarySchemaTable<List<DbConstraintColumnInfo>>(StringComparer.InvariantCultureIgnoreCase);

                    DataTable tb = null, tbAux = null;

                    if (provider.SupportInformationSchema)
                    {
                        tb = MetaDataInfo.GetColumnConstraintsByInformationSchema(this);
                    }
                    else if (providerType == EnumDbProviderType.Oracle)
                    {
                        tb = MetaDataInfo.GetColumnConstraintsOracle(this);
                    }
                    else if (providerType == EnumDbProviderType.SQLite)
                    {
                        tb = MetaDataInfo.GetColumnConstraintsSQLite(this);
                    }
                    else if (Connection is OleDbConnection)
                    {
                        tb = MetaDataInfo.GetColumnConstraintsOleDb(this);
                        tbAux = MetaDataInfo.GetForeign_Keys(this);
                    }
                    else
                    {
                        tb = GetSchema("IndexColumns");
                        //return columnConstraints;
                    }

                    //using (var tb = ExecuteDataTable(sql))
                    {
                        // ForeignKeys
                        List<DbConstraintColumnInfo> fks2 = new List<DbConstraintColumnInfo>();
                        foreach (DataRow row in tb.Rows)
                        {
                            DbConstraintColumnInfo fk = new DbConstraintColumnInfo(this, row, tbAux);
                            fks2.Add(fk);
                        }

                        var fkGroups = fks2.GroupByToDictionary(p => p.ConstraintFullName);
                        foreach (var group in fkGroups)
                        {
                            try
                            {
                                string key = group.Key;
                                if (!columnConstraints.ContainsKey(key))
                                    columnConstraints.Add(key, group.Value);
                            }
                            catch (Exception ex)
                            {
                                ex.ToString();
                            }
                        }
                    }
                }

                return columnConstraints;
            }
        }

        internal List<DbReferencialConstraintInfo> GetParentRelationsGeneric(string schemaName, string tableName)
        {
            List<DbReferencialConstraintInfo> fks = new List<DbReferencialConstraintInfo>();
            return ReferentialContraints.Find(this, schemaName, tableName);
        }

        internal List<DbReferencialConstraintInfo> GetParentRelationsGetSchema(string schemaName, string tableName)
        {
            List<DbReferencialConstraintInfo> fks = new List<DbReferencialConstraintInfo>();
            return ReferentialContraints.Find(this, schemaName, tableName);
        }

        DictionarySchemaTable<DbRoutineInfo> routines;
        public DictionarySchemaTable<DbRoutineInfo> Routines
        {
            get
            {
                if (routines == null)
                {
                    routines = new DictionarySchemaTable<DbRoutineInfo>(StringComparer.InvariantCultureIgnoreCase);

                    databaseName = Conv.Unquote(databaseName);

                    string sql = "select * from INFORMATION_SCHEMA.ROUTINES";
                    if (ProviderType == EnumDbProviderType.MySql)
                        sql = sql += " WHERE ROUTINE_SCHEMA = '" + databaseName + "'";

                    using (var tb = ExecuteDataTable(sql))
                    {
                        List<DbRoutineInfo> list = new List<DbRoutineInfo>();
                        foreach (DataRow row in tb.Rows)
                        {
                            DbRoutineInfo rec = new DbRoutineInfo(tb, row);

                            List<DbParameterInfo> pars = Parameters.GetValue(rec.FullName);
                            if (pars != null)
                                rec.Parameters = pars;

                            list.Add(rec);
                        }

                        list.ForEach(p => routines.Add(p.FullName, p));
                    }
                    ToString();
                }

                return routines;
            }
        }

        DictionarySchemaTable<List<DbParameterInfo>> parameters;
        public DictionarySchemaTable<List<DbParameterInfo>> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    parameters = new DictionarySchemaTable<List<DbParameterInfo>>(StringComparer.InvariantCultureIgnoreCase);

                    databaseName = Conv.Unquote(databaseName);

                    string sql = "select * from INFORMATION_SCHEMA.PARAMETERS";
                    if (ProviderType == EnumDbProviderType.MySql)
                        sql = sql += " WHERE ´SPECIFIC_SCHEMA = '" + databaseName + "'";

                    using (var tb = ExecuteDataTable(sql))
                    {
                        List<DbParameterInfo> list = new List<DbParameterInfo>();
                        foreach (DataRow row in tb.Rows)
                        {
                            DbParameterInfo rec = new DbParameterInfo(tb, row);
                            list.Add(rec);
                        }

                        var groups = list.GroupByToDictionary(p => p.RoutineFullName);
                        foreach (var group in groups)
                        {
                            string key = group.Key;
                            parameters.Add(key, group.Value.OrderBy(p => p.OrdinalPosition).ToList());
                        }
                    }
                    ToString();
                }

                return parameters;
            }
        }
        */

        public DbConnection NewConnection()
        {
            return provider.NewConnection(connectionString);
        }

        #region Linq

#if USELINQ

        public IQueryable<Record> Query;
        private IQueryProvider provider;
        public QueryPolicy StandardPolicy = new QueryPolicy(new ImplicitMapping(new TSqlLanguage()));
        TextWriter log;
        private bool islinqInit = false;

        public IQueryProvider Provider
        {
            get { return this.provider; }
        }

        public virtual void InitializeLinq()
        {
            log = File.CreateText("Linq.log");
            this.provider = new DbQueryProvider(db, StandardPolicy, log);
            this.Query = new Query<Record>(this.provider);
            islinqInit = true;
            // só pra tirar o warning de não usado
            islinqInit.ToString();
        }

        public virtual void TerminateLinq()
        {
            if (islinqInit)
            {
                islinqInit = false;
                if (log != null)
                {
                    log.Close();
                    log = null;
                }
            }
        }
        
#endif
        #endregion Linq

        #region Helper

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value)
        {
            return provider.AddWithValue(cmd, parameterName, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, string dataType, object value)
        {
            return provider.AddWithValue(cmd, parameterName, dataType, value);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, object value, int size)
        {
            return provider.AddWithValue(cmd, parameterName, value, size);
        }

        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType, object value, ParameterDirection direction = ParameterDirection.Input, int size = 0)
        {
            return provider.AddParameter(cmd, parameterName, dbType, direction, value, size);
        }

        public DbParameter AddWithValue(DbCommand cmd, string parameterName, string propTypeName, object value, int size)
        {
            if (propTypeName == "Byte[]")
                return provider.AddParBinary(cmd, parameterName, value, size);
            else
                return provider.AddWithValue(cmd, parameterName, value, size);
        }

        public DbParameter AddParameterRef(DbCommand cmd, string parameterName, ParameterDirection direction, object value = null)
        {
            return provider.AddParRef(cmd, parameterName, direction, value);
        }

        public DbParameter AddParameterRefCursor(DbCommand cmd, string parameterName, ParameterDirection direction, object value = null)
        {
            return provider.AddParRefCursor(cmd, parameterName, direction, value);
        }

        /*
        /// <summary>
        /// Retorna o nome do objeto formatado pro banco de dados
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetObjectName(string schemaName, string name, bool quote = true)
        {

            string ret = "";
            if (!string.IsNullOrEmpty(schemaName))
                ret += GetObjectName(schemaName) + ".";
            ret += GetObjectName(name);
            return ret;
        }

        /// <summary>
        /// Retorna o nome do objeto formatado pro banco de dados
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetObjectName(string name, bool quote = true)
        {
            return provider.GetObjectName(name);
        }
        */

        /*
        public string GetTableName(string name)
        {
            if (providerType == EnumDbProviderType.MySql)
                return GetObjectName(databaseName, null, name);
            else
                return GetObjectName(name);
        }

        public string GetTableName(string schemaName, string tableName)
        {
            if (providerType == EnumDbProviderType.MySql)
                return GetObjectName(databaseName, null, tableName);
            else if (string.IsNullOrEmpty(schemaName))
                return GetObjectName(tableName);
            else
                return GetObjectName(schemaName) + "." + GetObjectName(tableName);
        }
        */

        #endregion Helper

        #region Reflection

        /*
        /// <summary>
        /// Registrar classe de um assembly. Isto somente é necessário se desejar usar os métodos de reflexão
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterAssembly(string assemblyName)
        {
            string name = assemblyName.Trim().ToLower();
            // não registra duas vezes
            lock (assRegistered)
            {
                if (assRegistered.Contains(name))
                    return;

                assRegistered.Add(name);
            }
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            Type[] tps = assembly.GetExportedTypes();
            lock (refTypes)
            {
                foreach (Type tp in tps)
                {
                    // só registra as classes que herdam de Record
                    if ((Activator.CreateInstance(tp) as Record) != null)
                        if (!refTypes.ContainsKey(tp.Name))
                            refTypes.Add(tp.Name, tp);
                }
            }
        }
        */

        /*
        /// <summary>
        /// Registrar classe de um assembly. Isto somente é necessário se desejar usar os métodos de reflexão
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterAssembly(Assembly assembly)
        {
            string name = assembly.FullName.ToLower();
            // não registra duas vezes
            lock (assRegistered)
            {
                if (assRegistered.Contains(name))
                    return;

                assRegistered.Add(name);
            }

            //Type[] tps = assembly.GetExportedTypes();
            //lock (refTypes)
            //{
            //    foreach (Type tp in tps)
            //    {
            //        // só registra as classes que herdam de Record
            //        try
            //        {
            //            if ((Activator.CreateInstance(tp) as Record) != null)
            //                if (!refTypes.ContainsKey(tp.Name))
            //                    refTypes.Add(tp.Name, tp);
            //        }
            //        catch { }
            //    }
            //}
        }
        */

        public static void RegisterAssemblyAndCompile(Database db, Type type)
        {
            RegisterAssemblyAndCompile(db, type.Assembly, false);
        }

        public static void RegisterAssemblyAndCompile(Database db, Assembly assembly)
        {
            cache.RegisterAssemblyAndCompile(db, assembly, false);
        }

        public static void RegisterAssemblyAndCompile(Database db, Type type, bool refresh)
        {
            RegisterAssemblyAndCompile(db, type.Assembly, refresh);
        }

        public static void RegisterAssemblyAndCompile(Database db, Assembly assembly, bool refresh)
        {
            cache.RegisterAssemblyAndCompile(db, assembly, refresh);
        }

        /*
        /// <summary>
        /// Registrar classe de um assembly. Isto somente é necessário se desejar usar os métodos de reflexão
        /// </summary>
        /// <param name="type">Tipo de qualquer classe contida no assembly</param>
        public static void RegisterAssembly(Type type)
        {
            RegisterAssembly(Assembly.GetAssembly(type));
        }

        public static void ClearClache()
        {
            assRegistered.Clear();
            cache.Clear();
        }
        */

        /*
        /// <summary>
        /// Checa se o tipo foi registrado, usando RegisterAssembly.
        /// Se não, dispara um erro
        /// </summary>
        /// <param name="typeName"></param>
        private void checkType(string fullTypeName)
        {
            if (!refTypes.ContainsKey(typeName))
                throw new Exception("Tipo não registrado: " + typeName + ".\r\nUser o método 'RegisterAssembly' para registrar o assembly onde '" + typeName + "' existe, para chamada de métodos de reflection");
            else CheckCache(typeName);
        }
        */

        #region RefSelect

        public List<Record> RefSelect(string fullTypeName)
        {
            return RefSelect(fullTypeName, false);
        }

        public List<Record> RefSelect(string fullTypeName, bool concurrency)
        {
            DataClass dc = getCache(fullTypeName);
            var ret = dc.SelectArray(this, concurrency);
            return (new List<Record>((Record[])ret));

            /*
            checkType(typeName);

            object instance = refTypes[typeName].Assembly.CreateInstance(refTypes[typeName].FullName);
            MethodInfo method = refMethods["SelectArray(Boolean concurrency)"].MakeGenericMethod(new Type[] { instance.GetType() });
            var ret2 = method.Invoke(this, new object[] { concurrency });
            Array r2 = (Array)ret2;

            return (new List<Record>((Record[])ret2));
            */
        }

        public List<Record> RefSelect(string fullTypeName, string where)
        {
            return RefSelect(fullTypeName, where, false);
        }

        public List<Record> RefSelect(string fullTypeName, string where, bool concurrency)
        {
            DataClass dc = getCache(fullTypeName);
            var ret = dc.SelectArray(this, where, concurrency);
            return (new List<Record>((Record[])ret));
        }

        #endregion RefSelect

        #region SelectToJson

        public string SelectToJson(string fullTypeName)
        {
            return SelectToJson(fullTypeName, false);
        }

        public string SelectToJson(string fullTypeName, bool concurrency)
        {
            DataClass dc = getCache(fullTypeName);
            return dc.SelectToJson(this, concurrency);
        }

        public string SelectToJson(string fullTypeName, string where)
        {
            return SelectToJson(fullTypeName, where, false);
        }

        public string SelectToJson(string fullTypeName, string where, bool concurrency)
        {
            DataClass dc = getCache(fullTypeName);
            return dc.SelectToJson(this, where, concurrency);
        }

        public string UpdateFromJson(string fullTypeName, string json)
        {
            return UpdateFromJson(fullTypeName, json, EnumSaveMode.None);
        }

        public string UpdateFromJson(string fullTypeName, string json, EnumSaveMode saveMode)
        {
            DataClass dc = getCache(fullTypeName);
            return dc.UpdateFromJson(this, json, saveMode);
        }

        public string SaveFromJson(string fullTypeName, string json)
        {
            return SaveFromJson(fullTypeName, json, EnumSaveMode.None);
        }

        public string SaveFromJson(string fullTypeName, string json, EnumSaveMode saveMode)
        {
            DataClass dc = getCache(fullTypeName);
            return dc.SaveFromJson(this, json, saveMode);
        }

        public string SaveListFromJson(string fullTypeName, string json, EnumSaveMode saveMode, bool continueOnError)
        {
            DataClass dc = getCache(fullTypeName);
            return dc.SaveListFromJson(this, json, saveMode, continueOnError);
        }

        /*
        /// <summary>
        /// só pra testes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="json"></param>
        /// <param name="saveMode"></param>
        /// <param name="continueOnError"></param>
        /// <returns></returns>
        public string SaveListFromJsonDEBUG<T>(Database db, string json, EnumSaveMode saveMode, bool continueOnError)
        {
            List<T> rec = SerializationUtil.DeserializeFromJson<List<T>>(json);
            SaveList<T>(rec, saveMode, continueOnError);
            return SerializationUtil.SerializeToJson<List<T>>(rec);
        }
        */

        #endregion SelectToJson

        #region SelectArray

        public T[] SelectArray<T>()
        {
            return SelectArray<T>(false);
        }

        public T[] SelectArray<T>(bool concurrency)
        {
            DataClass dc = getCache<T>();
            return ((List<T>)dc.Select(this, concurrency)).ToArray();
        }

        public T[] SelectArray<T>(string where)
        {
            return SelectArray<T>(where, false);
        }

        public T[] SelectArray<T>(string where, bool concurrency)
        {
            DataClass dc = getCache<T>();
            return ((List<T>)dc.Select(this, where, concurrency)).ToArray();
        }

        #endregion SelectArray

        #endregion Reflection

        #region Generate
        /*
        /// <summary>
        /// Gera classes pra Tables e Views
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private EntityInfo GenerateRecord(Dictionary<string, EntityInfo> tables, string entityNameSpace, string businnesNameSpace, string classNameBL, string schemaName, string tableName)
        {
            return GenerateRecord(tables, entityNameSpace, businnesNameSpace, classNameBL, StringUtil.ToPascalCase(tableName), schemaName, tableName, false);
        }
        */

        /// <summary>
        /// Gera classe de um comando ou de tables e views.
        /// Se isComand = true, deve ser passado um comando.
        /// Se isComand = false, deve passado uma tabela ou view
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="sqlOrTableName"></param>
        /// <param name="isCommand"></param>
        /// <returns></returns>
        private GenTableResult GenerateRecord(Dictionary<string, GenTableResult> tables, string entityNameSpace, string businnesNameSpace, GenTable item, bool isCommand, EnumNameCase nameCase = EnumNameCase.Pascal, bool raisePropertyChanged = true)
        {
            StringBuilder rct = new StringBuilder();
            StringBuilder fct = new StringBuilder();
            StringBuilder beq = new StringBuilder();
            StringBuilder clone = new StringBuilder();
            StringBuilder createColumns = new StringBuilder();
            StringBuilder createParams = new StringBuilder();

            DbTableInfo table;
            //if (this.ProviderType == EnumDbProviderType.MySql)
            //    table = new DbTableInfo(this, this.databaseName, sql, isCommand);
            //else
            //    table = new DbTableInfo(this, sql, isCommand);
            table = new DbTableInfo(this, item.SchemaName, item.TableName, isCommand);

            item.SchemaName = table.SchemaName;
            if (string.IsNullOrEmpty(item.SequenceColumn))
                item.SequenceColumn = table.SequenceColumn;
            if (string.IsNullOrEmpty(item.SequenceName))
                item.SequenceName = table.SequenceName;

            if (item.DataClassName == null)
                item.DataClassName = GetName(item.TableName, nameCase);
            if (item.BusinessClassName == null)
                item.BusinessClassName = "BL_" + item.DataClassName;

            Dictionary<string, string> usings = new Dictionary<string, string>();

            GenTableResult info = new GenTableResult();
            info.Table = table;
            info.GenTable = item;

            bool hasEnum = false;

            int index = 0;

            var gcols = item.Columns.ToDictionary(p => p.ColumnName, StringComparer.InvariantCultureIgnoreCase);

            foreach (var pair in table.Columns)
            {
                DbColumnInfo col = pair.Value;
                GenColumn gcol = gcols.ContainsKey(col.ColumnName) ? gcols[col.ColumnName] : null;

                string propertyName;
                if (gcol != null && !string.IsNullOrWhiteSpace(gcol.PropertyName))
                    propertyName = gcol.PropertyName;
                else
                    propertyName = GetName(col.ColumnName, nameCase);

                if (!isCommand && col.ColumnName == item.EnumColumnName)
                    hasEnum = true;

                string colDataType = col.GetDataTypeName().Replace("SqlHierarchyId?", "SqlHierarchyId");
                string colDataTypeNullable = col.GetNullableDataTypeName().Replace("SqlHierarchyId?", "SqlHierarchyId");

                // RECORD_COLUMN_TEMPLATE
                string recTemplate = raisePropertyChanged ? Templates.RECORD_COLUMN_TEMPLATE_RAISEPROPERTYCHANGED : Templates.RECORD_COLUMN_TEMPLATE_POCO;

                if (!raisePropertyChanged)
                {
                    recTemplate = recTemplate
                        .Replace("[POCO]", "")
                        .Replace("[!POCO]", "//");
                }
                else
                {
                    recTemplate = recTemplate
                        .Replace("[POCO]", "//")
                        .Replace("[!POCO]", "");
                }

                rct.Append(recTemplate
                    .Replace("[ColumnName]", col.ColumnName)
                    .Replace("[PopertyName]", propertyName)
                    .Replace("[DataType]", colDataType)
                    .Replace("[DataTypeNullable]", colDataTypeNullable)
                    .Replace("[DataAnnotation]", !string.IsNullOrWhiteSpace(gcol.DataAnnotation) ? gcol.DataAnnotation.Trim() : "")
                    );



                // RECORD_FILTER_COLUMN_TEMPLATE
                fct.Append(Templates.RECORD_FILTER_COLUMN_TEMPLATE
                    .Replace("[ColumnName]", col.ColumnName)
                    .Replace("[PopertyName]", propertyName)
                    .Replace("[DataType]", colDataType)
                    .Replace("[DataTypeNullable]", colDataTypeNullable));

                if (!Provider.AddUsings(col, usings))
                {
                    // IsEqual
                    beq.AppendFormat("\t\t\t\tthis.{0} == value.{0}", propertyName);
                    if (index < table.Columns.Count - 1)
                        beq.AppendFormat(" &&\r\n", propertyName);
                    else
                        beq.AppendFormat(";\r\n", propertyName);

                    // Clone
                    clone.AppendFormat("\t\t\tvalue.{0} = this.{0};\r\n", propertyName);

                    // Create
                    if (!col.IsIdentity)
                    {
                        string colName = gcol != null && !string.IsNullOrWhiteSpace(gcol.PropertyName) ? gcol.PropertyName : GetName(col.ColumnName, nameCase);
                        string _colName = "_" + colName;
                        createColumns.AppendFormat("\t\t\t__value.{0} = {1};\r\n", colName, _colName);
                        createParams.AppendFormat("{0} {1}, ", colDataType, _colName);
                    }
                }
                index++;
            }

            // tem que torcar os wheres por 

            string usingAdd = "";
            if (usings.Count > 0)
            {
                foreach (var us in usings)
                    usingAdd += us.Key + "\r\n";
            }

            string pars = createParams.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(pars))
                pars = pars.Substring(0, pars.Length - 1);
            else
                ToString();

            string ret = Templates.GEN_WARNING + Templates.RECORD_TEMPLATE
                .Replace("[SchemaName]", !string.IsNullOrEmpty(item.SchemaName) ? "'" + item.SchemaName + "'" : "''")
                .Replace("[TableName]", item.TableName)
                .Replace("[SequenceColumn]", item.SequenceColumn)
                .Replace("[SequenceName]", item.SequenceName)
                .Replace("[UsingAdd]", usingAdd)
                .Replace("[ClassName]", item.DataClassName)
                .Replace("[Columns]", rct.ToString()).Replace("'", "\"")
                .Replace("[ColumnsIsEqual]", beq.ToString())
                .Replace("[ColumnsClone]", clone.ToString())
                .Replace("[CreateColumns]", createColumns.ToString())
                .Replace("[CreateParams]", pars)
                .Replace("[NameSpace]", entityNameSpace)
                .Replace("[DataAnnotation]", !string.IsNullOrWhiteSpace(item.DataAnnotation) ? item.DataAnnotation.Trim() : "");

            if (!raisePropertyChanged)
            {
                ret = ret
                    .Replace("[POCO]", "")
                    .Replace("[!POCO]", "//");
            }
            else
            {
                ret = ret
                    .Replace("[POCO]", "//")
                    .Replace("[!POCO]", "");
            }

            if (isCommand)
                ret = ret.Replace("[DbTable(", "[DbSqlCommand(");

            info.EntityClass = ret;

            ret = Templates.RECORD_EXT_TEMPLATE.Replace("[TableName]", item.TableName)
                .Replace("[ClassName]", item.DataClassName)
                .Replace("[NameSpace]", entityNameSpace);

            info.EntityClassExt = ret;

            // Filter
            ret = Templates.GEN_WARNING + Templates.RECORD_FILTER_TEMPLATE
                .Replace("[UsingAdd]", usingAdd)
                .Replace("[FilterClassName]", item.DataClassName + "")
                //.Replace("[FilterClassName]", item.DataClassName + "__Filter")
                .Replace("[Columns]", fct.ToString()).Replace("'", "\"")
                .Replace("[NameSpace]", entityNameSpace);

            info.EntityFilterClass = ret;

            // BL
            string pkDeclaration = "";
            string pkDeclarationNullable = "";
            string pkColumnsWhere = "";
            string pkColumnsValue = "";
            string pkColumnsParameters = "";
            string selectByFilter = "";
            string selectByPk = "";
            string deleteByPk = "";
            if (table.PrimaryKey.Count > 0)
            {
                selectByPk = Templates.BL_TEMPLATE_SELECT_BY_PK;
                selectByPk = Templates.BL_TEMPLATE_SELECT_BY_PK;
                deleteByPk = Templates.BL_TEMPLATE_DELETE_BY_PK;
                index = 0;
                foreach (var pair in table.PrimaryKey)
                {
                    DbColumnInfo col = pair.Value;
                    string pasCalColumn = GetName(col.ColumnName, nameCase);
                    string camelColumn = "_" + pasCalColumn;

                    pkDeclaration += string.Format("{0} {1}, ", col.DataTypeDotNet, camelColumn);
                    pkDeclarationNullable += string.Format("{0} {1}, ", col.GetNullableDataTypeName(), camelColumn);
                    pkColumnsValue += string.Format("{0}, ", col.GetQuoteValue(camelColumn));
                    pkColumnsWhere += string.Format("{0}={1} AND ", col.ColumnName, "{" + index + "}");
                    pkColumnsParameters += string.Format("{0}={1}{2} AND ", col.ColumnName, provider.ParameterSymbol, pasCalColumn);
                    index++;
                }
                pkDeclaration = pkDeclaration.Substring(0, pkDeclaration.Length - 2);
                pkDeclarationNullable = pkDeclarationNullable.Substring(0, pkDeclarationNullable.Length - 2);
                pkColumnsWhere = pkColumnsWhere.Substring(0, pkColumnsWhere.Length - 5);
                pkColumnsParameters = pkColumnsParameters.Substring(0, pkColumnsParameters.Length - 5);
                pkColumnsValue = pkColumnsValue.Substring(0, pkColumnsValue.Length - 2);

                selectByPk = selectByPk
                    .Replace("'", "\"")
                    .Replace("[NameSpaceEntity]", entityNameSpace)
                    .Replace("[NameSpaceBL]", businnesNameSpace)
                    .Replace("[Entity]", item.DataClassName)
                    .Replace("[PkDeclaration]", pkDeclaration)
                    .Replace("[PkDeclarationNullable]", pkDeclarationNullable)
                    .Replace("[PkColumnsWhere]", pkColumnsWhere)
                    .Replace("[PkColumnsValue]", pkColumnsValue);

                deleteByPk = deleteByPk
                    .Replace("'", "\"")
                    .Replace("[NameSpaceEntity]", entityNameSpace)
                    .Replace("[NameSpaceBL]", businnesNameSpace)
                    .Replace("[Entity]", item.DataClassName)
                    .Replace("[PkDeclaration]", pkDeclaration)
                    .Replace("[PkDeclarationNullable]", pkDeclarationNullable)
                    .Replace("[PkColumnsWhere]", pkColumnsWhere)
                    .Replace("[PkColumnsValue]", pkColumnsValue);
            }

            //string blName = StringUtil.ToPascalCase(sql);
            string blName = item.BusinessClassName;

            info.EntityName = item.DataClassName;
            info.BusinnesName = blName;

            info.BusinnesClass = Templates.GEN_WARNING + Templates.BL_TEMPLATE
                .Replace("'", "\"")
                .Replace("[BLName]", blName)
                .Replace("[UsingAdd]", usingAdd)
                .Replace("[NameSpaceEntity]", entityNameSpace)
                .Replace("[NameSpaceBL]", businnesNameSpace)
                .Replace("[Entity]", item.DataClassName)
                .Replace("[SelectByPk]", selectByPk)
                .Replace("[SelectByFilter]", selectByPk)
                .Replace("[DeleteByPk]", deleteByPk);

            info.BusinnesClassExt = Templates.BL_EXT_TEMPLATE
                .Replace("'", "\"")
                .Replace("[BLName]", blName)
                .Replace("[NameSpaceEntity]", entityNameSpace)
                .Replace("[NameSpaceBL]", businnesNameSpace)
                .Replace("[Entity]", item.DataClassName);

            // Enum
            if (hasEnum && table.PrimaryKey.Count > 0)
            {
                DbColumnInfo colEnumColumnId;
                if (!string.IsNullOrEmpty(item.EnumColumnId) && table.Columns.ContainsKey(item.EnumColumnId))
                    colEnumColumnId = table.Columns[item.EnumColumnId];
                else
                    colEnumColumnId = table.PrimaryKey.First().Value;

                //DataTable tb = this.ExecuteDataTable(provider.SetTop("select * from " + GetObjectName(null, table.SchemaName, table.TableName), 0) + " order by " + GetObjectName(colId.ColumnName));

                string sql = string.Format("select distinct {0}, {1} from {2} where not ({1} is null or {1} = '')",
                    colEnumColumnId.ColumnName, item.EnumColumnName, Provider.GetObjectName(table.SchemaName, table.TableName));
                //string.Join(",", (from c in table.PrimaryKey select Provider.GetObjectName(c.Value.ColumnName))), item.EnumColumnName, Provider.GetObjectName(table.SchemaName, table.TableName));
                DataTable tb = this.ExecuteDataTable(sql);

                if (tb.Rows.Count > 0)
                {
                    StringBuilder b2 = new StringBuilder();
                    for (int i = 0; i < tb.Rows.Count; i++)
                    {
                        DataRow row = tb.Rows[i];

                        string s = Conv.Trim(row[item.EnumColumnName]);
                        string _enum = "";
                        foreach (char c in s)
                        {
                            char cU = c.ToString().ToUpper()[0];
                            if ((cU >= 'A' && cU < 'Z') | (cU >= '0' && cU < '9') | c == '_')
                                _enum += c.ToString();
                        }

                        _enum = GetName(_enum, nameCase);

                        b2.Append("\t\t" + _enum + " = " + Conv.ToInt32(row[colEnumColumnId.ColumnName]));
                        if (i < tb.Rows.Count - 1)
                            b2.Append(",\r\n");
                        else
                            b2.Append("\r\n");
                    }

                    string _type = colEnumColumnId.DataTypeDotNet.ToLower();
                    _type = _type.Replace("16", "").Replace("32", "").Replace("64", "");

                    if (string.IsNullOrEmpty(info.EnumName))
                        info.EnumName = GetName("EnumDb[ClassName]".Replace("[ClassName]", item.DataClassName), nameCase);

                    info.Enum = Templates.ENUM_TEMPLATE
                        .Replace("[EnumName]", info.EnumName)
                        .Replace("[Type]", _type)
                        .Replace("[EnumColumns]", b2.ToString()
                        .Replace("[Entity]", item.DataClassName));
                }
            }

            return info;
        }

        /// <summary>
        /// Segunda fase de GenerateRecord
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="table"></param>
        /// <param name="info"></param>
        /// <param name="entityNameSpace"></param>
        /// <param name="businnesNameSpace"></param>
        /// <param name="classNameBL"></param>
        /// <param name="entityName"></param>
        /// <param name="schemaName"></param>
        /// <param name="sqlOrTableName"></param>
        /// <param name="isCommand"></param>
        /// <param name="enumColumnName"></param>
        private void GenerateRecord2(DictionarySchemaTable<GenTableResult> tables, GenTableResult info, string entityNameSpace, string businnesNameSpace, EnumNameCase nameCase = EnumNameCase.Pascal)
        {
            DbTableInfo table = info.Table;
            DbTableInfo table2 = info.Table;
            var item = info.GenTable;

            string classNameBL = item.BusinessClassName;
            string entityName = item.DataClassName;
            string schemaName = item.SchemaName;
            string sqlOrTableName = item.TableName;
            string enumColumnName = item.EnumColumnName;

            // GenColumns
            var gcols = item.Columns.ToDictionary(p => p.ColumnName, StringComparer.InvariantCultureIgnoreCase);

            // TODO: Já foi testado pro Sql Server. Para o MySql tá dando problemas, que está invertendo as colunas
            //if (!(ProviderType == EnumDbProviderType.SqlServer | ProviderType == EnumDbProviderType.SqlServerCe))
            //    return;

            // Dictionaries pra guardar os métodos gerados, pra evitar duplicidades. Isso ocorre em bases de dados
            // que possuem relacionamentos repetidos entre tabelas
            List<string> parentMethods = new List<string>();
            List<string> childMethods = new List<string>();

            // ------ ParentRelations ------
            foreach (var pairFk in table.ParentRelations)
            {
                DbReferencialConstraintInfo fk = pairFk.Value;

                string fkDeclaration = "";
                string fkDeclarationNullable = "";
                string fkTypes = "";
                string fkTypesNullable = "";
                string fkColumnsWhere = "";
                string fkColumnsValue = "";
                string selectByFk = "";
                string className;
                string fkColumnsValueEntity = "";

                if (fk.Columns.Count > 0)
                {
                    selectByFk = Templates.BL_TEMPLATE_SELECT_BY_PARENT_RELATIONS;
                    className = info.EntityName;

                    int index = 0;

                    var col0 = fk.Columns[0];
                    if (col0.ReferencedTableName == null)
                        col0.ReferencedTableName = null;
                    GenTableResult infoRef = tables.Find(this, col0.ReferencedTableSchema, col0.ReferencedTableName);
                    if (infoRef != null)
                    {
                        DbTableInfo tableRef = infoRef.Table;

                        foreach (DbConstraintColumnInfo fkCol in fk.Columns)
                        {
                            DbColumnInfo colRef = tableRef.Columns[fkCol.ReferencedColumnName];
                            GenTableResult gtableResRef = tables.Find(this, tableRef.SchemaName, tableRef.TableName);
                            GenTable gtableRef = gtableResRef != null ? gtableResRef.GenTable : null;
                            var gcolsRef = gtableRef != null ? gtableRef.Columns.ToDictionary(p => p.ColumnName, StringComparer.InvariantCultureIgnoreCase) : new Dictionary<string, GenColumn>();
                            GenColumn gcolRef = gcolsRef != null && gcolsRef.ContainsKey(fkCol.ColumnName) ? gcolsRef[fkCol.ColumnName] : null;
                            string propRef = gcolRef != null && !string.IsNullOrWhiteSpace(gcolRef.PropertyName) ? gcolRef.PropertyName : GetName(colRef.ColumnName, nameCase);
                            string _propRef = "_" + propRef;

                            DbColumnInfo col = table.Columns[fkCol.ColumnName];
                            GenColumn gcol = gcols != null && gcols.ContainsKey(col.ColumnName) ? gcols[col.ColumnName] : null;

                            string prop = gcol != null && !string.IsNullOrWhiteSpace(gcol.PropertyName) ? gcol.PropertyName : GetName(colRef.ColumnName, nameCase);
                            string _prop = "_" + prop;

                            string type = colRef.DataTypeDotNet;
                            // se um é nullable e ou outro não, usa nullable, senão dá erro de compilação
                            if (col.IsNullable && !colRef.IsNullable)
                                if (type != "String" & type != "Object" & type.ToLower() != "byte[]")
                                    type += "?";

                            fkTypes += string.Format("{0}, ", type);
                            fkTypesNullable += string.Format("{0}, ", col.GetNullableDataTypeName());
                            fkDeclaration += string.Format("{0} {1}, ", type, _propRef);
                            fkDeclarationNullable += string.Format("{0} {1}, ", col.GetNullableDataTypeName(), _propRef);
                            fkColumnsWhere += string.Format("{0}={1} AND ", colRef.ColumnName, "{" + index + "}");
                            fkColumnsValue += string.Format("{0}, ", colRef.GetQuoteValue(_propRef));

                            //fkColumnsValueEntity += string.Format("rec.{0}, ", pasCalColumn);
                            //fkColumnsValueEntity += string.Format("rec.{0}, ", GetName(col.ColumnName, nameCase));
                            fkColumnsValueEntity += string.Format("rec.{0}, ", prop);

                            index++;
                        }

                        fkDeclaration = fkDeclaration.Substring(0, fkDeclaration.Length - 2);
                        fkDeclarationNullable = fkDeclarationNullable.Substring(0, fkDeclarationNullable.Length - 2);
                        fkColumnsWhere = fkColumnsWhere.Substring(0, fkColumnsWhere.Length - 5);
                        fkColumnsValue = fkColumnsValue.Substring(0, fkColumnsValue.Length - 2);
                        fkColumnsValueEntity = fkColumnsValueEntity.Substring(0, fkColumnsValueEntity.Length - 2);

                        //var col0 = fk.Columns[0];
                        //string fkTableName = provider.GetObjectName(col0.ReferencedTableSchema, col0.ReferencedTableName);
                        string fkEntityName = tables.Find(this, col0.ReferencedTableSchema, col0.ReferencedTableName).EntityName;

                        string signature = "public static [NameSpaceEntity].[ForeignEntity] Select_Parent_[ForeignEntity](Database db, [FkTypesNullable])"
                            .Replace("[NameSpaceEntity]", entityNameSpace)
                            .Replace("[ForeignEntity]", fkEntityName)
                            .Replace("[FkDeclaration]", fkTypes)
                            .Replace("[FkDeclarationNullable]", fkTypes);

                        int countMethod = parentMethods.Where(p => p == signature).Count();
                        string methodName = countMethod == 0 ? fkEntityName : fkEntityName + countMethod;
                        parentMethods.Add(signature);

                        selectByFk = selectByFk
                            .Replace("'", "\"")
                            .Replace("[NameSpaceEntity]", entityNameSpace)
                            .Replace("[NameSpaceBL]", businnesNameSpace)
                            .Replace("[MethodName]", methodName)
                            .Replace("[ForeignEntity]", fkEntityName)
                            .Replace("[ClassName]", className)
                            .Replace("[FkDeclaration]", fkDeclaration)
                            .Replace("[FkDeclarationNullable]", fkDeclarationNullable)
                            .Replace("[FkColumnsWhere]", fkColumnsWhere)
                            .Replace("[FkColumnsValue]", fkColumnsValue)
                            .Replace("[FkColumnsValueEntity]", fkColumnsValueEntity);

                        info.ParentRelations.Add(selectByFk);
                    }
                }
            }
            // replace ParentRelations
            if (info.ParentRelations.Count > 0)
            {
                info.BusinnesClass = info.BusinnesClass
                    .Replace("[ParentRelations]", "        // ParentRelations\r\n" + string.Join("\r\n", info.ParentRelations));
            }
            else
                info.BusinnesClass = info.BusinnesClass.Replace("[ParentRelations]", "");


            // ------ ChildrenRelations ------
            foreach (var pairFk in table.ChildrenRelations)
            {
                DbReferencialConstraintInfo fk = pairFk.Value;

                string fkDeclarationPars = "";
                string fkDeclarationParsNullable = "";
                string fkColumnsWhere = "";
                string fkColumnsValue = "";
                string className;
                string fkColumnsValueEntity = "";
                string fkTypes = "";
                string fkTypesNullable = "";

                if (table.TableName == "Customer")
                    ToString();

                string selectByFk = "";
                if (fk.Columns.Count > 0)
                {
                    selectByFk = Templates.BL_TEMPLATE_SELECT_BY_CHILD_RELATIONS;
                    int index = 0;

                    var col0 = fk.Columns[0];
                    GenTableResult infoRef = tables.Find(this, col0.TableSchema, col0.TableName);

                    if (infoRef != null)
                    {
                        DbTableInfo tableRef = infoRef.Table;
                        GenTableResult gtableResRef = tables.Find(this, tableRef.SchemaName, tableRef.TableName);
                        GenTable gtableRef = gtableResRef != null ? gtableResRef.GenTable : null;
                        var gcolsRef = gtableRef != null ? gtableRef.Columns.ToDictionary(p => p.ColumnName, StringComparer.InvariantCultureIgnoreCase) : new Dictionary<string, GenColumn>();

                        className = info.EntityName;

                        foreach (DbConstraintColumnInfo fkCol in fk.Columns)
                        {
                            DbColumnInfo colRef = tableRef.Columns[fkCol.ColumnName];
                            GenColumn gcolRef = gcolsRef != null && gcolsRef.ContainsKey(fkCol.ColumnName) ? gcolsRef[fkCol.ColumnName] : null;
                            string propRef = gcolRef != null && !string.IsNullOrWhiteSpace(gcolRef.PropertyName) ? gcolRef.PropertyName : GetName(colRef.ColumnName, nameCase);
                            string _propRef = "_" + propRef;

                            DbColumnInfo col = table.Columns[fkCol.ReferencedColumnName];
                            if (table.Columns.ContainsKey(fkCol.ReferencedColumnName))
                                col = table.Columns[fkCol.ReferencedColumnName];

                            // TODO: para o MySql não acha a linha acima (col = table.Columns[fkCol.ReferencedColumnName]). Checar
                            if (col == null)
                                goto Skip;

                            GenColumn gcol = gcols != null && gcols.ContainsKey(col.ColumnName) ? gcols[col.ColumnName] : null;
                            string prop = gcol != null && !string.IsNullOrWhiteSpace(gcol.PropertyName) ? gcol.PropertyName : GetName(colRef.ColumnName, nameCase);
                            string _prop = "_" + prop;

                            string type = colRef.DataTypeDotNet;
                            if (col.IsNullable && !colRef.IsNullable)
                                type += "?";

                            fkTypes += string.Format("{0}, ", type);
                            fkTypesNullable += string.Format("{0}, ", colRef.GetNullableDataTypeName());

                            fkDeclarationPars += string.Format("{0} {1}, ", type, _propRef);
                            fkDeclarationParsNullable += string.Format("{0} {1}, ", col.GetNullableDataTypeName(), _propRef);
                            fkColumnsWhere += string.Format("{0}={1} AND ", colRef.ColumnName, "{" + index + "}");
                            fkColumnsValue += string.Format("{0}, ", colRef.GetQuoteValue(_propRef));

                            //if (col == null && tableRef.Columns.ContainsKey(fkCol.ReferencedColumnName))
                            //    col = tableRef.Columns[fkCol.ReferencedColumnName];
                            //fkColumnsValueEntity += string.Format("rec.{0}, ", pasCalColumn);
                            fkColumnsValueEntity += string.Format("rec.{0}, ", prop); // GetName(col.ColumnName, nameCase));

                            index++;
                        }

                        fkDeclarationPars = fkDeclarationPars.Substring(0, fkDeclarationPars.Length - 2);
                        fkDeclarationParsNullable = fkDeclarationParsNullable.Substring(0, fkDeclarationParsNullable.Length - 2);
                        fkColumnsWhere = fkColumnsWhere.Substring(0, fkColumnsWhere.Length - 5);
                        fkColumnsValue = fkColumnsValue.Substring(0, fkColumnsValue.Length - 2);
                        fkColumnsValueEntity = fkColumnsValueEntity.Substring(0, fkColumnsValueEntity.Length - 2);

                        string fkEntityName = infoRef.EntityName;

                        string signature = "public static [NameSpaceEntity].[ForeignEntity] Select_Parent_[ForeignEntity](Database db, [FkTypesNullable])"
                            .Replace("[NameSpaceEntity]", entityNameSpace)
                            .Replace("[ForeignEntity]", fkEntityName)
                            .Replace("[FkTypes]", fkTypes)
                            .Replace("[FkTypesNullable]", fkTypesNullable);

                        if (sqlOrTableName.Contains("MST_MENU_EST"))
                            ToString();

                        int countMethod = childMethods.Where(p => p == signature).Count();
                        string methodName = countMethod == 0 ? fkEntityName : fkEntityName + countMethod;
                        childMethods.Add(signature);

                        string selectByFkPars = selectByFk
                            .Replace("'", "\"")
                            .Replace("[NameSpaceEntity]", entityNameSpace)
                            .Replace("[NameSpaceBL]", businnesNameSpace)
                            .Replace("[MethodName]", methodName)
                            .Replace("[ForeignEntity]", fkEntityName)
                            .Replace("[ClassName]", className)
                            .Replace("[FkDeclaration]", fkDeclarationPars)
                            .Replace("[FkDeclarationNullable]", fkDeclarationParsNullable)
                            .Replace("[FkColumnsWhere]", fkColumnsWhere)
                            .Replace("[FkColumnsValue]", fkColumnsValue)
                            .Replace("[FkColumnsValueEntity]", fkColumnsValueEntity);

                        info.ChildRelations.Add(selectByFkPars);
                    }
                }

            Skip:
                ToString();
            }

            // replace ChildRelations
            if (info.ChildRelations.Count > 0)
            {
                info.BusinnesClass = info.BusinnesClass
                    .Replace("[ChildRelations]", "        // ChildRelations\r\n" + string.Join("\r\n", info.ChildRelations));
            }
            else
                info.BusinnesClass = info.BusinnesClass.Replace("[ChildRelations]", "");
        }

        /*
        /// <summary>
        /// Gera classes de uma stored procedure
        /// </summary>
        /// <param name="nameSpaceProcedure"></param>
        /// <param name="className"></param>
        /// <param name="sql">Comando sql pra executar a procedure para inferir seu resultado</param>
        /// <returns></returns>
        public string GenerateRecordToProcedure(string entityNameSpace, string businnesNameSpace, string classNameBL, string entityName, string schemaName, string procedureName, string methodName, string returnTypeName, EnumReturnType returnType, EnumNameCase nameCase = EnumNameCase.Pascal)
        {
            //PAREI AQUI. DESCOMENTAR ESSE BLOCO
            var routine = Routines.Find(this, schemaName, procedureName);

            var bSerParameters = new StringBuilder();

#if DEBUG2
            var tb = GetSchema();
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (DataRow row in tb.Rows)
            {
                string collectionName = (string)row["CollectionName"];
                values.Add(collectionName, GetSchema(collectionName).GetText());
            }
#endif

            string fkDeclaration = "";
            string fkTypes = "";
            string className;
            string fkColumnsValueEntity = "";

            foreach (DbParameterInfo par in routine.Parameters)
            {
                string propName = CodeGenerator.GetParameterName(par.ParameterName.Replace(Provider.ParameterSymbol, ""));
                DbDataType dataType = Provider.DataTypes[par.DataType];

                string pasCalColumn = GetName(par.ParameterName, nameCase);
                string camelColumn = StringUtil.ToCamelCase(par.ParameterName);

                bool allowNull = true;
                string type = par.GetDataTypeDotNet(allowNull);

                fkTypes += string.Format("{0}, ", type);
                fkDeclaration += string.Format("{0} {1}, ", type, camelColumn);
                fkColumnsValueEntity += string.Format("rec.{0}, ", pasCalColumn);

                if (par.ParameterMode == ParameterDirection.Input | par.ParameterMode == ParameterDirection.InputOutput)
                {
                    string template;
                    if (par.CharacterMaximumLength == 0)
                        template = Templates.PROCEDURE_TEMPLATE_PARAMETER;
                    else
                        template = Templates.PROCEDURE_TEMPLATE_PARAMETER_SIZE;

                    bSerParameters.AppendFormat(template
                            .Replace("[ParameterName]", par.ParameterName)
                            .Replace("[DbType]", Provider.DbType.Name + "." + Provider.DbTypes[dataType.ProviderDbType])
                            .Replace("[ParameterDirection]", "ParameterDirection." + par.ParameterMode)
                            .Replace("[PropertyName]", propName)
                            .Replace("[Size]", par.CharacterMaximumLength.ToString()));
                }
            }

            string signature = "public static [NameSpaceEntity].[ForeignEntity] Select_Parent_[ForeignEntity](Database db, [FkTypesNullable])"
                .Replace("[NameSpaceEntity]", entityNameSpace)
                //.Replace("[ForeignEntity]", fkEntityName)
                .Replace("[FkDeclaration]", fkTypes);


            string text = Templates.PROCEDURE_TEMPLATE_COMMAND_LIST
                .Replace("[MethodName]", methodName)
                .Replace("[ProcedureName]", procedureName)
                .Replace("[ReturnTypeName]", returnTypeName)
                .Replace("[ProcedureParameters]", fkDeclaration)
                .Replace("[SetParameters]", bSerParameters.Replace("'", "\"").ToString())
                ;

            //string entityNameSpace, string businnesNameSpace, string classNameBL, string entityName, string schemaName, string procedureName, string methodName, 
            //string returnTypeName, EnumReturnType returnType          


            string tPars = bSerParameters.Replace("'", "\"").ToString();
            routine.ToString();


            return null;
            //DbTableInfo table = new DbTableInfo(this, schemaName, sql, isCommand);
            //Dictionary<string, string> types = DataTypes;

            //foreach (var pair in table.Columns)
            //{
            //    DbColumnInfo col = pair.Value;

            //    // b.Append(col.IsIdentity ? Templates.RECORD_COLUMN_TEMPLATE_IDENTITY : Templates.RECORD_COLUMN_TEMPLATE
            //    b.Append(Templates.RECORD_COLUMN_TEMPLATE
            //        .Replace("[ColumnName]", col.ColumnName)
            //        .Replace("[DataType]", col.GetDataTypeName()));
            //}

            //DataTable tbProcCols = ExecuteDataTable("sp_sproc_columns " + Provider.GetObjectName(procedureName));
            //string procParameters = "";
            //string setParameters = "";
            //foreach (DataRow row in tbProcCols.Rows)
            //{
            //    string columnName = row["COLUMN_NAME"].ToString();
            //    string typeName = row["TYPE_NAME"].ToString();
            //    string propertyName = columnName;
            //    procParameters += Provider.GetObjectName(columnName) + ", ";
            //    setParameters += string.Format("            cmd.Parameters.AddWithValue(\"{0}\", value.{1});\r\n",
            //        Provider.GetObjectName(columnName), propertyName);
            //}

            //string text =
            //    Templates.PROCEDURE_TEMPLATE_USING +
            //    Templates.PROCEDURE_TEMPLATE_RECORD +
            //    Templates.PROCEDURE_TEMPLATE_COMMAND;

            //text = text
            //    .Replace("[NameSpace]", nameSpaceProcedure)
            //    .Replace("[ProcedureName]", procedureName)
            //    .Replace("[ProcedureNameRecord]", procedureName + "_RECORD")
            //    .Replace("[Columns]", b.ToString()).Replace("'", "\"")
            //    .Replace("[SetParameters]", setParameters)
            //    .Replace("[ProcedureParameters]", procParameters);
            //return text;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pars"></param>
        /// <param name="writeToDisk"></param>
        /// <returns></returns>
        public Dictionary<string, GenTableResult> Generate(GenParameters pars, bool writeToDisk = true)
        {
            DictionarySchemaTable<GenTableResult> result = new DictionarySchemaTable<GenTableResult>();
            //GenTableCollection items = new GenTableCollection();
            //foreach (var item in alias)
            //{
            //    if (item.IsSelected)
            //    {
            //        item.DataClassName = item.DataClassName.Trim();
            //        items.Add(item);
            //    }
            //}

            var items = pars.Tables.Where(p => p.IsSelected).ToList();
            items.AddRange(pars.Views.Where(p => p.IsSelected));

            // primeira fase
            var list = new List<GenTable>(items);
            foreach (var item in list)
            {
                try
                {
                    var _result = GenerateRecord(result, pars.DataClass.NameSpace, pars.BusinessClass.NameSpace, item, false, pars.DataClass.NameCase, pars.RaisePropertyChanged);
                    result.Add(provider.GetObjectName(item.SchemaName, item.TableName), _result);
                }
                catch (Exception ex)
                {
                    items.Remove(item);
                }
            }

            // segunda fase
            foreach (var item in items)
            {
                GenTableResult _result = result[provider.GetObjectName(item.SchemaName, item.TableName)];
                GenerateRecord2(result, _result, pars.DataClass.NameSpace, pars.BusinessClass.NameSpace, pars.DataClass.NameCase);
                //GenerateRecord2(result, _result, pars.DataClass.NameSpace, pars.BusinessClass.NameSpace, item.BusinessClassName, item.DataClassName, item.SchemaName, item.TableName, false, item.EnumColumnName, pars.DataClass.NameCase);
            }

            if (writeToDisk)
            {
                Directory.CreateDirectory(pars.DataClass.Directory);
                Directory.CreateDirectory(pars.DataClass.DirectoryExt);
                Directory.CreateDirectory(pars.BusinessClass.Directory);
                Directory.CreateDirectory(pars.BusinessClass.DirectoryExt);

                //StringBuilder enums = new StringBuilder();

                foreach (var pair in result)
                {
                    var tb = pair.Value;

                    // Entity
                    tb.EntityFileName = Path.Combine(pars.DataClass.Directory, tb.EntityName + ".cs");
                    tb.EntityFileNameChanged = WriteFile(tb.EntityFileName, tb.EntityClass, true);

                    // Entity_Ext
                    tb.EntityFileNameExt = Path.Combine(pars.DataClass.DirectoryExt, tb.EntityName + "_Ext.cs");
                    tb.EntityFileNameExtChanged = WriteFile(tb.EntityFileNameExt, tb.EntityClassExt, false);

                    // Businnes
                    tb.BusinnesFileName = System.IO.Path.Combine(pars.BusinessClass.Directory, tb.BusinnesName + ".cs");
                    tb.BusinnesFileNameChanged = WriteFile(tb.BusinnesFileName, tb.BusinnesClass, true);

                    // Businnes_Ext
                    tb.BusinnesFileNameExt = System.IO.Path.Combine(pars.BusinessClass.DirectoryExt, tb.BusinnesName + "_Ext.cs");
                    tb.BusinnesFileNameExtChanged = WriteFile(tb.BusinnesFileNameExt, tb.BusinnesClassExt, false);

                    // Filter
                    //string filterFileName = Path.Combine(pars.DataClass.Directory, "Filters", tb.EntityName + "__Filter" + ".cs");
                    //WriteFile(filterFileName, tb.EntityFilterClass, true);

                    // Enum
                    if (!string.IsNullOrEmpty(tb.Enum))
                    {
                        string enumClass = Templates.GEN_WARNING + Templates.ENUM_CLASS_TEMPLATE
                            .Replace("[NameSpace]", pars.DataClass.NameSpace)
                            .Replace("[EnumCode]", tb.Enum);

                        tb.EnumFileName = Path.Combine(pars.DataClass.DirectoryExt, "Enums", tb.EnumName + ".cs");
                        tb.EnumFileNameChanged = WriteFile(tb.EnumFileName, enumClass, true);
                    }
                }
            }

            return result;
        }

        //public Dictionary<string, GenTableResult> Generate(string entityNameSpace, string businnesNameSpace, string entityDirectory, string businnesDirectory, GenTableCollection alias, EnumNameCase nameCase = EnumNameCase.Pascal)
        //{
        //    return Generate(entityNameSpace, businnesNameSpace, entityDirectory, entityDirectory, businnesDirectory, businnesDirectory, alias, false, nameCase);
        //}

        public Dictionary<string, GenTableResult> GenerateTables(string entityNameSpace, string businnesNameSpace, string entityDirectoryExt, string businnesDirectoryExt, bool includeSchema, EnumNameCase nameCase = EnumNameCase.Pascal)
        {
            return GenerateTables(entityNameSpace, businnesNameSpace, Path.Combine(entityDirectoryExt, "Base"), entityDirectoryExt, Path.Combine(businnesDirectoryExt, "Base"), businnesDirectoryExt, includeSchema, nameCase);
        }

        public Dictionary<string, GenTableResult> GenerateTables(string entityNameSpace, string businnesNameSpace, string entityDirectory, string entityDirectoryExt, string businnesDirectory, string businnesDirectoryExt, bool includeSchema, EnumNameCase nameCase = EnumNameCase.Pascal)
        {
            var pars = new GenParameters();
            var allTables = provider.GetAllTables();
            foreach (var table in allTables)
            {
                string tableName = includeSchema ? GetName(table.TableSchema + "_" + table.TableName, nameCase) : GetName(table.TableName, nameCase);
                var gtb = pars.Tables.Add(table.TableSchema, table.TableName, "BL_" + GetName(table.TableName, nameCase), GetName(tableName, nameCase), null);

                var tb = this.GetTableInfo(table.TableSchema, table.TableName);

                Dictionary<string, GenColumn> dict = new Dictionary<string, GenColumn>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var col in gtb.Columns)
                    if (!dict.ContainsKey(col.ColumnName))
                        dict.Add(col.ColumnName, col);

                foreach (var pair in tb.Columns)
                {
                    var col = pair.Value;
                    GenColumn gcol;
                    if (dict.TryGetValue(col.ColumnName, out gcol))
                    {
                        dict.Remove(col.ColumnName); // remove as existentes
                        // se existe, mas o banco é required, força required;
                        if (!col.IsNullable)
                            gcol.IsRequired = true;

                        // o tipo sempre vem do banco
                        gcol.DataType = col.DataTypeDotNet;
                    }
                    else
                    {
                        gcol = new GenColumn();
                        gcol.ColumnName = col.ColumnName;
                        gcol.PropertyName = null;
                        gcol.Description = null;
                        gcol.IsRequired = !col.IsNullable;
                        gcol.DataType = col.DataTypeDotNet;
                        gtb.Columns.Add(gcol);
                    }
                }
            }

            pars.DataClass.NameSpace = entityNameSpace;
            pars.BusinessClass.NameSpace = businnesNameSpace;
            pars.DataClass.Directory = entityDirectory;
            pars.DataClass.DirectoryExt = entityDirectoryExt;
            pars.BusinessClass.Directory = businnesDirectory;
            pars.BusinessClass.DirectoryExt = businnesDirectoryExt;

            pars.DataClass.StartWithSchema = pars.BusinessClass.StartWithSchema = includeSchema;
            pars.DataClass.NameCase = pars.BusinessClass.NameCase = nameCase;

            return Generate(pars);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityNameSpace"></param>
        /// <param name="businnesNameSpace"></param>
        /// <param name="entityDirectory"></param>
        /// <param name="entityDirectoryExt"></param>
        /// <param name="businnesDirectory"></param>
        /// <param name="businnesDirectoryExt"></param>
        /// <param name="alias"></param>
        /// <param name="tableSchema">Nome do Schema. Para o MySql é importante, senão retornarão todas as tabelas do servidor</param>
        /// <returns></returns>
        public Dictionary<string, GenTableResult> Generate(string entityNameSpace, string businnesNameSpace, string entityDirectory, string entityDirectoryExt, string businnesDirectory, string businnesDirectoryExt, GenTableCollection alias, bool includeSchema = false, EnumNameCase nameCase = EnumNameCase.Pascal)
        {
            var pars = new GenParameters();
            pars.Tables.AddRange(alias);
            pars.DataClass.NameSpace = entityNameSpace;
            pars.BusinessClass.NameSpace = businnesNameSpace;
            pars.DataClass.Directory = entityDirectory;
            pars.DataClass.DirectoryExt = entityDirectoryExt;
            pars.BusinessClass.Directory = businnesDirectory;
            pars.BusinessClass.DirectoryExt = businnesDirectoryExt;

            pars.DataClass.StartWithSchema = pars.BusinessClass.StartWithSchema = includeSchema;
            pars.DataClass.NameCase = pars.BusinessClass.NameCase = nameCase;
            return Generate(pars);
        }

        public Dictionary<string, GenTableResult> Generate(string entityNameSpace, string businnesNameSpace, GenTableCollection alias, EnumNameCase nameCase = EnumNameCase.Pascal)
        {
            var pars = new GenParameters();

            pars.DataClass.NameSpace = entityNameSpace;
            pars.BusinessClass.NameSpace = businnesNameSpace;
            pars.DataClass.StartWithSchema = pars.BusinessClass.StartWithSchema = false;
            pars.DataClass.NameCase = pars.BusinessClass.NameCase = nameCase;

            pars.Tables = alias;
            //// Tables+Views
            //var tables = pars.Tables;
            //if (pars.Views.Count > 0)
            //    tables.AddRange(tables);

            return Generate(pars);
        }

        EnumFileChanged WriteFile(string fileName, string code, bool overwriteIfExists)
        {
            EnumFileChanged changed = EnumFileChanged.Unmodified;
            string dir = System.IO.Path.GetDirectoryName(fileName);

            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.WriteAllText(fileName, code);
                changed = EnumFileChanged.New;
            }
            else if (overwriteIfExists)
            {
                string curCode = System.IO.File.ReadAllText(fileName);
                if (code != curCode)
                {
                    // trata ReadOnly
                    System.IO.File.SetAttributes(fileName, FileAttributes.Archive);
                    System.IO.File.WriteAllText(fileName, code);
                    changed = EnumFileChanged.Modified;
                }
            }
            return changed;
        }

        public Dictionary<string, GenVal> GenerateAllValidators(string entityNameSpace, GenTableCollection alias, string prefix, string sufix, string addNameSpaces = null)
        {
            prefix = Conv.Trim(prefix);
            sufix = Conv.Trim(sufix);
            Dictionary<string, GenVal> tables = new Dictionary<string, GenVal>();
            Dictionary<string, string> items = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var item in alias)
            {
                string tableName = item.TableName;
                string className = prefix + item.DataClassName + sufix;
                tables.Add(item.DataClassName, GenerateValidator(entityNameSpace, tableName, className, item.DataClassName, addNameSpaces));
            }
            return tables;
        }

        public Dictionary<string, GenVal> GenerateAllValidators(string entityNameSpace, string targetDirectory, GenTableCollection alias, string addNameSpaces = null)
        {
            string prefix = "";
            string sufix = "Validator";
            var tbs = GenerateAllValidators(entityNameSpace, alias, prefix, sufix, addNameSpaces);
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);
            foreach (var pair in tbs)
            {
                var gen = pair.Value;
                string className = prefix + pair.Key + sufix;
                string fileName = System.IO.Path.Combine(targetDirectory, className + ".cs");
                if (!File.Exists(fileName) || File.ReadAllText(fileName) != gen.Validator)
                    System.IO.File.WriteAllText(fileName, gen.Validator);
                fileName = System.IO.Path.Combine(targetDirectory, className + "_Ext.cs");
                if (!File.Exists(fileName))
                    System.IO.File.WriteAllText(System.IO.Path.Combine(targetDirectory, className + "_Ext.cs"), gen.ValidatorExt);
            }
            return tbs;
        }

        public GenVal GenerateValidator(string entityNameSpace, string tableName, string className, string typeName, string addNameSpaces = null)
        {
            DbTableInfo table = new DbTableInfo(this, tableName);
            return ValidatorGen.Generate(table, entityNameSpace, className, typeName, addNameSpaces);
        }

        #endregion Generate

        #region GenerateMetaData

        public Dictionary<string, string> GenerateAllMetadata(string nameSpace, string addNameSpaces)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            List<TableInfo> tables = provider.GetAllTables();
            foreach (var table in tables)
                list.Add(table.FullName, MetaDataGen.Generate(new DbTableInfo(this, table.TableSchema, table.TableName, false), nameSpace, table.TableName, table.TableName + "MetaData", addNameSpaces));
            return list;
        }

        public Dictionary<string, string> GenerateAllMetadata(string nameSpace, string addNameSpaces, string targetDirectory, bool onlyIfNotExist)
        {
            var tbs = GenerateAllMetadata(nameSpace, addNameSpaces);
            foreach (var pair in tbs)
            {
                var tb = pair.Value;
                if (!System.IO.Directory.Exists(targetDirectory))
                    System.IO.Directory.CreateDirectory(targetDirectory);
                string fileName = System.IO.Path.Combine(targetDirectory, pair.Key + ".cs");
                bool gen = onlyIfNotExist ? !System.IO.File.Exists(fileName) : true;
                if (gen)
                    System.IO.File.WriteAllText(fileName, tb);
            }
            return tbs;
        }

        public Dictionary<string, string> GenerateAllMetadata(Dictionary<string, string> alias, string nameSpace, string addNameSpaces, string targetDirectory, bool onlyIfNotExist)
        {
            Dictionary<string, string> tbs = new Dictionary<string, string>();
            foreach (var a in alias)
            {
                var table = a.Key;
                var className = a.Value;
                string gen = MetaDataGen.Generate(new DbTableInfo(this, table), nameSpace, className, className + "MetaData", addNameSpaces);
                tbs.Add(className, gen);
            }

            foreach (var pair in tbs)
            {
                var tb = pair.Value;
                if (!System.IO.Directory.Exists(targetDirectory))
                    System.IO.Directory.CreateDirectory(targetDirectory);
                string fileName = System.IO.Path.Combine(targetDirectory, pair.Key + ".cs");
                bool gen = onlyIfNotExist ? !System.IO.File.Exists(fileName) : true;
                if (gen)
                    System.IO.File.WriteAllText(System.IO.Path.Combine(targetDirectory, pair.Key + ".cs"), tb);
            }
            return tbs;
        }


        #endregion GenerateMetaData

        #region Util

        public string ToSqlDateTime(DateTime date)
        {
            return date.ToString("MM/dd/yyyy hh:mm:ss");
        }

        public string ToSqlDate(DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }

        public string ToSqlTime(DateTime date)
        {
            return date.ToString("hh:mm:ss");
        }

        public List<T> SelectExcludeColumns<T>(bool concurrency, params string[] columnsToExclude)
        {
            var data = cache.GetDataClass(this, typeof(T));
            return (List<T>)data.SelectColumns(this, concurrency, GetExcludeColumns<T>(columnsToExclude).ToArray());
        }

        public List<T> SelectExcludeColumns<T>(string where, bool concurrency, params string[] columnsToExclude)
        {
            var data = cache.GetDataClass(this, typeof(T));
            return (List<T>)data.SelectColumns(this, where, concurrency, GetExcludeColumns<T>(columnsToExclude).ToArray());
        }

        /// <summary>
        /// Retorna todas as colunas da table, menos as de columnsToExclude
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnsToExclude"></param>
        /// <returns></returns>
        public List<string> GetExcludeColumns<T>(params string[] columnsToExclude)
        {
            var data = cache.GetDataClass(this, typeof(T));
            List<string> exclude = new List<string>(from c in columnsToExclude select c.ToLower());
            List<string> list = new List<string>();
            foreach (var pair in data.ColumnInfos)
            {
                if (!exclude.Contains(pair.Key.ToLower()))
                    list.Add(pair.Key);
            }
            return list;
        }

        #endregion Util

        #region ExecuteArray

        /// <summary>
        /// Retorna, uma matriz bi-dimensional dos dados, no formato Linha, Coluna
        /// Por exemplo: ret[4][1] é a primeira coluna do registro 5.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public object[,] ExecuteArray(string sql)
        {
            DataTable tb = ExecuteDataTable(sql);

            if (tb.Rows.Count == 0 || tb.Columns.Count == 0)
                return null;

            int cols = tb.Columns.Count;
            int rows = tb.Rows.Count;
            object[,] o = new object[rows, cols];
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    o[row, col] = tb.Rows[row][col];
            return o;
        }

        /// <summary>
        /// Retorna, uma matriz unidimensional dos valores do primeiro registro.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public object[] ExecuteArray1D(string sql)
        {
            DataTable tb = ExecuteDataTable(sql);

            if (tb.Rows.Count == 0 || tb.Columns.Count == 0)
                return null;

            int cols = tb.Columns.Count;
            object[] o = new object[cols];
            for (int col = 0; col < cols; col++)
                o[col] = tb.Rows[0][col];
            return o;
        }

        /// <summary>
        /// Retorna uma matriz unidimensional contendo os valores da primeira coluna
        /// Por exemplo: ret[4] é a primeira coluna (a única) do registro 5.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T[] ExecuteArray1D<T>(string sql)
        {
            DataTable tb = ExecuteDataTable(sql);
            int rows = tb.Rows.Count;
            T[] o = new T[rows];
            for (int row = 0; row < rows; row++)
                o[row] = (T)tb.Rows[row][0];
            return o;
        }

        /// <summary>
        /// Retorna uma matriz unidimensional contendo os valores da coluna columnIndex
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T[] ExecuteArray1D<T>(string sql, int columnIndex)
        {
            DataTable tb = ExecuteDataTable(sql);
            int rows = tb.Rows.Count;
            T[] o = new T[rows];
            for (int row = 0; row < rows; row++)
                o[row] = (T)tb.Rows[row][columnIndex];
            return o;
        }

        public void Truncate(string tableName)
        {
            string sql = "truncate table " + Provider.GetObjectName(tableName);
            ExecuteNonQuery(sql);
        }

        public Database Clone()
        {
            Database db = null;
            if (connectionStringBuilder != null)
                db = new Database(providerType, connectionStringBuilder.ToString());
            else
                db = new Database(providerType, connectionString);
            return db;
        }

        #endregion ExecuteArray

        #region ExecuteSequence

        public int ExecuteSequenceInt32(string sequenceName)
        {
            return provider.ExecuteSequenceInt32(sequenceName);
        }

        public long ExecuteSequenceInt64(string sequenceName)
        {
            return provider.ExecuteSequenceInt64(sequenceName);
        }

        #endregion ExecuteSequence

        /*
        void checkDb(ref string database)
        {
            if (string.IsNullOrEmpty(database) && !string.IsNullOrEmpty(this.databaseName))
                database = this.databaseName;
        }
        */

        public void RegisterAssemblyAndCompile<T>()
        {
            cache.RegisterAssemblyAndCompile(this, typeof(T).Assembly, false);
        }

        public static DbConnectionStringBuilder BuildConnectionString(EnumDbProviderType providerType, string server, string database, string userId, string password, bool integratedSecurity = false, int port = 0, bool embedded = false, string provider = null)
        {
            var _provider = ProviderFactory.CreateProvider(new Database(), providerType);
            return _provider.BuildConnectionString(providerType, server, database, userId, password, integratedSecurity, port, embedded, provider);
        }

        /// <summary>
        /// Método genérico de GetAllTables. Não foi testado em todos os providers
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="tableType"></param>
        /// <param name="useDatabase">Se usa o DatabaseName no lugar do Schema (caso do MySql)</param>
        /// <returns></returns>
        internal List<TableInfo> GetAllTables(string tableSchema = null, EnumTableType? tableType = null, bool useDatabase = false)
        {
            tableSchema = Conv.Unquote(tableSchema);

            string sql;
            if (string.IsNullOrEmpty(tableSchema) && tableType == null)
                sql = "select * from information_schema.tables order by table_name;";
            else if (!string.IsNullOrEmpty(tableSchema) && tableType == null)
                sql = "select * from information_schema.tables where table_schema = '" + tableSchema + "' order by table_name;";
            else if (string.IsNullOrEmpty(tableSchema) && tableType != null)
                sql = "select * from information_schema.tables where table_type = '" + (tableType.Value == EnumTableType.Table ? "BASE TABLE" : "") + "' order by table_name;";
            else // if (!string.IsNullOrEmpty(tableSchema) && tableType != null)
                sql = "select * from information_schema.tables where table_schema = '" + tableSchema + "' and table_type = '" + (tableType.Value == EnumTableType.Table ? "BASE TABLE" : "") + "' order by table_name;";

            List<TableInfo> tables = new List<TableInfo>();
            using (var dr = ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    TableInfo tb = new TableInfo();
                    tb.TableSchema = useDatabase ? databaseName : Conv.ToString(dr["TABLE_SCHEMA"]);
                    tb.TableCatalog = Conv.ToString(dr["TABLE_CATALOG"]);
                    tb.TableName = (string)dr["TABLE_NAME"];
                    tb.TableType = (string)dr["TABLE_TYPE"] == "BASE TABLE" ? EnumTableType.Table : EnumTableType.View;
                    tables.Add(tb);
                }
            }
            return tables;
        }

        /// <summary>
        /// Método de retorna ReservedWords usando GetSchema
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetReservedWordsGeneric()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            try
            {
                DataTable tb = cn.GetSchema("ReservedWords");
                foreach (DataRow row in tb.Rows)
                    ret.Add((string)row[0], null);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return ret;
        }

        /*
        /// <summary>
        /// Retorna o table baseado na chave
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private EntityInfo getTable(Dictionary<string, EntityInfo> tables, string schemaName, string tableName)
        {
            var table = tables.GetValue(schemaName + "_" + tableName);
            if (table == null)
                table = tables.GetValue(tableName);
            return table;
        }
        */

        public IDbDataParameter[] ConvertParameters(params Parameter[] parameters)
        {
            IDbDataParameter[] pars = new IDbDataParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                pars[i] = provider.Convert(parameters[i]);
            return pars;
        }

        public static string GetName(string name, EnumNameCase nameCase)
        {
            name = name.RemoveChars(@". ,;:$#[]/\");

            string ret = null;
            switch (nameCase)
            {
                case EnumNameCase.None:
                    ret = name;
                    break;

                case EnumNameCase.Camel:
                    ret = StringUtil.ToCamelCase(name);
                    break;

                case EnumNameCase.Pascal:
                    ret = StringUtil.ToPascalCase(name);
                    break;

                case EnumNameCase.Lower:
                    ret = name.ToLower();
                    break;

                case EnumNameCase.Upper:
                    ret = name.ToUpper();
                    break;

            }
            return ret;
        }

        /// <summary>
        /// Return all metada information, in text format
        /// </summary>
        /// <returns></returns>
        public string GetAllMetadata()
        {
            var tb = GetSchema();
            StringBuilder b = new StringBuilder();
            foreach (DataRow row in tb.Rows)
            {
                string collectionName = (string)row["CollectionName"];

                if (collectionName != "JavaClasses") // timeout
                {
                    b.AppendLine(collectionName);
                    b.AppendLine(GetSchema(collectionName).GetText());
                    b.AppendLine(new string('=', 80));
                    b.AppendLine();
                }
            }

            return b.ToString();
        }

        /// <summary>
        /// Validate field based on table properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        public void Validate<T>(T record)
        {
            getCache<T>().Validate(record);
        }

        public override string ToString()
        {
            return connectionString;
        }
    }

    #region DbSqlInfoMessage

    /*
    public class DbSqlInfoMessage : EventArgs
    {

        private List<DbSqlError> errors;

        private string message;
        private string source;
        private string toString;

        public DbSqlInfoMessage(MySql.Data.MySqlClient.MySqlInfoMessageEventArgs ev)
        {
            this.message = null;
            this.source = null;
            errors = new List<DbSqlError>();
            for (int i = 0; i < ev.errors.Length; i++)
            {
                MySqlError er = ev.errors[i];
                DbSqlError er2 = new DbSqlError();
                er2.Number = er.Code;
                er2.Level = er.Level;
                er2.Message = er.Message;
                er2.toString = er.ToString();
                errors.Add(er2);
            }
        }

        public DbSqlInfoMessage(SqlInfoMessageEventArgs ev)
        {
            this.message = ev.Message;
            this.source = ev.Source;
            errors = new List<DbSqlError>();
            for (int i = 0; i < ev.Errors.Count; i++)
            {
                var er = ev.Errors[i];
                DbSqlError er2 = new DbSqlError();
                er2.Class = er.Class;
                er2.LineNumber = er.LineNumber;
                er2.Message = er.Message;
                er2.Number = er.Number;
                er2.Procedure = er.Procedure;
                er2.Server = er.Server;
                er2.Source = er.Source;
                er2.State = er.State;
                er2.toString = er.ToString();
                errors.Add(er2);
            }
        }

        public DbSqlInfoMessage(System.Data.OleDb.OleDbInfoMessageEventArgs ev)
        {
            this.message = null;
            this.source = null;
            errors = new List<DbSqlError>();
            for (int i = 0; i < ev.Errors.Count; i++)
            {
                OleDbError er = ev.Errors[i];
                DbSqlError er2 = new DbSqlError();
                //er2.Number = er.Code;
                //er2.Level = er.Level;
                er2.Message = er.Message;
                er2.toString = er.ToString();
                errors.Add(er2);
            }
        }

        public DbSqlInfoMessage(NpgsqlNoticeEventArgs ev)
        {
            this.message = null;
            this.source = null;
            errors = new List<DbSqlError>();
            var er = ev.Notice;
            DbSqlError er2 = new DbSqlError();
            er2.Number = Conv.ToInt32(er.Code);
            er2.Level = er.Severity;
            er2.Message = er.Message;
            er2.toString = er.ToString();
            errors.Add(er2);
        }

        public List<DbSqlError> Errors
        {
            get { return errors; }
        }

        public string Message
        {
            get { return message; }
        }

        public string Source
        {
            get { return source; }
        }

        public override string ToString()
        {
            return toString;
        }

    }
    */

    #endregion DbSqlInfoMessage

    #region DbSqlError

    public class DbSqlError
    {

        public byte Class;
        public int LineNumber;
        public string Level;
        public string Message;
        public int Number;
        public string Procedure;
        public string Server;
        public string Source;
        public byte State;

        internal string toString;

        public override string ToString()
        {
            return toString;
        }

    }

    #endregion DbSqlError

    #region DictionarySchemaTable

    public class DictionarySchemaTable<TValue> : Dictionary<string, TValue>
    {

        public DictionarySchemaTable()
            : base()
        {
        }

        public DictionarySchemaTable(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        public TValue Find(Database db, string schemaName, string tableName)
        {
            TValue ret;

            schemaName = Conv.Unquote(schemaName);
            tableName = Conv.Unquote(tableName);

            string key = db.Provider.GetObjectName(schemaName, tableName, false);
            if (!TryGetValue(key, out ret))
            {
                if (!TryGetValue(tableName, out ret))
                {
                    var list = (from c in this where c.Key.ToUpper().EndsWith("." + db.Provider.GetObjectName(tableName).ToUpper()) select c.Value).ToList();
                    ret = list.FirstOrDefault();
                }

            }
            return ret;
        }

    }

    #endregion DictionarySchemaTable

}
