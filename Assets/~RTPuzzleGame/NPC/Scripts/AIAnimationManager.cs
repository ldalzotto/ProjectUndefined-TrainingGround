using CoreGame;
using GameConfigurationID;
using System.Collections;
using UnityEngine;

namespace RTPuzzle
{
    public class AIAnimationManager : MonoBehaviour
    {

        private PlayerAnimationDataManager NPCPAnimationDataManager;
        private INPCAIAnimationWorkflow CurrentNPCAIAnimationWorkflow;
        private Animator animator;
        private AnimationConfiguration animationConfiguration;

        public AIAnimationManager(Animator animator, AnimationConfiguration animationConfiguration)
        {
            this.animator = animator;
            this.animationConfiguration = animationConfiguration;

            this.NPCPAnimationDataManager = new PlayerAnimationDataManager(animator);

            //Initialize movement animation
            GenericAnimatorHelper.SetMovementLayer(animator, animationConfiguration, LevelType.PUZZLE);
        }

        public void TickAlways(float normalizedCurrentSpeed)
        {
            this.NPCPAnimationDataManager.Tick(normalizedCurrentSpeed);
        }

        #region External Events
        public void OnDisarmObjectStart(DisarmObjectModule disarmObjectModule)
        {
            this.CurrentNPCAIAnimationWorkflow = new DisarmObjectAnimationWorkflow(this.animator, this.animationConfiguration, disarmObjectModule);
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
        private DisarmObjectModule disarmObjectModule;

        public DisarmObjectAnimationWorkflow(Animator animator, AnimationConfiguration animationConfiguration, DisarmObjectModule disarmObjectModule) : base(animator, animationConfiguration)
        {
            this.disarmObjectModule = disarmObjectModule;
        }

        public override void OnStart()
        {
            this.animationCoroutine = Coroutiner.Instance.StartCoroutine(this.DisarmObjectAnimationWorkflowPlayAnimations());
        }

        private IEnumerator DisarmObjectAnimationWorkflowPlayAnimations()
        {
            yield return AnimationPlayerHelper.PlayAndWait(this.animator, this.animationConfiguration.ConfigurationInherentData[this.disarmObjectModule.DisarmObjectInherentConfigurationData.DisarmObjectAnimationLooped], 0.25f, animationEndCallback: null, framePerfectEndDetection: true);
            AnimationPlayerHelper.Play(this.animator, this.animationConfiguration.ConfigurationInherentData[AnimationID.ACTION_LISTENING], 0.25f);
            yield return new WaitForSeconds(1.5f);
            yield return this.DisarmObjectAnimationWorkflowPlayAnimations();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.animationCoroutine != null) { Coroutiner.Instance.StopCoroutine(this.animationCoroutine); }
        }


    }
}
