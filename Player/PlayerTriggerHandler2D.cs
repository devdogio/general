using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public class PlayerTriggerHandler2D : PlayerTriggerHandlerBase<Collider2D>
    {
        private CircleCollider2D _sphereCollider;
        private Rigidbody2D _rigidbody;

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
            _rigidbody.isKinematic = true;

            _sphereCollider = gameObject.GetOrAddComponent<CircleCollider2D>();
            _sphereCollider.isTrigger = true;
            _sphereCollider.radius = GeneralSettingsManager.instance.settings.triggerUseDistance;
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            NotifyTriggerEnter(other);
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            NotifyTriggerExit(other);
        }
    }
}