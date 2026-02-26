
using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class CostUIE : IconAndText
{
    [TitleGroup("Dependencies")]
    [SerializeField, Required] private Sprite m_freeIcon;
    [SerializeField] private CurrencyAsset m_currencyAsset;

    [TitleGroup("Settings")]
    [SerializeField] private Color m_validColor = Color.white;
    [TitleGroup("Settings")]
    [SerializeField] private Color m_invalidColor = Color.red;


    [TitleGroup("Variables")]
    private double m_value = 0;

    public CurrencyAsset CurrencyAsset => m_currencyAsset;


    public override void Init()
    {
        GameData.Instance.onCurrencyChanged += OnCurrencyChanged;
    }

    public void SetCurrencyAsset(CurrencyAsset currencyAsset)
    {
        m_currencyAsset = currencyAsset;
        if (m_currencyAsset == null)
        {
            SetIcon(m_freeIcon);
            return;
        }
        SetIcon(currencyAsset.Icon);
    }

    public override void SetValue(double value, TextFormat format = TextFormat.None)
    {
        // this.Log($"Setting CostUIE {m_currencyAsset.name} value to {value}");
        m_value = value;
        base.SetValue(value, format);
        UpdateValueColor();
    }

    private void OnCurrencyChanged(CurrencyAsset currency, double newAmount)
    {
        if (currency != m_currencyAsset)
            return;

        UpdateValueColor();
    }

    public void UpdateValueColor()
    {
        if (m_currencyAsset == null || GameData.Instance.HasEnoughCurrency(m_currencyAsset, m_value))
        {
            m_valueText.color = m_validColor;
        }
        else
        {
            m_valueText.color = m_invalidColor;
        }
    }

    public void SetColor(bool isValid)
    {
        if (isValid)
        {
            m_valueText.color = m_validColor;
        }
        else
        {
            m_valueText.color = m_invalidColor;
        }
    }
}