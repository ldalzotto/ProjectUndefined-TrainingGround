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
        public float CrossFade = 0f;
        [SerializeField]
        public bool InfiniteLoop = false;
        [SerializeField]
        public bool PlayImmediately = false;

        [NonSerialized]
        private bool animationEnded = false;
        [NonSerialized]
        private bool atLeatsOneFrame = false;

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
            if (this.animationEnded)
            {
                Debug.Log(MyLog.Format("CutsceneAnimationAction ComputeFinishedConditions"));
            }
            return this.atLeatsOneFrame && this.animationEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            Debug.Log(MyLog.Format("CutsceneAnimationAction FirstExecutionAction"));
            this.animationEnded = false;
            this.atLeatsOneFrame = false;
            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;
            this.actionInput = cutsceneActionInput;
            this.pointOfInterestCutsceneController = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId).GetPointOfInterestCutsceneController();

            Coroutiner.Instance.StartCoroutine(this.PlayAnimation(this.pointOfInterestCutsceneController));
            Coroutiner.Instance.StartCoroutine(this.WaitForNextFixedFrame());
        }

        private IEnumerator WaitForNextFixedFrame()
        {
            yield return new WaitForFixedUpdate();
            this.atLeatsOneFrame = true;
        }

        private IEnumerator PlayAnimation(PointOfInterestCutsceneController cutsceneController)
        {
            this.animationCoroutine = Coroutiner.Instance.StartCoroutine(cutsceneController.PlayAnimationAndWait(this.AnimationId, this.CrossFade, animationEndCallback: null, this.PlayImmediately));
            yield return this.animationCoroutine;
        }

        public override void Tick(float d)
        {
            Debug.Log(MyLog.Format("CutsceneAnimationAction Tick"));
            if (this.atLeatsOneFrame)
            {
                this.animationEnded = !this.pointOfInterestCutsceneController.IsSpecifiedAnimationPlaying(this.AnimationId);
                if (this.animationEnded)
                {
                    Debug.Log(MyLog.Format("CutsceneAnimationAction animationEnded"));
                }
            }
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
            this.CrossFade = NodeEditorGUILayout.FloatField("Crossfade : ", string.Empty, this.CrossFade);
            this.InfiniteLoop = NodeEditorGUILayout.BoolField("Infinite loop : ", string.Empty, this.InfiniteLoop);
            this.PlayImmediately = NodeEditorGUILayout.BoolField("Update model positions on start : ", string.Empty, this.PlayImmediately);
        }
#endif
    }

}
