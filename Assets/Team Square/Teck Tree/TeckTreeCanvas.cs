using Sirenix.OdinInspector;
using UnityEngine;

public class TeckTreeCanvas : CanvasHandler
{
    [TitleGroup("Dependencies")]
    [SerializeField] private PanelController m_ttPanelController;
    
    private TTNodeButton[] m_ttNodeButtons;

    public void GetReferences()
    {
        m_ttPanelController = GetComponent<PanelController>();
    }

    public override void Init()
    {
        base.Init();

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
            GameHandler.Instance.StartRun();
        });
    }

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

    #region Nodes

    public TTNodeButton GetButtonByNodeAsset(TTNodeAsset nodeAsset)
    {
        foreach (TTNodeButton button in m_ttNodeButtons)
        {
            if (button.LinkedNodeAsset == nodeAsset)
                return button;
        }
        return null;
    }

    public void ResetTechTree()
    {
        foreach (TTNodeButton nodeButton in m_ttNodeButtons)
            nodeButton.UpdateVisuals();

        m_ttPanelController.ResetView();
        HandleNodesHighlight();
    }

    #endregion

    #region Quest

    private void OnQuestStarted(Quest startedQuest)
    {
        if (startedQuest.linkedNode == null) return;

        TTNodeButton nodeButton = GetButtonByNodeAsset(startedQuest.linkedNode);
        if (nodeButton != null && GameData.Instance.GetNodeLevel(nodeButton.LinkedNodeAsset.ID) <= 0)
            nodeButton.SetHighlighted(true);
    }

    private void OnQuestCompleted(Quest completedQuest)
    {
        if (completedQuest.linkedNode == null) return;

        TTNodeButton nodeButton = GetButtonByNodeAsset(completedQuest.linkedNode);
        if (nodeButton != null)
            nodeButton.SetHighlighted(false);
    }

    private void HandleNodesHighlight()
    {
        Quest currentQuest = QuestManager.Instance.CurrentQuest;
        foreach (TTNodeButton ttNodeButton in m_ttNodeButtons)
        {
            bool shouldHighlight = ttNodeButton.LinkedNodeAsset != null
                && currentQuest != null
                && currentQuest.linkedNode == ttNodeButton.LinkedNodeAsset
                && GameData.Instance.GetNodeLevel(currentQuest.linkedNode.ID) <= 0;

            ttNodeButton.SetHighlighted(shouldHighlight);
        }
    }

    #endregion

    private void OnDestroy()
    {
        QuestManager.Instance.OnQuestCompleted -= OnQuestCompleted;
        QuestManager.Instance.OnQuestStarted -= OnQuestStarted;
    }
}