using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public static class GameObjectExtensionMethods
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var a = gameObject.GetComponent<T>();
            if (a != null)
            {
                return a;
            }

            return gameObject.AddComponent<T>();
        }



    }
}
