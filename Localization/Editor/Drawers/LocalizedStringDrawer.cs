using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Localization.Editors
{
    [CustomDrawer(typeof(LocalizedString))]
    public class LocalizedStringDrawer : SimpleValueDrawer
    {
        private readonly TextAreaAttribute _textArea;
        private readonly FieldInfo _keyField;

        public override bool isEmpty
        {
            get { return IsNullOrWhiteSpace(((LocalizedString)value).message); }
        }

        public LocalizedStringDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            _textArea = (TextAreaAttribute)fieldInfo.GetCustomAttributes(typeof(TextAreaAttribute), true).FirstOrDefault();
            _keyField = typeof(LocalizedString).GetField("_key", BindingFlags.Instance | BindingFlags.NonPublic);

            Update();
        }

        protected override int GetHeightInternal()
        {
            if (_textArea != null)
            {
                return ReflectionDrawerStyles.singleLineHeight * 2 + (ReflectionDrawerStyles.singleLineHeight * Mathf.Max(_textArea.maxLines, 3));
            }

            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        protected override object DrawInternal(Rect rect)
        {
            GUI.SetNextControlName(fieldName.text);
            var a = (LocalizedString)value;
            var keyName = _keyField.GetValue(value) as string;

            if (_textArea != null)
            {
                EditorGUI.LabelField(rect, fieldName);
                rect.y += ReflectionDrawerStyles.singleLineHeight;

                LocalizationEditorUtility.DrawLanguagePicker(ref rect, keyName, (newKeyName) =>
                {
                    _keyField.SetValue(value, newKeyName);
                });

                rect.height = ReflectionDrawerStyles.singleLineHeight * Mathf.Max(_textArea.maxLines, 3);

                EditorGUI.BeginChangeCheck();

                if (LocalizationManager.currentDatabase != null && LocalizationManager.currentDatabase.ContainsString(keyName) == false)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                }

                var str2 = EditorGUI.TextArea(rect, a.message ?? "");
                if (EditorGUI.EndChangeCheck() && LocalizationManager.defaultDatabase != null && str2 != LocalizationManager.defaultDatabase.GetString(keyName))
                {
                    if (IsNullOrWhiteSpace(keyName) || keyName == LocalizationManager.NoKeyConstant)
                    {
                        _keyField.SetValue(value, LocalizationManager.CreateNewStringKey());
                        NotifyValueChanged(value);
                    }

                    a.message = str2;
                    NotifyValueChanged(a);
                }

                GUI.color = Color.white;
                return a;
            }


            LocalizationEditorUtility.DrawLanguagePicker(ref rect, keyName, (newKeyName) =>
            {
                _keyField.SetValue(value, newKeyName);
            });

            EditorGUI.BeginChangeCheck();

            if (LocalizationManager.currentDatabase != null && LocalizationManager.currentDatabase.ContainsString(keyName) == false)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            }

            var str = EditorGUI.TextField(GetSingleLineHeightRect(rect), fieldName, a.message ?? "");
            if (EditorGUI.EndChangeCheck() && LocalizationManager.defaultDatabase != null && str != LocalizationManager.defaultDatabase.GetString(keyName))
            {
                if (IsNullOrWhiteSpace(keyName) || keyName == LocalizationManager.NoKeyConstant)
                {
                    _keyField.SetValue(value, LocalizationManager.CreateNewStringKey());
                    NotifyValueChanged(value);
                }

                a.message = str;
                NotifyValueChanged(a);
            }

            GUI.color = Color.white;
            return a;
        }
        
        private void Update()
        {
            var fieldType = GetFieldType(false);

            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType) && ReflectionDrawerUtility.TryGetCustomDrawerType(fieldType, fieldInfo == null) == null)
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
