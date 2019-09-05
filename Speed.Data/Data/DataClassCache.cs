using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Speed.Data.MetaData;
using Speed.Data.Generation;
using Speed.Common;
using Speed.IO;

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

        public void RegisterAssemblyAndCompile(Database db, Assembly assembly, Type type = null)
        {
            // TODO:
            //if (!string.IsNullOrWhiteSpace(Sys.DllCheck))
            //{
            //    string check = Path.Combine(Sys.AppDirectory, Sys.DllCheck);
            //    if (File.Exists(check))
            //    {

            //        Sys.CheckFiles()
            //}
            // se passar tipo nulo, ou se for DbTable, compila tudo
            if (type == null || type.GetCustomAttributes(typeof(DbTableAttribute), false) != null)
                RegisterAssemblyAndCompile(db, assembly, false);
            else
                RegisterAndCompileType(db, type, false);
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
                GenerateDataClasses(db, types, false);
            }
            //throw new Exception(tc.ToString());
        }

        public void RegisterAndCompileType(Database db, Type type, bool refresh)
        {
            List<Type> types = new List<Type>();

            //TimerCount tc = new TimerCount("RegisterAssemblyAndCompile");

            // não registra duas vezes
            lock (refTypes)
            {
                if (!refTypes.ContainsKey(type.Name))
                    refTypes.Add(type.Name, type);
                types.Add(type);
            }
            assTypes.Add(type.FullName.ToLower(), types);

            GenerateDataClasses(db, types, true);
            //throw new Exception(tc.ToString());
        }

        public DataClass GetDataClass(Database db, Type type)
        {
            if (cache.ContainsKey(type))
            {
                return cache[type].CloneT();
            }
            else
            {
                RegisterAssemblyAndCompile(db, type.Assembly, type);
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

        private void GenerateDataClasses(Database db, List<Type> types, bool singleClass = false)
        {
            if (types.Count == 0)
                return;

            string lastModified = db.Provider.GetLastModified();

            string dir = GetDirectory(types, db, lastModified);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fileCode = Path.Combine(dir, "Speed.Code.cs");
            string fileDll = Path.Combine(dir, "Speed.Compiled.dll");

            if (File.Exists(fileDll))
            {
                try
                {
                    var assCache = Assembly.LoadFile(fileDll);

                    var typesDataClass = assCache.GetExportedTypes();

                    foreach (var type in types)
                    {
                        addToCache(type, (DataClass)assCache.CreateInstance("DataClass" + type.Name));
                    }

                    return;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

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
            b.AppendLine(DataClassTemplate.DATACLASSTEMPLATE_USING);
            foreach (var pair in otherUsings)
                b.AppendLine(pair.Key);
            codes.ForEach(p => b.AppendLine(p));

#if DEBUG
            string code = b.ToString();
#else
            string code = string.Join("\r\n", b.ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(line => !string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("//")).Select(line => line.Trim()));
#endif

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
            //if (Assembly.GetEntryAssembly() != null)
            //{
            tc.Next("Compile");

            bool hasCache = false;

            if (hasCache)
            {
                try
                {
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

                        ass = CodeGenerator.Compile(db, types[0], code, "All Classes", fileDll, otherUsings);
                        File.WriteAllText(fileCode, code);
                    }
                }
                catch
                {
                }
            }

            if (ass == null)
            {
                // compila na memória
                if (!singleClass)
                    ass = CodeGenerator.Compile(db, types[0], code, "All Classes", fileDll, otherUsings);
                else
                    ass = CodeGenerator.Compile(db, types[0], code, types[0].FullName, null, otherUsings);
            }

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

                    if (objType == null)
                        throw new Exception("Not found in the database: " + type.Name + " - " + info.TableName);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="db"></param>
        /// <param name="lastModified">Última data de table ou view aterado na base</param>
        /// <returns></returns>
        string GetDirectory(List<Type> types, Database db, string lastModified)
        {
            string nspace = types.First().Namespace;
            string ntypes = string.Join(",", types.OrderBy(p => p.FullName).Select(p => p.FullName));

            string hashName = FileTools.ToValidPath(Cryptography.Hash(
                    lastModified + "-"
                    + ntypes
                    + types[0].Assembly.Location
                    + db.ProviderType
                    + db.Connection.ConnectionString)
                .Replace("=", "").Replace("/", "").Replace("\\", ""));

            string dirBase = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string dir = Path.Combine( dirBase, "Speed", db.ProviderType.ToString(), nspace, hashName);
            return dir;
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
