using CoreGame;
using GameConfigurationID;
using System.Collections;
using UnityEngine;

namespace RTPuzzle
{
    public class NPCAIAnimationManager : MonoBehaviour
    {

        private PlayerAnimationDataManager NPCPAnimationDataManager;
        private INPCAIAnimationWorkflow CurrentNPCAIAnimationWorkflow;
        private Animator animator;
        private AnimationConfiguration animationConfiguration;

        public NPCAIAnimationManager(Animator animator, AnimationConfiguration animationConfiguration)
        {
            this.animator = animator;
            this.animationConfiguration = animationConfiguration;

            this.NPCPAnimationDataManager = new PlayerAnimationDataManager(animator);

            //Initialize movement animation
            GenericAnimatorHelper.SetMovementLayer(animator, animationConfiguration, LevelType.PUZZLE);
        }

        public void TickAlways(float d, float timeAttenuationFactor)
        {
            this.NPCPAnimationDataManager.Tick(timeAttenuationFactor);
        }

        #region External Events
        public void OnDisarmObjectStart()
        {
            this.CurrentNPCAIAnimationWorkflow = new DisarmObjectAnimationWorkflow(this.animator, this.animationConfiguration);
            this.CurrentNPCAIAnimationWorkflow.OnStart();
        }
        public void OnDisarmObjectEnd()
        {
            this.CurrentNPCAIAnimationWorkflow.OnExit();
        }
        #endregion
    }

    abstract class INPCAIAnimationWorkflow
    {
        protected Animator animator;
        protected AnimationConfiguration animationConfiguration;

        protected INPCAIAnimationWorkflow(Animator animator, AnimationConfiguration animationConfiguration)
        {
            this.animator = animator;
            this.animationConfiguration = animationConfiguration;
        }

        public abstract void OnStart();

        public virtual void OnExit()
        {
            GenericAnimatorHelper.ResetAllLayers(this.animator, this.animationConfiguration, LevelType.PUZZLE);
        }
    }

    class DisarmObjectAnimationWorkflow : INPCAIAnimationWorkflow
    {
        private Coroutine animationCoroutine;

        public DisarmObjectAnimationWorkflow(Animator animator, AnimationConfiguration animationConfiguration) : base(animator, animationConfiguration)
        {
        }

        public override void OnStart()
        {
            this.animationCoroutine = Coroutiner.Instance.StartCoroutine(this.DisarmObjectAnimationWorkflowPlayAnimations());
        }

        private IEnumerator DisarmObjectAnimationWorkflowPlayAnimations()
        {
            yield return AnimationPlayerHelper.PlayAndWait(this.animator, this.animationConfiguration.ConfigurationInherentData[AnimationID.GENERIC_SquatDown], 0f, animationEndCallback: null, framePerfectEndDetection: true);
            AnimationPlayerHelper.Play(this.animator, this.animationConfiguration.ConfigurationInherentData[AnimationID.GENERIC_SquatDown_Idle], 0f);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.animationCoroutine != null) { Coroutiner.Instance.StopCoroutine(this.animationCoroutine); }
        }


    }
}
