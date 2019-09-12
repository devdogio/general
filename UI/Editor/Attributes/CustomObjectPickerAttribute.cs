using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace Devdog.General.Editors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomObjectPickerAttribute : Attribute
    {
        public int priority { get; protected set; }
        public Type type { get; protected set; }

        public CustomObjectPickerAttribute(Type type)
            : this(type, 0)
        { }

        public CustomObjectPickerAttribute(Type type, int priority)
        {
            this.type = type;
            this.priority = priority;

            Assert.IsTrue(typeof(UnityEngine.Object).IsAssignableFrom(type), "Given type " + type.Name + " does not inherit from UnityEngine.Object.");
        }
    }
}
