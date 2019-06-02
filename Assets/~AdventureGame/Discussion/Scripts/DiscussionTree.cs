using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    [System.Serializable]
    public class DiscussionTree
    {

        [SerializeField]
        private DiscussionTreeNode discussionRootNode;

        public DiscussionTree(DiscussionTreeNode discussionRootNode)
        {
            this.discussionRootNode = discussionRootNode;
        }

        public DiscussionTreeNode DiscussionRootNode { get => discussionRootNode; }
    }

    public interface DiscussionTreeNode
    {
    }

    [System.Serializable]
    public class DiscussionTextOnlyNode : DiscussionTreeNode
    {
        [SerializeField]
        private DiscussionNodeId discussionNodeId;
        [SerializeField]
        private DisucssionSentenceTextId displayedText;
        [SerializeField]
        private PointOfInterestId talker;
        
        [SerializeField]
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
    
}