using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeEditor : PropertyDrawer
    {
        public static readonly Color ErrorColor = Color.red;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var isRequiredAndEmpty = (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null) ||
                                     (property.propertyType == SerializedPropertyType.String && string.IsNullOrEmpty(property.stringValue));

            using (new ColorBlock(ErrorColor, isRequiredAndEmpty))
            {
                if (isRequiredAndEmpty && string.IsNullOrEmpty(label.tooltip))
                {
                    label.tooltip = "This field is required";
                }

                EditorGUI.PropertyField(position, property, label);
            }

            EditorGUI.EndProperty();
        }
    }
}