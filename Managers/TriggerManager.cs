using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General
{
    public partial class TriggerManager : ManagerBase<TriggerManager>
    {
        public static event Action<TriggerBase, TriggerBase> OnCurrentTriggerChanged;


        private static TriggerBase _currentActiveTrigger;
        public static TriggerBase currentActiveTrigger
        {
            get { return _currentActiveTrigger; }
            set
            {
                var before = _currentActiveTrigger;
                _currentActiveTrigger = value;
                if (before != _currentActiveTrigger)
                {
                    if (OnCurrentTriggerChanged != null)
                    {
                        OnCurrentTriggerChanged(before, _currentActiveTrigger);
                    }
                }
            }
        }
    }
}
