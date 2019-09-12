using System;
using System.Collections.Generic;

namespace Devdog.General
{
    public interface IPlayerTriggerHandler
    {
        List<TriggerBase> triggersInRange { get; }
        BestTriggerSelectorBase selector { get; set; }

        TriggerBase selectedTrigger { get; }
        event Action<TriggerBase, TriggerBase> OnSelectedTriggerChanged;

        bool IsInRangeOfTrigger(TriggerBase trigger);
    }
}
