using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Speed.Data.MetaData;
using Speed.Data.Generation;

namespace Speed.Data
{

    /// <summary>
    /// Classe de armazenamento em memória das classes geradas e compiladas dinamicamente,
    /// para otimizar a execução de código, pois Reflection é muito lento
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    internal class DataClassCache
    {

        private Dictionary<Type, DataClass> cache;
        private Dictionary<string, DataClass> cacheByName;
        /// <summary>
        /// Tipos em cache pra reflexão apenas.
        /// </summary>
        private static List<string> assRegistered = new List<string>();
        private static Dictionary<string, Type> refTypes = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
        private static Dictionary<string, List<Type>> assTypes = new Dictionary<string, List<Type>>();
        internal static Dictionary<string, MethodInfo> refMethods = new Dictionary<string, MethodInfo>();

        public DataClassCache()
        {
            cache = new Dictionary<Type, DataClass>();
            cacheByName = new Dictionary<string, DataClass>();
            assTypes = new Dictionary<string, List<Type>>();
        }

        /*
        public void Clear()
        {
            lock (cache)
            {
                cache.Clear();
                cacheByName.Clear();
            }
        }
        */

        /// <summary>
        /// Reset all compiled class
        /// </summary>
        public void Clear()
        {
            cache.Clear();
            cacheByName.Clear();
        }
        public DataClass GetDataClass(Database db, string typeName)
        {
            if (cacheByName.ContainsKey(typeName))
                return cacheByName[typeName];
            else
                return GetDataClass(db, getInstance(typeName).GetType());
        }

        private object getInstance(string typeName)
        {
            return refTypes[typeName].Assembly.CreateInstance(refTypes[typeName].FullName);
        }

        public void RegisterAssemblyAndCompile(Database db, Assembly assembly)
        {
            RegisterAssemblyAndCompile(db, assembly, false);
        }

        public void RegisterAssemblyAndCompile(Database db, Assembly assembly, bool refresh)
        {
            List<Type> types = new List<Type>();

            string name = assembly.FullName.ToLower();

            //TimerCount tc = new TimerCount("RegisterAssemblyAndCompile");

            // não registra duas vezes
            lock (assRegistered)
            {
                if (assRegistered.Contains(name))
                {
                    if (!refresh)
                        return;
                    else
                    {
                        assRegistered.Remove(name);
                        assTypes.Remove(name);
                        //types = assTypes[assembly];
                        //GenerateAllDataClass(db, types);
                        //return;
                    }
                }

                assRegistered.Add(name);
                List<Type> tps = assembly.GetExportedTypes().Where(p => p.BaseType == typeof(Record)).ToList();
                types = new List<Type>();
                lock (refTypes)
                {
                    foreach (Type tp in tps)
                    {
                        if (!refTypes.ContainsKey(tp.Name))
                            refTypes.Add(tp.Name, tp);
                        types.Add(tp);
                    }
                }
                assTypes.Add(name, types);
#if DEBUG2
            records = records.OrderBy(p => p.Name).ToList();
#endif
                types = types.OrderBy(p => p.Name).ToList();
                GenerateAllDataClass(db, types);
            }
            //throw new Exception(tc.ToString());
        }

        public DataClass GetDataClass(Database db, Type type)
        {
            if (cache.ContainsKey(type))
            {
                return cache[type];
                //var dcType = cache[type].GetType();
                //return (DataClass)dcType.Assembly.CreateInstance(dcType.FullName);
            }
            else
            {
                RegisterAssemblyAndCompile(db, type.Assembly);
                return cache[type];
            }
        }


        internal class dataInfo
        {
            public DbTableInfo Table;
            public Dictionary<string, DbColumnInfo> Infos;
            public string ClassName;
            public string TableName;
        }

        private void GenerateAllDataClass(Database db, List<Type> types)
        {
            TimerCount tc = new TimerCount("TestSelect");

            Dictionary<Type, dataInfo> dataInfos = new Dictionary<Type, dataInfo>();
            // other usings 
            Dictionary<string, string> otherUsings = new Dictionary<string, string>();

            List<string> codes = new List<string>();

            int i = 0;
            foreach (var type in types)
            {
                DbTableInfo table;
                var infos = new Dictionary<string, DbColumnInfo>();
                string className;
                string tableName;
                if (type.Name == "Parameter")
                    type.ToString();
                string text = CodeGenerator.GenerateDataClassCode(db, type, out className, out tableName,
                    out table, out infos, otherUsings);
                codes.Add(text);

                dataInfo di = new dataInfo { Table = table, Infos = infos, TableName = tableName, ClassName = className };
                dataInfos.Add(type, di);
                i++;
            }

            StringBuilder b = new StringBuilder();
            b.AppendLine(Templates.DATACLASSTEMPLATE_USING);
            foreach (var pair in otherUsings)
                b.AppendLine(pair.Key);
            codes.ForEach(p => b.AppendLine(p));
            string code = b.ToString();

            Assembly ass = null;

#if DEBUG2
            //Descomentar somente se for necessário. Impacta muito a performance
            try
            {
                if (!System.IO.Directory.Exists(@"..\..\DataClass"))
                    System.IO.Directory.CreateDirectory(@"..\..\DataClass");
                using (System.IO.StreamWriter w = new System.IO.StreamWriter(@"..\..\DataClass\AllClasses.cs", false))
                    w.Write(code);
            }
            catch { }
#endif

            // para aplicações web, Assembly.GetEntryAssembly() sempre retorna null
            if (Assembly.GetEntryAssembly() != null)
            {
                tc.Next("Compile");

                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Speed",
                    Speed.IO.FileTools.ToValidPath((Speed.Cryptography.Hash(Assembly.GetEntryAssembly().Location + db.ProviderType + db.Connection.ConnectionString).Replace("=", "")
                        .Replace("/", "")
                        .Replace("\\", ""))));

                // gera 1 diretório para o provider atual
                dir = Path.Combine(dir, db.ProviderType.ToString());

                string fileCode = Path.Combine(dir, "Speed.Code.cs");
                string fileDll = Path.Combine(dir, "Speed.Compiled.dll");
                bool hasCache = false;

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                else
                {
                    if (File.Exists(fileCode) && File.Exists(fileDll))
                    {
                        if (File.ReadAllText(fileCode) == code)
                        {
                            try
                            {
                                // ass = Assembly.Load(File.ReadAllBytes(fileDll));
                                ass = Assembly.LoadFile(fileDll);
                                hasCache = true;
                            }
                            catch (Exception ex)
                            {
                                ass = null;
                                ex.ToString();
                            }
                        }
                    }

                }
                if (ass == null)
                {
                    if (File.Exists(fileDll))
                        File.Delete(fileDll);

                    ass = CodeGenerator.Compile(types[0], code, "All Classes", fileDll, otherUsings);
                    File.WriteAllText(fileCode, code);
                }
            }
            else
                // compila na memória
                ass = CodeGenerator.Compile(types[0], code, "All Classes", null, otherUsings);

#if DEBUG2
            var exTypes = ass.GetExportedTypes().Where(p => p.Name.StartsWith("DataClass")).ToList();
#endif


            tc.Next("Depois");
            lock (cache)
            {
                foreach (var pair in dataInfos)
                {
                    Type type = pair.Key;
                    dataInfo info = pair.Value;
                    if (cache.ContainsKey(type))
                        cache.Remove(type);

                    Type objType = ass.GetType(info.ClassName);
                    DataClass dc = (DataClass)Activator.CreateInstance(objType);
                    if (dc == null)
                        return;
                    dc.TableInfo = info.Table;
                    dc.ColumnInfos = info.Infos;
                    addToCache(type, dc);
                }
            }
            string time = tc.ToString();
        }

        void addToCache(Type type, DataClass dc)
        {
            lock (cache)
            {
                if (!cache.ContainsKey(type))
                    cache.Add(type, dc);
                if (!cacheByName.ContainsKey(type.Name))
                    cacheByName.Add(type.Name, dc);
            }
        }


        public bool Contains(string typeName)
        {
            return cacheByName.ContainsKey(typeName);
        }
    }
}
