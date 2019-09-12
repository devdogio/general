using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General.Localization.Editors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The UnityEngine.Object type (for example AudioClip)</typeparam>
    /// <typeparam name="T2">The localized object that handles the type (for example LocalizedAudioClip)</typeparam>
    public abstract class LocalizedObjectDrawerBase<T, T2> : DrawerBase 
        where T : UnityEngine.Object
        where T2 : ILocalizedObject
    {
        private readonly FieldInfo _keyField;

        public override bool isEmpty
        {
            get { return ((T2)value).objectVal == null; }
        }

        protected LocalizedObjectDrawerBase(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            _keyField = ReflectionUtility.GetFieldInherited(typeof(T2), "_key");

            Update();
        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        protected override object DrawInternal(Rect rect)
        {
            GUI.SetNextControlName(fieldName.text);
            var a = (T2)value;
            var keyName = _keyField.GetValue(value) as string;

            LocalizationEditorUtility.DrawLanguagePicker(ref rect, keyName, (newKeyName) =>
            {
                _keyField.SetValue(value, newKeyName);
            });

            EditorGUI.BeginChangeCheck();
            if (LocalizationManager.currentDatabase != null && LocalizationManager.currentDatabase.ContainsObject(keyName) == false)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            }

            var obj = EditorGUI.ObjectField(GetSingleLineHeightRect(rect), fieldName, a.objectVal, typeof(T), false);
            if (EditorGUI.EndChangeCheck() && LocalizationManager.defaultDatabase != null && obj != LocalizationManager.defaultDatabase.GetObject<T>(keyName))
            {
                if (IsNullOrWhiteSpace(keyName) || keyName == LocalizationManager.NoKeyConstant)
                {
                    _keyField.SetValue(value, LocalizationManager.CreateNewObjectKey<T>());
                    NotifyValueChanged(value);
                }

                a.objectVal = obj;
                NotifyValueChanged(a);
            }

            GUI.color = Color.white;
            return a;
        }

        private void Update()
        {
            var fieldType = GetFieldType(false);

            if (typeof(T).IsAssignableFrom(fieldType) && ReflectionDrawerUtility.TryGetCustomDrawerType(fieldType, fieldInfo == null) == null)
            {
                return;
            }

            if (value == null)
            {
                // Check if we can create a new object of this type
                // TODO: Move to utility, can be reused - Utility.CanCreateNewObjectOfType
                if ((fieldType.IsClass && fieldType.IsAbstract == false) ||
                    fieldType.IsValueType)
                {
                    if (fieldType.GetConstructors().Any(o => o.GetParameters().Length == 0))
                    {
                        value = Activator.CreateInstance(fieldType);
                        NotifyValueChanged(value);
                    }
                }
            }
        }

        private static bool IsNullOrWhiteSpace(string a)
        {
            return string.IsNullOrEmpty(a) || a.Trim().Length == 0;
        }
    }
}
