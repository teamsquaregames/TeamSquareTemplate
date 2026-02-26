using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Utils.UI
{
    public class UIHandler : MyBox.Singleton<UIHandler>
    {
        #region Actions
        public Action onCurentIslandUpgrade;
        #endregion

        [SerializeField] private bool m_startInit;
        [SerializeField] private SerializableDictionary<Type, UIContainer> m_container;

        public bool IsOverUI => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        private void Start()
        {
            if (m_startInit)
                Init();
        }

        public void Init()
        {
            InitCanvas();
            SetupContainers();
        }

        private void InitCanvas()
        {
            foreach (CanvasHandler item in GetComponentsInChildren<CanvasHandler>())
                item.Init();
        }

        private void SetupContainers()
        {
            m_container = new SerializableDictionary<Type, UIContainer>();
            List<UIContainer> UIContainers = GetComponentsInChildren<UIContainer>().ToList();

            /// Set containers dictionary
            foreach (UIContainer container in UIContainers)
            {
                if (container != null)
                {
                    if (!m_container.ContainsKey(container.GetType()))
                    {
                        m_container.Add(container.GetType(), container);
                    }
                    else
                    {
                        this.LogWarning($"Container type already added: {container.GetType()}");
                    }
                    // this.Log($"Container type added: {container}");
                }
            }
            /// Init all containers
            foreach (UIContainer container in UIContainers)
            {
                // panel.Init();
                if (container != null)
                {
                    container.Init();
                }
            }
        }

        public T GetContainer<T>() where T : UIContainer
        {
            if (m_container.Keys.Contains(typeof(T)))
            {
                return m_container[typeof(T)] as T;
            }

            this.LogWarning($"Container type not founded: {typeof(T)}");
            return null;
        }

        public void CloseAllContainers()
        {
            foreach (UIContainer uiContainer in m_container.Values)
            {
                uiContainer.Close();
            }
        }
    }
}