using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.General.Editors
{
    public abstract class ObjectPickerBaseEditor : BetterEditorWindow
    {
        public enum SearchType
        {
            ObjectTypes,
            Components,
        }

        public int columns
        {
            get
            {
                return Mathf.FloorToInt(position.width / cellMinSize);
            }
        }

        protected const string ScaleSaveKey = "Devdog_ItemPickerScale";
        protected float scale = 1f;
        protected int cellMinSize
        {
            get { return (int)(120f*scale); }
        }

        public int cellSize
        {
            get { return (int)(position.width/columns) - innerPadding; }
        }

        protected int innerPadding = 3;

        public static bool isVisible
        {
            get
            {
                return Resources.FindObjectsOfTypeAll<ObjectPickerBaseEditor>().Length > 0;
            }
        }

        public SearchType searchType;

        protected Object selectedObject;
        private List<Object> _foundObjects = new List<Object>();
        public List<Object> foundObjects
        {
            get { return _foundObjects; }
            protected set { _foundObjects = value; }
        }

        public Dictionary<string, List<Object>> foundObjectsDict = new Dictionary<string, List<Object>>();

        public string currentSearchQuery = "";

        public Action<Object> callback;
        public Type type;
        public bool allowInherited;
        private Vector2 _scrollPos;


        public bool isSearching
        {
            get
            {
                return string.IsNullOrEmpty(currentSearchQuery) == false;
            }
        }

        public virtual void Init()
        {
            foundObjectsDict.Clear();
            foundObjects.Clear();
            scale = EditorPrefs.GetFloat(ScaleSaveKey, 1f);

            if (searchType == SearchType.Components)
            {
                foundObjects.AddRange(FindAssetsWithComponent(type, allowInherited));
            }
            else if (searchType == SearchType.ObjectTypes)
            {
                foundObjects.AddRange(FindAssetsOfType(type, allowInherited));
            }

            foundObjectsDict = CreateDictionary(foundObjects);

            selectedObject = null;
            if (foundObjectsDict.Count > 0)
            {
                selectedObject = foundObjectsDict[foundObjectsDict.Keys.First()].FirstOrDefault();
            }
        }

        protected virtual IEnumerable<UnityEngine.Object> FindAssetsWithComponent(Type type, bool allowInherited)
        {
            var l = new List<UnityEngine.Object>();
            if (typeof (UnityEngine.Component).IsAssignableFrom(type) == false)
            {
                return l; // Can't search, not a component type.
            }

            foreach (var asset in Resources.FindObjectsOfTypeAll(type))
            {
                if (UnityEditor.EditorUtility.IsPersistent(asset) == false)
                    continue;

                if (IsObjectValidType(asset, type, allowInherited))
                {
                    l.Add(asset);
                }
            }

            return l;
        }

        protected virtual IEnumerable<UnityEngine.Object> FindAssetsOfType(Type type, bool allowInherited)
        {
            var l = new List<UnityEngine.Object>();
            foreach (var asset in Resources.FindObjectsOfTypeAll(type).Where(o => UnityEditor.EditorUtility.IsPersistent(o)))
            {
                // Check for types
                if (IsObjectValidType(asset, type, allowInherited))
                {
                    l.Add(asset);
                }
            }

            return l;
        }

        public virtual bool IsObjectValidType(Object asset, Type type, bool allowInherited)
        {
            if (asset.GetType() == type)
            {
                return true;
            }

            if (allowInherited)
            {
                if (type.IsInstanceOfType(asset))
                {
                    return true;
                }
            }

            return false;
        }

        public new void Close()
        {
            base.Close();
        }

        public virtual bool IsSearchMatch(Object asset, string searchQuery)
        {
            if (isSearching == false)
            {
                return true;
            }

            return asset.name.ToLower().Contains(currentSearchQuery.ToLower()) ||
                   asset.GetType().Name.ToLower().Contains(currentSearchQuery.ToLower());
        }

        public override void OnGUI()
        {
            using (new ScrollableBlock(new Rect(0, 0, position.width, position.height), ref _scrollPos, GetInternalHeight(cellSize)))
            {
                base.OnGUI();

                DrawSearchBar();
                DrawZoomControls();
                DrawObjects(cellSize);
            }
        }

        protected virtual int GetInternalHeight(float cellSize)
        {
            return (int)(35 + foundObjectsDict.Sum(kvp => GetCategoryHeight(kvp.Key, cellSize)));
        }

        protected virtual float GetCategoryHeight(string category, float cellSize)
        {
            float height = EditorGUIUtility.singleLineHeight + 30;
            int rows;
            if (isSearching)
            {
                rows = Mathf.CeilToInt((float) foundObjectsDict[category].Count(o => IsSearchMatch(o, currentSearchQuery)) / columns);
            }
            else
            {
                rows = Mathf.CeilToInt((float)foundObjectsDict[category].Count / columns);
            }

            return height + (rows * cellSize + rows * innerPadding);
        }

        protected virtual void DrawObjects(float cellSize)
        {
            float yOffset = 0f;

            foreach (var kvp in foundObjectsDict)
            {
                float categoryHeight = GetCategoryHeight(kvp.Key, cellSize);
                using (new GroupBlock(new Rect(0, yOffset, position.width, categoryHeight)))
                {
                    DrawCategory(kvp.Key, kvp.Value, cellSize);
                }

                yOffset += categoryHeight;
            }
        }

        protected virtual void DrawCategory(string categoryName, IEnumerable<UnityEngine.Object> objs, float cellSize)
        {
            const float topPadding = 20f;

            int row = 0;
            int col = 0;

            EditorGUI.LabelField(new Rect(innerPadding, topPadding + innerPadding + 10f, 200f, EditorGUIUtility.singleLineHeight), categoryName, UnityEditor.EditorStyles.boldLabel);
            float yOffset = EditorGUIUtility.singleLineHeight + 10f;

            foreach (var obj in objs)
            {
                if (obj == null)
                {
                    continue;
                }

                if (IsSearchMatch(obj, currentSearchQuery) == false)
                {
                    continue;
                }

                var r = new Rect(innerPadding + (col * (cellSize + innerPadding)), topPadding + innerPadding + (row * (cellSize + innerPadding)) + yOffset, cellSize, cellSize);
                r.x = Mathf.RoundToInt(r.x); // Avoid ugly anti-aliasing in editor.
                r.y = Mathf.RoundToInt(r.y);

                GUI.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                if (r.Contains(Event.current.mousePosition))
                {
                    GUI.color = Color.white;

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        callback(obj);
                        Close();
                    }
                }

                DrawObject(r, obj);
                GUI.color = Color.white;

                col = (col + 1) % columns;
                if (col % columns == 0)
                {
                    row++;
                }
            }
        }

        private Dictionary<string, List<Object>> CreateDictionary(List<Object> objs)
        {
            var d = new Dictionary<string, List<Object>>();
            foreach (var o in objs)
            {
                if (IsSearchMatch(o, currentSearchQuery) == false)
                {
                    continue;
                }

                if (d.ContainsKey(o.GetType().Name) == false)
                {
                    d[o.GetType().Name] = new List<Object>();
                }

                d[o.GetType().Name].Add(o);
            }

            return d;
        }

        private void DrawSearchBar()
        {
            currentSearchQuery = EditorStyles.SearchBar(new Rect(innerPadding, innerPadding, position.width - innerPadding*2f - 20f, 30f), currentSearchQuery, this, isSearching);
            if (GUI.GetNameOfFocusedControl() != "SearchField")
            {
                GUI.FocusControl("SearchField");
            }
        }

        private void DrawZoomControls()
        {
            EditorGUI.BeginChangeCheck();
            scale = EditorGUI.Slider(new Rect(position.width - 100f, 30f, 140f, EditorGUIUtility.singleLineHeight), scale, 0.2f, 2f);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetFloat(ScaleSaveKey, scale);
            }
        }

        protected virtual string GetObjectName(Object obj)
        {
            return obj.name;
        }

        protected virtual void DrawObject(Rect r, Object obj)
        {
            if (obj == selectedObject)
            {
                var r2 = r;
                r2.x += 3;
                r2.y += 3;
                r2.width -= 6;
                r2.height -= 6;

                GUI.Label(r2, GUIContent.none, "LightmapEditorSelectedHighlight");
            }

            using (new GroupBlock(r, GUIContent.none, "box"))
            {
                var cellSize = r.width;

                var labelRect = new Rect(0, 0, cellSize, EditorGUIUtility.singleLineHeight);
                GUI.Label(labelRect, GetObjectName(obj));
                labelRect.y += EditorGUIUtility.singleLineHeight;
                GUI.Label(labelRect, obj.GetType().Name);

                DrawIcon(obj, cellSize);
            }
        }

        protected virtual void DrawIcon(Object obj, float cellSize)
        {
            var iconSize = Mathf.RoundToInt(cellSize * 0.6f);
            GUI.DrawTexture(new Rect(cellSize * 0.2f, cellSize * 0.4f - innerPadding, iconSize, iconSize), AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(obj)));
        }

        private int GetIndex(Object obj)
        {
            int index = 0;
            foreach (var kvp in foundObjectsDict)
            {
                if (kvp.Key != obj.GetType().Name)
                {
                    index += kvp.Value.Count;
                    continue;
                }

                index += kvp.Value.IndexOf(obj);
                break;
            }

            return index;
        }

        private Object GetObjectFromIndex(int index)
        {
            foreach (var kvp in foundObjectsDict)
            {
                if (index - kvp.Value.Count >= 0)
                {
                    index -= kvp.Value.Count;
                }
                else
                {
                    if (kvp.Value.Count == 0)
                        break;

                    return kvp.Value[index];
                }
            }

            return null;
        }

        private int GetStepSize(Object obj)
        {
            if (obj == null)
            {
                return columns;
            }

            return Mathf.Clamp(foundObjectsDict[obj.GetType().Name].Count, 0, columns);
        }

        protected override void OnKeyDown(KeyCode keyCode)
        {
            int index = 0;
            if (selectedObject != null)
            {
                index = GetIndex(selectedObject);
            }

            if (keyCode == KeyCode.LeftArrow)
                index--;
            else if (keyCode == KeyCode.RightArrow)
                index++;
            else if (keyCode == KeyCode.UpArrow)
                index -= GetStepSize(selectedObject);
            else if (keyCode == KeyCode.DownArrow)
                index += GetStepSize(selectedObject);

            index = Mathf.Clamp(index, 0, Mathf.Max(foundObjects.Count(o => IsSearchMatch(o, currentSearchQuery)) - 1, 0));
            selectedObject = GetObjectFromIndex(index);

            if (keyCode == KeyCode.KeypadEnter || keyCode == KeyCode.Return)
            {
                callback(selectedObject);
                Close();
            }
        }
    }
}
