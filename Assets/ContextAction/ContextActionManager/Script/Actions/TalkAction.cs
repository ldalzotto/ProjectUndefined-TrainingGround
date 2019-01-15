using UnityEngine;

[System.Serializable]
public class TalkAction : AContextAction
{

    #region External Event Dependencies
    private DiscussionEventHandler DiscussionEventHandler;
    #endregion

    private bool isConversationFinished;

    public TalkAction() : base()
    {
        DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
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
        DiscussionEventHandler.AddOnSleepExternalHanlder(OnConversationFinished);
        DiscussionEventHandler.OnDiscussionWindowAwake(Vector3.zero, DiscussionSentencesConstants.Sentences[talkActionInput.DiscussionTree.DisplayedText]);
    }

    private void OnConversationFinished()
    {
        isConversationFinished = true;
        DiscussionEventHandler.RemoveOnSleepExternalHanlder(OnConversationFinished);
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