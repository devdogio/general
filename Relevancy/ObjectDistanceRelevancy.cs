using System;
using UnityEngine;

namespace Devdog.General
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class ObjectDistanceRelevancy : MonoBehaviour, IObjectRelevancy
    {
        public event Action OnBecameRelevant;
        public event Action OnBecameIrrelevant;

        private bool _inTrigger;

        private void Awake()
        {

            OnValidate();
        }

        private void OnValidate()
        {
            var s = gameObject.GetOrAddComponent<SphereCollider>();
            s.radius = Mathf.Max(10f, s.radius);
            s.isTrigger = true;
            s.gameObject.layer = 2;
        }

        public bool IsRelevant(GameObject obj)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return true; // in-editor we always are considered relevant.
            }
#endif

            return _inTrigger;
        }

        private void OnTriggerEnter(Collider other)
        {
            DoOnTriggerEnter(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            DoOnTriggerEnter(other.gameObject);
        }

        private void DoOnTriggerEnter(GameObject obj)
        {
            if (obj.GetComponent<Player>() != null)
            {
                var before = _inTrigger;
                _inTrigger = true;
                if (before == false)
                {
                    if (OnBecameRelevant != null)
                    {
                        OnBecameRelevant();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            DoOnTriggerExit(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            DoOnTriggerExit(other.gameObject);
        }

        private void DoOnTriggerExit(GameObject obj)
        {
            if (obj.GetComponent<Player>() != null)
            {
                var before = _inTrigger;
                _inTrigger = false;
                if (before)
                {
                    if (OnBecameIrrelevant != null)
                    {
                        OnBecameIrrelevant();
                    }
                }
            }
        }
    }
}
