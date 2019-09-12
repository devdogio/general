using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Localization.Editors
{
    public static class LocalizationEditorUtility
    {
        private static readonly GUIStyle _toolbarButtonStyle = (GUIStyle)"toolbarbutton";


        private static FieldInfo _GetLocalizedObjectKey(object str)
        {
            return str.GetType().GetField("_key", BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static string GetLocalizedObjectKey(object str)
        {
            if (str == null)
            {
                return string.Empty;
            }

            return _GetLocalizedObjectKey(str).GetValue(str).ToString();
        }

        public static void SetLocalizedObjectKey(object str, string key)
        {
            _GetLocalizedObjectKey(str).SetValue(str, key);
        }

//        public static void SetLocalizedStringKey(LocalizedString str, string key)
//        {
//            var field = _GetLocalizedObjectKey(str);
////            var msg = str.message;
//            field.SetValue(str, key);
////            str.message = msg; // New key, set the message again.
//        }

        public static void DrawLanguagePicker(ref Rect rect, string key, Action<string> keyRenameCallback)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width -= 20f;

            var dbs = LocalizationManager.GetAvailableLanguageDatabases();
            var shownLanguages = dbs.Where(o => o != null).Select(o => new GUIContent(o.lang)).Take((int)Mathf.Max(3f, rect.width / 60f)).ToArray();
            if (shownLanguages.Length > 0)
            {
                EditorGUI.BeginChangeCheck();
                int index = GetCurrentLanguageIndex(shownLanguages, LocalizationManager.currentLanguage);
                index = GUI.Toolbar(rect, index, shownLanguages, _toolbarButtonStyle);
                if (EditorGUI.EndChangeCheck() && index >= 0 && index <= shownLanguages.Length - 1)
                {
                    LocalizationManager.SetLanguage(shownLanguages[index].text);
                    GUIUtility.keyboardControl = -1;
                    GUIUtility.hotControl = -1;
                }
            }
            else
            {
                GUI.Button(rect, "No languages", _toolbarButtonStyle);
            }


            var r = rect;
            r.width = 20f;
            r.x += rect.width;
            if (GUI.Button(r, "...", _toolbarButtonStyle))
            {
                var menu = new GenericMenu();
                foreach (var lang in dbs)
                {
                    menu.AddItem(new GUIContent("Languages/" + lang.lang), LocalizationManager.currentDatabase == lang, (db) =>
                    {
                        LocalizationManager.SetLanguage((LocalizationDatabase)db);

                    }, lang);
                }

                menu.AddItem(new GUIContent("Change key"), false, () =>
                {
                    ChangeKeyEditorWindow.ShowWindow(key, keyRenameCallback);
                });

                menu.ShowAsContext();
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            rect.width += 20f;
        }

        private static int GetCurrentLanguageIndex(GUIContent[] langs, string currentLanguage)
        {
            for (int i = 0; i < langs.Length; i++)
            {
                if (langs[i].text == currentLanguage)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
