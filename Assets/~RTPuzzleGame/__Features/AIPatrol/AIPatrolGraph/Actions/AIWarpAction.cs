using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class AIWarpAction : SequencedAction
    {
        [SerializeField]
        [CustomEnum()]
        public AIPositionMarkerID Position;

        #region External Dependencies
        [NonSerialized]
        private AIPositionsManager AIPositionsManager;
        #endregion

        public AIWarpAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed() { }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            var AIPatrolActionInput = (AIPatrolActionInput)ContextActionInput;
            var aiPatrolInteractiveObject = (InteractiveObjectType)AIPatrolActionInput.graphParameters[CutsceneParametersName.AIPatrol_InteractiveObject];
            this.AIPositionsManager = PuzzleGameSingletonInstances.AIPositionsManager;
            aiPatrolInteractiveObject.GetModule<InteractiveObjectCutsceneControllerModule>().InteractiveObjectCutsceneController.Warp(this.AIPositionsManager.GetPosition(this.Position).transform);
        }

        public override void Tick(float d)
        { }


#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.Position = (AIPositionMarkerID)NodeEditorGUILayout.EnumField("Position : ", string.Empty, this.Position);
        }
#endif
    }

}
