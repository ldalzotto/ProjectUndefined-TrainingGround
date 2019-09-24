using GameConfigurationID;
using System;
using UnityEngine;
using CoreGame;
using System.Collections.Generic;

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
        private AbstractDiscussionWindowManager PlayedDiscussionWindowManager;


#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.DiscussionTreeId = (DiscussionTreeId)NodeEditorGUILayout.EnumField("Discussion tree : ", string.Empty, this.DiscussionTreeId);
            this.ContextActionWheelNodeConfigurationId = (SelectionWheelNodeConfigurationId)NodeEditorGUILayout.EnumField("Wheel ID : ", string.Empty, this.ContextActionWheelNodeConfigurationId);
        }
#endif

        public TalkAction(DiscussionTreeId DiscussionTreeId, List<SequencedAction> nextContextActions, SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId) : base(nextContextActions, SelectionWheelNodeConfigurationId)
        {
            this.DiscussionTreeId = DiscussionTreeId;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return (this.PlayedDiscussionWindowManager != null && this.PlayedDiscussionWindowManager.IsDiscussionFinished()) || this.PlayedDiscussionWindowManager == null;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.PlayedDiscussionWindowManager = AdventureGameSingletonInstances.DiscussionEventHandler.OnDiscussionTreeStart(this.DiscussionTreeId);
        }
        
        public override void Tick(float d)
        { }

    }
}
