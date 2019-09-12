using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class GroupBlock : IDisposable
    {
        public GroupBlock(Rect rect)
            : this(rect, GUIContent.none, GUIStyle.none)
        { }

        public GroupBlock(Rect rect, GUIContent content)
            : this(rect, content, GUIStyle.none)
        { }

        public GroupBlock(Rect rect, GUIContent content, GUIStyle style)
        {
            GUI.BeginGroup(rect, content, style);
        }

        public void Dispose()
        {
            GUI.EndGroup();
        }
    }
}