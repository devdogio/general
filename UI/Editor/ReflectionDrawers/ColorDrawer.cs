using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class ColorDrawer : SimpleValueDrawer
    {
        public ColorDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override object DrawInternal(Rect rect)
        {
            return EditorGUI.ColorField(GetSingleLineHeightRect(rect), fieldName, (Color)value);
        }
    }
}
