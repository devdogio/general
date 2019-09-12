#if REWIRED

using UnityEngine;
using Devdog.General.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.General.Integration.RewiredIntegration
{
    public class TriggerRewiredInputHandler : TriggerInputHandlerBase
    {
        public override TriggerActionInfo actionInfo
        {
            get
            {
                return new TriggerActionInfo()
                {
                    actionName = _helper.rewiredActionName,
                    icon = uiSprite
                };
            }
        }

        [SerializeField]
        private RewiredHelper _helper = new RewiredHelper();

        /// <summary>
        /// The sprite shown in the UI (shown in TriggerUI)
        /// </summary>
        public Sprite uiSprite;
        public bool triggerMouseClick = true;

        public bool useCursorIcon = true;
        public CursorIcon cursorIcon; // TODO: probably move this to settings.

        private TriggerBase _trigger;
        protected override void Awake()
        {
            base.Awake();

            _helper.Init();

            _trigger = GetComponent<TriggerBase>();
            Assert.IsNotNull(_trigger, "No TriggerBase found on object.");
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (useCursorIcon)
            {
                cursorIcon.Enable();
            }
        }

        public override bool AreKeysDown()
        {
            return _helper.player.GetButtonDown(_helper.rewiredActionName);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (triggerMouseClick)
            {
                Use();
            }
        }
    }
}

#endif