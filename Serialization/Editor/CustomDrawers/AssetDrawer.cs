using System;
using System.Reflection;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    [CustomDrawer(typeof(Asset<>))]
    public class AssetDrawer : ChildrenValueDrawerBase
    {
        public override GUIContent overrideFieldName
        {
            set
            {
                foreach (var child in children)
                {
                    child.overrideFieldName = value;
                }
            }
        }

        public AssetDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            foreach (var child in children)
            {
                child.required = required;
            }
        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight;
        }

        protected override object DrawInternal(Rect rect)
        {
            DrawArrayControls(ref rect);
            return value;
        }

        protected virtual void DrawArrayControls(ref Rect rect)
        {
            foreach (var drawer in children)
            {
                if (drawer.overrideFieldName == null)
                {
                    drawer.overrideFieldName = new GUIContent(fieldName);
                }

                drawer.Draw(ref rect);
            }
        }
    }
}
