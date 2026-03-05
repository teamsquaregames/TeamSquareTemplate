using System.Collections.Generic;
using System.Text;

public static class DescriptionBuilder
{
    /// <summary>
    /// Builds a complete description with custom description text, currencies, and stat modifiers
    /// </summary>
    public static string BuildFullDescription(
        string customDescription = null,
        IEnumerable<KeyValuePair<CurrencyAsset, double>> currencies = null,
        IEnumerable<StatModifier> statModifiers = null,
        int statModifierLevel = 0)
    {
        StringBuilder description = new StringBuilder();

        // Add custom description at the beginning if it exists
        if (!string.IsNullOrEmpty(customDescription))
        {
            description.Append(customDescription);
        }

        // Add currencies
        if (currencies != null)
        {
            foreach (var currencyPair in currencies)
            {
                if (description.Length > 0)
                {
                    description.Append("\n");
                }

                string color = "#FFFF00";
                description.Append($"<color={color}>+{currencyPair.Value}</color> {currencyPair.Key.SpriteAssetString}");
            }
        }

        // Add stat modifiers
        if (statModifiers != null)
        {
            foreach (StatModifier modifier in statModifiers)
            {
                if (description.Length > 0)
                {
                    description.Append("\n");
                }

                double value = modifier.values != null && modifier.values.Length > 0
                    ? (statModifierLevel < modifier.values.Length ? modifier.values[statModifierLevel] : modifier.values[0])
                    : 0;

                string formattedModifier = FormatModifier(modifier.modifierType, value);
                string statName = GetStatName(modifier.statType);

                // Add percent symbol in the same color if specified (but not for Percentage type which already has %)
                if (modifier.addPercentSymbol && modifier.modifierType != OperationType.Percentage)
                {
                    string color = value >= 0 ? "#00FF00" : "#FF0000";
                    description.Append($"{formattedModifier}<color={color}>%</color> {statName}");
                }
                else
                {
                    description.Append($"{formattedModifier} {statName}");
                }
            }
        }

        if (description.Length == 0)
        {
            return "No effects";
        }

        return description.ToString();
    }

    public static string BuildStatModifiersDescription(IEnumerable<StatModifier> statModifiers, int level = 0)
    {
        return BuildFullDescription(null, null, statModifiers, level);
    }
    
    public static string BuildCurrenciesDescription(Cost[] currencies, int level = 0)
    {
        if (currencies == null || currencies.Length == 0)
            return string.Empty;

        StringBuilder description = new StringBuilder();

        foreach (Cost currency in currencies)
        {
            if (description.Length > 0)
            {
                description.Append("\n");
            }

            ulong amount = currency.GetAmount(level);
            string color = "#FFFF00";
            description.Append($"<color={color}>+{amount}</color> {currency.currencyAsset.SpriteAssetString}");
        }

        return description.ToString();
    }
    
    public static string FormatModifier(OperationType operationType, double value)
    {
        string color = value >= 0 ? "#00FF00" : "#FF0000";

        switch (operationType)
        {
            case OperationType.Additive:
                string sign = value >= 0 ? "+" : "";
                string formattedValue = value % 1 == 0 ? value.ToString("0") : value.ToString("0.##");
                return $"<color={color}>{sign}{formattedValue}</color>";

            case OperationType.Percentage:
                string percentSign = value >= 0 ? "+" : "";
                return $"<color={color}>{percentSign}{value:0.#}%</color>";

            default:
                return $"<color={color}>{value.ToString("0.##")}</color>";
        }
    }
    
    public static string FormatStatName(string statType)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < statType.Length; i++)
        {
            if (i > 0 && char.IsUpper(statType[i]))
            {
                result.Append(" ");
            }
            result.Append(statType[i]);
        }

        return result.ToString();
    }
    
    public static string GetStatName(StatType statType)
    {
        return FormatStatName(statType.ToString());
    }
}