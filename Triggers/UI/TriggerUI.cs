using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General.UI
{
    [RequireComponent(typeof(UIWindow))]
    public partial class TriggerUI : MonoBehaviour
    {
        /// <summary>
        /// Bind an KeyCode to a sprite.
        /// </summary>
        [System.Serializable]
        public class KeyIconBinding
        {
            public KeyCode keyCode;
            public Sprite sprite;
        }

        public UnityEngine.UI.Image imageIcon;
        public UnityEngine.UI.Text shortcutText;
        public bool moveToTriggerLocation = true;

        public KeyIconBinding[] keyIconBindings = new KeyIconBinding[0];



        private UIWindow _window;

        protected virtual void Awake()
        {
            _window = GetComponent<UIWindow>();
        }

        protected virtual void Start()
        {
            PlayerManager.instance.OnPlayerChanged += OnPlayerChanged;
            if (PlayerManager.instance.currentPlayer != null)
            {
                OnPlayerChanged(null, PlayerManager.instance.currentPlayer);
            }
        }

        protected void LateUpdate()
        {
            if(moveToTriggerLocation && PlayerManager.instance.currentPlayer != null)
                UpdatePosition(PlayerManager.instance.currentPlayer.triggerHandler.selectedTrigger);
        }
        
        private void OnPlayerChanged(Player oldPlayer, Player newPlayer)
        {
            if (oldPlayer != null)
            {
                oldPlayer.triggerHandler.OnSelectedTriggerChanged -= BestTriggerChanged;
            }

            newPlayer.triggerHandler.OnSelectedTriggerChanged += BestTriggerChanged;
            BestTriggerChanged(null, newPlayer.triggerHandler.selectedTrigger);
        }

        private void BestTriggerChanged(TriggerBase old, TriggerBase newBest)
        {
            if (newBest != null)
            {
                _window.Show();
                Repaint(newBest);
                if (moveToTriggerLocation)
                {
                    UpdatePosition(newBest);
                }
            }
            else
            {
                _window.Hide();
            }
        }

        protected virtual Sprite GetIcon(ITriggerInputHandler inputHandler, TriggerBase trigger)
        {
            var binding = keyIconBindings.FirstOrDefault(o => o.keyCode.ToString() == inputHandler.ToString());
            if (binding == null)
            {
                DevdogLogger.LogWarning("Couldn't find binding for keycode : " + inputHandler.ToString(), this);
                return null;
            }

            return binding.sprite;
        }
        
        protected virtual void UpdatePosition(TriggerBase trigger)
        {
            if(trigger != null)
                transform.position = Camera.main.WorldToScreenPoint(trigger.transform.position);
        }

        public virtual void Repaint(TriggerBase trigger)
        {
            _window.Show();

            Sprite icon = null;
            string actionName = "";
            if (trigger != null)
            {
                var input = trigger.GetComponent<ITriggerInputHandler>();
                if (input != null)
                {
                    icon = GetIcon(input, trigger);
                    actionName = input.ToString();
                }
            }

            if (imageIcon != null && imageIcon.sprite != icon)
            {
                imageIcon.sprite = icon;
            }

            if (shortcutText != null && shortcutText.text != actionName)
            {
                shortcutText.text = actionName;
            }
        }
    }    
}
