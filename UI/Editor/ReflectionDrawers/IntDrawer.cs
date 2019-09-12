using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class IntDrawer : SimpleValueDrawer
    {
        private RangeAttribute _range;

        public IntDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            _range = (RangeAttribute)fieldInfo.GetCustomAttributes(typeof(RangeAttribute), true).FirstOrDefault();
        }

        protected override object DrawInternal(Rect rect)
        {
            if (_range != null)
            {
                return EditorGUI.IntSlider(GetSingleLineHeightRect(rect), fieldName, (int) value, (int)_range.min, (int)_range.max);
            }

            return EditorGUI.IntField(GetSingleLineHeightRect(rect), fieldName, (int)value);
        }
    }
}
