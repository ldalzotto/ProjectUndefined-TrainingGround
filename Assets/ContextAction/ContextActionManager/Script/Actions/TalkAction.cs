using UnityEngine;

[System.Serializable]
public class TalkAction : AContextAction
{

    #region External Event Dependencies
    private DiscussionEventHandler DiscussionEventHandler;
    private PointOfInterestManager PointOfInterestManager;
    #endregion

    private bool isConversationFinished;
    private DiscussionSentence currentDiscussionSentence;

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

        SetupSentence(talkActionInput.DiscussionTree.DiscussionSentence);
    }

    private void SetupSentence(DiscussionSentence discussionSentence)
    {
        currentDiscussionSentence = discussionSentence;
        var sentenceTalkerPOI = PointOfInterestManager.GetActivePointOfInterest(currentDiscussionSentence.Talker);
        DiscussionEventHandler.OnDiscussionWindowAwake(sentenceTalkerPOI.transform, DiscussionSentencesConstants.Sentences[currentDiscussionSentence.DisplayedText]);
    }

    private void OnSentenceFinished()
    {
        if (currentDiscussionSentence.NextSentence != null)
        {
            SetupSentence(currentDiscussionSentence.NextSentence);
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