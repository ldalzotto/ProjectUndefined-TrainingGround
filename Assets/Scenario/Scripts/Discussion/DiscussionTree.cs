using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiscussionTree
{
    [SerializeField]
    private List<DisucssionSentenceId> possibleChoices;

    public DiscussionTree(List<DisucssionSentenceId> possibleChoices)
    {
        this.possibleChoices = possibleChoices;
    }
}

public enum DisucssionSentenceId
{
    BOUNCER_SENTENCE_1
}

public class DiscussionSentencesConstants
{
    public static Dictionary<DisucssionSentenceId, string> Sentences = new Dictionary<DisucssionSentenceId, string>()
    {
        {DisucssionSentenceId.BOUNCER_SENTENCE_1, "This is a test." }
    };
}

/**

public abstract class DiscussionTreeAction { }
public class DiscussionTreeActionPOI : DiscussionTreeAction
{
    private PointOfInterestId discussionPOIId;

    protected DiscussionTreeActionPOI(PointOfInterestId discussionPOIId)
    {
        this.discussionPOIId = discussionPOIId;
    }

    public PointOfInterestId DiscussionPOIId { get => discussionPOIId; }
}
**/
