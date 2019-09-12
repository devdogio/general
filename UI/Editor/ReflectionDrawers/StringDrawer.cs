using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class StringDrawer : SimpleValueDrawer
    {
        private readonly TextAreaAttribute _textArea;
        public override bool isEmpty
        {
            get { return string.IsNullOrEmpty((string) value); }
        }

        public StringDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            _textArea = (TextAreaAttribute)fieldInfo.GetCustomAttributes(typeof (TextAreaAttribute), true).FirstOrDefault();
        }

        protected override int GetHeightInternal()
        {
            if (_textArea != null)
            {
                return ReflectionDrawerStyles.singleLineHeight + (ReflectionDrawerStyles.singleLineHeight * Mathf.Max(_textArea.maxLines, 3));
            }

            return ReflectionDrawerStyles.singleLineHeight;
        }

        protected override object DrawInternal(Rect rect)
        {
            GUI.SetNextControlName(fieldName.text);
            
            if (_textArea != null)
            {
                EditorGUI.LabelField(rect, fieldName);

                rect.y += ReflectionDrawerStyles.singleLineHeight;
                rect.height = ReflectionDrawerStyles.singleLineHeight * Mathf.Max(_textArea.maxLines, 3);

                return EditorGUI.TextArea(rect, (string)value ?? "");
            }

            return EditorGUI.TextField(GetSingleLineHeightRect(rect), fieldName, (string)value ?? "");
        }
    }
}
