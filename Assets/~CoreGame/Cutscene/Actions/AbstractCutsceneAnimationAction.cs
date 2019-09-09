using UnityEngine;
using System.Collections;
using GameConfigurationID;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace CoreGame
{
    public abstract class AbstractCutsceneAnimationAction : SequencedAction
    {
        public ParametrizedAnimationID AnimationIdV2;

        public bool SkipToNextNode = false;
        public bool InfiniteLoop = false;
        public bool PlayImmediately = false;
        public float CrossFade = 0f;
        public bool FramePerfectEndDetection = false;

        [NonSerialized]
        private AnimationID resolvedAnimationID;
        [NonSerialized]
        private bool animationEnded = false;
        [NonSerialized]
        private Coroutine animationCoroutine;
        [NonSerialized]
        private AbstractCutsceneController pointOfInterestCutsceneController;

        protected abstract AbstractCutsceneController GetAbstractCutsceneController(SequencedActionInput ContextActionInput);

        public AbstractCutsceneAnimationAction(List<SequencedAction> nextActions) : base(nextActions)
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
            this.pointOfInterestCutsceneController = this.GetAbstractCutsceneController(ContextActionInput);
            this.resolvedAnimationID = this.AnimationIdV2.Resolve(ContextActionInput.graphParameters);
            if (this.SkipToNextNode)
            {
                this.animationEnded = true;
                this.pointOfInterestCutsceneController.Play(this.resolvedAnimationID, this.CrossFade, this.PlayImmediately);
            }
            else
            {
                Coroutiner.Instance.StartCoroutine(this.PlayAnimation(this.pointOfInterestCutsceneController));
            }

        }

        private IEnumerator PlayAnimation(AbstractCutsceneController cutsceneController)
        {
            this.animationCoroutine = Coroutiner.Instance.StartCoroutine(cutsceneController.PlayAnimationAndWait(this.resolvedAnimationID, this.CrossFade, animationEndCallback: () =>
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
            }, this.PlayImmediately, this.FramePerfectEndDetection));
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
                this.pointOfInterestCutsceneController.StopAnimation(this.resolvedAnimationID);
            }
        }

    }

}
