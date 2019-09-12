using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Devdog.General.Editors
{
    public abstract class IntegrationHelperEditorBase : EditorWindow
    {
        private Vector2 _scrollPos = new Vector2();
        private Color _grayishColor;

        public void OnEnable()
        {
            _grayishColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }

        public void OnGUI()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            DrawIntegrations();

            // ShowIntegration("Behavior Designer", "Behavior trees are used by AAA studios to create a lifelike AI. With Behavior Designer, you can bring the power of behaviour trees to Unity!", "https://www.assetstore.unity3d.com/en/#!/content/15277", "BEHAVIOR_DESIGNER");
            // ShowIntegration("Dialogue System", "Dialogue System for Unity makes it easy to add interactive dialogue to your game. It's a complete, robust solution including a visual node-based editor, dialogue UIs, cutscenes, quests, save/load, and more.", "https://www.assetstore.unity3d.com/en/#!/content/11672", "DIALOGUE_SYSTEM");

            GUILayout.EndScrollView();
        }

        protected abstract void DrawIntegrations();


        protected void ShowIntegration(string name, string description, string link, string defineName, bool showBox = true)
        {
            if (showBox)
                EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(IsEnabled(defineName), name))
            {
                EnableIntegration(defineName);
            }
            else
            {
                DisableIntegration(defineName);
            }
            if (GUILayout.Button("View in Asset store", UnityEditor.EditorStyles.toolbarButton))
                Application.OpenURL(link);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUI.color = _grayishColor;
            EditorGUILayout.LabelField(description, Devdog.General.Editors.EditorStyles.labelStyle);
            GUI.color = Color.white;

            if (showBox)
                EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        }

        protected bool IsEnabled(string name)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains(name);
        }

        protected List<BuildTargetGroup> GetAllTargets()
        {
            var nonObsolete = new List<BuildTargetGroup>();
            
            var t = typeof(BuildTargetGroup);
            foreach (var e in System.Enum.GetValues(t))
            {
                var l = e.ToString();
                if (l == BuildTargetGroup.Unknown.ToString())
                {
                    continue;
                }
                
                var member = t.GetMember(e.ToString()).FirstOrDefault();
                if (member != null)
                {
                    var isObsolete = member.GetCustomAttributes(typeof(System.ObsoleteAttribute), false).Any();
                    if (isObsolete == false)
                    {
                        nonObsolete.Add((BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), e.ToString(), true));
                    }
                }
            }

            return nonObsolete;
        }
        
        protected void DisableIntegration(string name)
        {
            if (IsEnabled(name) == false) // Already disabled
                return;

            foreach (var target in GetAllTargets())
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                string[] items = symbols.Split(';');
                var l = new List<string>(items);
                l.Remove(name);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", l.ToArray()));
            }
            
            ReimportRandomScript("Assets/Devdog");
        }

        protected void EnableIntegration(string name)
        {
            if (IsEnabled(name)) // Already enabled
                return;

            foreach (var target in GetAllTargets())
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                string[] items = symbols.Split(';');
                var l = new List<string>(items);
                l.Add(name);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", l.ToArray()));
            }

            ReimportRandomScript("Assets/Devdog");
        }
        
        private static void ReimportRandomScript (string targetFolder) {
            if (targetFolder != null && !AssetDatabase.IsValidFolder(targetFolder))
                return;

            string[] scripts;
            if (targetFolder == null)
                scripts = AssetDatabase.FindAssets("t:MonoScript");
            else
                scripts = AssetDatabase.FindAssets("t:MonoScript", new string[] { targetFolder });

            if (scripts == null || scripts.Length == 0)
                return;

            var scriptPath = AssetDatabase.GUIDToAssetPath(scripts[0]);
            Debug.Log("Force recompiling code: " + targetFolder + " at script: " + scriptPath);
            AssetDatabase.ImportAsset(scriptPath);
        }

        protected string GetUrlForProductWithID(string ID)
        {
            return "https://www.assetstore.unity3d.com/?asac=ZaKxxhQWGy#!/content/" + ID + "&utm_source=IntegrationWizard";
        }
    }
}