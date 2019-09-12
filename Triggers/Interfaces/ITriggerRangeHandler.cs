using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public interface ITriggerRangeHandler
    {
        IEnumerable<Player> GetPlayersInRange();
        bool IsPlayerInRange(Player player);
    }
}
