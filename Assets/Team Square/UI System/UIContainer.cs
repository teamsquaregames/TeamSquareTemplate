using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Utils.UI
{
    public class UIContainer : MonoBehaviour
    {
        [TitleGroup("Dependencies")]
        [SerializeField, Required] protected GameObject m_content;
        [SerializeField, Required] private CanvasGroup m_contentCanvasGroup;
        [SerializeField, Required] private RectTransform m_contentRectTransform;

        [TitleGroup("Settings")]
        [SerializeField] protected bool m_enableByDefault;

        [TitleGroup("Variables")]
        [SerializeField, ReadOnly] protected bool m_isOpen;

        [TitleGroup("Show Animation")]
        [SerializeField] private bool m_enableFadeIn;
        [SerializeField] private bool m_enableMoveIn;
        [SerializeField, ShowIf("@m_enableMoveIn")] private Vector2 m_moveInVector;
        [SerializeField, ShowIf("@m_enableMoveIn || m_enableFadeIn")] private float m_showDuration = 0.25f;

        [TitleGroup("Hide Animation")]
        [SerializeField] private bool m_enableFadeOut;
        [SerializeField] private bool m_enableMoveOut;
        [SerializeField, ShowIf("@m_enableMoveOut")] private Vector2 m_moveOutVector;
        [SerializeField, ShowIf("@m_enableMoveOut || m_enableFadeOut")] private float m_hideDuration = 0.25f;

        private Tween m_fadeTween;
        private Tween m_moveTween;
        private Tween m_waitTween;
        private Vector2 m_initialContentAnchoredPos;

        // private void OnValidate()
        // {
        //     if (m_content != null)
        //     {
        //         m_contentCanvasGroup = m_content.GetComponent<CanvasGroup>();
        //         if (m_contentCanvasGroup == null)
        //         {
        //             m_contentCanvasGroup = m_content.AddComponent<CanvasGroup>();
        //         }
        //         
        //         m_contentRectTransform = m_content.GetComponent<RectTransform>();
        //         if (m_contentRectTransform == null)
        //         {
        //             m_contentRectTransform = m_content.AddComponent<RectTransform>();
        //         }
        //     }
        // }


        public bool IsOpen => m_isOpen;
        public bool EnableByDefault => m_enableByDefault;

        public virtual void Init()
        {
            m_initialContentAnchoredPos = m_contentRectTransform.anchoredPosition;

            foreach (AUIElement item in m_content.GetComponentsInChildren<AUIElement>())
                item.Init();

            if (m_enableByDefault)
                Show();
            else
                Hide();
        }

        public virtual void Show()
        {
            if (m_waitTween.IsActive()) m_waitTween.Kill();

            // Fade In
            if (m_enableFadeIn)
            {
                if (m_fadeTween.IsActive()) m_fadeTween.Kill();
                m_contentCanvasGroup.alpha = 0;
                m_fadeTween = m_contentCanvasGroup.DOFade(1, m_showDuration);
            }
            else
            {
                m_contentCanvasGroup.alpha = 1;
            }

            // Move In
            if (m_enableMoveIn)
            {
                if (m_moveTween.IsActive()) m_moveTween.Kill();
                m_contentRectTransform.anchoredPosition = m_initialContentAnchoredPos - m_moveInVector;
                m_moveTween = m_contentRectTransform.DOAnchorPos(m_initialContentAnchoredPos, m_showDuration);
            }

            SetOpen(true);
        }

        public virtual void Hide()
        {
            if (m_waitTween.IsActive()) m_waitTween.Kill();

            if (!m_enableFadeOut && !m_enableMoveOut)
            {
                m_contentCanvasGroup.alpha = 0;
                SetOpen(false);
                return;
            }

            // Fade Out
            if (m_enableFadeOut)
            {
                if (m_fadeTween.IsActive()) m_fadeTween.Kill();
                m_fadeTween = m_contentCanvasGroup.DOFade(0, m_hideDuration);
            }

            // Move Out
            if (m_enableMoveOut)
            {
                if (m_moveTween.IsActive()) m_moveTween.Kill();
                m_contentRectTransform.anchoredPosition = m_initialContentAnchoredPos;
                m_moveTween = m_contentRectTransform.DOAnchorPos(m_initialContentAnchoredPos + m_moveOutVector, m_hideDuration);
            }

            m_waitTween = DOVirtual.DelayedCall(m_hideDuration, () => SetOpen(false));
        }

        private void SetOpen(bool isOpen)
        {
            m_isOpen = isOpen;
        }
    }
}