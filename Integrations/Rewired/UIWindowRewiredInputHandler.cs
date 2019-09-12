#if REWIRED

using System.Linq;
using UnityEngine;
using Devdog.General.UI;
using Rewired;
using UnityEngine.Assertions;

namespace Devdog.General.Integration.RewiredIntegration
{
    [RequireComponent(typeof(UIWindow))]
    public sealed class UIWindowRewiredInputHandler : MonoBehaviour, IUIWindowInputHandler
    {

        [SerializeField]
        private RewiredHelper _helper = new RewiredHelper();

        public string[] enableMapCategories = new string[0];
        public string[] disableMapCategories = new string[0];

        private UIWindow _window;

        private void Awake()
        {
            _helper.Init();

            _window = GetComponent<UIWindow>();
            Assert.IsNotNull(_window, "No window found on object");

            _window.OnShow += OnShow;
            _window.OnHide += OnHide;
        }

        private void OnDestroy()
        {
            _window.OnShow -= OnShow;
            _window.OnHide -= OnHide;
        }

        private void OnShow()
        {
            SetMapsEnabled(true);
        }

        private void OnHide()
        {
            SetMapsEnabled(false);
        }

        private void SetMapsEnabled(bool b)
        {
            foreach (var enableMapCategory in enableMapCategories)
            {
                _helper.player.controllers.maps.SetMapsEnabled(b, enableMapCategory);
            }

            foreach (var disableMapCategory in disableMapCategories)
            {
                _helper.player.controllers.maps.SetMapsEnabled(!b, disableMapCategory);
            }
        }

        private void Update()
        {
            if (_helper.player.GetButtonDown(_helper.rewiredActionName))
            {
                _window.Toggle();
            }
        }
    }
}

#endif