using UnityEngine;
using UnityEditor;
using System;

namespace Devdog.General.Editors
{
    [CustomEditor(typeof(TriggerRangeHandler), true)]
    [CanEditMultipleObjects]
    public class TriggerRangeHandlerEditor : Editor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        public static void DrawRangeHandler(TriggerRangeHandler rangeHandler, GizmoType gizmoType)
        {
            var color = Color.cyan;
            color.a = 0.2f;

            Handles.color = color;
            var euler = rangeHandler.transform.rotation.eulerAngles;
            euler.x += 90.0f;
            Handles.DrawSolidDisc(rangeHandler.transform.position, Vector3.up, rangeHandler.useRange);

            color.a = 1f;
            Handles.color = color;

#if UNITY_2017_1_OR_NEWER
            Handles.CircleHandleCap(0, rangeHandler.transform.position, Quaternion.Euler(euler), rangeHandler.useRange, EventType.Repaint);
#else
            Handles.CircleCap(0, rangeHandler.transform.position, Quaternion.Euler(euler), rangeHandler.useRange);
#endif
        }
    }
}