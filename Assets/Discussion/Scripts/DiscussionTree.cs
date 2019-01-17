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
    DiscussionTreeNode GetNextNode();
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

    public DiscussionTreeNode GetNextNode()
    {
        return nextNode;
    }
}


public class DiscussionChoiceNode : DiscussionTreeNode
{
    private PointOfInterestId talker;
    private List<DiscussionChoice> discussionChoices;

    public DiscussionChoiceNode(PointOfInterestId talker, List<DiscussionChoice> discussionChoices)
    {
        this.talker = talker;
        this.discussionChoices = discussionChoices;
    }

    public PointOfInterestId Talker { get => talker; }
    public List<DiscussionChoice> DiscussionChoices { get => discussionChoices; }

    public DiscussionTreeNode GetNextNode()
    {
        return null;
    }
}

public class DiscussionChoice
{
    private DiscussionChoiceTextId text;
    private DiscussionTreeNode nextNode;

    public DiscussionChoice(DiscussionChoiceTextId text, DiscussionTreeNode nextNode)
    {
        this.text = text;
        this.nextNode = nextNode;
    }

    public DiscussionChoiceTextId Text { get => text; }
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
                                    new DiscussionChoiceNode(PointOfInterestId.PLAYER,
                                             new List<DiscussionChoice>(){
                                                 new DiscussionChoice(DiscussionChoiceTextId.BOUNCER_CHOICE_1, null),
                                                 new DiscussionChoice(DiscussionChoiceTextId.BOUNCER_CHOICE_2, null),
                                                 new DiscussionChoice(DiscussionChoiceTextId.BOUNCER_CHOICE_3, null)
                                             }
                                        )
                                )}
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

#region Discussion Choice Text
public enum DiscussionChoiceIntroductionTextId
{
    BOUNCER_CHOICE_INTRO_1
}

public enum DiscussionChoiceTextId
{
    BOUNCER_CHOICE_1,
    BOUNCER_CHOICE_2,
    BOUNCER_CHOICE_3
}

public class DiscussionChoiceTextConstants
{
    public static Dictionary<DiscussionChoiceTextId, string> ChoiceTexts = new Dictionary<DiscussionChoiceTextId, string>()
    {
        {DiscussionChoiceTextId.BOUNCER_CHOICE_1, "Choice 1" },
        {DiscussionChoiceTextId.BOUNCER_CHOICE_2, "Choice 2" },
        {DiscussionChoiceTextId.BOUNCER_CHOICE_3, "Choice 3" }
    };
}
#endregion