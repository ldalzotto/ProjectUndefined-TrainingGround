using UnityEngine;

[System.Serializable]
public class TalkAction : AContextAction
{

    #region External Event Dependencies
    private DiscussionEventHandler DiscussionEventHandler;
    private PointOfInterestManager PointOfInterestManager;
    #endregion

    private bool isConversationFinished;
    private DiscussionTreeNode currentDiscussionTreeNode;

    public TalkAction() : base()
    {
        DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
    }

    public override void AfterFinishedEventProcessed()
    {

    }

    public override bool ComputeFinishedConditions()
    {
        return isConversationFinished;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        var talkActionInput = (TalkActionInput)ContextActionInput;
        isConversationFinished = false;
        DiscussionEventHandler.AddOnSleepExternalHanlder(OnSentenceFinished);

        SetupSentence(talkActionInput.DiscussionTree.DiscussionRootNode);
    }

    private void SetupSentence(DiscussionTreeNode discussionTreeNode)
    {
        currentDiscussionTreeNode = discussionTreeNode;

        if (discussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
        {
            var currentTextOnlyDiscussionNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
            var sentenceTalkerPOI = PointOfInterestManager.GetActivePointOfInterest(currentTextOnlyDiscussionNode.Talker);
            DiscussionEventHandler.OnDiscussionWindowAwake(sentenceTalkerPOI.transform, DiscussionSentencesTextConstants.SentencesText[currentTextOnlyDiscussionNode.DisplayedText]);
        }
        else if (discussionTreeNode.GetType() == typeof(DiscussionChoiceNode))
        {

        }
    }

    private void OnSentenceFinished()
    {
        var nextNodes = currentDiscussionTreeNode.GetNextNodes();
        if (nextNodes.Count > 0)
        {
            SetupSentence(nextNodes[0]);
        }
        else
        {
            isConversationFinished = true;
            DiscussionEventHandler.RemoveOnSleepExternalHanlder(OnSentenceFinished);
        }
    }

    public override void Tick(float d)
    {

    }
}


public class TalkActionInput : AContextActionInput
{
    private DiscussionTree discussionTree;

    public TalkActionInput(DiscussionTree discussionTree)
    {
        this.discussionTree = discussionTree;
    }

    public DiscussionTree DiscussionTree { get => discussionTree; }
}