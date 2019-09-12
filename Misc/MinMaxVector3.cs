using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [System.Serializable]
    public struct MinMaxVector3
    {
        public Vector3 min;
        public Vector3 max;

        public Vector3 Generate()
        {
            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z)
            );
        }
    }
}
