using System;
using UnityEngine;

namespace Devdog.General
{
    [RequireComponent(typeof(ParticleSystem))]
    public class DisableParticleSystemAfterSeconds : MonoBehaviour
    {
        public float seconds = 1f;

        protected void Awake()
        {
            Invoke("DisableParticleSystem", seconds);
        }

        protected void DisableParticleSystem()
        {
            var system = GetComponent<ParticleSystem>();
            system.Stop();
        }
    }
}
