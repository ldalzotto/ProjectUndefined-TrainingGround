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
    public class AIMoveToAction : SequencedAction
    {
        [SerializeField]
        [CustomEnum()]
        public AIPositionMarkerID Position;

        [NonSerialized]
        private bool destinationReached = false;
        [NonSerialized]
        private AIPatrolActionInput AIPatrolActionInput;

        #region External Dependencies
        [NonSerialized]
        private AIPositionsManager AIPositionsManager;
        #endregion

        public AIMoveToAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed() { }

        public override bool ComputeFinishedConditions()
        {
            return this.destinationReached;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.destinationReached = false;
            this.AIPositionsManager = PuzzleGameSingletonInstances.AIPositionsManager;
            this.AIPatrolActionInput = (AIPatrolActionInput)ContextActionInput;
        }

        public override void Tick(float d)
        {
            this.AIPatrolActionInput.AIPatrolGraphEventListener.DestinationSettedFromPatrolGraph(this.AIPositionsManager.GetPosition(this.Position).transform);
        }

        public void OnDestinationReached()
        {
            this.Interupt();
        }

        public override void Interupt()
        {
            this.destinationReached = true;
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.Position = (AIPositionMarkerID)NodeEditorGUILayout.EnumField("Position : ", string.Empty, this.Position);
        }
#endif
    }

}
