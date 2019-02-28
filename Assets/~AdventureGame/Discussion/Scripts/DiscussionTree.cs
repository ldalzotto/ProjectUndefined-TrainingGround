using System.Collections.Generic;

namespace AdventureGame
{

    [System.Serializable]
    public class DiscussionTree
    {
        private DiscussionTreeNode discussionRootNode;

        public DiscussionTree(DiscussionTreeNode discussionRootNode)
        {
            this.discussionRootNode = discussionRootNode;
        }

        public DiscussionTreeNode DiscussionRootNode { get => discussionRootNode; }

        public void BreakConnectionAtEndOfStack(Stack<DiscussionNodeId> nodeIdsStack)
        {
            discussionRootNode.BreakConnectionAtEndOfStack(ref nodeIdsStack);
        }
    }

    public interface DiscussionTreeNode
    {
        void BreakConnectionAtEndOfStack(ref Stack<DiscussionNodeId> nodeIdsStack);
    }

    [System.Serializable]
    public class DiscussionTextOnlyNode : DiscussionTreeNode
    {
        private DiscussionNodeId discussionNodeId;
        private DisucssionSentenceTextId displayedText;
        private PointOfInterestId talker;

        private DiscussionTreeNode nextNode;

        public DiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DisucssionSentenceTextId displayedText, PointOfInterestId talker, DiscussionTreeNode nextNode)
        {
            this.discussionNodeId = DiscussionNodeId;
            this.displayedText = displayedText;
            this.talker = talker;
            this.nextNode = nextNode;
        }

        public DisucssionSentenceTextId DisplayedText { get => displayedText; }
        public PointOfInterestId Talker { get => talker; }
        public DiscussionTreeNode NextNode { get => nextNode; }

        public void BreakConnectionAtEndOfStack(ref Stack<DiscussionNodeId> nodeIdsStack)
        {
            var idToSeek = nodeIdsStack.Pop();
            if (discussionNodeId == idToSeek)
            {
                if (nodeIdsStack.Count > 0)
                {
                    nextNode.BreakConnectionAtEndOfStack(ref nodeIdsStack);
                }
                else
                {
                    //break connection
                    nextNode = null;
                }
            }
        }

        public DiscussionTreeNode GetNextNode()
        {
            return nextNode;
        }
    }

    [System.Serializable]
    public class DiscussionChoiceNode : DiscussionTreeNode
    {
        private DiscussionNodeId discussionTreeNodeId;

        private PointOfInterestId talker;
        private List<DiscussionChoice> discussionChoices;

        public DiscussionChoiceNode(DiscussionNodeId DiscussionTreeNodeId, PointOfInterestId talker, List<DiscussionChoice> discussionChoices)
        {
            this.discussionTreeNodeId = DiscussionTreeNodeId;
            this.talker = talker;
            this.discussionChoices = discussionChoices;
        }

        public PointOfInterestId Talker { get => talker; }
        public List<DiscussionChoice> DiscussionChoices { get => discussionChoices; }

        public void BreakConnectionAtEndOfStack(ref Stack<DiscussionNodeId> nodeIdsStack)
        {
            var idToSeek = nodeIdsStack.Pop();
            DiscussionChoice choiceToRemove = null;

            if (idToSeek == discussionTreeNodeId)
            {
                if (nodeIdsStack.Count > 0)
                {
                    var choiceIdSeek = nodeIdsStack.Pop();
                    foreach (var discussionChoice in discussionChoices)
                    {
                        if (discussionChoice.DiscussionNodeId == choiceIdSeek)
                        {
                            if (nodeIdsStack.Count > 0)
                            {
                                discussionChoice.BreakConnectionAtEndOfStack(ref nodeIdsStack);
                            }
                            else
                            {
                                choiceToRemove = discussionChoice;
                            }
                        }
                    }

                    if (choiceToRemove != null)
                    {
                        discussionChoices.Remove(choiceToRemove);
                    }
                }
                else
                {
                    discussionChoices = new List<DiscussionChoice>();
                }

            }
        }

        public DiscussionTreeNode GetNextNode(DiscussionChoiceTextId selectedChoice)
        {
            foreach (var discussionChoice in discussionChoices)
            {
                if (discussionChoice.Text == selectedChoice)
                {
                    return discussionChoice.NextNode;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class DiscussionChoice
    {
        private DiscussionNodeId discussionNodeId;
        private DiscussionChoiceTextId text;
        private DiscussionTreeNode nextNode;

        public DiscussionChoice(DiscussionNodeId discussionNodeId, DiscussionChoiceTextId text, DiscussionTreeNode nextNode)
        {
            this.discussionNodeId = discussionNodeId;
            this.text = text;
            this.nextNode = nextNode;
        }

        public DiscussionChoiceTextId Text { get => text; }
        public DiscussionTreeNode NextNode { get => nextNode; }
        public DiscussionNodeId DiscussionNodeId { get => discussionNodeId; }

        internal void BreakConnectionAtEndOfStack(ref Stack<DiscussionNodeId> nodeIdsStack)
        {
            var idToSeek = nodeIdsStack.Pop();
            if (discussionNodeId == idToSeek)
            {
                if (nodeIdsStack.Count > 0)
                {
                    nextNode.BreakConnectionAtEndOfStack(ref nodeIdsStack);
                }
                else
                {
                    //break connection
                    nextNode = null;
                }
            }
        }
    }

    public class DiscussionChoiceEvent
    {
        private DiscussionChoiceTextId text;
    }

    #region Discussion Tree Workflow
    public enum DiscussionTreeId
    {
        BOUNCER_DISCUSSION_TREE,
        BOUNCER_OK_DISCUSSION
    }

    public enum DiscussionNodeId
    {
        BOUNCER_FORBIDDEN_INTRODUCTION,
        BOUNCER_OK_INTRODUCTION
    }

    public class DiscussionSentencesConstants
    {
        public static Dictionary<DiscussionTreeId, DiscussionTextOnlyNode> Sentenses = new Dictionary<DiscussionTreeId, DiscussionTextOnlyNode>()
    {
        {DiscussionTreeId.BOUNCER_DISCUSSION_TREE, new DiscussionTextOnlyNode(
                DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION, DisucssionSentenceTextId.BOUNCER_FORBIDDEN_INTRODUCTION, PointOfInterestId.BOUNCER, new DiscussionTextOnlyNode(
                       DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION, DisucssionSentenceTextId.BOUNCER_ASK_AGE, PointOfInterestId.BOUNCER,
                       new DiscussionChoiceNode(DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION, PointOfInterestId.PLAYER, new List<DiscussionChoice>(){
                           new DiscussionChoice(DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION, DiscussionChoiceTextId.PLAYER_AGE_CHOICE_17, null),
                           new DiscussionChoice(DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION, DiscussionChoiceTextId.PLAYER_AGE_CHOICE_18, null)
                       })
                    )
            )},
        { DiscussionTreeId.BOUNCER_OK_DISCUSSION, new DiscussionTextOnlyNode(
                   DiscussionNodeId.BOUNCER_OK_INTRODUCTION, DisucssionSentenceTextId.BOUNCER_ALLOWED, PointOfInterestId.BOUNCER, null
            )}
    };
    }

    #endregion

    #region Discussion Sentence Text
    public enum DisucssionSentenceTextId
    {
        BOUNCER_FORBIDDEN_INTRODUCTION,
        BOUNCER_ASK_AGE,
        BOUNCER_GET_OUT,
        BOUNCER_ALLOWED,
        PLAYER_TELL_AGE
    }

    public class DiscussionSentencesTextConstants
    {

        public static Dictionary<DisucssionSentenceTextId, string> SentencesText = new Dictionary<DisucssionSentenceTextId, string>()
    {
        {DisucssionSentenceTextId.BOUNCER_FORBIDDEN_INTRODUCTION, "I don't like your haircut.\nAge below 18 = NO ENTRY." },
        {DisucssionSentenceTextId.BOUNCER_ASK_AGE, "How old are you?"},
        {DisucssionSentenceTextId.BOUNCER_GET_OUT, "Ridiculous.\nGet out!" },
        {DisucssionSentenceTextId.BOUNCER_ALLOWED, "You have the right to pass."},
        {DisucssionSentenceTextId.PLAYER_TELL_AGE, "18 years old." }
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
        PLAYER_AGE_CHOICE_17,
        PLAYER_AGE_CHOICE_18
    }

    public class DiscussionChoiceTextConstants
    {
        public static Dictionary<DiscussionChoiceTextId, string> ChoiceTexts = new Dictionary<DiscussionChoiceTextId, string>()
    {
        { DiscussionChoiceTextId.PLAYER_AGE_CHOICE_17, "17"},
        { DiscussionChoiceTextId.PLAYER_AGE_CHOICE_18, "18"}
    };
    }
    #endregion
}