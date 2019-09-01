using System;
using System.Collections.Generic;
using System.IO;


namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class ConfigReader
    {

        Dictionary<string, string> keys;

        public ConfigReader(string path)
        {
            keys = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            using (var f = new StreamReader(path))
            {
                while (f.Peek() >= 0)
                {
                    string line = f.ReadLine().Trim();
                    if (line.Length > 0)
                    {
                        if (!line.StartsWith("//"))
                        {
                            int sep = line.IndexOf('=');
                            if (sep > 0 && sep < line.Length)
                            {
                                var key = line.Substring(0, sep).Trim();
                                var value = line.Substring(sep + 1, line.Length - (sep + 1)).Trim();
                                if (key.Length > 0 && value.Length > 0)
                                    keys.Add(key, value);
                            }
                        }
                    }
                }
            }
        }

        public string this[string key]
        {
            get { return keys[key]; }
        }

    }

}
