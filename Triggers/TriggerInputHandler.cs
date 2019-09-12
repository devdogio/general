using System;
using System.Collections.Generic;
using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.General
{
    public class TriggerInputHandler : TriggerInputHandlerBase
    {
        public override TriggerActionInfo actionInfo
        {
            get
            {
                return new TriggerActionInfo()
                {
                    actionName = triggerKeyCode.ToString()
                };
            }
        }


        [SerializeField]
        private bool _triggerMouseClick = true;
        public virtual bool triggerMouseClick
        {
            get { return _triggerMouseClick; }
        }

        [SerializeField]
        private KeyCode _triggerKeyCode = KeyCode.None;
        public virtual KeyCode triggerKeyCode
        {
            get { return _triggerKeyCode; }
        }


        public bool useCursorIcon = true;

        [SerializeField]
        private CursorIcon _cursorIcon;
        public virtual CursorIcon cursorIcon
        {
            get { return _cursorIcon; }
        }

//        protected override void Update()
//        {
//            base.Update();
//
//            if (useCursorIcon && trigger.inRange && TriggerUtility.mouseOnTrigger && UIUtility.isHoveringUIElement == false)
//            {
//                cursorIcon.Enable();
//            }
//        }

        public override bool AreKeysDown()
        {
            if (_triggerKeyCode == KeyCode.None)
            {
                return false;
            }

            return Input.GetKeyDown(_triggerKeyCode);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (useCursorIcon)
            {
                cursorIcon.Enable();
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (_triggerMouseClick)
            {
                Use();
            }
        }

        public override string ToString()
        {
            return triggerKeyCode.ToString();
        }
    }
}
