using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine.Assertions;

namespace Devdog.General
{
    public partial class IgnoreRangeTriggerRangeHandler : MonoBehaviour, ITriggerRangeHandler
    {
        public IEnumerable<Player> GetPlayersInRange()
        {
            return new Player[0];
        }

        public bool IsPlayerInRange(Player target)
        {
            return true;
        }
    }
}