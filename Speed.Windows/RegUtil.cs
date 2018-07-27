using System;
using Microsoft.Win32;

namespace Speed.Windows
{

    /// <summary>
    /// Classe com algumas funções úteis de registro.
    /// Trata tb diferenças em 32 e 64 bits, escrevendo sempre no registro correto.
    /// Se o OS é 32 bits, escreve no 32 bits. Se é 64 bits, escreve no 64 bits
    /// </summary>
    public class RegUtil : IDisposable
    {

        private string Register;
        RegistryKey reg;
        RegistryHive hive;
        private System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");

        ///// <summary>
        ///// Inicia um RegUtil, usando registerName = ProgramBase.AppName e source = EnumSource.LocalMachine
        ///// </summary>
        //public RegUtil()
        //    : this(EnumSource.LocalMachine, ProgramBase.AppName)
        //{
        //}

        public RegUtil(EnumSource source, string registerName)
        {
            Register = "Software\\" + registerName + "\\";
            if (source == EnumSource.CurrentUser)
            {
                reg = Registry.CurrentUser;
                hive = RegistryHive.CurrentUser;
            }
            else if (source == EnumSource.Users)
            {
                reg = Registry.Users;
                hive = RegistryHive.Users;
            }
            else
            {
                reg = Registry.LocalMachine;
                hive = RegistryHive.LocalMachine;
            }
        }

        public void Check()
        {
            // cria o root
            CreateRoot();

            // cria o id
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();

            if (!Date.HasValue)
                Date = DateTime.Now;

            // inicia com 0
            if (!IsOk)
                Ok = 0;
        }

        public void Dispose()
        {
            if (reg != null)
            {
                // reg.Dispose();
                reg = null;
            }
        }

        public void CreateRoot()
        {
            var skey = OpenSubKey(Register, true);
            if (skey == null)
            {
                skey = reg.CreateSubKey(Register);
                skey.Close();
                reg.Flush();
            }
        }

        public void CreateSubKey(string key)
        {
            var skey = OpenSubKey(Register, true);
            if (skey != null)
            {
                var skey2 = OpenSubKey(key, true);
                if (skey2 == null)
                {
                    skey2 = skey.CreateSubKey(key);
                }
                skey2.Close();
                skey.Close();
            }
            reg.Flush();
        }

        public void SetRootValue(string name, object value)
        {
            var skey = OpenSubKey(Register, true);
            if (skey != null)
            {
                skey.SetValue(name, value);
                skey.Close();
            }
        }

        public object GetRootValue(string name, object defaultValue = null)
        {
            //reg.DeleteSubKeyTree(DocTotal_REG, false);
            var skey = OpenSubKey(Register, true);
            object value = null;
            if (skey != null)
            {
                value = skey.GetValue(name, defaultValue);
                skey.Close();
            }
            return value;
        }

        public void SetSubKeyValue(string key, string name, object value)
        {
            var skey = OpenSubKey(Register + key, true);
            if (skey != null)
            {
                skey.SetValue(name, value);
                skey.Close();
            }
        }

        public Guid Id
        {
            get
            {
                string value = (string)GetRootValue("Id", null);
                return value != null ? new Guid(value) : Guid.Empty;
            }
            set { SetRootValue("Id", value); }
        }

        public DateTime? Date
        {
            get
            {
                string value = (string)GetRootValue("Date", null);
                return value != null ? DateTime.ParseExact(value, "MM/dd/yyyy hh:mm:ss", culture) : (DateTime?)null;
            }
            set { SetRootValue("Date", value.Value.ToString("MM/dd/yyyy hh:mm:ss")); }
        }

        public string Server
        {
            get { return (string)GetRootValue("Server", null); }
            set { SetRootValue("Server", value); }
        }

        public string Database
        {
            get { return (string)GetRootValue("Database", null) ?? ""; }
            set { SetRootValue("Database", value); }
        }

        public string UserId
        {
            get { return (string)GetRootValue("UserId", null) ?? ""; }
            set { SetRootValue("UserId", value); }
        }

        public string Password
        {
            get
            {
                string pass = (string)GetRootValue("Password", null) ?? "";
                if (!string.IsNullOrWhiteSpace(pass))
                    pass = Cryptography.Decrypt(pass);
                return pass;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    value = Cryptography.Encrypt(value);
                SetRootValue("Password", value);
            }
        }

        public int Port
        {
            get { return Conv.ToInt32(GetRootValue("Port", null)); }
            set { SetRootValue("Port", value); }
        }

        public int Ok
        {
            get { return Conv.ToInt32(GetRootValue("Ok", null)); }
            set { SetRootValue("Ok", value); }
        }

        public bool IsOk
        {
            get { return Ok != 0; }
        }

        public RegistryKey OpenSubKey(string name, bool writable)
        {
            RegistryKey registryKey;
            if (!ProgramBase.UseRegistry32 && Environment.Is64BitOperatingSystem == true)
            {
                registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            }
            else
            {
                registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32);
            }
            return registryKey.OpenSubKey(name, writable);
        }

    }

    public enum EnumSource
    {
        CurrentUser,
        LocalMachine,
        /// <summary>
        /// Default user
        /// </summary>
        Users
    }

}
