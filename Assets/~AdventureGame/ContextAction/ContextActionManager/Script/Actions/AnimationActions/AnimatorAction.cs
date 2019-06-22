using CoreGame;
using UnityEngine;
using GameConfigurationID;
#if UNITY_EDITOR
using NodeGraph_Editor;
#endif
namespace AdventureGame
{
    [System.Serializable]
    public class AnimatorAction : AContextAction
    {
        [SerializeField]
        private AnimationID animationID;
        [SerializeField]
        private bool animationEnded;

        public AnimatorAction(AnimationID animationID, AContextAction nextContextAction) : base(nextContextAction)
        {
            this.animationID = animationID;
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

            animatorActionInput.PlayerManager.StartCoroutine(AnimationPlayerHelper.PlayAndWait(animatorActionInput.PlayerAnimator, animatorActionInput.AnimationConfiguration.ConfigurationInherentData[this.animationID], 0f, () =>
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
            this.animationID = (AnimationID)NodeEditorGUILayout.EnumField("Animation : ", string.Empty, this.animationID);
        }
#endif
    }

    public class AnimatorActionInput : AContextActionInput
    {
        private Animator playerAnimator;
        private PlayerManager playerManager;
        private AnimationConfiguration animationConfiguration;

        public AnimatorActionInput(Animator playerAnimator, PlayerManager playerManager, AnimationConfiguration animationConfiguration)
        {
            this.playerAnimator = playerAnimator;
            this.playerManager = playerManager;
            this.animationConfiguration = animationConfiguration;
        }

        public Animator PlayerAnimator { get => playerAnimator; }
        public PlayerManager PlayerManager { get => playerManager; }
        public AnimationConfiguration AnimationConfiguration { get => animationConfiguration; }
    }
}