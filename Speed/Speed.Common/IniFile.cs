using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Speed
{
	/// <summary>
	/// Summary description for FileConfig.
	/// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class IniFile
	{
		private Dictionary<string, string> col;
		
		private string fileName;
		private string separator;

		public IniFile(string fileName)
		{
			ReadFile(fileName, "=");
		}

		public IniFile(string fileName, string separator)
		{
			ReadFile(fileName, separator);
		}

		private void ReadFile(string fileName, string separator)
		{
			this.fileName = fileName;
			this.separator = separator;
            
            col = new Dictionary<string, string>();
            
            try
			{
                string dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!File.Exists(fileName))
                {
                    File.CreateText(fileName);
                    return;
                }

                // lê o arquivo de configuração e coloca os valores em col.
                using (StreamReader r = File.OpenText(fileName))
                {
                    string line = "";
                    do
                    {
                        line = r.ReadLine();
                        if (line != null && line.Trim().Length > 0)
                        {
                            line = line.Trim();
                            // só trata linhas que possuem separator
                            int pos = line.IndexOf(separator);
                            if (pos > 0)
                            {
                                string key, value = "";
                                key = line.Substring(0, pos).Trim();
                                value = line.Substring(line.IndexOf(separator) + separator.Length).Trim();
                                col.Add(key, value);
                            }
                        }
                    } while (line != null);
                }
			}
			catch
			{
			}
		}

		public string this[string key]
		{
			get
			{
                try
                {
                    return col[key];
                }
                catch
                {
                    return null;
                }
			}
			set
			{
				col[key] = value;
			}
		}

		public void Save()
		{
			string[] keys = new string[col.Count];
			col.Keys.CopyTo(keys, 0 );

            using (StreamWriter w = File.CreateText(fileName))
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    string key = keys[i];
                    w.WriteLine(key + " " + separator + " " + col[key].ToString());
                }
            }
		}
	}
}
