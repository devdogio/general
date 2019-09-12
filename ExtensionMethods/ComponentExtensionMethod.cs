using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public static class ComponentExtensionMethod
    {
        public static T GetOrAddComponent<T>(this Component comp) where T : Component
        {
            var a = comp.GetComponent<T>();
            if (a != null)
            {
                return a;
            }

            return comp.gameObject.AddComponent<T>();
        }



    }
}
