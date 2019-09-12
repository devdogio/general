using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public sealed class QuaternionDrawer : SimpleValueDrawer
    {
        public QuaternionDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        protected override object DrawInternal(Rect rect)
        {
            return ToQuaternion(EditorGUI.Vector4Field(rect, fieldName.text, ToVec4((Quaternion)value)));
        }

        private Quaternion ToQuaternion(Vector4 vector)
        {
            return new Quaternion(vector.x, vector.y, vector.z, vector.w);
        }

        private Vector4 ToVec4(Quaternion quaternion)
        {
            return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}
