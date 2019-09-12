using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Devdog.General.Localization.Editors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The UnityEngine.Object type (for example AudioClip)</typeparam>
    /// <typeparam name="T2">The localized object that handles the type (for example LocalizedAudioClip)</typeparam>
    public abstract class LocalizedObjectEditorBase<T, T2> : PropertyDrawer 
        where T : UnityEngine.Object
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var keyField = property.FindPropertyRelative("_key");
            var inst = (ILocalizedObject)CreateDefaultObject<T2>(keyField.stringValue);

            LocalizationEditorUtility.DrawLanguagePicker(ref rect, keyField.stringValue, (newKeyName) =>
            {
                keyField.stringValue = newKeyName;
            });

            if (LocalizationManager.currentDatabase != null && LocalizationManager.currentDatabase.ContainsObject(keyField.stringValue) == false)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            }

            EditorGUI.BeginChangeCheck();
            rect.height = EditorGUIUtility.singleLineHeight;
            var obj = (T)EditorGUI.ObjectField(rect, label, inst.objectVal, typeof(T), false);
            if (EditorGUI.EndChangeCheck() && LocalizationManager.defaultDatabase != null)
            {
                if (obj != LocalizationManager.defaultDatabase.GetObject<T>(keyField.stringValue))
                {
                    if (IsNullOrWhiteSpace(keyField.stringValue) || keyField.stringValue == LocalizationManager.NoKeyConstant)
                    {
                        keyField.stringValue = LocalizationManager.CreateNewObjectKey<T>(obj);
                    }
                }

                inst.objectVal = obj;
                GUI.changed = true;
            }

            GUI.color = Color.white;
            EditorGUI.EndProperty();
        }

        protected T3 CreateDefaultObject<T3>(string key)
        {
            var constructor = typeof(T3).GetConstructor(new Type[] { typeof(string) });
            Assert.IsNotNull(constructor, "No constructor with string as argument found. Can't pass in key...");
            return (T3)constructor.Invoke(new object[] { key });
        }

        protected static bool IsNullOrWhiteSpace(string a)
        {
            return string.IsNullOrEmpty(a) || a.Trim().Length == 0;
        }
    }
}
