using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors
{
    public static class EditorStyles
    {

        private static GUIStyle _boxStyle;
        public static GUIStyle boxStyle
        {
            get
            {
                if (_boxStyle == null)
                {
                    _boxStyle = new GUIStyle("HelpBox");
                    _boxStyle.padding = new RectOffset(10, 10, 10, 10);
                }

                return _boxStyle;
            }
        }


        private static GUIStyle _titleStyle;
        public static GUIStyle titleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle("IN TitleText");
                    _titleStyle.alignment = TextAnchor.MiddleLeft;
                    _titleStyle.padding.left += 5;
                    _titleStyle.margin.top += 10;
                }

                return _titleStyle;
            }
        }


        private static GUIStyle _infoStyle;
        public static GUIStyle infoStyle
        {
            get
            {
                if (_infoStyle == null)
                {
                    _infoStyle = new GUIStyle(UnityEditor.EditorStyles.label);
                    _infoStyle.wordWrap = true;
                    //_infoStyle.normal.textColor = new Color(0.6f, 0.4f, 0.1f);
                }

                return _infoStyle;
            }
        }


        private static GUIStyle _richTextArea;
        public static GUIStyle richTextArea
        {
            get
            {
                if (_richTextArea == null)
                {
                    _richTextArea = new GUIStyle(UnityEditor.EditorStyles.textArea)
                    {
                        richText = true,
                        wordWrap = true,
                        fixedHeight = 40.0f,
                        stretchHeight = true
                    };
                }

                return _richTextArea;
            }
        }


        private static GUIStyle _labelStyle;
        public static GUIStyle labelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle(UnityEditor.EditorStyles.label)
                    {
                        wordWrap = true
                    };
                }

                return _labelStyle;
            }
        }

        public static float labelWidth
        {
            get { return 200.0f; }
        }

        private static GUIStyle _toolbarStyle;
        public static GUIStyle toolbarStyle
        {
            get
            {
                if (_toolbarStyle == null)
                {
                    _toolbarStyle = new GUIStyle(UnityEditor.EditorStyles.toolbarButton);
                    _toolbarStyle.fixedHeight = 40;                    
                }

                return _toolbarStyle;
            }
        }




        public static string SearchBar(string searchQuery, EditorWindow window, bool isSearching)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchField");
            string q = EditorGUILayout.TextField(searchQuery, (GUIStyle)"SearchTextField"); // , GUILayout.Width(width)
            if (isSearching)
            {
                if (GUILayout.Button("", (GUIStyle)"SearchCancelButton", GUILayout.Width(17)))
                {
                    q = ""; // Reset
                    if(window != null)
                        window.Repaint();
                }
            }
            else
            {
                GUILayout.Button("", (GUIStyle)"SearchCancelButtonEmpty", GUILayout.Width(17));
            }

            EditorGUILayout.EndHorizontal();

            return q;
        }

        public static string SearchBar(Rect rect, string searchQuery, EditorWindow window, bool isSearching)
        {
            GUI.BeginGroup(rect);
            GUI.SetNextControlName("SearchField");

            var searchFieldRect = rect;
            searchFieldRect.width -= 17;

            string q = EditorGUI.TextField(searchFieldRect, searchQuery, (GUIStyle)"SearchTextField");

            var searchButtonRect = rect;
            searchButtonRect.width = 17;
            searchButtonRect.x += searchFieldRect.width;

            if (isSearching)
            {
                if (GUI.Button(searchButtonRect, "", "SearchCancelButton"))
                {
                    q = ""; // Reset
                    if (window != null)
                        window.Repaint();
                }
            }
            else
            {
                GUI.Button(searchButtonRect, "", "SearchCancelButtonEmpty");
            }

            GUI.EndGroup();
            return q;
        }
    }
}
