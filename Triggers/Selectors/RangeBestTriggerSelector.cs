using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    [CreateAssetMenu(menuName = "Devdog/Range trigger selector")]
    public class RangeBestTriggerSelector : BestTriggerSelectorBase
    {
        /// <summary>
        /// Item infront has 20% effect on making the best decision
        /// </summary>
        public float forwardEffect = 0.2f;

        /// <summary>
        /// Get the best trigger based on distance and if it's in front of the player or not.
        /// </summary>
        public override TriggerBase GetBestTrigger(Player player, List<TriggerBase> triggersInRange)
        {
            float bestCheck = -999.0f;
            TriggerBase closestTrigger = null;
            foreach (var item in triggersInRange)
            {
                if (item == null || item.enabled == false || item.gameObject.activeSelf == false)
                {
                    continue;
                }

                var toPlayerVec = item.transform.position - player.transform.position;
                var dist = Vector3.Magnitude(toPlayerVec);

                float inFrontFactor = Mathf.Clamp01(Vector3.Dot(player.transform.forward, toPlayerVec / dist));
                inFrontFactor *= forwardEffect; 

                float final = (GeneralSettingsManager.instance.settings.triggerUseDistance - dist) * inFrontFactor;
                final = Mathf.Abs(final);

                if (final > bestCheck)
                {
                    closestTrigger = item;
                    bestCheck = final;
                }
            }

            return closestTrigger;
        }
    }
}