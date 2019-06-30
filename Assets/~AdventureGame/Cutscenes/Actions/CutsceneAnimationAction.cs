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
    public class CutsceneAnimationAction : SequencedAction
    {
        [CustomEnum()]
        [SerializeField]
        public PointOfInterestId PointOfInterestId;

        [CustomEnum()]
        [SerializeField]
        public AnimationID AnimationId;

        [NonSerialized]
        private bool animationEnded = false;

        public CutsceneAnimationAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.animationEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.animationEnded = false;
            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;
            var cutsceneController = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId).GetPointOfInterestCutsceneController();

            Coroutiner.Instance.StartCoroutine(cutsceneController.PlayAnimationAndWait(this.AnimationId, 0f, animationEndCallback: () =>
            {
                this.animationEnded = true;
                return null;
            }));
        }

        public override void Tick(float d)
        {

        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
            this.AnimationId = (AnimationID)NodeEditorGUILayout.EnumField("Animation : ", string.Empty, this.AnimationId);
        }
#endif
    }

}
