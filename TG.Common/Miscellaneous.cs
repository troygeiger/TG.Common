using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TG.Common
{
    /// <summary>
    /// Contains various miscellaneous functions and methods.
    /// </summary>
    public static class Miscellaneous
    {
        /// <summary>
        /// Clones an object by copying property values.
        /// </summary>
        /// <typeparam name="T">The type of object that is being cloned.</typeparam>
        /// <param name="obj">The <see cref="object"/> to clone.</param>
        /// <returns>A new instance of T with properties copied from obj.</returns>
        public static T CloneObject<T>(T obj)
        {
            if (obj == null) return default(T);
            Type otype = typeof(T);
            Type dicType = typeof(IDictionary);
            Type lstType = typeof(IList);
            object result = Activator.CreateInstance(otype);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(otype))
            {
                if (!property.IsReadOnly)
                {
                    property.SetValue(result, property.GetValue(obj));
                }
                else if (dicType.IsAssignableFrom(property.PropertyType))
                {
                    IDictionary src = property.GetValue(obj) as IDictionary;
                    IDictionary dest = property.GetValue(result) as IDictionary;
                    foreach (DictionaryEntry item in src)
                    {
                        dest.Add(item.Key, item.Value);
                    }
                }
                else if (lstType.IsAssignableFrom(property.PropertyType))
                {
                    IList src = property.GetValue(obj) as IList;
                    IList dest = property.GetValue(result) as IList;
                    foreach (object item in src)
                    {
                        dest.Add(item);
                    }
                }
            }
            return (T)result;
        }
    }
}
