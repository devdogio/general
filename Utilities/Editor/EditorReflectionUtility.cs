using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System;
using System.Reflection;

namespace Devdog.General.Editors
{
    public static class EditorReflectionUtility
    {


        public static T CreateDeepClone<T>(T obj)
        {
            return (T)CreateDeepClone((object)obj);
        }

        /// <summary>
        /// The method implements deep clone using reflection.
        /// </summary>
        /// <param name="obj">It is the object used to deep clone.</param>
        /// <returns>Return the deep clone.</returns>
        public static object CreateDeepClone(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var type = obj.GetType();

            // No need to clone simple types
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return obj;
            }

            // If the type of the object is the Array, we use the CreateInstance method to get
            // a new instance of the array. We also process recursively this method in the 
            // elements of the original array because the type of the element may be the reference 
            // type.
            if (type.IsArray)
            {
                string typeName = type.AssemblyQualifiedName.Replace("[]", string.Empty);
                Type typeElement = Type.GetType(typeName);
                if (typeElement != null)
                {
                    var array = obj as Array;
                    Array copiedArray = Array.CreateInstance(typeElement, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        copiedArray.SetValue(CreateDeepClone(array.GetValue(i)), i);
                    }

                    return copiedArray;
                }

                DevdogLogger.Log("Couldn't copy array of type " + typeName);
                return obj;
            }

//            // Scriptable objects should be created and saved in a folder; Use the same folder as source item...
//            if (typeof(ScriptableObject).IsAssignableFrom(type))
//            {
//                var currentFolder = AssetDatabase.GetAssetPath((UnityEngine.Object)obj);
//                currentFolder = currentFolder.Substring(0, currentFolder.LastIndexOf('/'));
//
////                DevdogLogger.Log("Creating deepclone of scriptable object: saving to :: " + currentFolder);
//
//                if (AssetDatabase.IsValidFolder(currentFolder))
//                {
//                    var copy = ScriptableObjectUtility.CreateAsset(type, currentFolder, DateTime.Now.ToFileTimeUtc() + ".asset");
//                    ReflectionUtility.CopySerializableValues(obj, copy);
//                    return copy;
//                }
//
//                return null;
//            }

            // If the type of the object is class or struct, it may contain the reference fields, 
            // so we use reflection and process recursively this method in the fields of the object 
            // to get the deep clone of the object. 
            // We use Type.IsValueType method here because there is no way to indicate directly whether 
            // the Type is a struct type.
            if (type.IsClass || type.IsValueType)
            {
                // A reference that we actually want to keep
                if (obj is UnityEngine.Object)
                {
                    return obj;
                }

                object copiedObject = Activator.CreateInstance(obj.GetType());

                // Get all FieldInfo.
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                    {
                        // Don't clone unity objects any further, just go with a ref.
                        if (field.FieldType.IsAssignableFrom(typeof(UnityEngine.Object)) || field.FieldType.IsAssignableFrom(typeof(MonoBehaviour)))
                        {
                            field.SetValue(copiedObject, fieldValue);
                            continue;
                        }

                        // Get the deep clone of the field in the original object and assign the 
                        // clone to the field in the new object.
                        field.SetValue(copiedObject, CreateDeepClone(fieldValue));
                    }
                }

                return copiedObject;
            }

            throw new ArgumentException("The object is unknown type (" + obj.GetType() + ")");
        }
    }
}