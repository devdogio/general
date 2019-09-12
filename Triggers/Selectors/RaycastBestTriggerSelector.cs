using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General
{
    [CreateAssetMenu(menuName = "Devdog/Raycast trigger selector")]
    public class RaycastBestTriggerSelector : BestTriggerSelectorBase
    {
        public LayerMask layerMask = -1;
        public float raycastDistance = 10f;

        /// <summary>
        /// Get the best trigger based on distance and if it's in front of the player or not.
        /// </summary>
        public override TriggerBase GetBestTrigger(Player player, List<TriggerBase> triggersInRange)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                return null;
            }

            // Raycast from center of screen
            Debug.DrawRay(camera.transform.position, (camera.transform.forward * raycastDistance), Color.red, 0.2f);

            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, raycastDistance, layerMask))
            {
                var trigger = hit.transform.GetComponent<TriggerBase>();
                if (trigger != null && trigger.enabled)
                {
                    return trigger;
                }
            }

            return null;
        }
    }
}