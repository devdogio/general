using System;
using System.Reflection;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class FoldoutBlock : IDisposable
    {
        private readonly DrawerBase _drawer;

        public bool isUnFolded
        {
            get { return FoldoutBlockUtility.IsUnFolded(_drawer); }
            set { FoldoutBlockUtility.Set(_drawer, value); }
        }

        public FoldoutBlock(DrawerBase drawer, Rect rect, GUIContent content)
        {
            _drawer = drawer;

            isUnFolded = EditorGUI.Foldout(rect, isUnFolded, content);
        }

        public void Dispose()
        {

        }
    }
}