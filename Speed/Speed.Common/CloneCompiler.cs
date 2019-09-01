using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Speed
{

    public interface IMyClone<T>
    {
        T Clone(T value);
    }

    /// <summary>
    /// Gera dinamicamente um proxy para fazer o clone de uma classe
    /// É milhares de vezes mais rápido que reflection
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class CloneCompiler
    {

        // Cache de assemblies compilados (static) .Não estou usando singleton aqui apenas
        // para facilitar o entendimento
        public static Dictionary<Type, object> cacheClone = new Dictionary<Type, object>();

        // o que está entre [] será substituído em run-time
        const string cloneTemplate =
@"using System;
using FastClone;

namespace FastClone
{
    class [ClassName] : Speed.IMyClone<[T]>
    {
        public [T] Clone([T] value)
        {
            [T] newValue = Activator.CreateInstance<[T]>();
[Set]
            return newValue;
        }
    }
}
";
        // método que retorna uma classe gerada dinamicamente para retornar o clone das propriedades de outra classe
        public static IMyClone<T> GetCloneClass<T>()
        {
            Type type = typeof(T);
            IMyClone<T> classeClone = CompileCloneClass<T>();

            // loca
            lock (cacheClone)
            {
                if (!cacheClone.ContainsKey(type))
                    cacheClone.Add(type, classeClone);
            }
            return classeClone;
        }

        public static IMyClone<T> CompileCloneClass<T>()
        {
            Type type = typeof(T);
            if (cacheClone.ContainsKey(type))
            {
                return (IMyClone<T>)cacheClone[type];
            }
            else
            {
                StringBuilder set = new StringBuilder();
                // Fields
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField))
                    set.AppendFormat("\t\t\tnewValue.{0} = value.{0};\r\n", field.Name);
                // Properties
                //foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty))
                foreach (PropertyInfo prop in type.GetProperties())
                    set.AppendFormat("\t\t\tnewValue.{0} = value.{0};\r\n", prop.Name);

                string className = "ClasseClone" + type.Name;
                // gera o código da classe
                string classCode = cloneTemplate
                    .Replace(@"[ClassName]", className)
                    .Replace("[T]", type.Namespace + "." + type.Name)
                    .Replace("[Set]", set.ToString());

                //compila
                CompilerParameters cp = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true, IncludeDebugInformation = false };
                CodeDomProvider provider = new CSharpCodeProvider();
                
                //Dictionary<string, string> providerOptions = new Dictionary<string,string>();
                //providerOptions.Add("CompilerVersion", "v3.5");
                //CodeDomProvider provider = new CSharpCodeProvider(providerOptions);

                cp.ReferencedAssemblies.Add(type.Assembly.Location);

                var x = type.Assembly.CreateInstance(type.FullName);
                var mods = type.Assembly.GetLoadedModules();


                foreach (AssemblyName asm in type.Assembly.GetReferencedAssemblies())
                    cp.ReferencedAssemblies.Add(asm.Name + ".dll");

                if (!cp.ReferencedAssemblies.Contains("System.dll"))
                    cp.ReferencedAssemblies.Add("System.dll");

                if (!cp.ReferencedAssemblies.Contains("System.Core.dll"))
                    cp.ReferencedAssemblies.Add("System.Core.dll");

                if (!cp.ReferencedAssemblies.Contains("System.Data.dll"))
                    cp.ReferencedAssemblies.Add("System.Data.dll");

                if (!cp.ReferencedAssemblies.Contains("System.Xml.dll"))
                    cp.ReferencedAssemblies.Add("System.Xml.dll");

                if (!cp.ReferencedAssemblies.Contains("System.Xml.Linq.dll"))
                    cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");

#if DEBUG
                StringBuilder debug = new StringBuilder();
                foreach (string asm in cp.ReferencedAssemblies)
                    debug.AppendLine(asm);
                string s = debug.ToString();
                s.ToString();
#endif


                //cp.ReferencedAssemblies.Add("FastClone.exe");
                CompilerResults cr = provider.CompileAssemblyFromSource(cp, classCode);
                if (cr.Errors.Count > 0)
                {
                    StringBuilder b = new StringBuilder();
                    b.AppendLine("Compiler Errors");
                    foreach (CompilerError error in cr.Errors)
                    {
                        b.AppendLine(error.ErrorText);
                        b.AppendLine(new string('-', 80));
                    }
                    string err = b.ToString();
                    throw new Exception(err);
                }

                Type objType = cr.CompiledAssembly.GetType("FastClone." + className);
                IMyClone<T> classeClone = (IMyClone<T>)Activator.CreateInstance(objType);
                return classeClone;
            }
        }


    }

}
