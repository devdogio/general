using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;


namespace Devdog.General.Editors
{
    public class BetterUnityEditorBase : Editor
    {
        protected List<DrawerBase> mainDrawers;
        private Vector2 _scrollPos;

        private List<DrawerBase> GetMainDrawers()
        {
            var drawers = ReflectionDrawerUtility.BuildEditorHierarchy(target.GetType(), target).ToList();
            var editor = ReflectionDrawerUtility.TryGetCustomDrawer(null, target, null, -1, target.GetType()) as ChildrenValueDrawerBase;
            if (editor != null)
            {
                editor.children = drawers;
                return new List<DrawerBase>() { editor };
            }

            return drawers;
        }

        public override void OnInspectorGUI()
        {
            if (mainDrawers == null)
            {
                mainDrawers = GetMainDrawers();
            }


            var width = Mathf.Min(EditorGUIUtility.currentViewWidth, 800f) - 50;
            var drawersHeight = mainDrawers.Sum(o => o.GetHeight());

            Rect r = GUILayoutUtility.GetRect(300, width, drawersHeight + 10, drawersHeight + 10, GUILayout.ExpandHeight(false));

            // Some extra padding
            r.y += 5f;

            _scrollPos = GUI.BeginScrollView(r, _scrollPos, new Rect(0, 0, r.width - 20, drawersHeight));

            var drawerRect = new Rect(10, 0, r.width - 35, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            foreach (var drawer in mainDrawers)
            {
                drawer.Draw(ref drawerRect);
            }

            if (EditorGUI.EndChangeCheck())
            {
                UnityEditor.EditorUtility.SetDirty(target);
                // TODO: Add Undo support; Default Unity undo probably won't do due to reflection? Implm. custom with command pattern.
            }

            GUI.EndScrollView();
        }
    }
}
