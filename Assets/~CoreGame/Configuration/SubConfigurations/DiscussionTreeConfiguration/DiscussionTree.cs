using GameConfigurationID;
using OdinSerializer;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class DiscussionTree : SerializedScriptableObject
    {
        public DiscussionNodeId DiscussionRootNode;
        public Dictionary<DiscussionNodeId, DiscussionTreeNode> DiscussionNodes;

        public AbstractDiscussionTextOnlyNode GetRootNode()
        {
            return (AbstractDiscussionTextOnlyNode)this.DiscussionNodes[DiscussionRootNode];
        }
    }

    [System.Serializable]
    public abstract class DiscussionTreeNode
    {
        [SerializeField]
        protected DiscussionNodeId discussionNodeId;
    }

    public abstract class AbstractDiscussionTextOnlyNode : DiscussionTreeNode
    {
        [SerializeField]
        private DiscussionTextID displayedText;
        [SerializeField]
        private DiscussionNodeId nextNode;

        protected AbstractDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode)
        {
            this.discussionNodeId = DiscussionNodeId;
            this.displayedText = displayedText;
            this.nextNode = nextNode;
        }

        public DiscussionTextID DisplayedText { get => displayedText; }
        public DiscussionNodeId NextNode { get => nextNode; }

        public DiscussionTreeNode GetNextNode(ref DiscussionTree discussionTreeRef)
        {
            return discussionTreeRef.DiscussionNodes[nextNode];
        }
    }

    [System.Serializable]
    public class AdventureDiscussionTextOnlyNode : AbstractDiscussionTextOnlyNode
    {
        [SerializeField]
        private PointOfInterestId talker;

        public AdventureDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode, PointOfInterestId talker) : base(DiscussionNodeId, displayedText, nextNode)
        {
            this.talker = talker;
        }

        public PointOfInterestId Talker { get => talker; }
    }

    [System.Serializable]
    public class PuzzleDiscussionTextOnlyNode : AbstractDiscussionTextOnlyNode
    {
        [SerializeField]
        private InteractiveObjectID talker;

        public PuzzleDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode, InteractiveObjectID talker) : base(DiscussionNodeId, displayedText, nextNode)
        {
            this.talker = talker;
        }

        public InteractiveObjectID Talker { get => talker; }
    }

    [System.Serializable]
    public class FixedScreenPositionDiscussionTextOnlyNode : AbstractDiscussionTextOnlyNode
    {
        [SerializeField]
        private DiscussionPositionMarkerID discussionScreenPosition;

        public FixedScreenPositionDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode, DiscussionPositionMarkerID discussionScreenPosition) : base(DiscussionNodeId, displayedText, nextNode)
        {
            this.discussionScreenPosition = discussionScreenPosition;
        }

        public DiscussionPositionMarkerID DiscussionScreenPosition { get => discussionScreenPosition; }
    }

    public abstract class AbstractDiscussionChoiceNode : DiscussionTreeNode
    {
        [SerializeField]
        private List<DiscussionNodeId> discussionChoices;

        protected AbstractDiscussionChoiceNode(DiscussionNodeId DiscussionTreeNodeId, List<DiscussionNodeId> discussionChoices)
        {
            this.discussionNodeId = DiscussionTreeNodeId;
            this.discussionChoices = discussionChoices;
        }

        public List<DiscussionNodeId> DiscussionChoices { get => discussionChoices; }

        public DiscussionTreeNode GetNextNode(DiscussionNodeId selectedChoice, ref DiscussionTree discussionTreeRef)
        {
            foreach (var discussionChoice in discussionChoices)
            {
                if (discussionChoice == selectedChoice)
                {
                    var selectedNode = discussionTreeRef.DiscussionNodes[selectedChoice];
                    if (selectedNode.GetType() == typeof(DiscussionChoice))
                    {
                        return ((DiscussionChoice)selectedNode).GetNextNode(ref discussionTreeRef);
                    }
                    else if (selectedNode.GetType().IsSubclassOf(typeof(AbstractDiscussionTextOnlyNode)))
                    {
                        return ((AbstractDiscussionTextOnlyNode)selectedNode).GetNextNode(ref discussionTreeRef);
                    }
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class AdventureDiscussionChoiceNode : AbstractDiscussionChoiceNode
    {
        [SerializeField]
        private PointOfInterestId talker;

        public AdventureDiscussionChoiceNode(DiscussionNodeId DiscussionTreeNodeId, List<DiscussionNodeId> discussionChoices, PointOfInterestId talker) : base(DiscussionTreeNodeId, discussionChoices)
        {
            this.talker = talker;
        }

        public PointOfInterestId Talker { get => talker; }
    }

    [System.Serializable]
    public class DiscussionChoice : DiscussionTreeNode
    {
        [SerializeField]
        private DiscussionTextID text;
        [SerializeField]
        private DiscussionNodeId nextNode;

        public DiscussionChoice(DiscussionNodeId discussionNodeId, DiscussionTextID text, DiscussionNodeId nextNode)
        {
            this.discussionNodeId = discussionNodeId;
            this.text = text;
            this.nextNode = nextNode;
        }

        public DiscussionTextID Text { get => text; }
        public DiscussionNodeId NextNode { get => nextNode; }
        public DiscussionNodeId DiscussionNodeId { get => discussionNodeId; }


        public DiscussionTreeNode GetNextNode(ref DiscussionTree discussionTreeRef)
        {
            return discussionTreeRef.DiscussionNodes[nextNode];
        }
    }

}