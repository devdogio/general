using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class ModuleList<T>
        where T : class
    {
        private static class Style
        {
            public static GUIStyle boxStyle;
            public static GUIStyle moduleTitleStyle;
            public static GUIStyle moduleBGStyle;
            public static GUIStyle removeModuleButtonStyle;
            public static GUIStyle moduleToggleStyle;
            public static GUIStyle moduleTitleLabelStyle;

            public static GUIStyle addModuleButtonStyle;
            public static GUIStyle moveModuleUpButtonStyle;
            public static GUIStyle moveModuleDownButtonStyle;

            static Style()
            {
                boxStyle = new GUIStyle("HelpBox");
                boxStyle.padding = new RectOffset(1, 1, 1, 1);
                boxStyle.fixedHeight = 0;

                moduleTitleStyle = new GUIStyle("ShurikenModuleTitle");
                moduleTitleStyle.alignment = TextAnchor.MiddleLeft;
                moduleTitleStyle.padding = new RectOffset(2, 0, 2, 2);
                moduleTitleStyle.richText = true;
                moduleTitleStyle.fixedHeight = EditorGUIUtility.singleLineHeight + 2;

                moduleBGStyle = new GUIStyle("ShurikenModuleBg");
                moduleBGStyle.fixedHeight = 0;
                moduleBGStyle.padding = new RectOffset(0, 0, 1, 3);
                moduleBGStyle.stretchHeight = false;

                removeModuleButtonStyle = new GUIStyle("ShurikenMinus");
                removeModuleButtonStyle.margin.right = -15;
                removeModuleButtonStyle.margin.top = 4;

                moduleToggleStyle = (GUIStyle)"ShurikenCheckMark";
                moduleTitleLabelStyle = (GUIStyle)"ShurikenLabel";
                addModuleButtonStyle = (GUIStyle)"TE toolbarbutton";

                moveModuleUpButtonStyle = new GUIStyle("Grad Down Swatch");
                moveModuleDownButtonStyle = new GUIStyle("Grad Up Swatch");
            }
        }

        public List<T> list;
        public Action<int> removeModuleAt;
        public Action<System.Type> addModule;
        public Action<UnityEngine.Component> drawModuleEditor;
        public Action drawHeader;
        public string title;
        public string description;
        public Editor editor;
        public UnityEngine.Component target;

        public bool allowMultipleImplementations = true;
        public bool allowDuplicateImplementations = false;
        public bool canDisableModules = true;
        public bool canReorderModules = true;
        public bool hideOriginalComponents = true;

        public ModuleList(UnityEngine.Component target, Editor editor, string title = "Modules")
        {
            this.target = target;
            this.title = title;
            this.editor = editor;

            this.drawHeader = () =>
            {
                GUILayout.Label(title, Devdog.General.Editors.EditorStyles.titleStyle);
                if (string.IsNullOrEmpty(description) == false)
                {
                    GUILayout.Label(description, UnityEditor.EditorStyles.wordWrappedLabel);
                }
            };

            this.addModule = type =>
            {
                Undo.AddComponent(target.gameObject, type);
                
                UpdateList();

                UnityEditor.EditorUtility.SetDirty(target);
                editor.Repaint();
            };

            this.removeModuleAt = i =>
            {
                Undo.DestroyObjectImmediate(list[i] as UnityEngine.Component);

                UpdateList();

                UnityEditor.EditorUtility.SetDirty(target);
                editor.Repaint();
            };

            this.drawModuleEditor = module =>
            {
                var e = UnityEditor.Editor.CreateEditor(module);
                e.serializedObject.Update();

                var childCount = GetChildCount(e.serializedObject);
                if (childCount > 0)
                {
                    EditorGUILayout.BeginVertical(Style.moduleBGStyle);
                    EditorGUI.indentLevel++;

                    SerializedProperty iterator = e.serializedObject.GetIterator();
                    bool enterChildren = true;
                    while (iterator.NextVisible(enterChildren))
                    {
                        enterChildren = false;
                        if (iterator.name != "m_Script")
                        {
                            EditorGUILayout.PropertyField(iterator, true);
                        }
                    }

                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }

                e.serializedObject.ApplyModifiedProperties();
            };
        }

        private int GetChildCount(SerializedObject obj)
        {
            int counter = 0;

            SerializedProperty iterator = obj.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name != "m_Script")
                {
                    counter++;
                }
            }

            return counter;
        }

        public void UpdateList()
        {
            list = target.GetComponents<T>().ToList();
            SetVisibility();
        }

        private void SetVisibility()
        {
            if (list == null)
            {
                return;
            }

            foreach (T a in list)
            {
                var comp = a as UnityEngine.Component;
                if (comp != null)
                {
                    if (hideOriginalComponents)
                    {
                        comp.hideFlags = HideFlags.HideInInspector;
                    }
                    else
                    {
                        comp.hideFlags = HideFlags.None;
                    }
                }
            }
        }

        public void DoLayoutList()
        {
            if (list == null)
            {
                UpdateList();
            }

            RemoveEmptyModules();
            drawHeader();

            EditorGUILayout.BeginVertical(Style.boxStyle);

            for (int i = list.Count - 1; i >= 0; i--)
            {
                DrawModule(list[i], i);
            }

            if (list.Count == 0)
            {
                GUILayout.Space(8);
                EditorGUILayout.LabelField("No " + title.ToLower());
                GUILayout.Space(8);
            }

            EditorGUILayout.EndVertical();

            ShowModuleTypePicker();
        }

        private void RemoveEmptyModules()
        {
            if (list == null)
            {
                return;
            }

            if (list.Contains(null))
            {
                UpdateList();
            }
        }

        private void ShowModuleTypePicker()
        {
            if (list == null)
            {
                UpdateList();
                return;
            }

            const float buttonWidth = 200f;

            Rect rect = GUILayoutUtility.GetRect(buttonWidth, 9999f, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, GUILayout.ExpandHeight(false));
            rect.y -= 4f;
            var width = rect.width;
            rect.width = buttonWidth;
            rect.x += (width - buttonWidth);

            if (allowMultipleImplementations || list.Count == 0)
            {
                if (GUI.Button(rect, new GUIContent("Add module", EditorGUIUtility.FindTexture("d_Toolbar Plus")), Style.addModuleButtonStyle))
                {
                    var ignoreList = new List<Type>();
                    if (allowDuplicateImplementations == false)
                    {
                        ignoreList = list.Select(o => o.GetType()).ToList();
                        ignoreList.AddRange(ReflectionUtility.GetAllTypesThatImplement(typeof(T)).Where(o => typeof(UnityEngine.Component).IsAssignableFrom(o) == false));
                    }

                    var window = ScriptPickerEditor.Get(typeof(T), ignoreList.ToArray());
                    window.Show();
                    window.OnPickObject += type =>
                    {
                        addModule(type);
                    };
                }
            }
        }

        public void DrawModule(T component, int index)
        {
            var module = component as UnityEngine.Component;
            if (module != null)
            {
                EditorGUILayout.BeginHorizontal(Style.moduleTitleStyle);

                var behaviour = module as UnityEngine.Behaviour;
                if (behaviour != null && canDisableModules)
                {
                    behaviour.enabled = GUILayout.Toggle(behaviour.enabled, "", Style.moduleToggleStyle);
                }

                GUILayout.Label(" " + module.GetType().Name, Style.moduleTitleLabelStyle);

                if (canReorderModules)
                {
                    Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, GUILayout.ExpandHeight(false));
                    rect.width = EditorGUIUtility.singleLineHeight;
                    rect.y -= 2f;
                    rect.x = EditorGUIUtility.currentViewWidth - 80f;

                    if (GUI.Button(rect, "", Style.moveModuleUpButtonStyle))
                    {
                        // Up
                        UnityEditorInternal.ComponentUtility.MoveComponentUp(module);
                        UpdateList();
                    }

                    rect.x += EditorGUIUtility.singleLineHeight;
                    if (GUI.Button(rect, "", Style.moveModuleDownButtonStyle))
                    {
                        // Down
                        UnityEditorInternal.ComponentUtility.MoveComponentDown(module);
                        UpdateList();
                    }
                }

                if (GUILayout.Button("", Style.removeModuleButtonStyle, GUILayout.Width(30f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    UnityEngine.Object.DestroyImmediate(module);
                    UpdateList();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if ((behaviour != null && behaviour.enabled) || behaviour == null)
                {
                    drawModuleEditor(module);
                }
            }
        }
    }
}