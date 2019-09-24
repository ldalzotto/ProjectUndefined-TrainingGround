using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneDiscussionAction : SequencedAction, IContextActionDyamicWorkflowListener
    {
        [CustomEnum()]
        [SerializeField]
        public DiscussionTreeId DiscussionTreeID;

        [NonSerialized]
        private bool discussionEnded = false;

        public CutsceneDiscussionAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.discussionEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.discussionEnded = false;
            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;
            cutsceneActionInput.ContextActionEventManager.OnContextActionAdd(new TalkAction(this.DiscussionTreeID, null), this);
        }

        public void OnContextActionFinished(AContextAction contextAction)
        {
            this.discussionEnded = true;
        }

        public override void Tick(float d)
        {

        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.DiscussionTreeID = (DiscussionTreeId)NodeEditorGUILayout.EnumField("Discussion : ", string.Empty, this.DiscussionTreeID);
        }
#endif
    }

}
