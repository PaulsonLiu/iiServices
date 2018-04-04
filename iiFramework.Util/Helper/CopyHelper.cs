using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace iiFramework.Util
{
    #region CopyHelper
    /// <summary>
    /// 提供深拷贝的类
    /// </summary>
    public class CopyHelper
    {
        #region DeepCopy
        /// <summary>
        /// 返回目标对象的深拷贝
        /// </summary>
        /// <typeparam name="T">目标对象的类型</typeparam>
        /// <param name="obj">需要深拷贝的目标对象</param>
        /// <returns>目标对象的深拷贝</returns>
        public static T DeepCopy<T>(T obj)
        {

            if (obj == null 
                || obj.GetType().IsSubclassOf(typeof(System.Delegate))
                )
            {
                return default(T);
            }

            Type ObjType = obj.GetType();

            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType)
            {
                return obj;
            }

            if (ObjType.IsArray)
            {
                string elementModuleName = ObjType.Module.Name.Replace(".dll", string.Empty);
                elementModuleName = elementModuleName.Replace(".exe", string.Empty);
                string elementTypeName = ObjType.FullName.Replace("[]", string.Empty) + "," + elementModuleName;

                Type elementType = Type.GetType(elementTypeName);

                var array = obj as Array;

                Array copied = Array.CreateInstance(elementType, array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(DeepCopy(array.GetValue(i)), i);
                }

                return (T)Convert.ChangeType(copied, ObjType);
            }
            
            

            object retval = Activator.CreateInstance(ObjType);

            List<FieldInfo> fields = new List<FieldInfo>();

            if (ObjType.BaseType != null)
            {
                fields.AddRange(ObjType.BaseType.GetFields(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Instance));
            }

            fields.AddRange(ObjType.GetFields(
                            BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance));

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(obj);
                field.SetValue(retval, DeepCopy(value));
            }

            return (T)retval;
        }
        #endregion

        #region DeepCopy
        /// <summary>
        /// 返回目标对象的深拷贝
        /// </summary>
        /// <typeparam name="T">目标对象的类型</typeparam>
        /// <param name="obj">需要深拷贝的目标对象</param>
        /// <returns>目标对象的深拷贝</returns>
        public static T DeepCopy<T>(object obj)
        {

            if (obj == null
                || obj.GetType().IsSubclassOf(typeof(System.Delegate))
                )
            {
                return default(T);
            }

            Type ObjType = obj.GetType();

            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType)
            {
                return (T)obj;
            }

            if (ObjType.IsArray)
            {
                string elementModuleName = ObjType.Module.Name.Replace(".dll", string.Empty);
                elementModuleName = elementModuleName.Replace(".exe", string.Empty);
                string elementTypeName = ObjType.FullName.Replace("[]", string.Empty) + "," + elementModuleName;

                Type elementType = Type.GetType(elementTypeName);

                var array = obj as Array;

                Array copied = Array.CreateInstance(elementType, array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(DeepCopy(array.GetValue(i)), i);
                }

                return (T)Convert.ChangeType(copied, ObjType);
            }



            object retval = Activator.CreateInstance(ObjType);

            List<FieldInfo> fields = new List<FieldInfo>();

            if (ObjType.BaseType != null)
            {
                fields.AddRange(ObjType.BaseType.GetFields(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Instance));
            }

            fields.AddRange(ObjType.GetFields(
                            BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance));

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(obj);
                field.SetValue(retval, DeepCopy(value));
            }

            return (T)retval;
        }
        #endregion

        #region CollectionDeepCopy
        public static ObservableCollection<T> CollectionDeepCopy<T>(IEnumerable<T> list)
        {
            ObservableCollection<T> Collection = new ObservableCollection<T>();

            foreach (var item in list)
            {
                Collection.Add(DeepCopy(item));
            }
            return Collection;
        }
        #endregion

        #region CollectionDeepCopy
        public static List<T> ListDeepCopy<T>(IEnumerable<T> list)
        {
            List<T> Collection = new List<T>();

            foreach (var item in list)
            {
                Collection.Add(DeepCopy(item));
            }
            return Collection;
        }
        #endregion
    }
    #endregion
}
