using System;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[System.Serializable]
public struct StatModifier
{
    public string id;
    public StatType statType;
    public OperationType modifierType;
    public bool addPercentSymbol;
    [FormerlySerializedAs("value")] public double[] values;
    [ReadOnly] public int level;

    public static StatModifier CreateCopy(StatModifier original)
    {
        StatModifier copy = new StatModifier
        {
            id = original.id,
            statType = original.statType,
            modifierType = original.modifierType,
            level = original.level,
            values = new double[original.values.Length]
        };
        Array.Copy(original.values, copy.values, original.values.Length);
        return copy;
    }
}


[System.Serializable]
public struct StatsModifiers
{
    public StatModifier[] statModifiers;
}