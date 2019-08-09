using GameConfigurationID;
using System;
using System.Collections.Generic;

namespace AdventureGame
{
    public class DiscussionTreePlayer
    {
        private bool isConversationFinished;

        private DiscussionTree playingDiscussionTree;
        private DiscussionTreeNode currentDiscussionTreeNode;
        private DiscussionNodeId discussionChoiceMade;
        
        private Action<DiscussionTextOnlyNode> OnDiscussionTextWindowAwake;
        private Action<List<DiscussionChoice>> OnChoicePopupAwake;

        #region Data Retrieval
        public bool IsConversationFinished { get => isConversationFinished; }
        #endregion

        public DiscussionTreePlayer(DiscussionTree playingDiscussionTree, Action<DiscussionTextOnlyNode> onDiscussionTextWindowAwake, Action<List<DiscussionChoice>> onChoicePopupAwake)
        {
            this.playingDiscussionTree = playingDiscussionTree;
            OnDiscussionTextWindowAwake = onDiscussionTextWindowAwake;
            OnChoicePopupAwake = onChoicePopupAwake;
            this.isConversationFinished = false;
        }

        private void OnNewCurrentNode(DiscussionTreeNode newDiscussionNode)
        {
            var oldDiscussionNode = currentDiscussionTreeNode;
            currentDiscussionTreeNode = newDiscussionNode;

            if (currentDiscussionTreeNode == null)
            {
                this.isConversationFinished = true;
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
            {
                this.OnDiscussionTextWindowAwake.Invoke((DiscussionTextOnlyNode)currentDiscussionTreeNode);
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionChoiceNode))
            {
                OnChoicePopupAwake.Invoke(((DiscussionChoiceNode)currentDiscussionTreeNode).DiscussionChoices.ConvertAll(discussionChoice => (DiscussionChoice)this.playingDiscussionTree.DiscussionNodes[discussionChoice]));
            }

        }

        public void StartDiscussion()
        {
            this.OnNewCurrentNode(this.playingDiscussionTree.GetRootNode());
        }

        public bool OnDiscussionTextNodeEnd()
        {
            if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
            {
                var currentTextNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
                var nextNode = currentTextNode.GetNextNode(ref this.playingDiscussionTree);
                if (nextNode != null && nextNode.GetType() == typeof(DiscussionChoiceNode))
                {
                    //choice node disaply
                    OnNewCurrentNode(nextNode);
                }
                else
                {
                    //finish discussion node
                    return true;
                }
            }
            return false;
        }

        public void OnDiscussionChoiceMade(DiscussionNodeId choiceMade)
        {
            discussionChoiceMade = choiceMade;
        }

        //This method is called when any discussion done is about to sleep
        public void OnDiscussionNodeFinished()
        {
            if (currentDiscussionTreeNode == null)
            {
                isConversationFinished = true;
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
            {
                var currentTextNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
                var nextNode = currentTextNode.GetNextNode(ref this.playingDiscussionTree);
                if (nextNode != null)
                {
                    OnNewCurrentNode(nextNode);
                }
                else
                {
                    isConversationFinished = true;
                }
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionChoiceNode))
            {
                var currentChoiceNode = (DiscussionChoiceNode)currentDiscussionTreeNode;
                var nextNode = currentChoiceNode.GetNextNode(discussionChoiceMade, ref this.playingDiscussionTree);
                if (nextNode != null)
                {
                    OnNewCurrentNode(nextNode);
                }
                else
                {
                    isConversationFinished = true;
                }
            }
        }
    }
}
