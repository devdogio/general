using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Devdog.General.Editors.GameRules
{
    public class ReplacedByAttributeRule : GameRuleBase
    {

        // TODO: Check if this is working...
        public override void UpdateIssue()
        {
            var types = ReflectionUtility.GetAllClassesWithAttribute(typeof (ReplacedByAttribute), true);
            foreach (var currentType in types)
            {
                var newComponentType = (ReplacedByAttribute)currentType.GetCustomAttributes(typeof (ReplacedByAttribute), true).First();
                if (typeof (UnityEngine.Component).IsAssignableFrom(currentType))
                {
                    var components = Resources.FindObjectsOfTypeAll(currentType).Cast<UnityEngine.Component>().ToArray();
                    foreach (var component in components)
                    {
                        try
                        {
                            var tempComponent = component;
                            var tempNewType = newComponentType;
                            issues.Add(new GameRuleIssue("Deprecated type " + tempComponent.GetType() + " is used", MessageType.Error, new GameRuleAction("Fix (replace)",
                                () =>
                                {
                                    if (tempComponent != null && tempComponent.gameObject != null)
                                    {
                                        var newComponent = tempComponent.gameObject.AddComponent(tempNewType.type);
                                        ReflectionUtility.CopySerializableValues(tempComponent, newComponent);
                                        UnityEngine.Object.DestroyImmediate(tempComponent, true);
                                    }

                                }), new GameRuleAction("Select object", () =>
                                {
                                    SelectObject(tempComponent);
                                })));
                        }
                        catch (Exception)
                        {
                            // Ignored
                        }

                        UnityEditor.EditorUtility.SetDirty(component);
                    }
                }
            }
        }
    }
}