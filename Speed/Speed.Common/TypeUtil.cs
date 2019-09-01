using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Speed
{

    public static class TypeUtil
    {

        public static void SetInfo(Type type, string property, string displayName, string description)
        {
            SetDisplayName(type, property, displayName);
            SetDescription(type, property, description);
        }

        public static PropertyDescriptor SetDisplayName(Type type, string property, string value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(type)[property];
            FieldInfo field_info = typeof(MemberDescriptor).GetField("displayName", BindingFlags.NonPublic | BindingFlags.Instance);
            field_info.SetValue(descriptor, value);
            return descriptor;
        }

        public static PropertyDescriptor SetDescription(Type type, string property, string value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(type)[property];
            FieldInfo field_info = typeof(MemberDescriptor).GetField("description", BindingFlags.NonPublic | BindingFlags.Instance);
            field_info.SetValue(descriptor, value);
            return descriptor;
        }

        public static PropertyDescriptor SetCategory(Type type, string property, string value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(type)[property];
            FieldInfo field_info = typeof(MemberDescriptor).GetField("category", BindingFlags.NonPublic | BindingFlags.Instance);
            field_info.SetValue(descriptor, value);
            return descriptor;
        }

        public static string GetDisplayName(Type type, string property)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(type)[property];
            DisplayNameAttribute atr = (descriptor != null) ? (DisplayNameAttribute)descriptor.Attributes[typeof(DisplayNameAttribute)] : null;
            return atr != null
                ? (!string.IsNullOrEmpty(atr.DisplayName) ? atr.DisplayName : property)
                : null;
        }

        public static string GetDescription(Type type, string property)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(type)[property];
            DescriptionAttribute atr = (descriptor != null) ? (DescriptionAttribute)descriptor.Attributes[typeof(DescriptionAttribute)] : null;
            return atr != null ? atr.Description : null;
        }

        public static string GetCategory(Type type, string property)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(type)[property];
            CategoryAttribute atr = (descriptor != null) ? (CategoryAttribute)descriptor.Attributes[typeof(CategoryAttribute)] : null;
            return atr != null
                ? (!string.IsNullOrEmpty(atr.Category) ? atr.Category : property)
                : null;
        }

        /// <summary>
        /// Hides a property by making it not browsable 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="propName"></param>
        public static void HideProperty(System.Collections.IDictionary properties, string propName)
        {
            properties[propName] = HideProperty(properties[propName] as PropertyDescriptor);
        }

        /// <summary> 
        /// hides a property by making it not browsable 
        /// </summary> 
        public static PropertyDescriptor HideProperty(PropertyDescriptor property)
        {
            if (property != null)
                property = ChangeAttribute<BrowsableAttribute>(property, BrowsableAttribute.No);
            return property;
        }

        public static PropertyDescriptor UnhideProperty(PropertyDescriptor property)
        {
            if (property != null)
                property = ChangeAttribute<BrowsableAttribute>(property, BrowsableAttribute.Yes);
            return property;
        }

        public static PropertyDescriptor ChangeAttribute<TAttribute>(PropertyDescriptor property, Attribute newValue) where TAttribute : Attribute
        {
            PropertyDescriptor oldProp = property;
            if (oldProp != null)
            {
                List<Attribute> copiedAtts = CopyAttributes(oldProp, typeof(TAttribute));
                copiedAtts.Add(newValue);
                //copy to an array 
                Attribute[] arr = new Attribute[copiedAtts.Count];
                copiedAtts.CopyTo(arr);
                //create the new property 
                PropertyDescriptor newProp = TypeDescriptor.CreateProperty(oldProp.ComponentType, oldProp, arr);
                property = newProp;
            }
            return property;
        }


        /// <summary> 
        /// unhides a property by making it browsable 
        /// </summary> 
        public static void UnhideProperty(System.Collections.IDictionary properties, string propName)
        {
            properties[propName] = UnhideProperty(properties[propName] as PropertyDescriptor);
        }

        public static List<Attribute> CopyAttributes(PropertyDescriptor property, params Type[] exclusions)
        {
            List<Attribute> ret = new List<Attribute>();
            List<Type> exclusionList = new List<Type>(exclusions);
            foreach (Attribute att in property.Attributes)
            {
                if (!exclusionList.Contains(att.GetType()))
                {
                    ret.Add(att);
                }
            }
            return ret;
        }



    }

}
