using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.General.Editors.GameRules;
using UnityEditor;
using UnityEngine;

namespace Assets.Devdog.General.Editor.GameRules.DefaultRules
{
    public class MissingComponentReferencesRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in gameObjects)
            {
                var comps = obj.GetComponents<Component>();
                foreach (var comp in comps)
                {
                    if (comp == null)
                    {
                        var o = obj; // Capture list and all
                        issues.Add(new GameRuleIssue("Missing component on object (" + obj.name + ")", MessageType.Warning, new GameRuleAction("Select object", () =>
                        {
                            Selection.activeGameObject = o;
                        })));
                    }
                }
            }
        }
    }
}
