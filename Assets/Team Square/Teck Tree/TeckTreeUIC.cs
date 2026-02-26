using Sirenix.OdinInspector;
using UnityEngine;
using System.Threading.Tasks;
using Utils;
using Utils.UI;

public class TeckTreeUIC : UIContainer
{
    [TitleGroup("Dependencies")]
    [SerializeField] private PanelController m_ttPanelController;
    [SerializeField] private Transform m_starIconTfm;

    private GameHandler m_gameHandler;
    private TTNodeButton[] m_ttNodeButtons;

    public Transform StarIconTfm => m_starIconTfm;

    public override void Init()
    {
        // this.Log("Init TeckTreeUIC");
        base.Init();
        m_gameHandler = GameHandler.Instance;

        m_ttNodeButtons = GetComponentsInChildren<TTNodeButton>();
        foreach (TTNodeButton ttNodeButton in m_ttNodeButtons)
            ttNodeButton.PanelController = m_ttPanelController;

        QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;
        QuestManager.Instance.OnQuestStarted += OnQuestStarted;
    }

    public void StartRun()
    {
        FadeManager.Instance.FadeIn(() =>
        {
            m_gameHandler.StartRun();
        });
    }

    #region Quest

    private void OnNodeLevelUp(TTNodeAsset nodeAsset)
    {
        HandleNodesHighlight();
    }

    private void OnQuestStarted(Quest startedQuest)
    {
        if (startedQuest.linkedNode == null) return;

        TTNodeButton _nodeButton = GetButtonByNodeAsset(startedQuest.linkedNode);
        if (_nodeButton != null && GameData.Instance.GetNodeLevel(_nodeButton.LinkedNodeAsset.ID) <= 0)
            _nodeButton.SetHighlighted(true);
    }
    private void OnQuestCompleted(Quest completedQuest)
    {
        if (completedQuest.linkedNode == null) return;

        TTNodeButton _nodeButton = GetButtonByNodeAsset(completedQuest.linkedNode);
        if (_nodeButton != null)
            _nodeButton.SetHighlighted(false);
    }

    private void HandleNodesHighlight()
    {
        Quest _currentQuest = QuestManager.Instance.CurrentQuest;
        foreach (TTNodeButton ttNodeButton in m_ttNodeButtons)
        {
            if (ttNodeButton.LinkedNodeAsset != null && _currentQuest != null && _currentQuest.linkedNode == ttNodeButton.LinkedNodeAsset && GameData.Instance.GetNodeLevel(_currentQuest.linkedNode.ID) <= 0)
                ttNodeButton.SetHighlighted(true);
            else
                ttNodeButton.SetHighlighted(false);
        }
    }

    #endregion


    public override void Open()
    {
        base.Open();

        m_ttPanelController.ResetView();
        HandleNodesHighlight();
    }

    public override void Close()
    {
        base.Close();
    }

    public TTNodeButton GetButtonByNodeAsset(TTNodeAsset nodeAsset)
    {
        foreach (var button in m_ttNodeButtons)
        {
            if (button.LinkedNodeAsset == nodeAsset)
                return button;
        }
        return null;
    }

    private void OnDestroy()
    {
        QuestManager.Instance.OnQuestCompleted -= OnQuestCompleted;
        QuestManager.Instance.OnQuestStarted -= OnQuestStarted;
    }

    public void ResetTeckTree()
    {
        foreach (TTNodeButton nodeButton in m_ttNodeButtons)
            nodeButton.UpdateVisuals();

        m_ttPanelController.ResetView();

        HandleNodesHighlight();
    }
}