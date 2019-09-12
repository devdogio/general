using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Reflection;
using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.General.UI
{
    public static class UIUtility
    {
        private static Dictionary<Type, MethodInfo> _reflectionCache = new Dictionary<Type, MethodInfo>();

        public static bool isFocusedOnInput
        {
            get
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                    if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
                        return true;

                return false;
            }
        }

        public static GameObject currentlySelectedGameObject
        {
            get
            {
                if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
                    return null;

                return EventSystem.current.currentSelectedGameObject;
            }
        }

        public static bool isHoveringUIElement
        {
            get
            {
                if (EventSystem.current == null || EventSystem.current.currentInputModule == null)
                {
                    return false;
                }

                var inputModuleType = EventSystem.current.currentInputModule.GetType();

                MethodInfo methodInfo;
                _reflectionCache.TryGetValue(inputModuleType, out methodInfo);
                if (methodInfo == null)
                {
                    methodInfo = inputModuleType.GetMethod("GetLastPointerEventData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    _reflectionCache[inputModuleType] = methodInfo;
                    // Assert.IsNotNull(methodInfo, "Couldn't find GetLastPointerEventData on input module! (please report this bug...)");
                }

                if(methodInfo == null)
                {
                    return false;
                }

                var eventData = (PointerEventData) methodInfo.Invoke(EventSystem.current.currentInputModule, new object[] { PointerInputModule.kMouseLeftId });
                if (eventData != null && eventData.pointerEnter)
                {
                    return eventData.pointerEnter.layer == 5; // 5 is Unity's UI layer
                }

                return false;
            }
        }

        /// <summary>
        /// Cast a ray to test if screenPosition is over any UI object in canvas. This is a replacement
        /// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
        /// </summary>
        public static bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
        {
            return PointerOverUIObject(canvas, screenPosition).Count > 0;
        }

        public static List<RaycastResult> PointerOverUIObject(Canvas canvas, Vector2 screenPosition)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;

            GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(eventDataCurrentPosition, results);

            return results;
        }


        public static void ResetTransform(Transform transform)
        {
            transform.ResetTRSRect();
        }

        public static void InheritParentSize(Transform transform)
        {
            var r = transform.GetComponent<RectTransform>();
            Assert.IsNotNull(r, "No RectTransform found on " + transform.name);

            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.sizeDelta = Vector2.one;
            r.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}
