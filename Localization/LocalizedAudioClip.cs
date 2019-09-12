using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.Localization
{
    [System.Serializable]
    public class LocalizedAudioClip : LocalizedObjectBase<AudioClip>
    {

        public LocalizedAudioClip()
        {

        }

        public LocalizedAudioClip(string key)
            : base(key)
        {

        }
    }
}
