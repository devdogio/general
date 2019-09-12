using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class ScrollableBlock : IDisposable
    {
//        public Vector2 scrollPos;

        public ScrollableBlock(Rect rect, ref Vector2 scrollPos, int scrollHeight)
        {
            scrollPos = GUI.BeginScrollView(rect, scrollPos, new Rect(Vector2.zero, new Vector2(rect.size.x - 20f, scrollHeight)));
        }

        public void Dispose()
        {
            GUI.EndScrollView();
        }
    }
}