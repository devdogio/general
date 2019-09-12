using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class EnumDrawer : SimpleValueDrawer
    {
        public EnumDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override object DrawInternal(Rect rect)
        {
            // TODO: Check if mask field, -> Different editor call.
            return EditorGUI.EnumPopup(GetSingleLineHeightRect(rect), fieldName, (Enum) value);
        }
    }
}
