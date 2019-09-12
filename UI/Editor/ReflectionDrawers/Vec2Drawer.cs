using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class Vec2Drawer : SimpleValueDrawer
    {
        public Vec2Drawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        protected override object DrawInternal(Rect rect)
        {
            return EditorGUI.Vector2Field(rect, fieldName, (Vector2)value);
        }
    }
}
