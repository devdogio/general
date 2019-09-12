using System;
using System.Collections.Generic;
using Devdog.General.Editors.ReflectionDrawers;

namespace Devdog.General.Editors
{
    public static class FoldoutBlockUtility
    {
        private static Dictionary<DrawerBase, bool> _folds = new Dictionary<DrawerBase, bool>();

        public static void Set(DrawerBase drawer, bool set)
        {
            _folds[drawer] = set;
        }

        public static bool IsFolded(DrawerBase drawer)
        {
            return !IsUnFolded(drawer);
        }

        public static bool IsUnFolded(DrawerBase drawer)
        {
            if (_folds.ContainsKey(drawer))
            {
                return _folds[drawer];
            }

            _folds[drawer] = false;
            return false;
        }
    }
}