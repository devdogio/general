using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public class PlayerTriggerHandler : PlayerTriggerHandlerBase<Collider>
    {
        private SphereCollider _sphereCollider;
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;

            _sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
            _sphereCollider.isTrigger = true;
            _sphereCollider.radius = GeneralSettingsManager.instance.settings.triggerUseDistance;
        }

        protected void OnTriggerEnter(Collider other)
        {
            NotifyTriggerEnter(other);
        }

        protected void OnTriggerExit(Collider other)
        {
            NotifyTriggerExit(other);
        }
    }
}