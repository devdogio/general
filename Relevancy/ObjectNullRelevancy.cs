#pragma warning disable 0067

using System;
using UnityEngine;

namespace Devdog.General
{
    public sealed class ObjectNullRelevancy : IObjectRelevancy
    {
        public event Action OnBecameRelevant;
        public event Action OnBecameIrrelevant;


        public bool IsRelevant(GameObject obj)
        {
            return true;
        }
    }
}
