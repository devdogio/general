using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.Localization
{
    [System.Serializable]
    public class LocalizedTexture2D : LocalizedObjectBase<Texture2D>
    {
        public LocalizedTexture2D()
        {

        }

        public LocalizedTexture2D(string key)
            : base(key)
        {

        }
    }
}
