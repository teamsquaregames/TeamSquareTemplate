using Sirenix.OdinInspector;
using UnityEngine;

namespace Stats
{
    public class StatModule : MonoBehaviour
    {
        [SerializeField] private EntityType m_entityType;
        [SerializeField] private StatModifier m_statModifier;

        private void Awake()
        {
            StatManager.Instance.RegisterInstance(gameObject, m_entityType);
        }

        private void OnDestroy()
        {
            if (StatManager.Instance != null)
                StatManager.Instance.UnregisterInstance(gameObject);
        }

        [Button]
        public void test()
        {
            AddModifier(m_statModifier);
        }

        [Button]
        public float GetValue(StatType type)         => StatManager.Instance.GetInstanceValue(gameObject, type);
        public void AddModifier(StatModifier mod)    => StatManager.Instance.AddInstanceModifier(gameObject, mod);
        public void RemoveModifier(StatModifier mod) => StatManager.Instance.RemoveInstanceModifier(gameObject, mod);
    }
}