using UnityEngine;
using System.Collections;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine.EventSystems;

namespace Devdog.General.UI
{
    /// <summary>
    /// A page inside an UIWindow. When a tab is clicked the insides of the window are changed, this is a page.
    /// </summary>
    public partial class UIWindowPage : UIWindow
    {
        [Header("Page specific")]
        public bool isDefaultPage = true;

        [SerializeField]
        protected bool _isEnabled = true;
        public bool isEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                myButton.enabled = !isEnabled;

                if (_isEnabled == false)
                {
                    Hide();
                }
            }
        }
  
        /// <summary>
        /// The button that "triggers" this page. leave empty if there is no button.
        /// </summary>
        public UnityEngine.UI.Button myButton;


        private UIWindow _windowParent;
        public UIWindow windowParent
        {
            get
            {
                if (_windowParent == null)
                {
                    _windowParent = transform.parent.GetComponentInParent<UIWindow>();
                }

                return _windowParent;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void LevelStart()
        {
            if (windowParent != null && windowParent.pages.Contains(this) == false)
            {
                windowParent.pages.Add(this);
            }

            base.LevelStart();
        }

        protected override void DoShow(bool resetCurrentPage)
        {
            if (isVisible)
            {
                return;
            }

            base.DoShow(resetCurrentPage);
            windowParent.NotifyChildShown(this);
        }

        protected override void DoHide()
        {
            base.DoHide();
        }
    }
}