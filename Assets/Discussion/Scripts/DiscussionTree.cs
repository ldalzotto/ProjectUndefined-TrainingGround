using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiscussionTree
{
    [SerializeField]
    private DiscussionSentence discussionSentence;

    public DiscussionTree(DiscussionSentence discussionSentence)
    {
        this.discussionSentence = discussionSentence;
    }

    public DiscussionSentence DiscussionSentence { get => discussionSentence; }
}

[System.Serializable]
public class DiscussionSentence
{
    private DisucssionSentenceId displayedText;
    private PointOfInterestId talker;

    private DiscussionSentence nextSentence;

    public DiscussionSentence(DisucssionSentenceId displayedText, PointOfInterestId talker, DiscussionSentence nextSentence)
    {
        this.displayedText = displayedText;
        this.talker = talker;
        this.nextSentence = nextSentence;
    }

    public DisucssionSentenceId DisplayedText { get => displayedText; }
    public PointOfInterestId Talker { get => talker; }
    public DiscussionSentence NextSentence { get => nextSentence; }
}

public enum DisucssionSentenceId
{
    BOUNCER_SENTENCE_1,
    BOUNCER_SENTENCE_2,
    PLAYER_SENTENCE_1
}

public class DiscussionSentencesConstants
{
    public static Dictionary<DisucssionSentenceId, string> Sentences = new Dictionary<DisucssionSentenceId, string>()
    {
        {DisucssionSentenceId.BOUNCER_SENTENCE_1, "This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test." },
            {DisucssionSentenceId.BOUNCER_SENTENCE_2, "Bouncer never stops talking." },
         {DisucssionSentenceId.PLAYER_SENTENCE_1, "The Player is talking." }
    };
}