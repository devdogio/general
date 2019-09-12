using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.EventSystems;

namespace Devdog.General.Editors.GameRules
{
    public class MainCameraRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
            var cams = UnityEngine.Object.FindObjectsOfType<Camera>();
            foreach (var cam in cams)
            {
                var c = cam;
                if (c.CompareTag("MainCamera"))
                {
                    if (cam.GetComponent<PhysicsRaycaster>() != null)
                    {
                        continue;
                    }

                    issues.Add(new GameRuleIssue("Main camera doesn't have a PhysicsRaycaster, this is required for triggers to work.", MessageType.Warning, new GameRuleAction("Fix (add)",
                        () =>
                        {
                            var raycaster = c.gameObject.AddComponent<PhysicsRaycaster>();
                            raycaster.eventMask ^= (1 << 2);

                        }), new GameRuleAction("Select object", () =>
                        {
                            SelectObject(c);
                        })));
                }
            }
        }
    }
}