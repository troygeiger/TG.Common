using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace TG.Common
{
    /// <summary>
    /// Contains various miscellaneous functions and methods.
    /// </summary>
    public static class Miscellaneous
    {

        static Dictionary<Type, bool> constructorCache = new Dictionary<Type, bool>();


        private static bool HasParameterlessConstructor(Type type)
        {
            if (constructorCache.ContainsKey(type))
            {
                return constructorCache[type];
            }
            bool flag = false;

            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                if (constructor.GetParameters().Length == 0)
                {
                    flag = true;
                    break;
                }
            }
            constructorCache.Add(type, flag);
            return flag;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// Clones an object by copying property values.
        /// </summary>
        /// <typeparam name="T">The type of object that is being cloned.</typeparam>
        /// <param name="src">The <see cref="object"/> to clone.</param>
        /// <returns>A new instance of T with properties copied from obj.</returns>
        public static T CloneObject<T>(T src)
        {
            return (T)InnerClone(typeof(T), src, false);
        }


        /// <summary>
        /// Clones an object by copying property values.
        /// </summary>
        /// <typeparam name="T">The type of object that is being cloned.</typeparam>
        /// <param name="src">The <see cref="object"/> to clone.</param>
        /// <param name="deepClone">Setting this to true causes a recursive clone.</param>
        /// <returns>A new instance of T with properties copied from obj.</returns>
        public static T CloneObject<T>(T src, bool deepClone)
        {
            return (T)InnerClone(typeof(T), src, deepClone);
        }

        private static object InnerClone(Type destType, object src, bool deepClone)
        {
            if (destType.IsValueType)
            {
                return src;
            }
            else if (!HasParameterlessConstructor(destType))
            {
                return src;
            }
            
            if (src == null) return GetDefault(destType);
            Type dicType = typeof(IDictionary);
            Type lstType = typeof(IList);
            object result = Activator.CreateInstance(destType);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(destType))
            {
                if (!property.IsReadOnly)
                {
                    if (deepClone)
                    {
                        property.SetValue(result, InnerClone(property.PropertyType, property.GetValue(src), true));
                    }
                    else
                    {
                        property.SetValue(result, property.GetValue(src));
                    }
                }
                else if (dicType.IsAssignableFrom(property.PropertyType))
                {
                    IDictionary srcDic = property.GetValue(src) as IDictionary;
                    IDictionary destDic = property.GetValue(result) as IDictionary;
                    if (deepClone)
                    {
                        foreach (DictionaryEntry item in srcDic)
                        {
                            destDic.Add(
                                InnerClone(item.Key.GetType(), item.Key, true), 
                                InnerClone(item.Value.GetType(), item.Value, true)
                                );
                        }
                    }
                    else
                    {
                        foreach (DictionaryEntry item in srcDic)
                        {
                            destDic.Add(item.Key, item.Value);
                        }
                    }
                }
                else if (lstType.IsAssignableFrom(property.PropertyType))
                {
                    IList srcLst = property.GetValue(src) as IList;
                    IList destLst = property.GetValue(result) as IList;
                    if (deepClone)
                    {
                        foreach (object item in srcLst)
                        {
                            destLst.Add(InnerClone(item.GetType(), item, true));
                        } 
                    }
                    else
                    {
                        foreach (object item in srcLst)
                        {
                            destLst.Add(item);
                        }
                    }
                }
            }
            return result;
        }
    }
}
