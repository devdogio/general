using System;
using UnityEngine;

namespace Devdog.General
{
    public class PositionAtPlayer : MonoBehaviour
    {
        public Vector3 offset;
        public bool continuous = false;


        protected void OnEnable()
        {
            PositionNow();
        }

        protected void LateUpdate()
        {
            if (continuous)
            {
                PositionNow();
            }
        }

        private void PositionNow()
        {
            if (PlayerManager.instance == null || PlayerManager.instance.currentPlayer == null)
            {
                return;
            }

            transform.position = PlayerManager.instance.currentPlayer.transform.position;
        }
    }
}
