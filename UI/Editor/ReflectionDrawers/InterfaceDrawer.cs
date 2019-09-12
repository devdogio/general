using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public class InterfaceDrawer : ChildrenValueDrawerBase
    {
        public InterfaceDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override void DrawChildren(Rect rect)
        {
            if (children.Count > 0)
            {
                base.DrawChildren(rect);
            }
        }
    }
}
