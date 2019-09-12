using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Localization.Editors
{
    [CustomPropertyDrawer(typeof(LocalizedString), true)]
    public class LocalizedStringEditor : PropertyDrawer
    {
        private TextAreaAttribute _textArea;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _textArea = (TextAreaAttribute)fieldInfo.GetCustomAttributes(typeof(TextAreaAttribute), true).FirstOrDefault();
            if (_textArea != null)
            {
                return ReflectionDrawerStyles.singleLineHeight * 2 + (ReflectionDrawerStyles.singleLineHeight * Mathf.Max(_textArea.maxLines, 3));
            }

            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var keyField = property.FindPropertyRelative("_key");
            var msgInst = new LocalizedString(keyField.stringValue);

            if (_textArea != null)
            {
                EditorGUI.LabelField(rect, label);
                rect.y += ReflectionDrawerStyles.singleLineHeight;

                LocalizationEditorUtility.DrawLanguagePicker(ref rect, keyField.stringValue, (newKeyName) =>
                {
                    keyField.stringValue = newKeyName;
                });

                rect.height = ReflectionDrawerStyles.singleLineHeight * Mathf.Max(_textArea.maxLines, 3);

                EditorGUI.BeginChangeCheck();

                if (LocalizationManager.currentDatabase != null && LocalizationManager.currentDatabase.ContainsString(keyField.stringValue) == false)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                }

                var str2 = EditorGUI.TextArea(rect, msgInst.message ?? "");
                if (EditorGUI.EndChangeCheck() && LocalizationManager.defaultDatabase != null && str2 != LocalizationManager.defaultDatabase.GetString(keyField.stringValue))
                {
                    if (IsNullOrWhiteSpace(keyField.stringValue) || keyField.stringValue == LocalizationManager.NoKeyConstant)
                    {
                        keyField.stringValue = LocalizationManager.CreateNewStringKey();
                    }

                    msgInst.message = str2;
                }

                GUI.color = Color.white;
                return;
            }

            LocalizationEditorUtility.DrawLanguagePicker(ref rect, keyField.stringValue, (newKeyName) =>
            {
                keyField.stringValue = newKeyName;
            });
            EditorGUI.BeginChangeCheck();

            if (LocalizationManager.currentDatabase != null && LocalizationManager.currentDatabase.ContainsString(keyField.stringValue) == false)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            var str = EditorGUI.TextField(rect, label, msgInst.message ?? "");
            if (EditorGUI.EndChangeCheck() && LocalizationManager.defaultDatabase != null && str != LocalizationManager.defaultDatabase.GetString(keyField.stringValue))
            {
                if (IsNullOrWhiteSpace(keyField.stringValue) || keyField.stringValue == LocalizationManager.NoKeyConstant)
                {
                    keyField.stringValue = LocalizationManager.CreateNewStringKey();
                }

                msgInst.message = str;
            }

            GUI.color = Color.white;
            EditorGUI.EndProperty();
        }

        private static bool IsNullOrWhiteSpace(string a)
        {
            return string.IsNullOrEmpty(a) || a.Trim().Length == 0;
        }
    }
}
