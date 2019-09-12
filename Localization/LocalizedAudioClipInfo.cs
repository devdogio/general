using System;
using Devdog.General.Localization;
using UnityEngine;

namespace Devdog.General
{
    [Serializable]
    public class LocalizedAudioClipInfo
    {
        public LocalizedAudioClip audioClip;
        public float volume = 1f;
        public float pitch = 1f;
        public bool loop = false;
    }
}
