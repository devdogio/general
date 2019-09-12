using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General.UI
{
    [RequireComponent(typeof(Animator))]
    public partial class UIWindow : MonoBehaviour
    {
        [System.Serializable]
        public class UIWindowActionEvent : UnityEngine.Events.UnityEvent
        { }


        /// <summary>
        /// Event is fired when the window is hidden.
        /// </summary>
        public event Action OnHide;

        /// <summary>
        /// Event is fired when the window becomes visible.
        /// </summary>
        public event Action OnShow;



        [Header("Behavior")]
        public string windowName = "MyWindow";

        /// <summary>
        /// Should the window be hidden when the game starts?
        /// </summary>
        public bool hideOnStart = true;

        /// <summary>
        /// Set the position to 0,0 when the game starts
        /// </summary>
        public bool resetPositionOnStart = true;


        public bool blockUIInput = false;
        public bool blockPlayerInput = false;

        /// <summary>
        /// The animation played when showing the window, if null the item will be shown without animation.
        /// </summary>
        [Header("Audio & Visuals")]
        [SerializeField]
        private MotionInfo _showAnimation;
        private int _showAnimationHash;

        /// <summary>
        /// The animation played when hiding the window, if null the item will be hidden without animation. 
        /// </summary>
        [SerializeField]
        private MotionInfo _hideAnimation;
        private int _hideAnimationHash;

        public AudioClipInfo showAudioClip;
        public AudioClipInfo hideAudioClip;

        [Header("Actions")]
        public UIWindowActionEvent onShowActions = new UIWindowActionEvent();
        public UIWindowActionEvent onHideActions = new UIWindowActionEvent();

        /// <summary>
        /// Is the window visible or not? Used for toggling.
        /// </summary>
        public bool isVisible { get; protected set; }

        public virtual UIWindowPage defaultPage
        {
            get { return pages.FirstOrDefault(o => o.isDefaultPage); }
        }

        private List<UIWindowPage> _pages = new List<UIWindowPage>();
        public List<UIWindowPage> pages
        {
            get { return _pages; }
        }


        public UIWindowPage currentPage { get; set; }
        private IEnumerator _animationCoroutine;

        private Animator _animator;
        public Animator animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();

                return _animator;
            }
        }

        private RectTransform _rectTransform;
        protected RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }


        private UIWindow _parentWindow;


        protected virtual void Awake()
        {
            if (_showAnimation.motion != null)
                _showAnimationHash = Animator.StringToHash(_showAnimation.motion.name);

            if (_hideAnimation.motion != null)
                _hideAnimationHash = Animator.StringToHash(_hideAnimation.motion.name);

            if (resetPositionOnStart)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }

#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
#endif
        }

#if UNITY_5_4_OR_NEWER
        private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            LevelStart();
        }
#endif

        protected void OnDestroy()
        {

            if (_parentWindow != null)
            {
                _parentWindow.OnShow -= OnShow;
                _parentWindow.OnHide -= Hide;
            }

#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoaded;
#endif
        }

        protected virtual void Start()
        {
            LevelStart();
        }

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3

        public void OnLevelWasLoaded(int level)
        {
            LevelStart();
        }

#endif

        protected virtual void LevelStart()
        {
            if (hideOnStart)
            {
                HideFirst();
            }
            else
            {
                isVisible = true;
            }

            if (transform.parent != null)
            {
                _parentWindow = transform.parent.GetComponentInParent<UIWindow>();
                if (_parentWindow != null)
                {
                    _parentWindow.OnShow += OnShow;
                    _parentWindow.OnHide += Hide;
                }
            }
        }

        public void NotifyChildShown(UIWindowPage page)
        {
            currentPage = page;
            HideAllPagesExceptCurrent();

            DoShow(false);
        }

        protected void HideAllPagesExceptCurrent(float waitTime = 0.0f)
        {
            var cur = currentPage;
            foreach (var p in pages)
            {
                if (p == cur)
                {
                    continue;
                }

                p.Hide(waitTime);
            }
        }


        protected void NotifyWindowHidden()
        {
            if (currentPage != null && currentPage.isVisible)
            {
                currentPage.NotifyWindowHidden();
            }

            if (blockUIInput)
            {
                InputManager.RemoveUILimitInput(gameObject);
            }

            if (blockPlayerInput)
            {
                InputManager.RemovePlayerLimitInput(gameObject);
            }

            foreach (var page in pages)
            {
                page.NotifyWindowHidden();
            }

            onHideActions.Invoke();
            if (OnHide != null)
            {
                OnHide();
            }
        }

        protected void NotifyWindowShown()
        {
            if (blockUIInput)
            {
                InputManager.LimitUIInputTo(gameObject);
            }

            if (blockPlayerInput)
            {
                InputManager.LimitPlayerInputTo(gameObject);
            }

            foreach (var page in pages)
            {
                page.NotifyWindowShown();
            }

            onShowActions.Invoke();
            if (OnShow != null)
            {
                OnShow();
            }
        }

        protected virtual void SetChildrenActive(bool active)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(active);
            }

            var img = gameObject.GetComponents<UnityEngine.UI.Graphic>();
            foreach (var graphic in img)
            {
                graphic.enabled = active;
            }
        }

        public void PlayAudio(AudioClipInfo clip)
        {
            var source = GetComponent<AudioSource>();
            if (source != null)
            {
                source.Play(clip);
            }
        }

        private void PlayAnimation(MotionInfo clip, int hash, Action callback)
        {
            if (clip.motion != null)
            {
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                }

                _animationCoroutine = _PlayAnimationAndDisableAnimator(clip.motion.averageDuration + 0.1f, hash, callback);
                StartCoroutine(_animationCoroutine);
            }
            else
            {
                animator.enabled = false;
                if (callback != null)
                {
                    callback();
                }
            }
        }

        public void Toggle()
        {
            if (isVisible)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            DoShow();
        }

        public void Show(float waitTime)
        {
            if (waitTime > 0f)
            {
                StartCoroutine(_Show(waitTime));
            }
            else
            {
                DoShow();
            }
        }

        protected IEnumerator _Show(float waitTime)
        {
            if (isVisible)
            {
                yield break;
            }

            // Show default page before wait is completed, otherwise it'd pop in after time.
            if (defaultPage != null)
            {
                defaultPage.Show();
            }

            yield return StartCoroutine(CoroutineUtility.WaitRealtime(waitTime));

            DoShow();
        }

        protected void DoShow()
        {
            DoShow(true);
        }

        protected virtual void DoShow(bool resetCurrentPage)
        {
            if (isVisible)
            {
                return;
            }

            isVisible = true;
            SetChildrenActive(true);
            if (resetCurrentPage)
            {
                currentPage = defaultPage;
                if (currentPage != null)
                {
                    currentPage.Show();
                }
            }

            PlayAnimation(_showAnimation, _showAnimationHash, null);
            PlayAudio(showAudioClip);

            NotifyWindowShown();
        }

        public virtual void HideFirst()
        {
            isVisible = false;
            animator.enabled = false;

            SetChildrenActive(false);
        }

        /// <summary>
        /// Convenience method for easy upgrading...
        /// </summary>
        public void Hide()
        {
            DoHide();
        }

        public void Hide(float waitTime)
        {
            if (waitTime > 0f)
            {
                StartCoroutine(_Hide(waitTime));
            }
            else
            {
                DoHide();
            }
        }

        protected IEnumerator _Hide(float waitTime)
        {
            if (isVisible == false)
                yield break;

            yield return StartCoroutine(CoroutineUtility.WaitRealtime(waitTime));
 
            DoHide();
        }

        protected virtual void DoHide()
        {
            if (isVisible == false)
            {
                return;
            }

            isVisible = false;
            PlayAnimation(_hideAnimation, _hideAnimationHash, () =>
            {
                // Still invisible? Maybe it got shown while we waited.
                if (isVisible == false)
                {
                    SetChildrenActive(false);
                }
            });

            PlayAudio(hideAudioClip);

            NotifyWindowHidden();
        }

        /// <summary>
        /// Hides object after animation is completed.
        /// </summary>
        protected IEnumerator _PlayAnimationAndDisableAnimator(float waitTime, int hash, Action callback)
        {
            yield return null; // Needed for some reason, Unity bug??

            var before = _animationCoroutine;
            animator.enabled = true;
            animator.Play(hash);

            yield return StartCoroutine(CoroutineUtility.WaitRealtime(waitTime));

            // If action completed without any other actions overriding isVisible should be true. It could be hidden before the coroutine finished.
            animator.enabled = false;
            if (callback != null)
                callback();

            if (before == _animationCoroutine)
            {
                // Didn't change curing coroutine
                _animationCoroutine = null;
            }
        }
    }
}
