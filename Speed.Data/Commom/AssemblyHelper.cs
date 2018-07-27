#if NETSTANDARD2_0

using Speed.Data;
using Speed.Data.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;

namespace Speed.Common
{

    public class AssemblyHelper
    {

        public static Assembly LoadAssembly(string assName)
        {
            var loader = new AssemblyLoader();
            var path = Path.Combine(Sys.AppDirectory, assName);
            if (!File.Exists(path))
                throw new FileNotFoundException("Error loading assembly " + assName, path);
            //return loader.LoadFromAssemblyPath(path);
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        }

        public static IDbProvider GetProvider(string assName, Database db)
        {
            //try
            //{
                var asm = LoadAssembly(assName);
                var x = asm.GetTypes();

                var types = asm.GetExportedTypes();
                Type type = types.FirstOrDefault(p => p.GetTypeInfo().IsClass && typeof(IDbProvider).IsAssignableFrom(p));
                return (IDbProvider)Activator.CreateInstance(type, db);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Error loading assembly: " + assName, ex);
            //}
        }

        public static List<MetadataReference> CollectReferences()
        {
            // first, collect all assemblies
            var assemblies = new HashSet<Assembly>();

            Collect(Assembly.Load(new AssemblyName("netstandard")));

            //// add extra assemblies which are not part of netstandard.dll, for example:
            //Collect(typeof(Uri).Assembly);

            // second, build metadata references for these assemblies
            var result = new List<MetadataReference>(assemblies.Count);
            foreach (var assembly in assemblies)
            {
                result.Add(MetadataReference.CreateFromFile(assembly.Location));
            }

            return result;

            // helper local function - add assembly and its referenced assemblies
            void Collect(Assembly assembly)
            {
                if (!assemblies.Add(assembly))
                {
                    // already added
                    return;
                }

                var referencedAssemblyNames = assembly.GetReferencedAssemblies();

                foreach (var assemblyName in referencedAssemblyNames)
                {
                    var loadedAssembly = Assembly.Load(assemblyName);
                    assemblies.Add(loadedAssembly);
                }
            }
        }

    }

    public sealed class AssemblyLoader : AssemblyLoadContext
    {
        //protected override Assembly Load(AssemblyName assemblyName)
        //{
        //    return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
        //    //throw new Exception("assemblyName = " + assemblyName);
        //    //return this.GetType().Assembly;
        //    //throw new NotImplementedException();
        //}

        //public new Assembly LoadFromStream(Stream assembly, Stream assemblySymbols)
        //{
        //    return base.LoadFromStream(assembly, assemblySymbols);
        //}

        //public new Assembly LoadFromAssemblyPath(string assemblyPath)
        //{
        //    return base.LoadFromAssemblyPath(assemblyPath);
        //}

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
        }
    }

}
#else

using Speed.Data;
using Speed.Data.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Speed.Common
{

    public class AssemblyHelper
    {

        public static Assembly LoadAssembly(string assName)
        {
            var path = Path.Combine(Sys.AppDirectory, assName);
            if (!File.Exists(path))
                throw new FileNotFoundException("Error loading assembly " + assName, path);
            return Assembly.LoadFile(path);
        }

        public static IDbProvider GetProvider(string assName, Database db)
        {
            //try
            //{
                var asm = LoadAssembly(assName);
                var x = asm.GetTypes();

                var types = asm.GetExportedTypes();
                Type type = types.FirstOrDefault(p => p.GetType().IsClass && typeof(IDbProvider).IsAssignableFrom(p));
                return (IDbProvider)Activator.CreateInstance(type, db);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Error loading assembly: " + assName, ex);
            //}
        }

    }
}

#endif
