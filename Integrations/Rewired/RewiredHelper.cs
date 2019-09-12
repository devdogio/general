#if REWIRED

using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine.Assertions;

namespace Devdog.General.Integration.RewiredIntegration
{
    [System.Serializable]
    public sealed class RewiredHelper
    {
        public bool useSystemPlayer = false;
        public int rewiredPlayerID = 0;
        public string rewiredActionName;

        public Rewired.Player player;

        public RewiredHelper()
        {
            
        }

        public void Init()
        {
            if (useSystemPlayer)
            {
                player = Rewired.ReInput.players.GetSystemPlayer();
            }
            else
            {
                player = Rewired.ReInput.players.GetPlayer(rewiredPlayerID);
            }

            Assert.IsNotNull(player, "Rewired player with ID " + rewiredPlayerID + " could not be found!");
            Assert.IsTrue(Rewired.ReInput.mapping.Actions.Any(o => o.name == rewiredActionName), "No rewired action found with name: " + rewiredActionName);
        }
    }
}

#endif