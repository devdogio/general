#if REWIRED

using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General.UI
{
    [RequireComponent(typeof(UIWindow))]
    public sealed class RewiredTriggerUI : MonoBehaviour
    {
        [System.Serializable]
        public class Binder
        {
            public string actionName;
            public Sprite icon;
        }

        public UnityEngine.UI.Image imageIcon;
        public UnityEngine.UI.Text shortcutText;
        public bool moveToTriggerLocation = true;


        [SerializeField]
        private Binder[] _binders = new Binder[0];
        private UIWindow _window;

        private void Awake()
        {
            _window = GetComponent<UIWindow>();
        }

        private void Start()
        {
            PlayerManager.instance.OnPlayerChanged += OnPlayerChanged;
            if (PlayerManager.instance.currentPlayer != null)
            {
                OnPlayerChanged(null, PlayerManager.instance.currentPlayer);
            }
        }

        private void LateUpdate()
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

        private void UpdatePosition(TriggerBase trigger)
        {
            if(trigger != null)
                transform.position = Camera.main.WorldToScreenPoint(trigger.transform.position);
        }

        private void Repaint(TriggerBase trigger)
        {
            _window.Show();

            Sprite icon = null;
            string actionName = "";
            if (trigger != null)
            {
                var input = trigger.GetComponent<ITriggerInputHandler>();
                if (input != null)
                {
                    var binder = _binders.FirstOrDefault(o => o.actionName == input.actionInfo.actionName);
                    if(binder != null)
                    {
                        icon = binder.icon;
                        actionName = binder.actionName;
                    }
                    else
                    {
                        icon = input.actionInfo.icon;
                        actionName = input.actionInfo.actionName;
                    }
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

#endif