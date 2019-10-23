using System;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;
using SequencedAction;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace CoreGame
{
    public class BaseCutsceneAnimationAction : ASequencedAction
    {
        public ParametrizedAnimationID AnimationIdV2;

        /// <summary>
        /// When set to true, the squencer will play the animation then immediately skip to the next node.
        /// </summary>
        public bool SkipToNextNode = false;

        public bool InfiniteLoop = false;

        /// <summary>
        /// When set to true, the animation engine will immediately sample and set animations position at first frame.
        /// </summary>
        public bool PlayImmediately = false;

        public float CrossFade = 0f;

        /// <summary>
        /// When set to true, the animation engine calculate if animation is ended based on current delta time and remaining animation clip time.
        /// </summary>
        public bool FramePerfectEndDetection = false;

        [NonSerialized] private AnimationID PlayedAnimationID;
        [NonSerialized] private bool animationEnded = false;
        [NonSerialized] private Coroutine animationCoroutine;
        [NonSerialized] private BaseCutsceneController pointOfInterestCutsceneController;

        protected virtual BaseCutsceneController GetAbstractCutsceneController()
        {
            return default;
        }

        public BaseCutsceneAnimationAction(BaseCutsceneAnimationActionInput BaseCutsceneAnimationActionInput,
            BaseCutsceneController cutsceneController, Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
            PlayedAnimationID = BaseCutsceneAnimationActionInput.playedAnimationID;
            SkipToNextNode = BaseCutsceneAnimationActionInput.skipToNextNode;
            InfiniteLoop = BaseCutsceneAnimationActionInput.infiniteLoop;
            PlayImmediately = BaseCutsceneAnimationActionInput.playImmediately;
            CrossFade = BaseCutsceneAnimationActionInput.crossFade;
            FramePerfectEndDetection = BaseCutsceneAnimationActionInput.framePerfectEndDetection;
            this.pointOfInterestCutsceneController = cutsceneController;
        }

        public BaseCutsceneAnimationAction(Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.animationEnded;
        }

        public override void FirstExecutionAction()
        {
            this.animationEnded = false;
            if (this.pointOfInterestCutsceneController == null)
            {
                this.pointOfInterestCutsceneController = this.GetAbstractCutsceneController();
                this.PlayedAnimationID = this.AnimationIdV2.Resolve(null);
            }

            if (this.SkipToNextNode)
            {
                this.animationEnded = true;
                this.pointOfInterestCutsceneController.Play(this.PlayedAnimationID, this.CrossFade, this.PlayImmediately);
            }
            else
            {
                Coroutiner.Instance.StartCoroutine(this.PlayAnimation(this.pointOfInterestCutsceneController));
            }
        }

        private IEnumerator PlayAnimation(BaseCutsceneController cutsceneController)
        {
            this.animationCoroutine = Coroutiner.Instance.StartCoroutine(cutsceneController.PlayAnimationAndWait(this.PlayedAnimationID, this.CrossFade, animationEndCallback: () =>
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
                this.pointOfInterestCutsceneController.StopAnimation(this.PlayedAnimationID);
            }
        }
    }

    [Serializable]
    public struct BaseCutsceneAnimationActionInput
    {
        [CustomEnum()] public AnimationID playedAnimationID;
        public bool skipToNextNode;
        public bool infiniteLoop;
        public bool playImmediately;
        public float crossFade;
        public bool framePerfectEndDetection;
    }
}