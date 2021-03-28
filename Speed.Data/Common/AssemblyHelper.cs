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
            Assembly asm = null;

            var path = Path.Combine(Sys.AppDirectory, assName);
            try
            {
                return Assembly.LoadFile(path);
            }
            catch
            {
                asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name.Equals(assName, StringComparison.OrdinalIgnoreCase));
                if (asm != null)
                    return asm;

                try
                {
                    asm = Assembly.Load(Path.Combine(Directory.GetCurrentDirectory(), assName));
                }
                catch
                {
                    try
                    {
                        asm = Assembly.Load(assName);
                    }
                    catch { }
                }


                if (asm == null)
                {
                    try
                    {
                        asm = Assembly.Load(Path.GetFileNameWithoutExtension(assName));
                    }
                    catch { }
                }

            }

            if (asm == null)
                throw new Exception("Assembly not found: " + assName);

            return asm;

            //var loader = new AssemblyLoader();
            //return loader.LoadFromAssemblyPath(path);
            //return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName { Name = assName });
        }

        public static IDbProvider GetProvider(string assName, Database db)
        {
            try
            {
                var asm = LoadAssembly(assName);

                //Type type = asm.GetExportedTypes().FirstOrDefault(p => TypeExt.IsClass(p) && typeof(IDbProvider).IsAssignableFrom(p));
                Type type = null;
                Exception exg = null;

                foreach (Type _type in asm.GetExportedTypes())
                {
                    try
                    {
                        if (TypeExt.IsClass(_type) && typeof(IDbProvider).IsAssignableFrom(_type))
                        {
                            type = _type;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        exg = ex;
                    }
                }

                if (type != null)
                {
                    IDbProvider prov = null;
                    try
                    {
                        return (IDbProvider)Activator.CreateInstance(type, db);
                    }
                    catch (Exception ex)
                    {
                        exg = new Exception("Erro on create " + type.Name);
                    }
                }
                if (exg != null)
                    throw new Exception("Erro on loading assembly " + assName, exg);
                else
                    throw new Exception("Erro on loading assembly " + assName);

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
