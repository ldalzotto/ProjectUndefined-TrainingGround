using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiscussionTree
{
    [SerializeField]
    private DisucssionSentenceId displayedText;

    public DiscussionTree(DisucssionSentenceId displayedText)
    {
        this.displayedText = displayedText;
    }

    public DisucssionSentenceId DisplayedText { get => displayedText; }
}

public enum DisucssionSentenceId
{
    BOUNCER_SENTENCE_1
}

public class DiscussionSentencesConstants
{
    public static Dictionary<DisucssionSentenceId, string> Sentences = new Dictionary<DisucssionSentenceId, string>()
    {
        {DisucssionSentenceId.BOUNCER_SENTENCE_1, "This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test." }
    };
}