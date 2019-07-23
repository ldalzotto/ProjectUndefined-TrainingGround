using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneSpawnPOIAction : SequencedAction
    {
        [CustomEnum()]
        public PointOfInterestId PointOfInterestId;
        [CustomEnum()]
        public CutscenePositionMarkerID Position;

        public CutsceneSpawnPOIAction(List<SequencedAction> nextActions) : base(nextActions)
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

            if (cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId) == null)
            {
                PointOfInterestType.InstanciateNow(this.PointOfInterestId);
            }

            var cutsceneController = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId).GetPointOfInterestCutsceneController();
            if (cutsceneController != null && cutsceneActionInput.CutscenePositionsManager != null)
            {
                cutsceneController.Warp(cutsceneActionInput.CutscenePositionsManager.GetCutscenePosition(cutsceneActionInput.CutsceneId, this.Position).transform);
            }
        }

        public override void Tick(float d)
        {
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
            this.Position = (CutscenePositionMarkerID)NodeEditorGUILayout.EnumField("Position : ", string.Empty, this.Position);
        }
#endif
    }

}
