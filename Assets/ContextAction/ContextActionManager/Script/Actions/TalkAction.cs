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
    private DiscussionChoiceTextId discussionChoiceMade;

    public TalkAction(AContextAction nextContextAction) : base(nextContextAction)
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
        DiscussionEventHandler.InitializeEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);

        OnNewCurrentNode((DiscussionTextOnlyNode)talkActionInput.DiscussionTree.DiscussionRootNode);
    }

    private void OnNewCurrentNode(DiscussionTreeNode newDiscussionNode)
    {
        var oldDiscussionNode = currentDiscussionTreeNode;
        currentDiscussionTreeNode = newDiscussionNode;

        if (currentDiscussionTreeNode == null)
        {
            DiscussionEventHandler.StartCoroutine(DiscussionEventHandler.OnDiscussionWindowSleep());
        }
        else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
        {
            AwakeDiscussionWindow((DiscussionTextOnlyNode)newDiscussionNode);
        }
        else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionChoiceNode))
        {
            DiscussionEventHandler.OnDiscussionChoiceStart((DiscussionChoiceNode)currentDiscussionTreeNode);
        }

    }

    private void AwakeDiscussionWindow(DiscussionTextOnlyNode discussionTreeNode)
    {
        var sentenceTalkerPOI = PointOfInterestManager.GetActivePointOfInterest(discussionTreeNode.Talker);
        DiscussionEventHandler.OnDiscussionWindowAwake(discussionTreeNode, sentenceTalkerPOI.transform);
    }

    private void OnDiscussionTextNodeEnd()
    {
        if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
        {
            var currentTextNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
            var nextNode = currentTextNode.GetNextNode();
            if (nextNode != null && nextNode.GetType() == typeof(DiscussionChoiceNode))
            {
                //choice node disaply
                OnNewCurrentNode(nextNode);
            }
            else
            {
                //finish discussion node
                DiscussionEventHandler.StartCoroutine(DiscussionEventHandler.OnDiscussionWindowSleep());
            }
        }
    }

    private void OnDiscussionChoiceNodeEnd(DiscussionChoiceTextId choiceMade)
    {
        discussionChoiceMade = choiceMade;
        DiscussionEventHandler.StartCoroutine(DiscussionEventHandler.OnDiscussionWindowSleep());
    }

    //This method is called when any discussion done is about to sleep
    private void OnDiscussionNodeFinished()
    {
        if (currentDiscussionTreeNode == null)
        {
            isConversationFinished = true;
            DiscussionEventHandler.DeleteEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);
        }
        else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
        {
            var currentTextNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
            var nextNode = currentTextNode.GetNextNode();
            if (nextNode != null)
            {
                OnNewCurrentNode(nextNode);
            }
            else
            {
                isConversationFinished = true;
                DiscussionEventHandler.DeleteEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);
            }
        }
        else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionChoiceNode))
        {
            var currentChoiceNode = (DiscussionChoiceNode)currentDiscussionTreeNode;
            var nextNode = currentChoiceNode.GetNextNode(discussionChoiceMade);
            if (nextNode != null)
            {
                OnNewCurrentNode(nextNode);
            }
            else
            {
                isConversationFinished = true;
                DiscussionEventHandler.DeleteEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);
            }
        }
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