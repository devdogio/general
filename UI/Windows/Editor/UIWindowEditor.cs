using System;
using UnityEngine;
using System.Collections;
using Devdog.General.UI;
using UnityEditor;

namespace Devdog.General.Editors
{
    [CustomEditor(typeof(UIWindow), true)]
    [CanEditMultipleObjects]
    public class UIWindowEditor : Editor
    {
        private static ModuleList<IUIWindowInputHandler> _inputHandlers;

        protected void OnEnable()
        {
            var t = (UIWindow) target;
            _inputHandlers = new ModuleList<IUIWindowInputHandler>(t, this, "Input handler");
            _inputHandlers.description = "Input handlers can be used to make the window respond to keypresses.";
            _inputHandlers.allowDuplicateImplementations = true;
        }

        public override void OnInspectorGUI()
        {
            var t = (UIWindow)target;
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                if (t.isVisible)
                {
                    EditorGUILayout.LabelField("Window is Visible");
                    if (GUILayout.Button("Hide"))
                    {
                        t.Hide();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Window is Hidden");
                    if (GUILayout.Button("Show"))
                    {
                        t.Show();
                    }
                }
            }

            _inputHandlers.DoLayoutList();
        }
    }
}