using System;
using System.Collections.Generic;
using System.Reflection;

namespace Speed.Common
{

    /// <summary>
    /// Métodos úties de reflexão
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class ReflectionUtil
    {

        /// <summary>
        /// Clona Fields e Properties de um objeto e retorna uma cópia de objSource
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static T Clone<T>(object objSource)
        {
            return CopyProperties<T>(objSource);
        }

        /// <summary>
        /// Copia Fields e Properties comuns entre objSource e objTarget
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget"></param>
        public static void Clone(object objSource, object objTarget)
        {
            CopyProperties(objSource, objTarget);
        }

        /// <summary>
        /// Copia todos os valores das propriedades de objSource para objTarget
        /// É um "Clone" simplificado e genérico
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="objTarget"></param>
        public static void CopyProperties(object objSource, object objTarget)
        {
            IList<FieldInfo> fields = GetFields(objSource);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    SetField(objTarget, field.Name, GetField(objSource, field.Name));
                }
                catch { }
            }

            IList<PropertyInfo> props = GetProperties(objSource);
            foreach (PropertyInfo prop in props)
            {
                try
                {
                    SetProperty(objTarget, prop.Name, GetProperty(objSource, prop.Name));
                }
                catch { }
            }
        }

        public static object CopyProperties(object objSource)
        {
            object objTarget = Activator.CreateInstance(objSource.GetType());
            CopyProperties(objSource, objTarget);
            return objTarget;
        }

        public static T CopyProperties<T>(object objSource)
        {
            object objTarget = Activator.CreateInstance<T>();
            CopyProperties(objSource, objTarget);
            return (T)objTarget;
        }

        public static IList<FieldInfo> GetFields(Object obj)
        {
            return obj.GetType().GetFields();
        }

        public static IList<PropertyInfo> GetProperties(Object obj)
        {
            return obj.GetType().GetProperties();
        }

        public static void SetField(Object obj, string propertyName, object value)
        {
            FieldInfo field = obj.GetType().GetField(propertyName);
            if (field != null)
                field.SetValue(obj, value);
        }

        public static object GetField(Object obj, string propertyName)
        {
            FieldInfo field = obj.GetType().GetField(propertyName);
            if (field != null)
                return field.GetValue(obj);
            else
                return null;
        }

        public static void SetProperty(Object obj, string propertyName, object value)
        {
            PropertyInfo Property = obj.GetType().GetProperty(propertyName);
            if (Property != null)
                Property.SetValue(obj, value, null);
        }

        public static object GetProperty(Object obj, string propertyName)
        {
            PropertyInfo Property = obj.GetType().GetProperty(propertyName);
            if (Property != null)
                return Property.GetValue(obj, null);
            else
                return null;
        }

    }

}
