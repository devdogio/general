using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;

namespace Devdog.General.Editors.GameRules
{
    public class RequiredAttributeRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
            var comps = Resources.FindObjectsOfTypeAll<UnityEngine.Component>();
            foreach (var component in comps)
            {
                var t = component.GetType();
                if (t.Namespace == null || t.Namespace.Contains("Devdog") == false)
                {
                    continue;
                }

                UpdateIssuesFromType(component);
            }
            
            var scriptableObjects = Resources.FindObjectsOfTypeAll<UnityEngine.ScriptableObject>();
            foreach (var scriptableObject in scriptableObjects)
            {
                var t = scriptableObject.GetType();
                if (t.Namespace == null || t.Namespace.StartsWith("Devdog") == false)
                {
                    continue;
                }

                UpdateIssuesFromType(scriptableObject);
            }
        }

        private void UpdateIssuesFromType<T>(T t) where T: UnityEngine.Object
        {
            var drawers = ReflectionDrawerUtility.BuildEditorHierarchy(t.GetType(), t);
            foreach (var drawer in drawers)
            {
                UpdateIssueFromReflectionDrawerRecursive(drawer, t);
            }
        }

        private void UpdateIssueFromReflectionDrawerRecursive(DrawerBase parent, UnityEngine.Object root)
        {
            var childrenDrawer = parent as IChildrenDrawer;
            if (childrenDrawer != null)
            {
                foreach (var child in childrenDrawer.children)
                {
                    UpdateIssueFromReflectionDrawerRecursive(child, root);
                }

                return;
            }

            if (parent != null && parent.required && parent.isEmpty)
            {
                CreateIssue(parent.fieldInfo, root);
            }
        }

        private void CreateIssue<T>(FieldInfo field, T comp) where T : UnityEngine.Object
        {
            var compTemp = comp;
            var fieldTemp = field;
            issues.Add(new GameRuleIssue("Field '" + field.Name + "' (" + field.FieldType.Name + ") on '" + comp.GetType().Name + "' is required.", MessageType.Error,
                new GameRuleAction("Select", () =>
                {
                    SelectObject(compTemp, fieldTemp);
                })));
        }
    }
}