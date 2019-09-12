using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    [DisallowMultipleComponent]
    public abstract class TriggerBase : MonoBehaviour // , IMouseCallbacks
    {
        public bool useWhenPlayerComesInRange = false;
        public bool blockPlayerInput = false;

        public bool isInUse
        {
            get { return TriggerManager.currentActiveTrigger == this; }
        }

        [Obsolete("Use TriggerManager.currentActiveTrigger instead.")]
        protected static TriggerBase currentTrigger
        {
            get { return TriggerManager.currentActiveTrigger; }
            set { TriggerManager.currentActiveTrigger = value; }
        }


        private bool _inRange;
        public virtual bool inRange
        {
            get
            {
                if (rangeHandler == null || rangeHandler.Equals(null))
                {
                    return _inRange;
                }

                return rangeHandler.IsPlayerInRange(PlayerManager.instance.currentPlayer);
            }
            protected set { _inRange = value; }
        }

        private ITriggerRangeHandler _rangeHandler;
        public ITriggerRangeHandler rangeHandler
        {
            get
            {
                if (_rangeHandler == null || _rangeHandler.Equals(null))
                {
                    _rangeHandler = GetComponentInChildren<ITriggerRangeHandler>();
                }

                return _rangeHandler;
            }
        }

        private static List<ITriggerCallbacks> _callbacks = new List<ITriggerCallbacks>();

        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {
            if (isInUse)
            {
                if (PlayerManager.instance == null)
                {
                    return; // No need to reset, the player has already been destroyed..
                }

                UnUse();

                Destroy(GetComponent<ITriggerInputHandler>() as UnityEngine.Component);
                Destroy(GetComponent<ITriggerRangeHandler>() as UnityEngine.Component);
            }
        }

        public virtual bool Toggle()
        {
            return Toggle(PlayerManager.instance.currentPlayer);
        }

        public virtual bool Toggle(Player player)
        {
            if (isInUse)
            {
                return UnUse(player);
            }

            return Use(player);
        }

        protected bool CanUse()
        {
            return CanUse(PlayerManager.instance.currentPlayer);
        }

        public virtual bool CanUse(Player player)
        {
            if (enabled == false || inRange == false)
                return false;

            if (isInUse)
                return true;

            if (InputManager.CanReceiveUIInput(gameObject) == false)
                return false;

            return true;
        }

        protected bool CanUnUse()
        {
            return CanUnUse(PlayerManager.instance.currentPlayer);
        }

        public virtual bool CanUnUse(Player player)
        {
            if (enabled == false || inRange == false)
                return false;

            if (isInUse == false)
                return false;

            if (InputManager.CanReceiveUIInput(gameObject) == false)
                return false;

            return true;
        }


        public bool Use()
        {
            return Use(PlayerManager.instance.currentPlayer);
        }

        public abstract bool Use(Player player);

        public bool UnUse()
        {
            return UnUse(PlayerManager.instance.currentPlayer);
        }

        public abstract bool UnUse(Player player);



        public abstract void DoVisuals();
        public abstract void UndoVisuals();


        public virtual void NotifyCameInRange(Player player)
        {
            inRange = true;
            if (useWhenPlayerComesInRange)
            {
                Use(player);
            }
        }
        
        public virtual void NotifyWentOutOfRange(Player player)
        {
            if (isInUse)
            {
                UnUse(player);
            }
            inRange = false;
        }

        private void UpdateTriggerCallbacks()
        {
            GetComponents<ITriggerCallbacks>(_callbacks);
        }

        /// <summary>
        /// Only the first active and enabled component gets the callbacks. In order for the next component to receive the callbacks the first one has to be disabled or removed.
        /// </summary>
        protected virtual void NotifyTriggerUsed(Player player)
        {
            if (blockPlayerInput)
            {
                InputManager.LimitPlayerInputTo(gameObject);
            }

            UpdateTriggerCallbacks();
            for (int i = 0; i < _callbacks.Count; i++)
            {
                var callback = _callbacks[i];
                var callbackBehaviour = callback as Behaviour;
                if (callbackBehaviour != null)
                {
                    if (callbackBehaviour.isActiveAndEnabled == false)
                    {
                        continue;
                    }
                }

                bool eventIsConsumed = callback.OnTriggerUsed(player);
                if (eventIsConsumed)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Only the first active and enabled component gets the callbacks. In order for the next component to receive the callbacks the first one has to be disabled or removed.
        /// </summary>
        protected virtual void NotifyTriggerUnUsed(Player player)
        {
            if (blockPlayerInput)
            {
                InputManager.RemovePlayerLimitInput(gameObject);
            }

            UpdateTriggerCallbacks();
            for (int i = 0; i < _callbacks.Count; i++)
            {
                var callback = _callbacks[i];
                var callbackBehaviour = callback as Behaviour;
                if (callbackBehaviour != null)
                {
                    if (callbackBehaviour.isActiveAndEnabled == false)
                    {
                        continue;
                    }
                }

                bool eventIsConsumed = callback.OnTriggerUnUsed(player);
                if (eventIsConsumed)
                {
                    break;
                }
            }
        }
    }
}
