using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class UintDrawer : SimpleValueDrawer
    {
        private RangeAttribute _range;

        public UintDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            _range = (RangeAttribute)fieldInfo.GetCustomAttributes(typeof(RangeAttribute), true).FirstOrDefault();

        }

        protected override object DrawInternal(Rect rect)
        {
            if (_range != null)
            {
                return (uint)EditorGUI.IntSlider(GetSingleLineHeightRect(rect), fieldName, (int)((uint)value), (int)_range.min, (int)_range.max);
            }

            return (uint)EditorGUI.IntField(GetSingleLineHeightRect(rect), fieldName, (int)((uint)value));
        }
    }
}
