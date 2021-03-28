using System;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Speed.Common;
using System.Diagnostics;

namespace Speed.Data
{

    public class Sys
    {

        //[DllImport("kernel32.dll", EntryPoint="LoadLibraryW", SetLastError = true)]
        //internal static extern IntPtr LoadLibraryW(string lpszLib); 

        public static string ConnectionString;
        public static EnumDbProviderType ProviderType = EnumDbProviderType.SqlServer;
        public static int CommandTimeout = 30;
        public static string AppDirectory;

        static Sys()
        {
            var ass = Assembly.GetEntryAssembly() ?? (Assembly.GetCallingAssembly() ?? Assembly.GetExecutingAssembly());
            if (ass.Location == null)
            {
                AppDirectory = Directory.GetCurrentDirectory();
            }
            else
            {
                AppDirectory = Path.GetDirectoryName(ass.Location);
            }
        }

        public static Database NewDb()
        {
            Database db = new Database(ProviderType, ConnectionString, CommandTimeout);
            db.Open();
            return db;
        }

        public static Database NewDb(int commandTimeout)
        {
            Database db = new Database(ProviderType, ConnectionString, commandTimeout);
            db.Open();
            return db;
        }

        public static Database NewDb(EnumDbProviderType providerType, string connectionString)
        {
            Database db = new Database(providerType, connectionString, CommandTimeout);
            db.Open();
            return db;
        }

        public static Database NewDb(ConnectionInfo cnInfo)
        {
            Database db = new Database(cnInfo);
            db.Open();
            return db;
        }

        public static bool RunSafe(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static T RunSafe<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static void RunInThread(bool isBackground, Action action)
        {
            (new Task(action)).Start();
        }

        public static bool RunSafeInDb(Action<Database> action)
        {
            bool ret = false;
            try
            {
                using (var db = NewDb())
                    action(db);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public static void RunInDb(Action<Database> action)
        {
            using (var db = NewDb())
                action(db);
        }

        /// <summary>
        /// Executa uma action, passando uma instância de Database
        /// </summary>
        /// <param name="action"></param>
        public static T RunInDb<T>(Func<Database, T> action)
        {
            using (var db = NewDb())
                return action(db);
        }

        #region Transaction

        /// <summary>
        /// Método útil pra executar um comando dentro de uma transaction, usando uma base de dados já criada
        /// A transaction é iniciada em RunInTran. Se sucesso, fará um Commit. Se não, Roolback
        /// </summary>
        /// <param name="db"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool RunInTran(Database db, Action action)
        {
            try
            {
                db.BeginTransaction();
                action();
                db.Commit();
                return true;
            }
            catch (Exception ex)
            {
                db.Rollback();
                return false;
            }
        }

        public static bool RunSafeInTran(Action<Database> action)
        {
            bool ret = false;
            try
            {
                using (var db = NewDb())
                {
                    db.BeginTransaction();
                    action(db);
                    db.Commit();
                }
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public static void RunInTran(Action<Database> action)
        {
            using (var db = NewDb())
            {
                db.BeginTransaction();
                action(db);
                db.Commit();
            }
        }

        #endregion Transaction

        public static void CheckFiles(params string[] files)
        {
            //checkDll("MySql.Data.dll");
            foreach (var file in files)
            {
                bool copy = checkFile(file);
                if (!copy)
                    copy.ToString();
            }
        }

        private static bool checkFile(string fileName)
        {
            bool copy = false;
            try
            {
                FileInfo filex = new FileInfo(GetLocalFileX(fileName));
                FileInfo file = new FileInfo(GetLocalFile(fileName));
                if (filex.Exists)
                {
                    if (!file.Exists)
                    {
                        filex.CopyTo(file.FullName, true);
                        file.Refresh();
                        Speed.IO.FileTools.SetDatesAndAttributes(filex.FullName, file.FullName);
                        copy = true;
                    }
                    if (filex.Length != file.Length || filex.LastWriteTime != file.LastWriteTime)
                    {
                        file.IsReadOnly = false;
                        file.Refresh();
                        filex.CopyTo(file.FullName, true);
                        Speed.IO.FileTools.SetDatesAndAttributes(filex.FullName, file.FullName);
                        copy = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return copy;
        }

        /// <summary>
        /// Returns the full path of a file located in the directory of the executable
        /// </summary>
        /// <param name="relativeFileName"></param>
        /// <returns></returns>
        public static string GetLocalFile(string relativeFileName)
        {
            return Path.Combine(AppDirectory, relativeFileName);
        }

        /// <summary>
        /// Returns the full path of a file located in a subdirectory x86 or x64 in the directory of the executable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetLocalFileX(string relativeFileName)
        {
            return Path.Combine(Path.Combine(AppDirectory, Is64Bits ? "x64" : "x86", relativeFileName));
        }

        public static bool Is64Bits
        {
            get { return IntPtr.Size * 8 == 64; }
        }

        public static void ToConsole(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ") + message);
        }

        public static void ToConsole(string message, params object[] args)
        {
            ToConsole(string.Format(message, args));
        }

        public static void ToConsole(Exception ex)
        {
            ToConsole(Conv.GetErrorMessage(ex, true));
        }
        public static void ToConsole(Exception ex, string message)
        {
            ToConsole(message + "\r\n" + Conv.GetErrorMessage(ex, true));
        }

        public static void Trace(string message)
        {
#if DEBUG && NET40
            try
            {
                string source = "Speed";
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, source);
                }

                EventLog.WriteEntry(source, message, EventLogEntryType.Information);
            }
            catch { }
#endif
        }
        public static void Trace(Exception ex, string message = null)
        {
            Trace(message + " " + ex.Message + " " + Conv.GetErrorMessage(ex, true));
        }

    }

}
