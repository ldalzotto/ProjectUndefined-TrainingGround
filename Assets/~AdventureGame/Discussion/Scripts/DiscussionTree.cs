using OdinSerializer;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class DiscussionTree : SerializedScriptableObject
    {
        [SerializeField]
        private DiscussionTreeNode discussionRootNode;

        public DiscussionTree(DiscussionTreeNode discussionRootNode)
        {
            this.discussionRootNode = discussionRootNode;
        }

        public DiscussionTreeNode DiscussionRootNode { get => discussionRootNode; }
    }

    [System.Serializable]
    public abstract class DiscussionTreeNode
    {
        [SerializeField]
        protected DiscussionNodeId discussionNodeId;
    }

    [System.Serializable]
    public class DiscussionTextOnlyNode : DiscussionTreeNode
    {
        [SerializeField]
        private DisucssionSentenceTextId displayedText;
        [SerializeField]
        private PointOfInterestId talker;
        [SerializeField]
        private DiscussionNodeId nextNode;

        public DiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DisucssionSentenceTextId displayedText, PointOfInterestId talker, DiscussionNodeId nextNode)
        {
            this.discussionNodeId = DiscussionNodeId;
            this.displayedText = displayedText;
            this.talker = talker;
            this.nextNode = nextNode;
        }

        public DisucssionSentenceTextId DisplayedText { get => displayedText; }
        public PointOfInterestId Talker { get => talker; }
        public DiscussionNodeId NextNode { get => nextNode; }

        public DiscussionTreeNode GetNextNode()
        {
            return DiscussionSentencesConstants.DiscussionNodes[nextNode];
        }
    }

    [System.Serializable]
    public class DiscussionChoiceNode : DiscussionTreeNode
    {

        private PointOfInterestId talker;
        private List<DiscussionNodeId> discussionChoices;

        public DiscussionChoiceNode(DiscussionNodeId DiscussionTreeNodeId, PointOfInterestId talker, List<DiscussionNodeId> discussionChoices)
        {
            this.discussionNodeId = DiscussionTreeNodeId;
            this.talker = talker;
            this.discussionChoices = discussionChoices;
        }

        public PointOfInterestId Talker { get => talker; }

        public List<DiscussionChoice> GetDiscussionChoices()
        {
            return this.discussionChoices.ConvertAll(c => (DiscussionChoice)DiscussionSentencesConstants.DiscussionNodes[c]);
        }

        public DiscussionTreeNode GetNextNode(DiscussionNodeId selectedChoice)
        {
            foreach (var discussionChoice in discussionChoices)
            {
                if (discussionChoice == selectedChoice)
                {
                    var selectedNode = DiscussionSentencesConstants.DiscussionNodes[selectedChoice];
                    if (selectedNode.GetType() == typeof(DiscussionChoice))
                    {
                        return ((DiscussionChoice)selectedNode).GetNextNode();
                    }
                    else if (selectedNode.GetType() == typeof(DiscussionTextOnlyNode))
                    {
                        return ((DiscussionTextOnlyNode)selectedNode).GetNextNode();
                    }
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class DiscussionChoice : DiscussionTreeNode
    {
        private DisucssionSentenceTextId text;
        private DiscussionNodeId nextNode;

        public DiscussionChoice(DiscussionNodeId discussionNodeId, DisucssionSentenceTextId text, DiscussionNodeId nextNode)
        {
            this.discussionNodeId = discussionNodeId;
            this.text = text;
            this.nextNode = nextNode;
        }

        public DisucssionSentenceTextId Text { get => text; }
        public DiscussionNodeId NextNode { get => nextNode; }
        public DiscussionNodeId DiscussionNodeId { get => discussionNodeId; }


        public DiscussionTreeNode GetNextNode()
        {
            return DiscussionSentencesConstants.DiscussionNodes[nextNode];
        }
    }

    #region Discussion Tree Workflow
    public enum DiscussionTreeId
    {
        BOUNCER_DISCUSSION_TREE,
        BOUNCER_OK_DISCUSSION
    }

    public enum DiscussionNodeId
    {
        NONE = 0,
        BOUNCER_FORBIDDEN_INTRODUCTION,
        BOUNCER_ASK_AGE,
        BOUNCER_OK_INTRODUCTION,
        PLAYER_AGE_CHOICE,
        PLAYER_AGE_CHOICE_17,
        PLAYER_AGE_CHOICE_18,
        BOUNCER_GET_OUT
    }

    public class DiscussionSentencesConstants
    {
        public static Dictionary<DiscussionNodeId, DiscussionTreeNode> DiscussionNodes = new Dictionary<DiscussionNodeId, DiscussionTreeNode>()
        {
            { DiscussionNodeId.NONE, null },
            { DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION, new DiscussionTextOnlyNode(DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION, DisucssionSentenceTextId.BOUNCER_FORBIDDEN_INTRODUCTION,  PointOfInterestId.BOUNCER, DiscussionNodeId.BOUNCER_ASK_AGE  ) },
            { DiscussionNodeId.BOUNCER_ASK_AGE, new DiscussionTextOnlyNode(DiscussionNodeId.BOUNCER_ASK_AGE, DisucssionSentenceTextId.BOUNCER_ASK_AGE, PointOfInterestId.BOUNCER, DiscussionNodeId.PLAYER_AGE_CHOICE ) },
            { DiscussionNodeId.PLAYER_AGE_CHOICE, new DiscussionChoiceNode(DiscussionNodeId.PLAYER_AGE_CHOICE, PointOfInterestId.PLAYER, new List<DiscussionNodeId>(){ DiscussionNodeId.PLAYER_AGE_CHOICE_17, DiscussionNodeId.PLAYER_AGE_CHOICE_18 } ) },
            { DiscussionNodeId.PLAYER_AGE_CHOICE_17, new DiscussionChoice(DiscussionNodeId.PLAYER_AGE_CHOICE_17, DisucssionSentenceTextId.PLAYER_AGE_CHOICE_17,  DiscussionNodeId.BOUNCER_GET_OUT ) },
            { DiscussionNodeId.BOUNCER_GET_OUT, new DiscussionTextOnlyNode(DiscussionNodeId.BOUNCER_GET_OUT, DisucssionSentenceTextId.BOUNCER_GET_OUT, PointOfInterestId.BOUNCER, DiscussionNodeId.NONE) },
            { DiscussionNodeId.PLAYER_AGE_CHOICE_18, new DiscussionChoice(DiscussionNodeId.PLAYER_AGE_CHOICE_18, DisucssionSentenceTextId.PLAYER_AGE_CHOICE_18,  DiscussionNodeId.NONE ) },
            { DiscussionNodeId.BOUNCER_OK_INTRODUCTION, new DiscussionTextOnlyNode(DiscussionNodeId.BOUNCER_OK_INTRODUCTION, DisucssionSentenceTextId.BOUNCER_ASK_AGE,  PointOfInterestId.BOUNCER, DiscussionNodeId.NONE  ) }
        };

        public static Dictionary<DiscussionTreeId, DiscussionTreeNode> DiscussionTrees = new Dictionary<DiscussionTreeId, DiscussionTreeNode>()
        {
            {DiscussionTreeId.BOUNCER_DISCUSSION_TREE, DiscussionSentencesConstants.DiscussionNodes[DiscussionNodeId.BOUNCER_FORBIDDEN_INTRODUCTION]},
            {DiscussionTreeId.BOUNCER_OK_DISCUSSION, DiscussionSentencesConstants.DiscussionNodes[DiscussionNodeId.BOUNCER_OK_INTRODUCTION] }

        };
    }

    #endregion

}