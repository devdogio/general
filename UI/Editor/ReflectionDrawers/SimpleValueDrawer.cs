using System;
using System.Reflection;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public abstract class SimpleValueDrawer : DrawerBase
    {
        protected SimpleValueDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        public override DrawerBase FindDrawerRelative(string fieldName)
        {
            return null;
        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight;
        }
    }
}
