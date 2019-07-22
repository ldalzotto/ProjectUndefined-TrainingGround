using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

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

        [SerializeField]
        public bool InfiniteLoop = false;
        [SerializeField]
        public bool PlayImmediately = false;

        [NonSerialized]
        private bool animationEnded = false;

        [NonSerialized]
        private Coroutine animationCoroutine;
        [NonSerialized]
        private CutsceneActionInput actionInput;
        [NonSerialized]
        private PointOfInterestCutsceneController pointOfInterestCutsceneController;

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
            this.actionInput = cutsceneActionInput;
            this.pointOfInterestCutsceneController = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId).GetPointOfInterestCutsceneController();
            Coroutiner.Instance.StartCoroutine(this.PlayAnimation(this.pointOfInterestCutsceneController));
        }

        private IEnumerator PlayAnimation(PointOfInterestCutsceneController cutsceneController)
        {
            this.animationCoroutine = Coroutiner.Instance.StartCoroutine(cutsceneController.PlayAnimationAndWait(this.AnimationId, 0f, animationEndCallback: () =>
            {
                if (this.InfiniteLoop)
                {
                    return this.PlayAnimation(cutsceneController);
                }
                else
                {
                    this.animationEnded = true;
                    return null;
                }
            }, this.PlayImmediately));
            yield return this.animationCoroutine;
        }

        public override void Tick(float d)
        {

        }

        public override void Interupt()
        {
            base.Interupt();
            if (this.animationCoroutine != null)
            {
                Coroutiner.Instance.StopCoroutine(this.animationCoroutine);
                this.animationCoroutine = null;
                this.pointOfInterestCutsceneController.StopAnimation(this.AnimationId);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
            this.AnimationId = (AnimationID)NodeEditorGUILayout.EnumField("Animation : ", string.Empty, this.AnimationId);
            this.InfiniteLoop = (bool)NodeEditorGUILayout.BoolField("Infinite loop : ", string.Empty, this.InfiniteLoop);
            this.PlayImmediately = (bool)NodeEditorGUILayout.BoolField("Update model positions on start : ", string.Empty, this.PlayImmediately);
        }
#endif
    }

}
