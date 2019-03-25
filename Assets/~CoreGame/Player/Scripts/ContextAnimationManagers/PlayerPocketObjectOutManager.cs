using System;
using UnityEngine;

namespace CoreGame
{
    public class PlayerAnimationWithObjectManager
    {
        private GameObject involvedObjectInstanciated;
        private Animator playerAnimator;
        private GameObject righthandContext;
        private PlayerAnimatioNamesEnum playerAnimationContextName;
        private float crossFadeTime;
        Action onAnimationEndAction;
        private bool destroyObjectOnEnd;
        private bool animationEnded;
        private bool animationPlaying;
        private Coroutine endCoroutine;

        public PlayerAnimationWithObjectManager(GameObject involvedObjectInstanciated,
            Animator playerAnimator,
            PlayerAnimatioNamesEnum playerAnimationContextName, float crossFadeTime,
            bool destroyObjectOnEnd,
            Action onAnimationEndAction)
        {
            this.involvedObjectInstanciated = involvedObjectInstanciated;
            this.playerAnimator = playerAnimator;
            this.destroyObjectOnEnd = destroyObjectOnEnd;
            this.righthandContext = PlayerBoneRetriever.GetPlayerBone(PlayerBone.RIGHT_HAND_CONTEXT, this.playerAnimator);
            this.playerAnimationContextName = playerAnimationContextName;
            this.crossFadeTime = crossFadeTime;
            this.onAnimationEndAction = onAnimationEndAction;
        }

        public void Play()
        {
            this.endCoroutine = Coroutiner.Instance.StartCoroutine(AnimationPlayerHelper.Play(this.playerAnimator, this.playerAnimationContextName, this.crossFadeTime, OnAnimationEnd));
            this.animationPlaying = true;
        }

        public void Tick(float d)
        {
            if (!animationEnded && animationPlaying)
            {
                this.involvedObjectInstanciated.transform.position = righthandContext.transform.position;
                this.involvedObjectInstanciated.transform.rotation = righthandContext.transform.rotation;
            }
        }

        #region Internal Events
        public void OnAnimationEnd()
        {
            this.animationEnded = true;
            if (this.endCoroutine != null)
            {
                Coroutiner.Instance.StopCoroutine(this.endCoroutine);
            }
            if (this.destroyObjectOnEnd)
            {
                MonoBehaviour.Destroy(this.involvedObjectInstanciated);
            }
            if (this.onAnimationEndAction != null)
            {
                this.onAnimationEndAction.Invoke();
            }
        }

        public void Kill()
        {
            this.playerAnimator.Play(AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNamesEnum.PLAYER_ACTION_LISTENING].AnimationName);
            this.OnAnimationEnd();
        }
        #endregion
    }
}
