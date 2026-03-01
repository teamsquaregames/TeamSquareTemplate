using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.UI;


public class TTNodeDetails : AUIElement
{
    [TitleGroup("Dependencies")]
    [SerializeField, Required] private RectTransform m_rectTransform;
    [SerializeField, Required] private Image m_icon;
    [SerializeField, Required] private TMP_Text m_name;
    [SerializeField, Required] private TMP_Text m_description;
    [SerializeField, Required] private CurrencyCostUIE m_currencyCostUIE;
    [SerializeField, Required] private GameObject[] m_levelObjects;
    [SerializeField, Required] private GameObject[] m_enabledLevelObjects;
    
    private TTNodeAsset m_currentAsset;

    public void Setup(TTNodeAsset _asset)
    {
        m_currentAsset = _asset;
        int level = GameData.Instance.GetNodeLevel(_asset.ID);

        /// Set new informations
        m_icon.sprite = _asset.Icon;
        m_name.text = _asset.DisplayName;
        m_description.text = BuildNodeDescription(_asset, level);

        if (level >= _asset.MaxLevel)
            m_currencyCostUIE.Hide();
        else
        {
            m_currencyCostUIE.Show();
            m_currencyCostUIE.SetCurrencyCost(_asset.Cost[0].currencyAsset, _asset.Cost[0].GetAmount(level));
        }

        HandleLevelDisplay();
    }

    private void HandleLevelDisplay()
    {
        for (int i = 0; i < m_levelObjects.Length; i++)
            m_levelObjects[i].SetActive(i < m_currentAsset.MaxLevel);

        for (int i = 0; i < m_enabledLevelObjects.Length; i++)
            m_enabledLevelObjects[i].SetActive(i < GameData.Instance.GetNodeLevel(m_currentAsset.ID));
    }

    public void LevelUp(TTNodeAsset _asset, int level)
    {
        m_currentAsset = _asset;
        m_description.text = BuildNodeDescription(_asset, level);

        if (level >= _asset.MaxLevel)
            m_currencyCostUIE.Hide();
        else
        {
            m_currencyCostUIE.Show();
            m_currencyCostUIE.SetCurrencyCost(m_currencyCostUIE.CurrencyAsset, _asset.Cost[0].GetAmount(level));
        }

        HandleLevelDisplay();
    }

    private string BuildNodeDescription(TTNodeAsset _asset, int _level)
    {
        StringBuilder description = new StringBuilder();

        // Add the asset's description at the beginning if it exists
        if (!string.IsNullOrEmpty(_asset.Description))
        {
            description.Append(_asset.Description);
        }

        // Determine if this is the first unlock (level 0 = not yet unlocked)
        bool isFirstUnlock = _level == 0;
        // Determine if this is the max level
        bool isMaxLevel = _level >= _asset.MaxLevel;

        // Add stat modifiers showing current -> next value
        if (_asset.StatModifiers != null && _asset.StatModifiers.Length > 0)
        {
            foreach (StatModifier modifier in _asset.StatModifiers)
            {
                if (description.Length > 0)
                {
                    description.Append("\n");
                }

                if (isFirstUnlock)
                {
                    // Show only the first value for initial unlock in white
                    double firstValue = modifier.values != null && modifier.values.Length > 0
                        ? modifier.values[0]
                        : 0;

                    string formattedFirst = FormatModifierValue(modifier.modifierType, firstValue, "#FFFFFF");

                    // Add percent symbol if specified (but not for Percentage type which already has %)
                    if (modifier.addPercentSymbol && modifier.modifierType != OperationType.Percentage)
                    {
                        description.Append($"{formattedFirst}<color=#FFFFFF>%</color>");
                    }
                    else
                    {
                        description.Append($"{formattedFirst}");
                    }
                }
                else if (isMaxLevel)
                {
                    // Show only the current value for max level
                    double currentValue = modifier.values != null && modifier.values.Length > 0
                        ? (_level - 1 < modifier.values.Length ? modifier.values[_level - 1] : modifier.values[modifier.values.Length - 1])
                        : 0;

                    string formattedCurrent = FormatModifierValue(modifier.modifierType, currentValue, "#FFFFFF");

                    // Add percent symbol if specified (but not for Percentage type which already has %)
                    if (modifier.addPercentSymbol && modifier.modifierType != OperationType.Percentage)
                    {
                        description.Append($"{formattedCurrent}<color=#FFFFFF>%</color>");
                    }
                    else
                    {
                        description.Append($"{formattedCurrent}");
                    }
                }
                else
                {
                    // Show current -> next for upgrades
                    double currentValue = modifier.values != null && modifier.values.Length > 0
                        ? (_level - 1 < modifier.values.Length ? modifier.values[_level - 1] : modifier.values[modifier.values.Length - 1])
                        : 0;

                    double nextValue = modifier.values != null && modifier.values.Length > 0
                        ? (_level < modifier.values.Length ? modifier.values[_level] : modifier.values[modifier.values.Length - 1])
                        : 0;

                    string formattedCurrent = FormatModifierValue(modifier.modifierType, currentValue, "#FFFFFF");
                    string formattedNext = DescriptionBuilder.FormatModifier(modifier.modifierType, nextValue);

                    // Add percent symbol if specified (but not for Percentage type which already has %)
                    if (modifier.addPercentSymbol && modifier.modifierType != OperationType.Percentage)
                    {
                        string nextColor = nextValue >= 0 ? "#00FF00" : "#FF0000";
                        description.Append($"{formattedCurrent}<color=#FFFFFF>%</color> <sprite=\"arrow\" name=\"arrow\"> {formattedNext}<color={nextColor}>%</color>");
                    }
                    else
                    {
                        description.Append($"{formattedCurrent} <sprite=\"arrow\" name=\"arrow\"> {formattedNext}");
                    }
                }
            }
        }

        // Add currencies showing current -> next value
        if (_asset.Currencies != null && _asset.Currencies.Length > 0)
        {
            foreach (Cost currency in _asset.Currencies)
            {
                if (description.Length > 0)
                {
                    description.Append("\n");
                }

                string color = "#FFFF00";

                if (isFirstUnlock)
                {
                    // Show only the first value for initial unlock in white
                    ulong firstAmount = currency.GetAmount(0);
                    description.Append($"<color=#FFFFFF>+{firstAmount}</color> {currency.currencyAsset.SpriteAssetString}");
                }
                else if (isMaxLevel)
                {
                    // Show only the current value for max level
                    ulong currentAmount = currency.GetAmount(_level - 1);
                    description.Append($"<color={color}>+{currentAmount}</color> {currency.currencyAsset.SpriteAssetString}");
                }
                else
                {
                    // Show current -> next for upgrades
                    ulong currentAmount = currency.GetAmount(_level - 1);
                    ulong nextAmount = currency.GetAmount(_level);

                    description.Append($"<color=#FFFFFF>+{currentAmount}</color> {currency.currencyAsset.SpriteAssetString} <sprite=\"arrow\" name=\"arrow\"> <color={color}>+{nextAmount}</color> {currency.currencyAsset.SpriteAssetString}");
                }
            }
        }

        if (description.Length == 0)
        {
            return "No effects";
        }

        return description.ToString();
    }

    private string FormatModifierValue(OperationType operationType, double value, string color)
    {
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
}