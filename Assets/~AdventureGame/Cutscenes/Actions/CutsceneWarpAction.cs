using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneWarpAction : SequencedAction
    {
        [CustomEnum()]
        public PointOfInterestId PointOfInterestId;
        [CustomEnum()]
        public CutscenePositionMarkerID Destination;

        public CutsceneWarpAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;
            var cutsceneController = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId).GetPointOfInterestCutsceneController();
            if (cutsceneController != null && cutsceneActionInput.CutscenePositionsManager != null)
            {
                cutsceneController.Warp(cutsceneActionInput.CutscenePositionsManager.GetCutscenePosition(cutsceneActionInput.CutsceneId, this.Destination).transform);
            }
        }

        public override void Tick(float d)
        {
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
            this.Destination = (CutscenePositionMarkerID)NodeEditorGUILayout.EnumField("Destination : ", string.Empty, this.Destination);
        }
#endif
    }

}
