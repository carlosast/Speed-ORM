using System;
using System.Collections.Generic;
using System.Reflection;

namespace Speed.Common
{

    internal static class TypeExt
    {


        public static Assembly GetAssembly(this Type type)
        {
#if NETCOREAPP20
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        public static bool IsClass(Type type)
        {
#if NETCOREAPP20
            return p.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        public static string GetAssemblyPath(this Type type)
        {
            return GetAssembly(type).Location;
        }

        public static T[] GetCustomAttributesEx<T>(this Type attributeType, bool inherit) where T : Attribute
        {
            var list = new List<T>();
            Type type = attributeType;
#if NETCOREAPP2_0
            foreach (var item in type.GetCustomAttributes(inherit))
                if (item is T)
                    list.Add((T)item);
            //return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
#else
            foreach (var item in type.GetCustomAttributes(inherit))
                if (item is T)
                    list.Add((T)item);
#endif
            return list.ToArray();
        }

        public static bool IsInstanceOfTypeEx(this Type type, object o)
        {
#if NETCOREAPP2_0
            return type.GetTypeInfo().IsInstanceOfType(o);
#else
            return type.IsInstanceOfType(o);
#endif
        }

        public static void LoadAssemblies<T>(List<string> assemblies)
        {
            LoadAssemblies(typeof(T), assemblies);
        }

        public static void LoadAssemblies(Type type, List<string> assemblies)
        {
            var asms = type.Assembly.GetReferencedAssemblies();
            foreach (var asm in asms)
            {
                var ass = Assembly.Load(asm);
                if (!assemblies.Contains(ass.Location))
                    assemblies.Add(ass.Location);
            }
        }

    }
}
