using System;
using UnityEngine;

namespace Devdog.General
{
    /// <summary>
    /// Used to detect wether an object is relevant to the game or not. 
    /// When not relevant performance can be saved to disable or delete it.
    /// </summary>
    public interface IObjectRelevancy
    {
        event Action OnBecameRelevant;
        event Action OnBecameIrrelevant;

        bool IsRelevant(GameObject obj);
    }
}
