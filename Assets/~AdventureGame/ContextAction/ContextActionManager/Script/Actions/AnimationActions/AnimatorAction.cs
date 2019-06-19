using CoreGame;
using UnityEngine;
#if UNITY_EDITOR
using NodeGraph_Editor;
#endif
namespace AdventureGame
{
    [System.Serializable]
    public class AnimatorAction : AContextAction
    {
        [SerializeField]
        private PlayerAnimatioNamesEnum playerAnimationEnum;
        [SerializeField]
        private bool animationEnded;

        public AnimatorAction(PlayerAnimatioNamesEnum playerAnimationEnum, AContextAction nextContextAction) : base(nextContextAction)
        {
            this.playerAnimationEnum = playerAnimationEnum;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return animationEnded;
        }

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {
            animationEnded = false;
            var animatorActionInput = (AnimatorActionInput)ContextActionInput;

            animatorActionInput.PlayerManager.StartCoroutine(AnimationPlayerHelper.Play(animatorActionInput.PlayerAnimator, playerAnimationEnum, 0f, () =>
            {
                animationEnded = true;
                return null;
            }));
        }

        public override void Tick(float d)
        {
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.playerAnimationEnum = (PlayerAnimatioNamesEnum)NodeEditorGUILayout.EnumField("Animation : ", string.Empty, this.playerAnimationEnum);
        }
#endif
    }

    public class AnimatorActionInput : AContextActionInput
    {
        private Animator playerAnimator;
        private PlayerManager playerManager;

        public AnimatorActionInput(Animator playerAnimator, PlayerManager playerManager)
        {
            this.playerAnimator = playerAnimator;
            this.playerManager = playerManager;
        }

        public Animator PlayerAnimator { get => playerAnimator; }
        public PlayerManager PlayerManager { get => playerManager; }
    }
}