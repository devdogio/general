using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [System.Serializable]
    public struct MinMaxRange
    {
        public int min;
        public int max;

        public int Generate()
        {
            return Random.Range(min, max);
        }
    }
}
