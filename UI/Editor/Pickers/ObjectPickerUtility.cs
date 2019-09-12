using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General.Editors
{
    public static class ObjectPickerUtility
    {
//        private static Dictionary<Rect, int> _unityObjectPickerControlID = new Dictionary<Rect, int>();
        private static readonly Dictionary<Type, Type> _objectPickerTypeCache = new Dictionary<Type, Type>();
        private static Type[] _allEditorTypes = new Type[0];
        private static Dictionary<Type, CustomObjectPickerAttribute> _allEditorTypeAttributes = new Dictionary<Type, CustomObjectPickerAttribute>();

        private static Type builtInObjectPickerType
        {
            get
            {
                var type = Type.GetType("UnityEditor.ObjectSelector,UnityEditor");
                if (type == null)
                {
                    DevdogLogger.LogError("Built-in Unity ObjectSelector not found!");
                }

                return type;
            }
        }

        private static bool isUnityObjectPickerOpen
        {
            get
            {
                var isVisibleField = builtInObjectPickerType.GetProperty("isVisible", BindingFlags.Public | BindingFlags.Static);
                if (isVisibleField == null)
                {
                    DevdogLogger.LogError("Couldn't find isVisible field on UnityEditor.ObjectSelector's internal class");
                    return false;
                }

                return (bool)isVisibleField.GetValue(null, null);
            }
        }


        static ObjectPickerUtility()
        {
            _allEditorTypes = ReflectionUtility.GetAllClassesWithAttribute(typeof(CustomObjectPickerAttribute), false);
            foreach (var type in _allEditorTypes)
            {
                if (typeof(ObjectPickerBaseEditor).IsAssignableFrom(type) == false)
                {
                    DevdogLogger.LogError("Class " + type.Name + " has the " + typeof(CustomObjectPickerAttribute).Name + " attribute, but doesn't inherit from " + typeof(ObjectPickerBaseEditor).Name);
                }
            }

            _allEditorTypes = _allEditorTypes.OrderByDescending(o => ((CustomObjectPickerAttribute)o.GetCustomAttributes(typeof(CustomObjectPickerAttribute), true).First()).priority).ToArray();
            foreach (var type in _allEditorTypes)
            {
                _allEditorTypeAttributes[type] = (CustomObjectPickerAttribute)type.GetCustomAttributes(typeof(CustomObjectPickerAttribute), true).First();
            }
        }


        public static bool HasCustomObjectPickerForType<T>()
        {
            return HasCustomObjectPickerForType(typeof (T));
        }

        public static bool HasCustomObjectPickerForType(Type type)
        {
            return GetObjectPickerTypeFor(type) != null;
        }

        public static ObjectPickerBaseEditor GetObjectPickerForType<T>(Action<T> callback)
            where T : UnityEngine.Object
        {
            return GetObjectPickerForType(typeof(T), val => callback((T)val));
        }

        public static ObjectPickerBaseEditor GetObjectPickerForType(Type objectType, Action<UnityEngine.Object> callback)
        {
            var editorType = GetObjectPickerTypeFor(objectType);

            var window = (ObjectPickerBaseEditor)EditorWindow.GetWindow(editorType, true);
            window.searchType = typeof(UnityEngine.Component).IsAssignableFrom(objectType) ? ObjectPickerBaseEditor.SearchType.Components : ObjectPickerBaseEditor.SearchType.ObjectTypes;
            window.callback = callback;
            window.type = objectType;
            window.allowInherited = true;
            window.wantsMouseMove = false;
            window.minSize = new Vector2(300, 300);
            window.Init();
            window.Focus();

            return window;
        }

        public static Type GetObjectPickerTypeFor(Type objectType)
        {
            if (_objectPickerTypeCache.ContainsKey(objectType))
            {
                return _objectPickerTypeCache[objectType];
            }

            foreach (var kvp in _allEditorTypeAttributes)
            {
                if (kvp.Value.type.IsAssignableFrom(objectType))
                {
                    _objectPickerTypeCache[objectType] = kvp.Key;
                    return kvp.Key;
                }
            }

            return null;
        }

        public static void RenderObjectPickerForType<T>(SerializedProperty prop, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            Assert.IsNotNull(prop, "Given SerializedProperty is null!");
            RenderObjectPickerForType<T>(prop.displayName, prop, callback);
        }

        public static void RenderObjectPickerForType<T>(Rect rect, SerializedProperty prop, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            Assert.IsNotNull(prop, "Given SerializedProperty is null!");
            RenderObjectPickerForType<T>(rect, prop.displayName, prop, callback);
        }

        public static void RenderObjectPickerForType<T>(string fieldName, SerializedProperty prop, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            RenderObjectPickerForType(fieldName, prop, typeof (T), val =>
            {
                if (callback != null)
                {
                    callback((T) val);
                }
            });
        }

        public static void RenderObjectPickerForType<T>(Rect rect, string fieldName, SerializedProperty prop, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            RenderObjectPickerForType(rect, fieldName, prop, typeof (T), val =>
            {
                if (callback != null)
                {
                    callback((T) val);
                }
            });
        }

        public static void RenderObjectPickerForType(string fieldName, SerializedProperty prop, Type type, Action<UnityEngine.Object> callback = null)
        {
            var rect = GUILayoutUtility.GetRect(GetGUIContentFromSerializedProperty(prop, type), UnityEditor.EditorStyles.objectField);
            RenderObjectPickerForType(rect, fieldName, prop, type, callback);
        }

        public static void RenderObjectPickerForType(Rect rect, string fieldName, SerializedProperty prop, Type type, Action<UnityEngine.Object> callback = null)
        {
            Assert.IsNotNull(prop, "Given SerializedProperty is null!");

            var isRequiredAndNull = false;
            if (prop.serializedObject.targetObject != null)
            {
                var targetObject = prop.serializedObject;
                var targetObjectType = targetObject.targetObject.GetType();
                var fieldNames = prop.propertyPath.Split('.');
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    var field = ReflectionUtility.GetFieldInherited(targetObjectType, fieldNames[i]);
                    if (field != null)
                    {
                        targetObjectType = field.FieldType;

                        if (i == fieldNames.Length - 1)
                        {
                            isRequiredAndNull = field.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0 &&
                                            prop.objectReferenceValue == null;
                        }
                    }
                }
            }

            using (new ColorBlock(RequiredAttributeEditor.ErrorColor, isRequiredAndNull))
            {
                RenderObjectPickerForType(rect, type, new GUIContent(fieldName), GetGUIContentFromSerializedProperty(prop, type), prop.objectReferenceValue, (val) =>
                {
                    prop.objectReferenceValue = val;

                    if (callback != null)
                    {
                        callback(val);
                    }

                    prop.serializedObject.ApplyModifiedProperties();
                    prop.serializedObject.Update();
                });
            }
        }



        public static void RenderObjectPickerForType<T>(string fieldName, T prop, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            var rect = GUILayoutUtility.GetRect(GetGUIContentFromUnityObject(prop, typeof(T)), UnityEditor.EditorStyles.objectField);
            RenderObjectPickerForType<T>(rect, fieldName, prop, callback);
        }

        public static void RenderObjectPickerForType<T>(Rect rect, string fieldName, T prop, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            RenderObjectPickerForType(rect, typeof(T), new GUIContent(fieldName), GetGUIContentFromUnityObject(prop, typeof(T)), prop, (obj) =>
            {
                if (callback != null)
                {
                    callback((T) obj);
                }
            });
        }

        public static void RenderObjectPickerForType(string fieldName, UnityEngine.Object obj, Type type, Action<UnityEngine.Object> callback = null)
        {
            var rect = GUILayoutUtility.GetRect(GetGUIContentFromUnityObject(obj, type), UnityEditor.EditorStyles.objectField);
            RenderObjectPickerForType(rect, type, new GUIContent(fieldName), GetGUIContentFromUnityObject(obj, type), obj, callback);
        }

        public static void RenderObjectPickerForType(Rect rect, string fieldName, UnityEngine.Object prop, Type type, Action<UnityEngine.Object> callback = null)
        {
            RenderObjectPickerForType(rect, type, new GUIContent(fieldName), GetGUIContentFromUnityObject(prop, type), prop, callback);
        }

        private static void RenderObjectPickerForType(Rect rect, Type type, GUIContent fieldName, GUIContent objectName, UnityEngine.Object obj, Action<UnityEngine.Object> callback = null)
        {
            DoObjectField(rect, rect, fieldName, 1, obj, type, callback);
        }

        internal static void DoObjectField(Rect position, Rect dropRect, GUIContent fieldName, int id, UnityEngine.Object obj, Type objType, Action<UnityEngine.Object> callback)
        {
            if (IsBuiltInUnityType(objType) || HasCustomObjectPickerForType(objType) == false)
            {
                var returnValue = EditorGUI.ObjectField(position, fieldName, obj, objType, false);
                GUI.changed = true;

                callback(returnValue);
                return;
            }
            
            Event current = Event.current;
            EventType eventType = current.type;
            if (!GUI.enabled && Event.current.rawType == EventType.MouseDown)
            {
                eventType = Event.current.rawType;
            }

            if (fieldName != null && string.IsNullOrEmpty(fieldName.text) == false)
            {
                var labelPos = position;
                labelPos.width = EditorGUIUtility.labelWidth;
                EditorGUI.PrefixLabel(labelPos, fieldName);

                position.x += labelPos.width;
                position.width -= labelPos.width;
            }

            DrawObjectFieldCustom(position, id, obj, objType, callback);
            
            EventType eventType2 = eventType;
            switch (eventType2)
            {
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                case EventType.Layout:
                case EventType.Ignore:
                case EventType.Used:
                case EventType.ValidateCommand:
                case EventType.Repaint:
                case EventType.ExecuteCommand:
                {
                    break;
                }
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropRect.Contains(Event.current.mousePosition) && GUI.enabled)
                    {
                        UnityEngine.Object[] objectReferences = UnityEditor.DragAndDrop.objectReferences;
                        var object2 = objectReferences.FirstOrDefault();
                        if (object2 != null && !UnityEditor.EditorUtility.IsPersistent(object2))
                        {
                            object2 = null;
                        }

                        if (object2 != null)
                        {
                            UnityEditor.DragAndDrop.visualMode = UnityEditor.DragAndDropVisualMode.Generic;
                            if (eventType == EventType.DragPerform && ValidateObjectFieldAssignment(new [] { object2 }, objType) != null)
                            {
                                obj = object2;
                                GUI.changed = true;

                                var objAsGameObject = obj as GameObject;
                                if (objAsGameObject != null)
                                {
                                    obj = objAsGameObject.GetComponent(objType);
                                }

                                callback(obj);

                                UnityEditor.DragAndDrop.AcceptDrag();
                                UnityEditor.DragAndDrop.activeControlID = 0;
                            }
                            else
                            {
                                UnityEditor.DragAndDrop.activeControlID = id;
                            }
                            Event.current.Use();
                        }
                    }
                    break;
                case EventType.DragExited:
                {
                    if (GUI.enabled)
                    {
                        HandleUtility.Repaint();
                    }
                    break;
                }
            }
        }

        private static void DrawObjectFieldCustom(Rect position, int id, UnityEngine.Object obj, Type objType, Action<UnityEngine.Object> callback)
        {
            EditorGUI.LabelField(position, ObjectNames.GetDragAndDropTitle(obj), UnityEditor.EditorStyles.objectField);
            position.width -= 32;

            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
            {
                if (position.Contains(Event.current.mousePosition) && obj != null)
                {
                    Selection.objects = new[] { obj };
                    EditorGUIUtility.PingObject(obj);
                    Event.current.Use();
                    GUIUtility.ExitGUI();
                }

                position.x += position.width;
                position.width = 32;

                if (position.Contains(Event.current.mousePosition))
                {
                    GetObjectPickerForType(objType, (val) =>
                    {
                        if (callback != null)
                        {
                            callback(val);
                        }

                        GUI.changed = true;
                    });
                }
            }
        }

        internal static UnityEngine.Object ValidateObjectFieldAssignment(UnityEngine.Object[] references, Type objType)
        {
            if (references.Length > 0)
            {
                if (references[0] != null && references[0].GetType() == typeof(GameObject) && typeof(Component).IsAssignableFrom(objType))
                {
                    GameObject gameObject = (GameObject)references[0];
                    references = gameObject.GetComponents(typeof(Component));
                }
                UnityEngine.Object[] array = references;
                for (int i = 0; i < array.Length; i++)
                {
                    UnityEngine.Object @object = array[i];
                    if (@object != null && objType.IsAssignableFrom(@object.GetType()))
                    {
                        return @object;
                    }
                }
            }

            return null;
        }



        private static bool IsBuiltInUnityType(Type type)
        {
            return type.Namespace != null && (type.Namespace.StartsWith("UnityEngine") || type.Namespace.StartsWith("UnityEditor"));
        }

        private static GUIContent GetGUIContentFromSerializedProperty(SerializedProperty obj, Type type)
        {
            return GetGUIContentFromUnityObject(obj.objectReferenceValue, type);
        }

        private static GUIContent GetGUIContentFromUnityObject(UnityEngine.Object obj, Type type)
        {
            return new GUIContent(obj == null ? "None (" + type.Name + ")" : obj.name + " (" + type.Name + ")");
        }
    }
}
