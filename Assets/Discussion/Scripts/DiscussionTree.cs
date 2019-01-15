using System.Collections.Generic;

[System.Serializable]
public class DiscussionTree
{
    private DiscussionTreeNode discussionRootNode;

    public DiscussionTree(DiscussionTreeNode discussionRootNode)
    {
        this.discussionRootNode = discussionRootNode;
    }

    public DiscussionTreeNode DiscussionRootNode { get => discussionRootNode; }
}

public interface DiscussionTreeNode
{
    List<DiscussionTreeNode> GetNextNodes();
}

public class DiscussionTextOnlyNode : DiscussionTreeNode
{
    private DisucssionSentenceTextId displayedText;
    private PointOfInterestId talker;

    private DiscussionTreeNode nextNode;

    public DiscussionTextOnlyNode(DisucssionSentenceTextId displayedText, PointOfInterestId talker, DiscussionTreeNode nextNode)
    {
        this.displayedText = displayedText;
        this.talker = talker;
        this.nextNode = nextNode;
    }

    public DisucssionSentenceTextId DisplayedText { get => displayedText; }
    public PointOfInterestId Talker { get => talker; }
    public DiscussionTreeNode NextNode { get => nextNode; }

    public List<DiscussionTreeNode> GetNextNodes()
    {
        if (nextNode == null)
        {
            return new List<DiscussionTreeNode>();
        }
        else
        {
            return new List<DiscussionTreeNode>() { nextNode };
        }
    }
}


public class DiscussionChoiceNode : DiscussionTreeNode
{
    private string introText;
    private List<DiscussionChoice> discussionChoices;

    public DiscussionChoiceNode(string introText, List<DiscussionChoice> discussionChoices)
    {
        this.introText = introText;
        this.discussionChoices = discussionChoices;
    }

    public List<DiscussionTreeNode> GetNextNodes()
    {
        return null;
    }
}

public class DiscussionChoice
{
    private string text;
    private DiscussionTreeNode nextNode;

    public DiscussionChoice(string text, DiscussionTreeNode nextNode)
    {
        this.text = text;
        this.nextNode = nextNode;
    }
}

#region Discussion Sentence Workflow
public enum DiscussionSentenceId
{
    BOUNCER_SENTENCE
}

public class DiscussionSentencesConstants
{
    public static Dictionary<DiscussionSentenceId, DiscussionTextOnlyNode> Sentenses = new Dictionary<DiscussionSentenceId, DiscussionTextOnlyNode>()
    {
        {DiscussionSentenceId.BOUNCER_SENTENCE,
                            new DiscussionTextOnlyNode(DisucssionSentenceTextId.BOUNCER_SENTENCE_TEXT_1, PointOfInterestId.BOUNCER,
                            new DiscussionTextOnlyNode(DisucssionSentenceTextId.PLAYER_SENTENCE_TEXT_1, PointOfInterestId.PLAYER,
                            new DiscussionTextOnlyNode(DisucssionSentenceTextId.BOUNCER_SENTENCE_TEXT_2, PointOfInterestId.BOUNCER, null)
                        )) }
    };
}

#endregion

#region Discussion Sentence Text
public enum DisucssionSentenceTextId
{
    BOUNCER_SENTENCE_TEXT_1,
    BOUNCER_SENTENCE_TEXT_2,
    PLAYER_SENTENCE_TEXT_1
}

public class DiscussionSentencesTextConstants
{

    public static Dictionary<DisucssionSentenceTextId, string> SentencesText = new Dictionary<DisucssionSentenceTextId, string>()
    {
        {DisucssionSentenceTextId.BOUNCER_SENTENCE_TEXT_1, "This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test. This is a test." },
         {DisucssionSentenceTextId.BOUNCER_SENTENCE_TEXT_2, "Bouncer never stops talking." },
         {DisucssionSentenceTextId.PLAYER_SENTENCE_TEXT_1, "The Player is talking." }
    };
}
#endregion

