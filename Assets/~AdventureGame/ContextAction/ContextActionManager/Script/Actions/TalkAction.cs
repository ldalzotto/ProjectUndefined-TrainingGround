using GameConfigurationID;
using System;
using UnityEngine;
using CoreGame;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{

    [System.Serializable]
    public class TalkAction : AContextAction
    {

        [SerializeField]
        public DiscussionTreeId DiscussionTreeId;

        #region External Event Dependencies
        [NonSerialized]
        private DiscussionEventHandler DiscussionEventHandler;
        [NonSerialized]
        private PointOfInterestManager PointOfInterestManager;
        #endregion

        [NonSerialized]
        private bool isConversationFinished;
        [NonSerialized]
        private DiscussionTree readingDiscussionTree;
        [NonSerialized]
        private DiscussionTreeNode currentDiscussionTreeNode;
        [NonSerialized]
        private DiscussionNodeId discussionChoiceMade;

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.DiscussionTreeId = (DiscussionTreeId)NodeEditorGUILayout.EnumField("Discussion tree : ", string.Empty, this.DiscussionTreeId);
        }
#endif

        public TalkAction(DiscussionTreeId DiscussionTreeId, AContextAction nextContextAction) : base(nextContextAction)
        {
            this.DiscussionTreeId = DiscussionTreeId;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return isConversationFinished;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
            PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            var AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();

            this.readingDiscussionTree = AdventureGameConfigurationManager.DiscussionTreeConf()[this.DiscussionTreeId];
            isConversationFinished = false;
            DiscussionEventHandler.InitializeEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);

            OnNewCurrentNode(this.readingDiscussionTree.GetRootNode());
        }

        private void OnNewCurrentNode(DiscussionTreeNode newDiscussionNode)
        {
            var oldDiscussionNode = currentDiscussionTreeNode;
            currentDiscussionTreeNode = newDiscussionNode;

            if (currentDiscussionTreeNode == null)
            {
                DiscussionEventHandler.StartCoroutine(DiscussionEventHandler.OnDiscussionWindowSleep());
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
            {
                AwakeDiscussionWindow((DiscussionTextOnlyNode)newDiscussionNode);
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionChoiceNode))
            {
                DiscussionEventHandler.OnDiscussionChoiceStart(((DiscussionChoiceNode)currentDiscussionTreeNode).DiscussionChoices.ConvertAll(discussionChoice => (DiscussionChoice)this.readingDiscussionTree.DiscussionNodes[discussionChoice]));
            }

        }

        private void AwakeDiscussionWindow(DiscussionTextOnlyNode discussionTreeNode)
        {
            var sentenceTalkerPOI = PointOfInterestManager.GetActivePointOfInterest(discussionTreeNode.Talker);
            DiscussionEventHandler.OnDiscussionWindowAwake(discussionTreeNode, sentenceTalkerPOI.transform);
        }

        private void OnDiscussionTextNodeEnd()
        {
            if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
            {
                var currentTextNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
                var nextNode = currentTextNode.GetNextNode(ref this.readingDiscussionTree);
                if (nextNode != null && nextNode.GetType() == typeof(DiscussionChoiceNode))
                {
                    //choice node disaply
                    OnNewCurrentNode(nextNode);
                }
                else
                {
                    //finish discussion node
                    DiscussionEventHandler.StartCoroutine(DiscussionEventHandler.OnDiscussionWindowSleep());
                }
            }
        }

        private void OnDiscussionChoiceNodeEnd(DiscussionNodeId choiceMade)
        {
            discussionChoiceMade = choiceMade;
            DiscussionEventHandler.StartCoroutine(DiscussionEventHandler.OnDiscussionWindowSleep());
        }

        //This method is called when any discussion done is about to sleep
        private void OnDiscussionNodeFinished()
        {
            if (currentDiscussionTreeNode == null)
            {
                isConversationFinished = true;
                DiscussionEventHandler.DeleteEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionTextOnlyNode))
            {
                var currentTextNode = (DiscussionTextOnlyNode)currentDiscussionTreeNode;
                var nextNode = currentTextNode.GetNextNode(ref this.readingDiscussionTree);
                if (nextNode != null)
                {
                    OnNewCurrentNode(nextNode);
                }
                else
                {
                    isConversationFinished = true;
                    DiscussionEventHandler.DeleteEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);
                }
            }
            else if (currentDiscussionTreeNode.GetType() == typeof(DiscussionChoiceNode))
            {
                var currentChoiceNode = (DiscussionChoiceNode)currentDiscussionTreeNode;
                var nextNode = currentChoiceNode.GetNextNode(discussionChoiceMade, ref this.readingDiscussionTree);
                if (nextNode != null)
                {
                    OnNewCurrentNode(nextNode);
                }
                else
                {
                    isConversationFinished = true;
                    DiscussionEventHandler.DeleteEventHanlders(OnDiscussionNodeFinished, OnDiscussionTextNodeEnd, OnDiscussionChoiceNodeEnd);
                }
            }
        }

        public override void Tick(float d)
        { }

    }
}
