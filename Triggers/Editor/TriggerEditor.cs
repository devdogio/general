using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Devdog.General.Editors
{
    [CustomEditor(typeof(Trigger), true)]
    [CanEditMultipleObjects]
    public class TriggerEditor : Editor
    {
        private SerializedProperty _window;

        private static Color _outOfRangeColor;
        private static Color _inRangeColor;

        private static ModuleList<ITriggerInputHandler> _inputHandlers;
        private static ModuleList<ITriggerRangeHandler> _rangeHandlers;

        public virtual void OnEnable()
        {
            _window = serializedObject.FindProperty("_window");

            var t = (Trigger) target;
            _inputHandlers = new ModuleList<ITriggerInputHandler>(t, this, "Input handler");
            _inputHandlers.description = "Add an input handler to trigger this trigger with a key press, button press, etc";
            _inputHandlers.allowMultipleImplementations = false;

            var rangeHandlerTarget = t.transform.Find("_RangeHandler");
            if (rangeHandlerTarget == null)
            {
                rangeHandlerTarget = t.transform;
            }

            _rangeHandlers = new ModuleList<ITriggerRangeHandler>(rangeHandlerTarget, this, "Range handler");
            _rangeHandlers.description = "Add a range handler to override the default range behavior.";
            _rangeHandlers.allowMultipleImplementations = false;
            _rangeHandlers.hideOriginalComponents = false;
            _rangeHandlers.addModule = AddRangeHandlerModule;

            _outOfRangeColor = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.2f);
            _inRangeColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.3f);
        }

        private void AddRangeHandlerModule(Type type)
        {
            var t = (Trigger) target;
            var rangeHandlerTarget = t.transform.Find("_RangeHandler");
            if (rangeHandlerTarget == null)
            {
                rangeHandlerTarget = new GameObject("_RangeHandler").transform;
                rangeHandlerTarget.SetParent(t.transform);
                rangeHandlerTarget.ResetTRS();
            }

            _rangeHandlers.target = rangeHandlerTarget;
            Undo.AddComponent(rangeHandlerTarget.gameObject, type);

            _rangeHandlers.UpdateList();

            UnityEditor.EditorUtility.SetDirty(target);
            Repaint();
        }

        public void OnSceneGUI()
        {
            var trigger = (TriggerBase)target;
            if (trigger.rangeHandler != null && trigger.rangeHandler.Equals(null) == false)
            {
                return;
            }

            var discColor = _outOfRangeColor;
            if (Application.isPlaying && trigger.inRange)
            {
                discColor = _inRangeColor;
            }

            if (GeneralSettingsManager.instance != null && GeneralSettingsManager.instance.settings != null)
            {
                var useRange = GeneralSettingsManager.instance.settings.triggerUseDistance;

                Handles.color = discColor;
                var euler = trigger.transform.rotation.eulerAngles;
                euler.x += 90.0f;
                Handles.DrawSolidDisc(trigger.transform.position, Vector3.up, useRange);

                discColor.a += 0.2f;
                Handles.color = discColor;

#if UNITY_2017_1_OR_NEWER
                Handles.CircleHandleCap(0, trigger.transform.position, Quaternion.Euler(euler), useRange, EventType.Repaint);
#else
                Handles.CircleCap(0, trigger.transform.position, Quaternion.Euler(euler), useRange);
#endif
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var script = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(script);

            // Draws remaining items
            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "_window",
                script.name
            });

            DrawAddModules();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAddModules()
        {
            var t = (TriggerBase) target;

            DrawWindowContainer(t);

            _inputHandlers.DoLayoutList();
            _rangeHandlers.DoLayoutList();
        }
        
        private void DrawWindowContainer(TriggerBase t)
        {
            var windowHandler = t.gameObject.GetComponent<ITriggerWindowContainer>();
            if (windowHandler == null)
            {
                EditorGUILayout.PropertyField(_window);
            }
            else
            {
                EditorGUILayout.HelpBox("Window is managed by " + windowHandler.GetType().Name, MessageType.Info);
            }
        }
    }
}