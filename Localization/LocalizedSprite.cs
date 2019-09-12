using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.Localization
{
    [System.Serializable]
    public class LocalizedSprite : LocalizedObjectBase<Sprite>
    {
        
        public LocalizedSprite()
        {

        }

        public LocalizedSprite(string key)
            : base(key)
        {

        }
    }
}
