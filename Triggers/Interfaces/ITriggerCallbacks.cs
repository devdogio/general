using System;
using System.Collections.Generic;

namespace Devdog.General
{
    public interface ITriggerCallbacks
    {
        /// <returns>Return true if event is consumed. Return false if the others should still receive the event callback.</returns>
        bool OnTriggerUsed(Player player);

        /// <returns>Return true if event is consumed. Return false if the others should still receive the event callback.</returns>
        bool OnTriggerUnUsed(Player player);
    }
}
