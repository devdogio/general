using System;
using System.CodeDom;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors
{
    public abstract class GenericObjectPickerBaseEditor<T> : BetterEditorWindow where T : class
    {
        public Action<T> OnPickObject { get; set; }

        public virtual EditorWindow window { get; protected set; }
        public string windowTitle { get; set; }

        public int selectedIndex { get; protected set; }
        public T selectedObject
        {
            get { return objects[selectedIndex]; }
        }

        protected bool focusOnSearch = true;
        protected string searchQuery;
        protected List<T> objects = new List<T>();
        protected Vector2 scrollPosition;

        public bool isUtility { get; protected set; }

        public bool isSearching
        {
            get { return string.IsNullOrEmpty(searchQuery) == false && searchQuery != "Search..."; }
        }

        protected void NotifyPickedObject(T obj)
        {
            if (OnPickObject != null)
            {
                OnPickObject(obj);
            }

            Close();
        }

        /// <summary>
        /// Find objects of type in asset database.
        /// </summary>
        /// <returns></returns>
        protected abstract List<T> FindObjects(bool searchProjectFolder);


        public new virtual void Show()
        {
            Show(true);
        }

        /// <summary>
        /// Show the window, searches all available items in the asset database.
        /// </summary>
        public new virtual void Show(bool searchProjectFolder)
        {
            Show(FindObjects(searchProjectFolder));
        }

        public virtual void Show(List<T> objectsToPickFrom)
        {
            window = GetWindow(GetType(), isUtility);

            objects = objectsToPickFrom;
            selectedIndex = 0;
            focusOnSearch = true;
            window.titleContent = new GUIContent(windowTitle);
            window.minSize = new Vector2(200, 200);

            window.Show();
        }

        public new virtual void Close()
        {
            if (window != null)
            {
                window.Close();
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            searchQuery = EditorStyles.SearchBar(searchQuery, this, isSearching);
            if (focusOnSearch)
            {
                EditorGUI.FocusTextInControl("SearchField");
                focusOnSearch = false;
            }

            ShowInfoBox();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                EditorGUILayout.BeginHorizontal();

                if (isSearching)
                {
                    string search = searchQuery.ToLower();
                    if (MatchesSearch(obj, search))
                    {
                        if (i == selectedIndex)
                        {
                            GUI.color = Color.green;
                        }

                        DrawObjectButton(obj);
                    }
                }
                else
                {
                    if (i == selectedIndex)
                    {
                        GUI.color = Color.green;
                    }

                    DrawObjectButton(obj);
                }

                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        protected override void OnKeyUp(KeyCode keyCode)
        {
            if (keyCode == KeyCode.DownArrow)
            {
                selectedIndex++;
            }
            else if (keyCode == KeyCode.UpArrow)
            {
                selectedIndex--;
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, objects.Count - 1);


            if (keyCode == KeyCode.Return)
            {
                if (selectedIndex > 0 && selectedObject != null)
                {
                    NotifyPickedObject(selectedObject);
                }
                else
                {
                    EditorGUI.FocusTextInControl("SearchField");
                }
            }

            Repaint();
        }


        protected virtual void DrawObjectButton(T obj)
        {
            if (GUILayout.Button(obj.ToString()))
            {
                NotifyPickedObject(obj);
            }
        }


        protected abstract bool MatchesSearch(T obj, string search);

        protected virtual void ShowInfoBox()
        {
            EditorGUILayout.HelpBox("Use the up and down arrow keys to select an item.\nHit enter to pick the highlighted item.", MessageType.Info);            
        }
    }
}