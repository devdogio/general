using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [System.Serializable]
    public struct FMinMaxRange
    {
        public float min;
        public float max;

        public float Generate()
        {
            return Random.Range(min, max);
        }
    }
}
