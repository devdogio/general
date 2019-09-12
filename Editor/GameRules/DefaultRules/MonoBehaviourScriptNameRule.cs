#if __

// Disabled, as it caused confusion..

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;

namespace Devdog.General.Editors.GameRules
{
    public class MonoBehaviourScriptNameRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
            // Check if there are monobehaviours who's file name don't match the class name.
            var r = new Regex(@"([a-zA-Z0-9_-]+)\.+(?:cs|js)+");

            var scripts = AssetDatabase.FindAssets("t:MonoScript");
            foreach (var guid in scripts)
            {
                var scriptPath = AssetDatabase.GUIDToAssetPath(guid);
                var s = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
                if (s != null)
                {
                    if (string.IsNullOrEmpty(s.name) == false && s.text.Contains("MonoBehaviour"))
                    {
                        var match = r.Match(scriptPath);
                        if (match.Groups.Count != 2 || s.GetClass() == null)
                        {
                            CreateIssue(scriptPath);
                        }
                        else
                        {
                            if (s.GetClass().Name != match.Groups[1].Value)
                            {
                                CreateIssue(scriptPath);
                            }
                        }
                    }
                }
            }
        }

        private void CreateIssue(string path)
        {
            issues.Add(new GameRuleIssue("MonoBehaviour file name doesn't match class name", MessageType.Warning, new GameRuleAction("Select",
                () =>
                {
                    UnityEditor.Selection.objects = new Object[] { AssetDatabase.LoadAssetAtPath<MonoScript>(path) };

                })));
        }
    }
}

#endif