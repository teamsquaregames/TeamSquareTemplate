using UnityEngine;
using Sirenix.OdinInspector;
using Utils;

public interface IStatModifiers
{
    StatHandler StatHandler { get; }
    StatModifier[] StatModifiers { get; }
}

public static class StatModifierExtensions
{
    public static void ApplyStatModifiers(this IStatModifiers _I)
    {        
        foreach (var statModifier in _I.StatModifiers)
        {
            _I.StatHandler.ApplyModifier(statModifier);
            // Debug.Log($"Applied Stat Modifier {statModifier.id} to Stat {statModifier.statType} with value {statModifier.value}.");
        }
    }
    
    public static void RemoveStatModifiers(this IStatModifiers _I)
    {        
        foreach (var statModifier in _I.StatModifiers)
        {
            _I.StatHandler.RemoveModifier(statModifier.statType, statModifier.id);
        }
    }
}