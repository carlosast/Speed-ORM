using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class Log
    {

        #region Declarations

        public static string LogPath;
        public static bool IsErrorEnabled { get; set; }
        public static bool IsWarningEnabled { get; set; }
        public static bool IsInfoEnabled { get; set; }
        public static bool IsFatalEnabled { get; set; }
        static string line = new string('=', 80);

        #endregion Declarations

        #region Constuctors

        static Log()
        {
            LogPath = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, ".log");
#if DEBUG
            IsErrorEnabled = true;
            IsFatalEnabled = true;
            IsWarningEnabled = true;
            IsInfoEnabled = true;
#else
            IsErrorEnabled = true;
            IsFatalEnabled = true;
            IsWarningEnabled = true;
            IsInfoEnabled = true;
#endif
        }

        #endregion Constuctors

        #region Debug

        /// <summary>
        /// Só funciona em modo DEBUG
        /// </summary>
        /// <param name="message"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Debug(string message)
        {
#if DEBUG
            WriteLine(LogType.Debug, message);
#endif
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Debug(string message, params string[] args)
        {
#if DEBUG
            WriteLine(LogType.Debug, message, args);
#endif
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        /// Só funciona em modo DEBUG
        public static void Debug(Exception exception, string message)
        {
#if DEBUG
            WriteLine(LogType.Debug, exception, message);
#endif
        }

        #endregion Debug

        #region Info

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Info(string message)
        {
            if (IsInfoEnabled && message != null)
                WriteLine(LogType.Info, message);
        }

        public static void Info(string message, params string[] args)
        {
            if (IsInfoEnabled && message != null)
                WriteLine(LogType.Info, message, args);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Info(Exception exception, string message)
        {
            if (IsInfoEnabled && message != null)
                WriteLine(LogType.Info, exception, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Info(Exception exception, string message, params string[] args)
        {
            if (IsInfoEnabled && message != null)
                WriteLine(LogType.Info, exception, message, args);
        }


        #endregion Info

        #region Erro

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error(string message)
        {
            if (IsErrorEnabled && message != null)
                WriteLine(LogType.Error, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error(string message, params string[] args)
        {
            if (IsErrorEnabled && message != null)
                WriteLine(LogType.Error, message, args);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error(Exception exception, string message)
        {
            if (IsErrorEnabled && message != null)
                WriteLine(LogType.Error, exception, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error(Exception exception, string message, params string[] args)
        {
            if (IsErrorEnabled && message != null)
                WriteLine(LogType.Error, exception, message, args);
        }

        #endregion Erro

        #region Warning

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Warning(string message)
        {
            if (IsWarningEnabled && message != null)
                WriteLine(LogType.Warning, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Warning(string message, params string[] args)
        {
            if (IsWarningEnabled && message != null)
                WriteLine(LogType.Warning, message, args);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Warning(Exception exception, string message)
        {
            if (IsWarningEnabled && message != null)
                WriteLine(LogType.Warning, exception, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Warning(Exception exception, string message, params string[] args)
        {
            if (IsWarningEnabled && message != null)
                WriteLine(LogType.Warning, exception, message, args);
        }

        #endregion Warning

        #region Fatal

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Fatal(string message)
        {
            if (IsFatalEnabled && message != null)
                WriteLine(LogType.Fatal, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Fatal(string message, params string[] args)
        {
            if (IsFatalEnabled && message != null)
                WriteLine(LogType.Fatal, message, args);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Fatal(Exception exception, string message)
        {
            if (IsFatalEnabled && message != null)
                WriteLine(LogType.Fatal, exception, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Fatal(Exception exception, string message, params string[] args)
        {
            if (IsFatalEnabled && message != null)
                WriteLine(LogType.Fatal, exception, message, args);
        }

        #endregion Fatal

        #region Methods

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Clear()
        {
            try
            {
                using (StreamWriter sw = GetStream(false))
                {
                }
            }
            catch { }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteSeparator()
        {
            try
            {
                using (StreamWriter sw = GetStream(true))
                    sw.WriteLine(line);
            }
            catch { }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static StreamWriter GetStream(bool append)
        {
            return new StreamWriter(LogPath, append, System.Text.Encoding.UTF8);
        }

        public static string Now
        {
            get
            {
                DateTime d = DateTime.Now;
                return d.ToString("dd/MM/yy") + " " + d.ToString("HH:mm:ss");
            }
        }

        #endregion Methods

        #region WriteLine

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteLine(LogType type, string message)
        {
            try
            {
                using (StreamWriter sw = GetStream(true))
                    sw.WriteLine(type.ToString().ToUpper() + ":: " + Now + " - " + message);
            }
            catch { }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteLine(LogType type, string message, params string[] args)
        {
            WriteLine(type, string.Format(message, args));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteLine(LogType type, Exception exception, string message)
        {
            try
            {
                using (StreamWriter sw = GetStream(true))
                    sw.WriteLine(type.ToString().ToUpper() + ":: " + Now + " - " + message + " --> " + exception.Message + "\r\n\r\n" + exception.StackTrace);
            }
            catch { }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void WriteLine(LogType type, Exception exception, string message, params string[] args)
        {
            WriteLine(type, exception, message, args);
        }

        #endregion WriteLine


        public static bool IsExceptionEnabled { get; set; }
    }

    public enum LogType
    {
        Error = 1,
        Warning = 2,
        Confirm = 3,
        Info = 4,
        Debug = 5,
        Fatal = 6
    }

}
