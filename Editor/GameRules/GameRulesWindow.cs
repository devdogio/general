using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.GameRules
{
    public class GameRulesWindow : BetterEditorWindow
    {
        internal static event Action<List<IGameRule>> OnIssuesUpdated;

        protected static List<IGameRule> allRules = new List<IGameRule>(); 
        protected static List<IGameRule> activeRules = new List<IGameRule>();
        protected static Vector2 scrollPos;
        protected static GameRulesWindow window;
        protected static bool showRules = false;

        protected static bool needsUpdate = false;
        private static Type[] _allGameRuleTypes = new Type[0];
        private static IGameRule[] _rules = new IGameRule[0];

        [MenuItem("Tools/Devdog/Setup wizard", false, 2)] // Always at bottom
        public static void ShowWindow()
        {
            window = GetWindow<GameRulesWindow>(true, "Devdog - Setup wizard", true);
            window.minSize = new Vector2(400, 500);
            window.continuousRepaint = false;

            CheckForIssues();
            window.Repaint();
        }

        [InitializeOnLoadMethod]
        protected static void InitializeOnLoadMethod()
        {
            needsUpdate = true;
            UpdateRules();
        }

        public static IGameRule[] GetAllRules()
        {
            if (_rules != null && _rules.Length > 0)
            {
                return _rules;
            }

            _allGameRuleTypes = ReflectionUtility.GetAllTypesThatImplement(typeof (IGameRule), true);
            var rules = new List<IGameRule>(_allGameRuleTypes.Length);
            foreach (var type in _allGameRuleTypes)
            {
                rules.Add((IGameRule)Activator.CreateInstance(type));
            }

            for (int i = rules.Count - 1; i >= 0; i--)
            {
                var hideAttribute = (HidesGameRuleAttribute)rules[i].GetType().GetCustomAttributes(typeof(HidesGameRuleAttribute), true).FirstOrDefault();
                if (hideAttribute != null)
                {
                    rules.RemoveAll(o => o.GetType() == hideAttribute.type);
                }
            }

            _rules = rules.ToArray();
            return _rules;
        }

        public static IGameRule[] GetAllActiveRules()
        {
            var rules = GetAllRules();
            return rules.Where(o => o.ignore == false).ToArray();
        }

        public static void UpdateRules()
        {
            allRules.Clear();
            allRules.AddRange(GetAllRules());
            foreach (var rule in allRules)
            {
                if (rule.issues != null)
                {
                    rule.issues.Clear();
                }
            }

            activeRules.Clear();
            activeRules.AddRange(GetAllActiveRules());
        }

        public static void CheckForIssues()
        {
            needsUpdate = false;

            UpdateRules();

            for (int i = 0; i < activeRules.Count; i++)
            {
                UnityEditor.EditorUtility.DisplayProgressBar("Scanning...", "Searching project for issues...", ((float)(i + 1)) / activeRules.Count);
                try
                {
                    activeRules[i].UpdateIssue();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e.Message + "\n" + e.StackTrace);
                }
            }

            UnityEditor.EditorUtility.ClearProgressBar();

            if (OnIssuesUpdated != null)
            {
                OnIssuesUpdated(activeRules);
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            DrawHeaderToolbar();

            if (showRules)
            {
                EditorGUILayout.BeginVertical(Devdog.General.Editors.EditorStyles.boxStyle);

                foreach (var rule in allRules)
                {
                    if (rule.ignore)
                    {
                        GUI.color = Color.grey;
                    }

                    EditorGUI.BeginChangeCheck();
                    var result = EditorGUILayout.ToggleLeft(rule.saveName, !rule.ignore);
                    if (EditorGUI.EndChangeCheck())
                    {
                        rule.ignore = !result;
                    }

                    GUI.color = Color.white;
                }

                EditorGUILayout.EndVertical();
            }

            if (needsUpdate)
            {
                EditorGUILayout.HelpBox("Not scanned. Hit Force rescan", MessageType.Warning);
            }
            else
            {
                if (activeRules.Sum(o => o.ignore == false && o.issues.Count != 0 ? 1 : 0) == 0)
                {
                    EditorGUILayout.HelpBox("No problems found...", MessageType.Info);
                }
            }

            foreach (var rule in activeRules)
            {
                var issues = rule.issues;
                for (int i = issues.Count - 1; i >= 0; i--)
                {
                    var issue = issues[i];
                    EditorGUILayout.HelpBox(issue.message, issue.messageType);

                    GUILayout.BeginHorizontal("Toolbar");
                    foreach (var action in issue.actions)
                    {
                        if (action.name.ToLower().Contains("fix"))
                        {
                            GUI.color = Color.green;
                        }

                        if (GUILayout.Button(action.name, "toolbarbutton"))
                        {
                            action.action();

                            if (action.name.ToLower().Contains("fix"))
                            {
                                issues.RemoveAt(i);
                            }
                        }

                        GUI.color = Color.white;
                    }

                    if (issue.messageType < MessageType.Error)
                    {
                        GUI.color = Color.yellow;
                        if (GUILayout.Button("Ignore", "toolbarbutton"))
                        {
                            rule.ignore = true;
                            issues.RemoveAt(i);
                        }
                        GUI.color = Color.white;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }

        protected void DrawHeaderToolbar()
        {
            GUILayout.BeginHorizontal("Toolbar");

            if (GUILayout.Button("Show rules", "toolbarbutton"))
            {
                showRules = !showRules;
            }

            GUI.color = Color.green;
            if (GUILayout.Button("Force rescan", "toolbarbutton"))
            {
                CheckForIssues();
                Repaint();
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }

    }
}
