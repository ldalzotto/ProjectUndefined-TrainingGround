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
        DiscussionEventHandler.AddOnDiscussionTextNodeEndEventHandler(OnDiscussionNodeEnd);

        SetupSentence(talkActionInput.DiscussionTree.DiscussionRootNode);
    }

    private void SetupSentence(DiscussionTreeNode discussionTreeNode)
    {
        currentDiscussionTreeNode = discussionTreeNode;

        if (discussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
        {
            var currentTextOnlyDiscussionNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
            var sentenceTalkerPOI = PointOfInterestManager.GetActivePointOfInterest(currentTextOnlyDiscussionNode.Talker);
            DiscussionEventHandler.OnDiscussionWindowAwake(currentTextOnlyDiscussionNode, sentenceTalkerPOI.transform);
        }
    }

    private void OnSentenceFinished()
    {
        var nextNodes = currentDiscussionTreeNode.GetNextNode();
        if (nextNodes != null)
        {
            SetupSentence(nextNodes);
        }
        else
        {
            isConversationFinished = true;
            DiscussionEventHandler.RemoveOnSleepExternalHanlder(OnSentenceFinished);
            DiscussionEventHandler.RemoveOnDiscussionTextNodeEndEventHandler(OnDiscussionNodeEnd);
        }
    }

    private DiscussionChoiceNode OnDiscussionNodeEnd()
    {
        if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
        {
            var nextNode = ((DiscussionTextOnlyNode)currentDiscussionTreeNode).GetNextNode();
            if (nextNode != null && nextNode.GetType() == typeof(DiscussionChoiceNode))
            {
                var nextChoiceNode = (DiscussionChoiceNode)nextNode;
                currentDiscussionTreeNode = nextChoiceNode;
                return nextChoiceNode;
            }
        }
        return null;
    }

    public override void Tick(float d)
    { }
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