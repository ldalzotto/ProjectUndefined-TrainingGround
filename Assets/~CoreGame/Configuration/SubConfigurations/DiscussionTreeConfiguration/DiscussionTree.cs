using System;
using System.Collections.Generic;
using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    public class DiscussionTree : SerializedScriptableObject
    {
        public Dictionary<DiscussionNodeId, DiscussionTreeNode> DiscussionNodes;
        public DiscussionNodeId DiscussionRootNode;

        public AbstractDiscussionTextOnlyNode GetRootNode()
        {
            return (AbstractDiscussionTextOnlyNode) DiscussionNodes[DiscussionRootNode];
        }
    }

    [Serializable]
    public abstract class DiscussionTreeNode
    {
        [SerializeField] protected DiscussionNodeId discussionNodeId;
    }

    public abstract class AbstractDiscussionTextOnlyNode : DiscussionTreeNode
    {
        [SerializeField] private DiscussionTextID displayedText;
        [SerializeField] private DiscussionNodeId nextNode;

        protected AbstractDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode)
        {
            discussionNodeId = DiscussionNodeId;
            this.displayedText = displayedText;
            this.nextNode = nextNode;
        }

        public DiscussionTextID DisplayedText => displayedText;

        public DiscussionNodeId NextNode => nextNode;

        public DiscussionTreeNode GetNextNode(ref DiscussionTree discussionTreeRef)
        {
            return discussionTreeRef.DiscussionNodes[nextNode];
        }
    }

    [Serializable]
    public class FixedScreenPositionDiscussionTextOnlyNode : AbstractDiscussionTextOnlyNode
    {
        [SerializeField] private DiscussionPositionMarkerID discussionScreenPosition;

        public FixedScreenPositionDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode, DiscussionPositionMarkerID discussionScreenPosition) : base(DiscussionNodeId, displayedText, nextNode)
        {
            this.discussionScreenPosition = discussionScreenPosition;
        }

        public DiscussionPositionMarkerID DiscussionScreenPosition => discussionScreenPosition;
    }

    public abstract class AbstractDiscussionChoiceNode : DiscussionTreeNode
    {
        [SerializeField] private List<DiscussionNodeId> discussionChoices;

        protected AbstractDiscussionChoiceNode(DiscussionNodeId DiscussionTreeNodeId, List<DiscussionNodeId> discussionChoices)
        {
            discussionNodeId = DiscussionTreeNodeId;
            this.discussionChoices = discussionChoices;
        }

        public List<DiscussionNodeId> DiscussionChoices => discussionChoices;

        public DiscussionTreeNode GetNextNode(DiscussionNodeId selectedChoice, ref DiscussionTree discussionTreeRef)
        {
            foreach (var discussionChoice in discussionChoices)
                if (discussionChoice == selectedChoice)
                {
                    var selectedNode = discussionTreeRef.DiscussionNodes[selectedChoice];
                    if (selectedNode.GetType() == typeof(DiscussionChoice))
                        return ((DiscussionChoice) selectedNode).GetNextNode(ref discussionTreeRef);
                    else if (selectedNode.GetType().IsSubclassOf(typeof(AbstractDiscussionTextOnlyNode))) return ((AbstractDiscussionTextOnlyNode) selectedNode).GetNextNode(ref discussionTreeRef);
                }

            return null;
        }
    }

    [Serializable]
    public class DiscussionChoice : DiscussionTreeNode
    {
        [SerializeField] private DiscussionNodeId nextNode;
        [SerializeField] private DiscussionTextID text;

        public DiscussionChoice(DiscussionNodeId discussionNodeId, DiscussionTextID text, DiscussionNodeId nextNode)
        {
            this.discussionNodeId = discussionNodeId;
            this.text = text;
            this.nextNode = nextNode;
        }

        public DiscussionTextID Text => text;

        public DiscussionNodeId NextNode => nextNode;

        public DiscussionNodeId DiscussionNodeId => discussionNodeId;


        public DiscussionTreeNode GetNextNode(ref DiscussionTree discussionTreeRef)
        {
            return discussionTreeRef.DiscussionNodes[nextNode];
        }
    }
}