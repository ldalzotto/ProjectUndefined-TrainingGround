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
        public PointOfInterestDefinitionID PointOfInterestDefinitionID;
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
            var pointOfInterestID = cutsceneActionInput.AdventureGameConfigurationManager.PointOfInterestDefinitionConfiguration()[this.PointOfInterestDefinitionID].PointOfInterestId;
            if (cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(pointOfInterestID) == null)
            {
                PointOfInterestType.InstanciateNow(this.PointOfInterestDefinitionID);
            }

            var cutsceneController = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(pointOfInterestID).GetPointOfInterestCutsceneController();
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
            this.PointOfInterestDefinitionID = (PointOfInterestDefinitionID)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestDefinitionID);
            this.Position = (CutscenePositionMarkerID)NodeEditorGUILayout.EnumField("Position : ", string.Empty, this.Position);
        }
#endif
    }

}
