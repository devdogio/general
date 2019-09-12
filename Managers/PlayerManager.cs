using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    public partial class PlayerManager : ManagerBase<PlayerManager>
    {
        public event Action<Player, Player> OnPlayerChanged;

        private Player _currentPlayer;
        public Player currentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                var before = _currentPlayer;
                _currentPlayer = value;
                if (OnPlayerChanged != null)
                {
                    OnPlayerChanged(before, _currentPlayer);
                }
            }
        }
    }
}
