using System;
using UnityEngine;

namespace Devdog.General
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        protected void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}
