using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public class ArrayDrawer : DrawerBase, IChildrenDrawer
    {
        public List<DrawerBase> children { get; set; }
        private ArrayControlOptionsAttribute _arrayControlOptionsAttribute;
        public bool drawAddButton
        {
            get
            {
                if (_arrayControlOptionsAttribute == null)
                {
                    return true;
                }

                return _arrayControlOptionsAttribute.canAddItems;
            }
        }

        public bool drawRemoveButton
        {
            get
            {
                if (_arrayControlOptionsAttribute == null)
                {
                    return true;
                }

                return _arrayControlOptionsAttribute.canRemoveItems;
            }
        }

        public ArrayDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            if (fieldInfo != null)
            {
                _arrayControlOptionsAttribute = (ArrayControlOptionsAttribute)fieldInfo.GetCustomAttributes(typeof(ArrayControlOptionsAttribute), true).FirstOrDefault();
            }

            children = new List<DrawerBase>();
            Update();
        }

        protected override int GetHeightInternal()
        {
            int add = 0;
            if (drawAddButton)
            {
                add += ReflectionDrawerStyles.singleLineHeight;
            }

            if (hideGroup)
            {
                return children.Sum(o => o.GetHeight()) + add;
            }

            if (FoldoutBlockUtility.IsUnFolded(this))
            {
                add += ReflectionDrawerStyles.singleLineHeight;
                return children.Sum(o => o.GetHeight()) + add;
            }

            return add;
        }

        protected override object DrawInternal(Rect rect)
        {
            if (hideGroup)
            {
                DrawChildren(ref rect);
                if (drawAddButton)
                {
                    DrawArrayAddButton(rect);
                }
            }
            else
            {
                using (var foldout = new FoldoutBlock(this, rect, new GUIContent(fieldName)))
                {
                    rect.y += ReflectionDrawerStyles.singleLineHeight;
                    if (foldout.isUnFolded)
                    {
                        using (var indent = new IndentBlock(rect))
                        {
                            DrawChildren(ref indent.rect);
                            if (drawAddButton)
                            {
                                DrawArrayAddButton(indent.rect);
                            }
                        }
                    }
                }
            }

            return value;
        }

        private void DrawChildren(ref Rect rect)
        {
            if (drawRemoveButton)
            {
                rect.width -= 20f;
            }

            int elementRemoved = -1;
            for (int index = 0; index < children.Count; index++)
            {
                if (drawRemoveButton)
                {
                    var removed = DrawArrayRemoveButton(rect);
                    if (removed)
                    {
                        elementRemoved = index;
                        break;
                    }
                }

                children[index].Draw(ref rect);
            }

            if (elementRemoved != -1)
            {
                RemoveElementAt(elementRemoved);
                Update();
            }

            if (drawRemoveButton)
            {
                rect.width += 20f;
            }
        }

        public override DrawerBase FindDrawerRelative(string fieldName)
        {
            return null;
        }

        private bool DrawArrayRemoveButton(Rect rect)
        {
            rect.x += rect.width + 4;
            rect.width = EditorGUIUtility.singleLineHeight;
            rect.height = EditorGUIUtility.singleLineHeight;

            return GUI.Button(rect, "X");
        }

        private void DrawArrayAddButton(Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            if (GUI.Button(rect, "Add type", "DropDownButton"))
            {
                var derivedTypeInfo = ReflectionDrawerUtility.GetDerivedTypesFrom(fieldInfo.FieldType.GetElementType(), onlyDerivedTypesAttribute != null ? onlyDerivedTypesAttribute.type : null);

                var arrFieldType = fieldInfo.FieldType.GetElementType();
                if (onlyDerivedTypesAttribute != null)
                {
                    arrFieldType = onlyDerivedTypesAttribute.type;
                }

                rect.width -= 25;
                if (rect.Contains(Event.current.mousePosition) && arrFieldType.IsAbstract == false && arrFieldType.IsInterface == false && ReflectionDrawerUtility.IsGenericTypeWithoutGenericArguments(arrFieldType) == false)
                {
                    // Clicked left side
                    AddElement(arrFieldType);
                    Update();
                }
                else
                {
                    // Clicked right side (pick implementation)
                    var n = new GenericMenu();
                    for (int i = 0; i < derivedTypeInfo.types.Length; i++)
                    {
                        n.AddItem(derivedTypeInfo.content[i], false, (obj) =>
                        {
                            
                            AddElement((Type)obj);
                            Update();

                        }, derivedTypeInfo.types[i]);
                    }

                    n.ShowAsContext();
                    Event.current.Use();
                }
            }
        }

        private void AddElement(Type createOfType)
        {
            var arrayValue = (Array) value;

//            Debug.Log("Add array element");
            var newArray = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), arrayValue.Length + 1);
            for (int j = 0; j < newArray.Length - 1; j++)
            {
                newArray.SetValue(arrayValue.GetValue(j), j);
            }

            object newValue = null;

            // Can't construct UnityEngine.Object values
            if (typeof (UnityEngine.Object).IsAssignableFrom(createOfType) == false)
            {
                if(createOfType.IsGenericTypeDefinition)
                {
                    // TODO: User choice for generic type.
                    createOfType = createOfType.MakeGenericType(typeof(float));
                }

                newValue = Activator.CreateInstance(createOfType);
            }

            newArray.SetValue(newValue, newArray.Length - 1);
            value = newArray;

            NotifyValueChanged(value);
        }

        private void RemoveElementAt(int i)
        {
            var arrayValue = (Array) value;

//            Debug.Log("Removing array element " + i);
            var newArray = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), arrayValue.Length - 1);
            for (int j = 0; j < i; j++)
            {
                newArray.SetValue(arrayValue.GetValue(j), j);
            }
            for (int j = i; j < arrayValue.Length - 1; j++)
            {
                newArray.SetValue(arrayValue.GetValue(j + 1), j); // Move items over
            }

            value = newArray;
            NotifyValueChanged(value);
        }

        public void Update()
        {
            children.Clear();
            
            if (value == null)
            {
                value = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), 0);
                NotifyValueChanged(value);
            }

            var arrValue = (Array)value;
            for (int i = 0; i < arrValue.Length; i++)
            {
                var child = ReflectionDrawerUtility.BuildEditorHierarchy(fieldInfo, arrValue, i);
                children.Add(child);
            }

            GUI.changed = true;
        }
    }
}
