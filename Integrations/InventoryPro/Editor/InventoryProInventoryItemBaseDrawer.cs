//#if INVENTORY_PRO
//
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using Devdog.General;
//using Devdog.General.Editors.ReflectionDrawers;
//using Devdog.InventoryPro;
//using UnityEditor;
//using UnityEngine;
//
//namespace Devdog.QuestSystemPro.Integration.InventoryPro.Editor
//{
//    [CustomDrawer(typeof(Asset<InventoryItemBase>))]
//    public class InventoryProInventoryItemBaseDrawer : AssetDrawer
//    {
//        public InventoryProInventoryItemBaseDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
//            : base(fieldInfo, value, parentValue, arrayIndex)
//        {
//        }
//
//        protected override int GetHeightInternal()
//        {
//            return ReflectionDrawerStyles.singleLineHeight;
//        }
//
//        protected override object DrawInternal(Rect rect)
//        {
//            var c = children[0];
//            if (c.value == null)
//                GUI.color = Color.yellow;
//
//            rect.width -= 30;
//            var obj = (UnityEngine.Object) c.value;
//            if (GUI.Button(rect, obj != null ? obj.name : "(No item selected)", EditorStyles.objectField))
//            {
//                var picker = EditorWindow.GetWindow<InventoryItemPicker>(true);
//                picker.Show(ItemManager.database);
//
//                picker.OnPickObject += (item) =>
//                {
//                    c.value = item;
//                    c.NotifyValueChanged(c.value);
//                };
//            }
//
//            var p = rect;
//            p.width = 30;
//            p.x += rect.width + 8; // 8 for margin
//            if (GUI.Toggle(p, true, "", "VisibilityToggle") == false)
//            {
//                Selection.activeObject = (UnityEngine.Object)c.value;
//            }
//
//            GUI.color = Color.white;
//            return value;
//        }
//    }
//}
//
//#endif