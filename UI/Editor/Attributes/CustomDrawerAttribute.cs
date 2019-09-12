using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace Devdog.General.Editors.ReflectionDrawers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomDrawerAttribute : Attribute
    {
        public Type type { get; protected set; }
        public bool onlyForRoot { get; protected set; }

        public CustomDrawerAttribute(Type type)
            :this(type, false)
        { }

        public CustomDrawerAttribute(Type type, bool onlyForRoot)
        {
            this.type = type;
            this.onlyForRoot = onlyForRoot;
        }
    }
}
