using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneDestroyPOIAction : SequencedAction
    {
        [CustomEnum()]
        public PointOfInterestId PointOfInterestId;

        public CutsceneDestroyPOIAction(List<SequencedAction> nextActions) : base(nextActions)
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

            var selectedPOI = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId);
            if (selectedPOI != null)
            {
                selectedPOI.DestroyWithoutSave();
            }
            
        }

        public override void Tick(float d)
        {
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
        }
#endif
    }

}
