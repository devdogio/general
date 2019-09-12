using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.Localization
{
    [System.Serializable]
    public class LocalizedObject : LocalizedObjectBase<UnityEngine.Object>
    {
        public LocalizedObject()
        {

        }

        public LocalizedObject(string key)
            : base(key)
        {

        }
    }
}
