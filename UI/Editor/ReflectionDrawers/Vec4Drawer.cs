using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class Vec4Drawer : SimpleValueDrawer
    {
        public Vec4Drawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        protected override object DrawInternal(Rect rect)
        {
            return EditorGUI.Vector4Field(rect, fieldName.text, (Vector4)value);
        }
    }
}
