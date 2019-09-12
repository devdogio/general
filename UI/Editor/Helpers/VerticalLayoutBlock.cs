using System;
using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class VerticalLayoutBlock : IDisposable
    {

        public VerticalLayoutBlock()
            : this(GUIStyle.none)
        { }

        public VerticalLayoutBlock(GUIStyle style)
        {
            EditorGUILayout.BeginVertical(style);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }
}