using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Config/GameConfig")]
public class GameConfig : ScriptableObject
{
    private static GameConfig _instance;
    public static GameConfig Instance => _instance ?? Load();

    private static GameConfig Load()
    {
        _instance = Resources.Load<GameConfig>("GameConfig");
#if UNITY_EDITOR
        if (_instance == null)
            UnityEngine.Debug.LogError("GameConfig asset not found in Resources folder!");
#endif
        return _instance;
    }


    //-------------------------------------

    [SerializeField] private Debuging debuging = new Debuging();
    public Debuging DebugSettings => debuging;
    
    [SerializeField] private Cheat cheat = new Cheat();
    public Cheat CheatSettings => cheat;
    
    [SerializeField] private Game game = new Game();
    public Game GameSettings => game;
    

    public StatHandler StatHandler => throw new System.NotImplementedException();

    public StatModifier[] StatModifiers => cheat.cheatStats;

    //-------------------------------------

    [System.Serializable]
    public partial class Cheat
    {
        public StatModifier[] cheatStats = new StatModifier[0];
        public bool preventSave = false;
        public bool startResetData = false;
        public bool noFTUE = false;
        public bool noCurrencyRequired = false;
        public bool noMenu = false;
    }

    [System.Serializable]
    public partial class Debuging
    {
        public bool developmentBuild;
    }

    [System.Serializable]
    public partial class Game
    {
        public bool isDemo = false;

        [Space]
        public CurrencyAsset[] resetedCurrency;
        public double[] resetCurrenciesNeeded;

        [Space, Header("Tutorial")]
        public float delayBeforeCanValidateOnClick = 0.2f;
        public int buildingPlacedForClaimCostlineTutorial = 6;
        

        public double GetResetCurrencyNeeded(int index)
        {
            if (resetCurrenciesNeeded == null || resetCurrenciesNeeded.Length == 0)
            {
                Debug.LogError("resetCurrenciesNeeded is not initialized.");
                return 0;
            }

            if (index < 0 || index >= resetCurrenciesNeeded.Length)
            {
                Debug.LogWarning($"Index {index} is out of bounds for resetCurrenciesNeeded array.");
                return resetCurrenciesNeeded[resetCurrenciesNeeded.Length - 1];
            }

            return resetCurrenciesNeeded[index];
        }
    }
}