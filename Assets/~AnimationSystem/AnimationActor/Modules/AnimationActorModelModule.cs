using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AnimationSystem
{
    public class AnimationActorModelModule : AbstractAnimationActorModule
    {

        private CoreConfigurationManager CoreConfigurationManager;

        #region Internal Dependencies
        private Animator associatedAnimator;
        #endregion
        
        public override void Init(AnimationActor AnimationActorRef)
        {
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            this.associatedAnimator = GetComponent<Animator>();
        }

        public void PlayAnimation(AnimationID animationID, float crossFadeDuration)
        {
            AnimationPlayerHelper.Play(this.associatedAnimator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration);
        }
        
    }
}
