using System;
using System.Reflection;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

#if NETFX_CORE
using WinRTLegacy;
#endif

namespace Devdog.General
{
    public static class ReflectionUtility 
    {
        public static Type[] GetAllClassesWithAttribute(Type attribute, bool inherit = true)
        {
            var allClasses = GetAllTypesThatImplement(typeof(object));
            return allClasses.Where(o => o.GetCustomAttributes(attribute, inherit).Length > 0).ToArray();
        }

        public static FieldInfo[] GetFieldsWithAttributeInherited(object obj, Type attribute)
        {
            var fieldInfo = new List<FieldInfo>();
            GetAllFieldsInherited(obj.GetType(), fieldInfo);

            var fieldNames = new List<FieldInfo>();
            foreach (var info in fieldInfo.Where(o => o.GetCustomAttributes(attribute, true).Length > 0).ToArray())
            {
                fieldNames.Add(info);
            }

            return fieldNames.ToArray();
        }

        public static void GetAllSerializableFieldsInherited(System.Type startType, List<FieldInfo> appendList)
        {
            GetAllFieldsInherited(startType, appendList);
            appendList.RemoveAll(o => 
                    (
                        (o.IsPrivate || o.IsFamily) &&
                        o.GetCustomAttributes(typeof(SerializeField), true).Length == 0 &&
                        o.GetCustomAttributes(typeof(CustomSerializationAttribute), true).Length == 0
                    ) ||

                    o.GetCustomAttributes(typeof(IgnoreCustomSerializationAttribute), true).Length > 0 ||
                    o.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length > 0 ||
                    o.GetCustomAttributes(typeof(HideInInspector), true).Length > 0);
        }

        public static void GetAllFieldsInherited(System.Type startType, List<FieldInfo> appendList)
        {
            GetAllFieldsInherited(startType, appendList, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static void GetAllFieldsInherited(System.Type startType, List<FieldInfo> appendList, BindingFlags flags)
        {
            if (startType == typeof (MonoBehaviour) || startType == null || startType == typeof (object))
            {
                return;
            }

            var fields = startType.GetFields(flags);
            foreach (var fieldInfo in fields)
            {
                if (appendList.Any(o => o.Name == fieldInfo.Name) == false)
                {
                    appendList.Add(fieldInfo);
                }
            }

            // Keep going untill we hit UnityEngine.MonoBehaviour type or null.
            GetAllFieldsInherited(startType.BaseType, appendList, flags);
        }

        public static FieldInfo GetFieldInherited(System.Type startType, string fieldName)
        {
            if (startType == typeof(MonoBehaviour) || startType == null || startType == typeof(object))
                return null;

            // Copied fields can be restricted with BindingFlags
            var field = startType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field;
            }

            // Keep going untill we hit UnityEngine.MonoBehaviour type or null.
            return GetFieldInherited(startType.BaseType, fieldName);
        }

        public static MethodInfo GetMethodByName(Type type, string methodName)
        {
            return GetAllMethodsFromType(type).FirstOrDefault(o => o.Name == methodName);
        }

        public static MethodInfo[] GetAllMethodsFromType(Type type)
        {
            return GetAllMethodsFromType(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MethodInfo[] GetAllMethodsFromType(Type type, BindingFlags flags)
        {
            return type.GetMethods(flags);
        }

        public static Type[] GetAllTypesThatImplement(Type type)
        {
            return GetAllTypesThatImplement(type, true);
        }

        public static Type[] GetAllTypesThatImplement(Type type, bool creatableTypesOnly)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//            var assemblyList = new List<Assembly>();
//            foreach (var assembly in assemblies)
//            {
//                if (assembly.FullName.StartsWith("Mono.Cecil") ||
//                    assembly.FullName.StartsWith("UnityScript") ||
//                    assembly.FullName.StartsWith("Boo.Lan") ||
//                    assembly.FullName.StartsWith("System") ||
//                    assembly.FullName.StartsWith("JetBrains") ||
//                    assembly.FullName.StartsWith("nunit") ||
//                    assembly.FullName.StartsWith("NUnit") ||
//                    assembly.FullName.StartsWith("I18N") ||
////                    assembly.FullName.StartsWith("UnityEngine") ||
//                    //assembly.FullName.StartsWith("UnityEditor") ||
//                    assembly.FullName.StartsWith("mscorlib"))
//                {
//                    continue;
//                }

//                assemblyList.Add(assembly);
//            }

            var types = new List<Type>(assemblies.Length);
            foreach(var assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes());
                }
                catch (Exception e)
                {
                    DevdogLogger.LogError(e.Message);
                }
            }

            types = types.Where(o => type.IsAssignableFrom(o)).ToList();
            if (creatableTypesOnly)
            {
                types = types.Where(o => o.IsAbstract == false && o.IsInterface == false).ToList();
            }

            return types.ToArray();
        }
        
        /// <summary>
        /// Note that this is a really slow method, and should be used with caution...
        /// </summary>
        public static void CopySerializableValues(object from, object to)
        {
            var fromFields = new List<FieldInfo>();
            GetAllSerializableFieldsInherited(from.GetType(), fromFields);

            var toFields = new List<FieldInfo>();
            GetAllSerializableFieldsInherited(to.GetType(), toFields);

            foreach (var fromField in fromFields)
            {
                var toField = toFields.FirstOrDefault(o => o.Name == fromField.Name);
                if (toField != null)
                {
                    try
                    {
                        toField.SetValue(to, fromField.GetValue(from));
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }
                }
            }
        }
        
        public static bool IsBuiltInUnityObjectType(Type type)
        {
            return type.Namespace != null && type.Name.Contains("UnityEngine");
        }
    }
}