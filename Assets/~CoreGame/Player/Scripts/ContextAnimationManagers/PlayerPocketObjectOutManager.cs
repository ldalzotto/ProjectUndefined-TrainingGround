using GameConfigurationID;
using System;
using UnityEngine;
using static PlayerAnimationConstants;

namespace CoreGame
{
    public class PlayerAnimationWithObjectManager
    {
        private GameObject involvedObjectInstanciated;
        AnimationConfiguration animationConfiguration;
        AnimationID animationID;
        private Animator playerAnimator;
        private GameObject rightHandContext;
        private float crossFadeTime;
        Action onAnimationEndAction;
        private bool destroyObjectOnEnd;
        private bool animationEnded;
        private Coroutine endCoroutine;

        public PlayerAnimationWithObjectManager(GameObject involvedObjectInstanciated, AnimationConfiguration animationConfiguration, AnimationID animationID,
            Animator playerAnimator,
            float crossFadeTime,
            bool destroyObjectOnEnd,
            Action onAnimationEndAction)
        {
            this.involvedObjectInstanciated = involvedObjectInstanciated;
            this.animationConfiguration = animationConfiguration;
            this.animationID = animationID;
            this.playerAnimator = playerAnimator;
            this.destroyObjectOnEnd = destroyObjectOnEnd;
            this.rightHandContext = PlayerBoneRetriever.GetPlayerBone(PlayerBone.RIGHT_HAND_CONTEXT, this.playerAnimator);
            this.crossFadeTime = crossFadeTime;
            this.onAnimationEndAction = onAnimationEndAction;
        }

        public void Play()
        {
            this.endCoroutine = Coroutiner.Instance.StartCoroutine(AnimationPlayerHelper.PlayAndWait(this.playerAnimator, this.animationConfiguration.ConfigurationInherentData[this.animationID], this.crossFadeTime, () =>
            {
                this.OnAnimationEnd();
                return null;
            }));
            this.Tick(0); //for position initialization
        }

        public void Tick(float d)
        {
            if (this.IsPlaying())
            {
                this.involvedObjectInstanciated.transform.position = rightHandContext.transform.position;
                this.involvedObjectInstanciated.transform.rotation = rightHandContext.transform.rotation;
            }
        }

        public bool IsPlaying()
        {
            return !this.animationEnded;
        }

        #region Internal Events
        private void OnAnimationEnd()
        {
            this.animationEnded = true;
            if (this.endCoroutine != null)
            {
                Coroutiner.Instance.StopCoroutine(this.endCoroutine);
            }
            if (this.destroyObjectOnEnd && this.involvedObjectInstanciated != null)
            {
                MonoBehaviour.Destroy(this.involvedObjectInstanciated);
            }
            if (this.onAnimationEndAction != null)
            {
                this.onAnimationEndAction.Invoke();
            }
        }
        #endregion

        #region External Events
        public void Kill()
        {
            this.SetContextActionAnimatorToListening();
            this.OnAnimationEnd();
        }
        public void KillSilently()
        {
            this.OnAnimationEnd();
        }
        #endregion

        private void SetContextActionAnimatorToListening()
        {
            this.playerAnimator.Play(this.animationConfiguration.ConfigurationInherentData[AnimationID.ACTION_LISTENING].AnimationName);
        }
    }
}
