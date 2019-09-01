using System;
using System.Collections.Generic;
using System.Reflection;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class FindKnowTypes
    {

        static Dictionary<Type, List<Type>> cache;

        static FindKnowTypes()
        {
            cache = new Dictionary<Type, List<Type>>();
        }

        public static List<Type> Find(Type type)
        {
            if (!cache.ContainsKey(type))
            {
                var types = new List<Type>();
                Find(types, type);
                cache.Add(type, types);
                return types;
            }
            else
                return cache[type];
        }

        private static void Find(List<Type> types, Type type)
        {
            // if (!types.Contains(type) && !type.Namespace.StartsWith("System"))
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (FieldInfo field in type.GetFields())
                    Find(types, field.FieldType);
                foreach (PropertyInfo prop in type.GetProperties())
                    Find(types, prop.PropertyType);
            }
        }

    }

}

