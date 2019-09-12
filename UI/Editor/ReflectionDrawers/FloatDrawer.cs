using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class FloatDrawer : SimpleValueDrawer
    {
        private RangeAttribute _range;

        public FloatDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            _range = (RangeAttribute)fieldInfo.GetCustomAttributes(typeof(RangeAttribute), true).FirstOrDefault();

        }

        protected override object DrawInternal(Rect rect)
        {
            if (_range != null)
            {
                return EditorGUI.Slider(GetSingleLineHeightRect(rect), fieldName, (float)value, _range.min, _range.max);
            }

            return EditorGUI.FloatField(GetSingleLineHeightRect(rect), fieldName, (float)value);
        }
    }
}
