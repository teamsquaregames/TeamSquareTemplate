using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Utils.UI
{
    public class UIContainer : MonoBehaviour
    {
        public Action onOpen;
        public Action onClose;

        [TitleGroup("Dependencies")]
        [SerializeField, Required] protected CanvasHandler m_canvasHandler;
        [SerializeField, Required] protected GameObject m_content;
        
        [SerializeField] private CanvasGroup m_canvasGroup;

        [TitleGroup("Settings")]
        [SerializeField] protected bool m_enableByDefault;

        [TitleGroup("Variables")]
        [SerializeField, ReadOnly] protected bool m_isOpen;

        [TitleGroup("Open Animation")]
        [SerializeField] private bool m_enableFadeIn;
        [SerializeField] private bool m_enableMoveIn;
        [SerializeField, ShowIf("@m_enableMoveIn")] private Vector2 m_moveInVector;
        [SerializeField, ShowIf("@m_enableMoveIn || m_enableFadeIn")] private float m_openDuration = 0.25f;
        
        [TitleGroup("Close Animation")]
        [SerializeField] private bool m_enableFadeOut;
        [SerializeField] private bool m_enableMoveOut;
        [SerializeField, ShowIf("@m_enableMoveOut")] private Vector2 m_moveOutVector;
        [SerializeField, ShowIf("@m_enableMoveOut || m_enableFadeOut")] private float m_closeDuration = 0.25f;
        
        private Tween m_fadetween;
        private Tween m_moveTween;
        private Tween m_waitTween;
        private Vector2 m_initialContentAnchoredPos;
        private RectTransform m_contentRectTransform;
        private CanvasGroup m_contentConvasGroup;
        
        public bool IsOpen => m_isOpen;

        private void OnValidate()
        {
            if (m_enableFadeIn || m_enableFadeOut)
            {            
                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
                else
                    m_canvasGroup = canvasGroup;
            }
        }

        private void Awake()
        {
            m_contentConvasGroup = m_content.GetComponent<CanvasGroup>();
            if (m_contentConvasGroup == null)
                m_contentConvasGroup = m_content.AddComponent<CanvasGroup>();
            
            m_contentRectTransform = m_content.GetComponent<RectTransform>();
        }

        public virtual void Init()
        {
            // this.Log("Init UIContainer");

            m_initialContentAnchoredPos = m_contentRectTransform.anchoredPosition;
                
            foreach (AUIElement item in m_content.GetComponentsInChildren<AUIElement>())
            {
                item.Init();
            }
            
            if (m_enableByDefault)
                Open();
            else
                Close();
        }

        public virtual void Open()
        {
            //this.Log($"Open");
            //m_content.SetActive(true);
            
            if (m_waitTween.IsActive()) m_waitTween.Kill();
            
            //Fade In
            if (m_enableFadeIn)
            {
                if (m_fadetween.IsActive()) m_fadetween.Kill();
                m_canvasGroup.alpha = 0;
                m_fadetween = m_canvasGroup.DOFade(1, m_openDuration);
            }
            else
            {
                m_contentConvasGroup.alpha = 1;
            }
            
            //Move In
            if (m_enableMoveIn)
            {
                if (m_moveTween.IsActive()) m_moveTween.Kill();
                m_contentRectTransform.anchoredPosition = m_initialContentAnchoredPos - m_moveInVector;
                m_moveTween = m_contentRectTransform.DOAnchorPos(m_initialContentAnchoredPos, m_openDuration);
            }
            
            Show();
        }

        public virtual void Close()
        {
            //this.Log($"Close");
            
            if (m_waitTween.IsActive()) m_waitTween.Kill();
            
            if (!m_enableFadeOut && !m_enableMoveOut)
            {
                m_contentConvasGroup.alpha = 0;

                Hide();
            }

           
            //Fade Out
            if (m_enableFadeOut)
            {
                if (m_fadetween.IsActive()) m_fadetween.Kill();
                //m_canvasGroup.alpha = 1;
                m_fadetween = m_canvasGroup.DOFade(0, m_closeDuration);
            }

            //Move Out
            if (m_enableMoveOut)
            {
                if (m_moveTween.IsActive()) m_moveTween.Kill();
                m_contentRectTransform.anchoredPosition = m_initialContentAnchoredPos;
                m_moveTween = m_contentRectTransform.DOAnchorPos(m_initialContentAnchoredPos + m_moveOutVector, m_closeDuration);
            }

            if (m_enableFadeOut || m_enableMoveOut)
            {
                m_waitTween = DOVirtual.DelayedCall(m_closeDuration, () =>
                {
                    Hide();
                });
            }
        }

        private void Hide()
        {
            m_isOpen = false;
            onClose?.Invoke();
        }

        private void Show()
        {
            
            m_isOpen = true;
            onOpen?.Invoke();
        }
        
    }
}