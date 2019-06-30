using CoreGame;
using GameConfigurationID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneMoveAction : SequencedAction
    {
        [CustomEnum()]
        [SerializeField]
        public PointOfInterestId PointOfInterestId;
        [SerializeField]
        public CutscenePositionMarkerID Destination;

        [Range(0f, 1f)]
        [SerializeField]
        public float NormalizedSpeedMagnitude = 1f;

        [NonSerialized]
        private bool destinationReached = false;

        public CutsceneMoveAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.destinationReached;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.destinationReached = false;

            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;
            var cutsceneController = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId).GetPointOfInterestCutsceneController();
            Coroutiner.Instance.StartCoroutine(this.SetDestination(cutsceneController, cutsceneActionInput.CutscenePositionsManager, cutsceneActionInput.CutsceneId));
        }

        private IEnumerator SetDestination(PointOfInterestCutsceneController pointOfInterestCutsceneController,
                        CutscenePositionsManager cutscenePositionsManager, CutsceneId cutsceneId)
        {
            this.destinationReached = false;
            yield return pointOfInterestCutsceneController.SetAIDestination(cutscenePositionsManager.GetCutscenePosition(cutsceneId, this.Destination).transform, this.NormalizedSpeedMagnitude);
            this.destinationReached = true;
        }

        public override void Tick(float d)
        {

        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
            this.Destination = (CutscenePositionMarkerID)NodeEditorGUILayout.EnumField("Destination : ", string.Empty, this.Destination);
            this.NormalizedSpeedMagnitude = NodeEditorGUILayout.FloatField("Normalized Speed : ", string.Empty, this.NormalizedSpeedMagnitude);
        }
#endif
    }

}
