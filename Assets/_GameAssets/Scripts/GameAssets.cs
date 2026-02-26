using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Config/GameAssets")]
public class GameAssets : ScriptableObject
{
    private static GameAssets _instance;
    public static GameAssets Instance => _instance ?? Load();

    private static GameAssets Load()
    {
        _instance = Resources.Load<GameAssets>("GameAssets");
        return _instance;
    }


    // ----------------------------------------------------------

    public CurrencyAsset[] currencyAssets;
    public SerializableDictionary<Currency, CurrencyAsset> currencyAssetDico;
    public FloatingTextConfig currencyTextConfig;
    public FloatingTextConfig coastLineClickTextConfig;
    public SoundKeys positiveButtonClick;
    public SoundKeys negativeButtonClick;
}