using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public static class ReflectionDrawerUtility
    {
        private static Dictionary<Type, Type> _drawers = new Dictionary<Type, Type>()
        {
            { typeof(string), typeof(StringDrawer) },
            { typeof(bool), typeof(BoolDrawer) },
            { typeof(float), typeof(FloatDrawer) },
            { typeof(int), typeof(IntDrawer) },
            { typeof(uint), typeof(UintDrawer) },
            { typeof(Vector2), typeof(Vec2Drawer) },
            { typeof(Vector3), typeof(Vec3Drawer) },
            { typeof(Vector4), typeof(Vec4Drawer) },
            { typeof(Quaternion), typeof(QuaternionDrawer) },
            { typeof(Color), typeof(ColorDrawer) },
            { typeof(LayerMask), typeof(LayerMaskDrawer) },
            { typeof(AnimationCurve), typeof(AnimationCurveDrawer) },
//            { typeof(KeyValuePair<,>), typeof(KeyValuePairDrawer) }
        }; 

        private static Dictionary<Type, CustomDrawerAttribute> _customDrawerAttributes;
        private static Dictionary<Type, List<FieldInfo>> _serializableFieldsCache = new Dictionary<Type, List<FieldInfo>>(); 

        static ReflectionDrawerUtility()
        {
            _customDrawerAttributes = new Dictionary<Type, CustomDrawerAttribute>();
            var customDrawerClasses = ReflectionUtility.GetAllClassesWithAttribute(typeof(CustomDrawerAttribute));
            foreach (var c in customDrawerClasses)
            {
                _customDrawerAttributes[c] = (CustomDrawerAttribute)c.GetCustomAttributes(typeof (CustomDrawerAttribute), true).FirstOrDefault();
            }
        }

        public static List<DrawerBase> BuildEditorHierarchy(Type type, object value)
        {
            var oldMode = GCSettings.LatencyMode;
            try
            {
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;

                var l = new List<DrawerBase>();
                List<FieldInfo> fields = new List<FieldInfo>();
                if (_serializableFieldsCache.ContainsKey(type))
                {
                    fields = _serializableFieldsCache[type];
                }
                else
                {
                    ReflectionUtility.GetAllSerializableFieldsInherited(type, fields);
                    _serializableFieldsCache[type] = fields;
                }

                foreach (var field in fields)
                {
                    var drawer = BuildEditorHierarchy(field, value);
                    if (drawer != null)
                    {
                        l.Add(drawer);
                    }
                    else
                    {
                        DevdogLogger.LogWarning("Couldn't create drawer for serializable type: " + field.FieldType);
                    }
                }

                return l;
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }
        }

        public static DrawerBase BuildEditorHierarchy(FieldInfo field, object value, int index = -1)
        {
            var fieldType = field.FieldType;
            object fieldValue = null;

            object parentValue = value;
            if (index >= 0)
            {
                // Is array
                Assert.IsTrue(value.GetType().IsArray, "Index given but value is not an array!!");

                fieldValue = ((Array) value).GetValue(index);
                if (fieldValue == null)
                {
                    // Emtpy array element, get type from array
                    fieldType = value.GetType().GetElementType();
                }
                else
                {
                    fieldType = fieldValue.GetType();
                }
            }
            else
            {
                // Is not an array
                if (value != null)
                {
                    fieldValue = field.GetValue(value);
                }
            }

            Type drawerType;
            bool found = _drawers.TryGetValue(fieldType, out drawerType);
            if (found == false)
            {
                drawerType = TryGetCustomDrawerType(fieldType, field == null);
            }

            if (drawerType == null)
            {
                if (fieldType.IsEnum)
                {
                    drawerType = typeof (EnumDrawer);
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
                {
                    drawerType = typeof (UnityObjectDrawer);
                }
                else if (fieldType.IsArray)
                {
                    drawerType = typeof (ArrayDrawer);
                }
                else if (typeof(IDictionary).IsAssignableFrom(fieldType))
                {
                    drawerType = typeof(DictionaryDrawer);
                }
                else if (fieldType.IsInterface || (value.GetType().IsArray && value.GetType().GetElementType().IsInterface))
                {
                    drawerType = typeof (InterfaceDrawer);
                }
                else if (fieldType.IsClass)
                {
                    drawerType = typeof (ClassDrawer);
                }
                else if (fieldType.IsValueType)
                {
                    drawerType = typeof (ValueTypeDrawer);
                }

                // Cache it for future usage.
                if (drawerType != null)
                {
                    _drawers[fieldType] = drawerType;
                }
            }

            if (drawerType == null)
            {
//                DevdogLogger.LogWarning("No drawer type found for " + fieldType.Name);
                return null;
            }

            return CreateNewDrawerFromType(field, fieldValue, parentValue, index, drawerType);
        }

        public static Type TryGetCustomDrawerType(Type forType, bool isRootType)
        {
            Type drawerType = null;
            foreach (var c in _customDrawerAttributes)
            {
                if (c.Value.onlyForRoot && isRootType == false)
                {
                    continue;
                }

                if (c.Value.type.IsAssignableFrom(forType))
                {
                    drawerType = c.Key;
                    break;
                }

                if (c.Value.type.IsGenericType &&
                    forType.IsGenericType &&
                    c.Value.type.GetGenericArguments()[0] == forType)
                {
                    drawerType = c.Key;
                    break;
                }
            }

            if (drawerType == null)
            {
                // If no specific drawer has been found try a 'wide' generic one Type<> (without specific type).
                foreach (var c in _customDrawerAttributes)
                {
                    if (c.Value.onlyForRoot && isRootType == false)
                    {
                        continue;
                    }

                    if (IsGenericTypeWithoutGenericArguments(c.Value.type) && forType.IsGenericType)
                    {
                        var a = c.Value.type.GetGenericTypeDefinition();
                        var b = forType.GetGenericTypeDefinition();
                        if (a == b)
                        {
                            // Generic for all types (Type<>)
                            drawerType = c.Key;
                            break;
                        }
                    }
                }
            }

            return drawerType;
        }

        public static DrawerBase TryGetCustomDrawer(FieldInfo field, object fieldValue, object parentValue, int index, Type fieldType)
        {
            var drawerType = TryGetCustomDrawerType(fieldType, field == null);
            if (drawerType == null)
            {
                return null;
            }

            return CreateNewDrawerFromType(field, fieldValue, parentValue, index, drawerType);
        }

        private static DrawerBase CreateNewDrawerFromType(FieldInfo field, object fieldValue, object parentValue, int index, Type drawerType)
        {
            var constructor = drawerType.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                    null,
                    new[] {typeof (FieldInfo), typeof (object), typeof (object), typeof (int)},
                    null
                );

            // Create new instance of this drawer editor.
            return (DrawerBase) constructor.Invoke(new object[] {field, fieldValue, parentValue, index});
        }

        public static DerivedTypeInformation GetDerivedTypesFrom(Type fromType, Type onlyDerivedFrom)
        {
            if (fromType.IsGenericType)
            {
                fromType = fromType.GetGenericTypeDefinition();
            }

            if (onlyDerivedFrom != null)
            {
                fromType = onlyDerivedFrom;
            }


            var implementable = new DerivedTypeInformation();
            if (fromType.IsGenericType)
            {
                // Get standard things like string : TODO: Make this dynamic in the future, allowing users to select their own types.
                implementable = new DerivedTypeInformation()
                {
                    types = new Type[]
                    {
                        fromType.MakeGenericType(typeof (string)),
                        fromType.MakeGenericType(typeof (bool)),
                        fromType.MakeGenericType(typeof (int)),
                        fromType.MakeGenericType(typeof (float)),
                        fromType.MakeGenericType(typeof (uint)),
                        fromType.MakeGenericType(typeof (Vector2)),
                        fromType.MakeGenericType(typeof (Vector3)),
                        fromType.MakeGenericType(typeof (Vector4)),
                        fromType.MakeGenericType(typeof (Quaternion)),
                        fromType.MakeGenericType(typeof (UnityEngine.Object)),
                    }
                };

                implementable.content = implementable.types.Select(o => new GUIContent(GetGenericTypeNiceName(o))).ToArray();
            }
            else
            {
                implementable.types = ReflectionUtility.GetAllTypesThatImplement(fromType);
                implementable.content = implementable.types.Select(o => new GUIContent(GetPopupNameForType(o))).ToArray();
            }

            return implementable;
        }

        public static string GetGenericTypeNiceName(Type type)
        {
            if (type.IsGenericType == false)
            {
                return type.Name;
            }

            string[] names = type.GetGenericArguments().Select(o => o.Name).ToArray();
            var fullName = type.GetGenericTypeDefinition().Name;
            return fullName.Substring(0, fullName.Length - 2) + "<" + string.Join(",", names) + ">";
        }


        public static string GetPopupNameForType(Type type)
        {
            if (string.IsNullOrEmpty(type.Namespace))
            {
                return type.Name;
            }

            string name = "";
            foreach (var s in type.Namespace.Split('.'))
            {
                name += s + "/";
            }

            name += type.Name;
            return name;
        }

        public static bool IsGenericTypeWithoutGenericArguments(Type type)
        {
            return type.IsGenericTypeDefinition;
        }
    }
}
