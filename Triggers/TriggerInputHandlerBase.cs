using System;
using System.Collections.Generic;
using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.General
{
    public abstract class TriggerInputHandlerBase : MonoBehaviour, ITriggerInputHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public abstract TriggerActionInfo actionInfo { get; }

        protected TriggerBase trigger;

        
//        protected static CursorIcon cursorIconBefore;

        protected virtual void Awake()
        {
            trigger = GetComponent<TriggerBase>();
            Assert.IsNotNull(trigger, "TriggerInputHandlerBase used but no TriggerBase found on object.");
        }

        protected virtual void Update()
        {
            // Key events are handled in PlayerTriggerHandlerBase for performance reasons.
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            TriggerUtility.mouseOnTrigger = trigger;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (TriggerUtility.mouseOnTrigger == trigger)
            {
                TriggerUtility.mouseOnTrigger = null;
            }
            
            // Also called on game quit, which might destroy the manager first.
            if(GeneralSettingsManager.instance != null)
            {
                GeneralSettingsManager.instance.settings.defaultCursor.Enable();
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {

        }

        public abstract bool AreKeysDown();

        public virtual void Use()
        {
            trigger.Toggle();
        }
    }
}
