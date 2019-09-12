using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    [DisallowMultipleComponent]
    public partial class Player2D : Player
    {
        protected override void SetTriggerHandler()
        {
            var obj = new GameObject("_TriggerHandler2D");
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.gameObject.layer = 2; // Ignore raycasts

            var handler = obj.AddComponent<PlayerTriggerHandler2D>();
            handler.player = this;
            handler.selector = triggerSelector;

            triggerHandler = handler;
        }
    }
}