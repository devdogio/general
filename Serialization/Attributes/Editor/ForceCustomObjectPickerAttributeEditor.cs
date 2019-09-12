using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    [CustomPropertyDrawer(typeof(ForceCustomObjectPickerAttribute))]
    public class ForceCustomObjectPickerAttributeEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool isRequiredAndNull = fieldInfo.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0 &&
                                        property.objectReferenceValue == null;

            using (new ColorBlock(RequiredAttributeEditor.ErrorColor, isRequiredAndNull))
            {
                ObjectPickerUtility.RenderObjectPickerForType(position, label.text, property, fieldInfo.FieldType);
            }

            EditorGUI.EndProperty();
        }
    }
}