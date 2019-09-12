using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Devdog.General.Editors
{
    public class ScriptPickerEditor : GenericObjectPickerBaseEditor<Type>
    {
        private static Type _type;
        private static Type[] _ignoreTypes = new Type[0];
        public static ScriptPickerEditor Get(System.Type type, params System.Type[] ignoreTypes)
        {
            _type = type;
            _ignoreTypes = ignoreTypes;

            var window = GetWindow<ScriptPickerEditor>(true);
            window.windowTitle = "Script type picker";
            window.isUtility = true;

            return window;
        }

        protected override List<Type> FindObjects(bool searchProjectFolder)
        {
            return ReflectionUtility.GetAllTypesThatImplement(_type, true).Where(o => _ignoreTypes.Contains(o) == false).ToList();
        }

        protected override bool MatchesSearch(Type obj, string search)
        {
            return obj.Name.ToLower().Contains(search);
        }

        protected override void DrawObjectButton(Type item)
        {
            if (GUILayout.Button(item.Name))
            {
                NotifyPickedObject(item);
            }
        }
    }
}
