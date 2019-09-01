﻿using Speed.Data;
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
            return Assembly.LoadFrom(path);

            //var loader = new AssemblyLoader();
            //return loader.LoadFromAssemblyPath(path);
            //return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName { Name = assName });
        }

        public static IDbProvider GetProvider(string assName, Database db)
        {
            try
            {
                var asm = LoadAssembly(assName);
                Type type = asm.GetExportedTypes().FirstOrDefault(p => TypeExt.IsClass(p) && typeof(IDbProvider).IsAssignableFrom(p));
                return (IDbProvider)Activator.CreateInstance(type, db);
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading assembly: " + assName, ex);
            }
        }

    }

    //public sealed class AssemblyLoader : AssemblyLoadContext
    //{
    //    protected override Assembly Load(AssemblyName assemblyName)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public new Assembly LoadFromStream(Stream assembly, Stream assemblySymbols)
    //    {
    //        return base.LoadFromStream(assembly, assemblySymbols);
    //    }

    //    public new Assembly LoadFromAssemblyPath(string assemblyPath)
    //    {
    //        return base.LoadFromAssemblyPath(assemblyPath);
    //    }

    //}

}