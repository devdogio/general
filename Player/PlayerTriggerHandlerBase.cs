using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public abstract class PlayerTriggerHandlerBase<T> : MonoBehaviour, IPlayerTriggerHandler where T : UnityEngine.Component
    {
        public event Action<TriggerBase, TriggerBase> OnSelectedTriggerChanged;

        public Player player { get; set; }

        [SerializeField]
        private BestTriggerSelectorBase _selector;

        public BestTriggerSelectorBase selector
        {
            get { return _selector; }
            set { _selector = value; }
        }

        private TriggerBase _selectedTrigger;
        public TriggerBase selectedTrigger
        {
            get { return _selectedTrigger; }
            protected set
            {
                var before = _selectedTrigger;
                _selectedTrigger = value;
                if (before != _selectedTrigger)
                {
                    if (OnSelectedTriggerChanged != null)
                    {
                        OnSelectedTriggerChanged(before, _selectedTrigger);
                    }
                }
            }
        }

        public List<TriggerBase> triggersInRange { get; protected set; }

        protected PlayerTriggerHandlerBase()
        {
            triggersInRange = new List<TriggerBase>();
        }
        
        protected virtual void Awake()
        {
            gameObject.layer = 2;

            InvokeRepeating("UpdateSelectedTrigger", 0f, 0.2f);
        }

        protected virtual void UpdateSelectedTrigger()
        {
            if (selector == null)
            {
                return;
            }

            selectedTrigger = selector.GetBestTrigger(player, triggersInRange);
        }

        protected virtual void Update()
        {
            if (selectedTrigger != null)
            {
                var input = selectedTrigger.GetComponent<ITriggerInputHandler>();
                if (input != null && input.AreKeysDown())
                {
                    input.Use();
                    selectedTrigger = null; // Clear it in case the trigger use removes the object. If not the next cycle will find the best trigger again.
                    UpdateSelectedTrigger();
//                    selectedTrigger.Toggle();
                }
            }
            else
            {
                UpdateSelectedTrigger();
            }
        }

        public virtual bool IsInRangeOfTrigger(TriggerBase trigger)
        {
            return triggersInRange.Contains(trigger);
        }

        protected void NotifyTriggerEnter(T other)
        {
            var c = other.GetComponentInChildren<TriggerBase>();
            if (c != null && (c.rangeHandler == null || c.rangeHandler.Equals(null)))
            {
                triggersInRange.Add(c);
                c.NotifyCameInRange(player);
            }
        }

        protected void NotifyTriggerExit(T other)
        {
            var c = other.GetComponentInChildren<TriggerBase>();
            if (c != null && (c.rangeHandler == null || c.rangeHandler.Equals(null)))
            {
                c.NotifyWentOutOfRange(player);
                triggersInRange.Remove(c);
            }
        }
    }
}