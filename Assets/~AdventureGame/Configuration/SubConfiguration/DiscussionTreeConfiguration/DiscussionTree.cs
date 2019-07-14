using GameConfigurationID;
using OdinSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class DiscussionTree : SerializedScriptableObject
    {
        public DiscussionNodeId DiscussionRootNode;
        public Dictionary<DiscussionNodeId, DiscussionTreeNode> DiscussionNodes;

        public DiscussionTextOnlyNode GetRootNode()
        {
            return (DiscussionTextOnlyNode)this.DiscussionNodes[DiscussionRootNode];
        }
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

        public DiscussionTreeNode GetNextNode(ref DiscussionTree discussionTreeRef)
        {
            return discussionTreeRef.DiscussionNodes[nextNode];
        }
    }

    [System.Serializable]
    public class DiscussionChoiceNode : DiscussionTreeNode
    {
        [SerializeField]
        private PointOfInterestId talker;
        [SerializeField]
        private List<DiscussionNodeId> discussionChoices;

        public DiscussionChoiceNode(DiscussionNodeId DiscussionTreeNodeId, PointOfInterestId talker, List<DiscussionNodeId> discussionChoices)
        {
            this.discussionNodeId = DiscussionTreeNodeId;
            this.talker = talker;
            this.discussionChoices = discussionChoices;
        }

        public PointOfInterestId Talker { get => talker; }
        public List<DiscussionNodeId> DiscussionChoices { get => discussionChoices; }

        public DiscussionTreeNode GetNextNode(DiscussionNodeId selectedChoice,ref DiscussionTree discussionTreeRef)
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
                    else if (selectedNode.GetType() == typeof(DiscussionTextOnlyNode))
                    {
                        return ((DiscussionTextOnlyNode)selectedNode).GetNextNode(ref discussionTreeRef);
                    }
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class DiscussionChoice : DiscussionTreeNode
    {
        [SerializeField]
        private DisucssionSentenceTextId text;
        [SerializeField]
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


        public DiscussionTreeNode GetNextNode(ref DiscussionTree discussionTreeRef)
        {
            return discussionTreeRef.DiscussionNodes[nextNode];
        }
    }
    
}