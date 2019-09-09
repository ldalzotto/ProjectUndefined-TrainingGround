using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneAnimationAction : AbstractCutsceneAnimationAction
    {
        [CustomEnum()]
        public PointOfInterestId PointOfInterestId;

        public CutsceneAnimationAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        protected override AbstractCutsceneController GetAbstractCutsceneController(SequencedActionInput ContextActionInput)
        {
            return ((CutsceneActionInput)ContextActionInput).PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId).GetPointOfInterestCutsceneController();
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
            this.AnimationIdV2.ActionGUI("Animation V2 : ");
            this.SkipToNextNode = (bool)NodeEditorGUILayout.BoolField("Skip immediately : ", string.Empty, this.SkipToNextNode);
            this.CrossFade = NodeEditorGUILayout.FloatField("Crossfade : ", string.Empty, this.CrossFade);
            this.PlayImmediately = (bool)NodeEditorGUILayout.BoolField("Update model positions on start : ", string.Empty, this.PlayImmediately);

            EditorGUI.BeginDisabledGroup(this.SkipToNextNode);
            this.InfiniteLoop = (bool)NodeEditorGUILayout.BoolField("Infinite loop : ", string.Empty, this.InfiniteLoop);
            this.FramePerfectEndDetection = (bool)NodeEditorGUILayout.BoolField("Frame perfect end detection : ", string.Empty, this.FramePerfectEndDetection);
            EditorGUI.EndDisabledGroup();
        }
#endif
    }

}
