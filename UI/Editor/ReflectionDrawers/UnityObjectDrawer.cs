using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public class UnityObjectDrawer : SimpleValueDrawer
    {
        protected bool allowSceneObjects;
        protected bool forceUnityPicker;

        public override bool isEmpty
        {
            get
            {
                var obj = (UnityEngine.Object) value; // Cast first, otherwise unity thinks it's not null (wrapped C# / C++ object fails check for some reason)
                return obj == null || obj.Equals(null);
            }
        }

        public UnityObjectDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            if (fieldInfo != null)
            {
                allowSceneObjects = fieldInfo.GetCustomAttributes(typeof(AllowSceneObjectsAttribute), true).Length > 0;
                forceUnityPicker = fieldInfo.GetCustomAttributes(typeof (ForceStandardObjectPickerAttribute), true).Length > 0;
            }
        }

        protected override object DrawInternal(Rect rect)
        {
            rect = GetSingleLineHeightRect(rect);

            var unityEngineObject = (UnityEngine.Object)value; // Cast first, otherwise unity thinks it's not null (wrapped C# / C++ object fails check for some reason)
            if (forceUnityPicker)
            {
                EditorGUI.BeginChangeCheck();
                this.value = EditorGUI.ObjectField(rect, unityEngineObject, GetFieldType(false), true);
                if (EditorGUI.EndChangeCheck() || ReferenceEquals(unityEngineObject, value) == false || GUI.changed)
                {
                    NotifyValueChanged(value);
                }
            }
            else
            {

                ObjectPickerUtility.RenderObjectPickerForType(rect, fieldName.text, unityEngineObject, GetFieldType(false), o =>
                {
                    this.value = o;
                    NotifyValueChanged(value);
                });
            }

            return value;
        }
    }
}
