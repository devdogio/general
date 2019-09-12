using System;
using System.Collections;
using UnityEngine;

namespace Devdog.General.UI
{
    [RequireComponent(typeof(UIWindow))]
    public partial class UIWindowInputHandler : MonoBehaviour, IUIWindowInputHandler
    {
        public KeyCode keyCode = KeyCode.None;

        protected UIWindow window;
        protected virtual void Awake()
        {
            window = GetComponent<UIWindow>();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                window.Toggle();
            }
        }
    }
}
