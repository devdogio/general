using System;
using UnityEngine;

namespace Devdog.General
{
    [Serializable]
    public class MotionInfo
    {
        public Motion motion;
        public float speed = 1f;
        public bool crossFade = false;
        public float crossFadeSpeed = 0.3f;
    }
}
