using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

// Enum for how to apply the modifier value
public enum ModifierApplicationMode
{
    Replace,      // Default behavior - replaces existing modifier
    Add           // Adds to the existing modifier's value
}

public class StatHandler : MyBox.Singleton<StatHandler>
{
    public Action<StatType> onStatChanged;
    
    [TitleGroup("Dependencies")]
    [SerializeField] private SerializableDictionary<StatType, double> m_statsValues = new SerializableDictionary<StatType, double>();
    public SerializableDictionary<StatType, double> StatsValues => m_statsValues;
    [SerializeField] private SerializableDictionary<StatType, List<StatModifier>> m_statsModifiers = new SerializableDictionary<StatType, List<StatModifier>>();
    public SerializableDictionary<StatType, List<StatModifier>> StatsModifiers => m_statsModifiers;

    public StatsModifiers m_baseModifiers;

    private void Awake()
    {
        ApplyBaseModifiers();
    }

    private void ApplyBaseModifiers()
    {
        foreach (StatModifier statModifier in m_baseModifiers.statModifiers)
            ApplyModifier(statModifier);
        
#if UNITY_EDITOR
        foreach (StatModifier modifier in GameConfig.Instance.CheatSettings.cheatStats)
            ApplyModifier(modifier);
#endif
    }

    public double GetValue(StatType statType)
    {
        if (m_statsValues.TryGetValue(statType, out double value))
        {
            return value;
        }
        return 0;
    }

    public int GetIntValue(StatType statType)
    {
        if (m_statsValues.TryGetValue(statType, out double value))
        {
            // this.Log($"GetIntValue for {statType} returning {(int)value}");
            return (int)value;
        }
        // this.LogWarning($"GetIntValue for {statType}: not found, returning 0");
        return 0;
    }

    public double GetSpecificValue(StatType _statType, OperationType _modifierType)
    {
        if (!m_statsModifiers.ContainsKey(_statType) || _statType == StatType.UNINITIALIZED)
            return 0;

        double finalValue = 0;
        foreach (var modifier in m_statsModifiers[_statType])
        {
            if (modifier.modifierType == _modifierType)
            {
                finalValue += modifier.values[modifier.level];
            }
        }
        // this.Log($"GetSpecificValue for {_statType} and modifier type {_modifierType}: returning {finalValue}");
        return finalValue;
    }

    private void AddModifier(StatModifier _modifier)
    {
        if (_modifier.statType == StatType.UNINITIALIZED)
            return;
        
        //this.Log(_modifier.statType);
        
        if (!m_statsModifiers.ContainsKey(_modifier.statType))
            m_statsModifiers.Add(_modifier.statType, new List<StatModifier>());
        
        StatModifier modifierCopy = StatModifier.CreateCopy(_modifier);
        m_statsModifiers[_modifier.statType].Add(modifierCopy);
        ComputeValue(_modifier.statType);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [Button]
    public void ApplyModifier(StatModifier _modifier, ModifierApplicationMode _applicationMode = ModifierApplicationMode.Replace)
    {
        if (!m_statsModifiers.ContainsKey(_modifier.statType))
        {
            AddModifier(_modifier);
            return;
        }

        for (int i = 0; i < m_statsModifiers[_modifier.statType].Count; i++)
        {
            if (m_statsModifiers[_modifier.statType][i].id == _modifier.id)
            {
                StatModifier existingModifier = m_statsModifiers[_modifier.statType][i];
                
                switch (_applicationMode)
                {
                    case ModifierApplicationMode.Replace:
                        m_statsModifiers[_modifier.statType][i] = StatModifier.CreateCopy(_modifier);
                        break;
                        
                    case ModifierApplicationMode.Add:
                        int currentLevel = existingModifier.level;
                        existingModifier.values[currentLevel] += _modifier.values[_modifier.level];
                        m_statsModifiers[_modifier.statType][i] = existingModifier;
                        break;
                }

                ComputeValue(_modifier.statType);
                return;
            }
        }
        
        AddModifier(_modifier);
    }

    [Button]
    public void AddToModifierValue(StatType _statType, string _modifierID, double _valueToAdd)
    {
        if (!m_statsModifiers.ContainsKey(_statType))
        {
            this.LogWarning($"Cannot add to modifier {_modifierID} value: no modifiers exist for {_statType}");
            return;
        }

        for (int i = 0; i < m_statsModifiers[_statType].Count; i++)
        {
            if (m_statsModifiers[_statType][i].id == _modifierID)
            {
                StatModifier modifier = m_statsModifiers[_statType][i];
                modifier.values[modifier.level] += _valueToAdd;
                m_statsModifiers[_statType][i] = modifier;
                ComputeValue(_statType);
                return;
            }
        }
        
        this.LogWarning($"Cannot add to modifier {_modifierID} value: modifier not found in {_statType}");
    }

    [Button]
    public void RemoveModifier(StatType _statType, string _modifierID)
    {
        if (!m_statsModifiers.ContainsKey(_statType))
            return;

        m_statsModifiers[_statType].RemoveAll(modifier => modifier.id == _modifierID);

        ComputeValue(_statType);
    }

    public void Reset()
    {
        // Clear all current modifiers
        m_statsModifiers.Clear();
    
        // Clear all computed values
        m_statsValues.Clear();
    
        // Reapply base modifiers to restore initial state
        ApplyBaseModifiers();
    
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void ComputeValue(StatType _statType)
    {
        if (!m_statsModifiers.ContainsKey(_statType))
            return;

        double flatValue = 0;
        double percent = 0;
        foreach (var modifier in m_statsModifiers[_statType])
        {
            int _index = Mathf.Clamp(modifier.level, 0, modifier.values.Length - 1);
            switch (modifier.modifierType)
            {
                case OperationType.Additive:
                    flatValue += modifier.values[_index];
                    break;
                case OperationType.Percentage:
                    percent += modifier.values[_index];
                    break;
            }
        }

        double finalValue = flatValue * (100 + percent) / 100;
        m_statsValues[_statType] = finalValue;
        // this.Log($"Computed Stat {_statType}: Base Value = {flatValue}, Total Percentage = {percent}%, final Value = {finalValue}");
        
        onStatChanged?.Invoke(_statType);
    }
}