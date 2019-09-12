using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class LayerMaskDrawer : SimpleValueDrawer
    {
        public LayerMaskDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override object DrawInternal(Rect rect)
        {
            var mask = (LayerMask) value;
            return (LayerMask)EditorGUI.MaskField(GetSingleLineHeightRect(rect), fieldName, mask.value, UnityEditorInternal.InternalEditorUtility.layers);
        }
    }
}
