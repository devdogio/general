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
    public abstract class DrawerBase : IEquatable<DrawerBase>
    {
        public object value { get; set; }
        public bool boolValue { get { return (bool) value; } }
        public float floatValue { get { return (float)value; } }
        public int intValue { get { return (int)value; } }
        public uint uintValue { get { return (uint)value; } }
        public string stringValue { get { return (string)value; } }
        public UnityEngine.Object unityObjectValue { get { return (UnityEngine.Object) value; } }


        public object parentValue { get; protected set; }
        public FieldInfo fieldInfo { get; protected set; }
        public virtual bool isEmpty
        {
            get { return value == null || value.Equals(null); }
        }

        public bool hideInProperties { get; protected set; }
        public bool readOnly { get; protected set; }
        public bool required { get; set; }

        public HeaderAttribute headerAttribute { get; protected set; }
        public HideGroupAttribute hideGroupAttribute { get; protected set; }
        public OnlyDerivedTypesAttribute onlyDerivedTypesAttribute { get; protected set; }

        public bool hideGroup
        {
            get
            {
                if (hideGroupAttribute == null)
                {
                    return false;
                }

                if (isInArray)
                {
                    return hideGroupAttribute.includeArrayChildren;
                }

                return true;
            }
        }


        public virtual GUIContent overrideFieldName { get; set; }
        public virtual GUIContent fieldName
        {
            get
            {
                if (overrideFieldName != null)
                {
                    return overrideFieldName;
                }

                string lengthText = "";
                var arr = value as Array;
                if (arr != null)
                {
                    lengthText = " : " + arr.Length;
                }

                if (isInArray)
                {
                    return new GUIContent("Element " + arrayIndex);
                }

                if (fieldInfo == null)
                {
                    return new GUIContent(UnityEditor.ObjectNames.NicifyVariableName(value.GetType().Name) + lengthText);
                }

                return new GUIContent(UnityEditor.ObjectNames.NicifyVariableName(fieldInfo.Name) + lengthText);
            }
        }


        public bool isInArray
        {
            get { return arrayIndex >= 0; }
        }
        public int arrayIndex { get; protected set; }

        protected DrawerBase(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
        {
            this.fieldInfo = fieldInfo;
            this.value = value;
            this.parentValue = parentValue;
            this.arrayIndex = arrayIndex;

            if (fieldInfo != null)
            {
                // Faster than fieldInfo.GetCustomAttributes(type, true), as it creates a LOT of GC
                var customAttributes = fieldInfo.GetCustomAttributes(true);
                
                hideInProperties = customAttributes.Any(o => o.GetType() == typeof (HideInPropertiesAttribute));
                readOnly = customAttributes.Any(o => o.GetType() == typeof(InspectorReadOnlyAttribute));
                required = customAttributes.Any(o => o.GetType() == typeof(RequiredAttribute));
                headerAttribute = (HeaderAttribute)customAttributes.FirstOrDefault(o => o.GetType() == typeof(HeaderAttribute));
                hideGroupAttribute = (HideGroupAttribute)customAttributes.FirstOrDefault(o => o.GetType() == typeof(HideGroupAttribute));
                onlyDerivedTypesAttribute = (OnlyDerivedTypesAttribute)customAttributes.FirstOrDefault(o => o.GetType() == typeof(OnlyDerivedTypesAttribute));
            }
        }


        public static bool operator ==(DrawerBase left, DrawerBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DrawerBase left, DrawerBase right)
        {
            return !Equals(left, right);
        }

        public void RefreshValue()
        {
            if (parentValue != null)
            {
                if (isInArray)
                {
                    var a = (Array)parentValue;
                    value = a.GetValue(arrayIndex);
                }
                else
                {
                    value = fieldInfo.GetValue(parentValue);
                }

                NotifyValueChanged(value);
            }
        }

        public string GetFieldTypeName(bool getLowestType)
        {
            var fieldType = GetFieldType(getLowestType);
            if (fieldType.IsGenericType)
            {
                if (isInArray)
                {
                    var arr = (Array)parentValue;
                    var v = arr.GetValue(arrayIndex);

                    // Couldn't determine type, going with array's type.
                    if (v == null || getLowestType)
                    {
                        return ReflectionDrawerUtility.GetGenericTypeNiceName(fieldType);
                    }

                    if (v.GetType().IsGenericType)
                    {
                        return ReflectionDrawerUtility.GetGenericTypeNiceName(v.GetType());
                    }

                    return v.GetType().Name;
                }

                return ReflectionDrawerUtility.GetGenericTypeNiceName(fieldType);
            }

            return fieldType.Name;
        }

        public Type GetFieldType(bool getLowestType)
        {
            if (isInArray)
            {
                var arr = (Array)parentValue;
                var v = arr.GetValue(arrayIndex);

                // Couldn't determine type, going with array's type.
                if (v == null || getLowestType)
                {
                    return fieldInfo.FieldType.GetElementType();
                }

                return v.GetType();
            }

//            if (onlyDerivedTypesAttribute != null)
//            {
//                return onlyDerivedTypesAttribute.type;
//            }

            if ((getLowestType || value == null) && fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            return value.GetType();
        }

        protected Rect GetSingleLineHeightRect(Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += (ReflectionDrawerStyles.singleLineHeight - rect.height) / 2;

            return rect;
        }

        protected int GetExtraHeight()
        {
            if (headerAttribute != null && isInArray == false)
            {
                return ReflectionDrawerStyles.headerSize;
            }

            return 0;
        }

        public int GetHeight()
        {
            return GetHeightInternal() + GetExtraHeight();
        }

        protected abstract int GetHeightInternal();

        public void Draw(ref Rect rect)
        {
            GUIDisabledBlock block = null;
            if (readOnly)
            {
                block = new GUIDisabledBlock();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
            }


            if (headerAttribute != null && isInArray == false)
            {
                GUI.color = new Color(1, 1, 1, 0.85f);

                rect.y += 12;
                EditorGUI.LabelField(rect, headerAttribute.header, UnityEditor.EditorStyles.boldLabel);
                rect.y += ReflectionDrawerStyles.headerSize - 12;

                GUI.color = Color.white;
            }

            if (required && isEmpty)
            {
                GUI.color = Color.red;
            }

            var val = DrawInternal(rect);
            rect.y += GetHeightInternal();

            if (readOnly && block != null)
            {
                block.Dispose();
            }
            else
            {
                if (EditorGUI.EndChangeCheck())
                {
                    NotifyValueChanged(val);
                }
            }

            GUI.color = Color.white;
        }

        protected abstract object DrawInternal(Rect rect);
        public abstract DrawerBase FindDrawerRelative(string fieldName);


        public virtual void NotifyValueChanged(object newValue)
        {
            if (isInArray)
            {
                var arr = (Array) parentValue;
                arr.SetValue(newValue, arrayIndex);
            }
            else
            {
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(parentValue, newValue);
                }
            }

            GUI.changed = true;
            this.value = newValue;
        }

        public bool Equals(DrawerBase other)
        {
            if (other == null)
            {
                return false;
            }

            return ReferenceEquals(this.value, other.value) && this.arrayIndex == other.arrayIndex;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DrawerBase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((fieldInfo != null ? fieldInfo.GetHashCode() : 0) * 397) ^ arrayIndex;
            }
        }
    }
}
