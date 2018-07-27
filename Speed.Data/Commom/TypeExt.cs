using System;
using System.Collections.Generic;
using System.Reflection;

namespace Speed.Common
{

    public static class TypeExt
    {


        public static Assembly GetAssembly(this Type type)
        {
#if NETCOREAPP2_0
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        public static string GetAssemblyPath(this Type type)
        {
            return GetAssembly(type).Location;
        }

    }
}
