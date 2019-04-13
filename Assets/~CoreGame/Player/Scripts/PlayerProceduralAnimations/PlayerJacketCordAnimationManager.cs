using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class PlayerJacketCordAnimationManagerComponent
    {
        public float MinNormalizedSpeedToConsiderMove;

        public AnimationCurve DampingCurve;
        public float DampingSpeed;
    }

    public class PlayerJacketCordAnimationManager
    {
        private AnimationPositionTrackerManager chestPositionTrackerManager;
        private PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent;
        private PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;

        public PlayerJacketCordAnimationManager(Animator playerAnimator, AnimationPositionTrackerManager chestPositionTrackerManager, PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent,
            PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent)
        {
            this.PlayerJacketCordAnimationManagerComponent = PlayerJacketCordAnimationManagerComponent;
            this.chestPositionTrackerManager = chestPositionTrackerManager;
            this.PlayerInputMoveManagerComponent = PlayerInputMoveManagerComponent;

            this.standingState = new StandingState(playerAnimator, PlayerJacketCordAnimationManagerComponent);
            this.standingState.onJustEnabled += () =>
            {
                this.movingState.SetIsEnabled(false);
            };
            this.movingState = new MovingState(playerAnimator);

            this.movingState.onJustEnabled += () =>
            {
                this.standingState.SetIsEnabled(false);
            };
        }

        private AnimationProceduralState standingState;
        private AnimationProceduralState movingState;

        public void LateTick(float d)
        {
            if (chestPositionTrackerManager.AnimationPositionTrackerInformations.CurrentSpeedSigned.HasValue)
            {
                var normalizedSpeed = chestPositionTrackerManager.AnimationPositionTrackerInformations.CurrentSpeedSigned.Value.magnitude / this.PlayerInputMoveManagerComponent.SpeedMultiplicationFactor;
                this.standingState.SetIsEnabled(normalizedSpeed <= PlayerJacketCordAnimationManagerComponent.MinNormalizedSpeedToConsiderMove);
                this.movingState.SetIsEnabled(normalizedSpeed > PlayerJacketCordAnimationManagerComponent.MinNormalizedSpeedToConsiderMove);

                this.standingState.LateTick(d);
                this.movingState.LateTick(d);
            }

        }

        class MovingState : AnimationProceduralState
        {
            private Animator playerAnimator;

            public MovingState(Animator playerAnimator)
            {
                this.playerAnimator = playerAnimator;
            }

            protected override void LateTickImpl(float d)
            {
            }
        }

        class StandingState : AnimationProceduralState
        {
            private Animator playerAnimator;
            private PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent;

            public StandingState(Animator playerAnimator, PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent)
            {
                this.playerAnimator = playerAnimator;
                this.PlayerJacketCordAnimationManagerComponent = PlayerJacketCordAnimationManagerComponent;
                this.onJustEnabled += () =>
                {
                    this.dampingEnded = false;
                };
            }

            private bool dampingEnded;

            protected override void LateTickImpl(float d)
            {
                if (!this.dampingEnded)
                {
                    var jitterAnimationConstant = AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNamesEnum.PLAYER_JACKET_CORD_JITTER_TREE];
                    if (!this.playerAnimator.GetCurrentAnimatorStateInfo(jitterAnimationConstant.LayerIndex).IsName(jitterAnimationConstant.AnimationName))
                    {
                        this.playerAnimator.Play(jitterAnimationConstant.AnimationName);
                    }

                    var evaluationPoint = this.currentElapsedTimeFromActive * PlayerJacketCordAnimationManagerComponent.DampingSpeed;
                    if (evaluationPoint > 1)
                    {
                        this.dampingEnded = true;
                        this.playerAnimator.SetFloat(AnimationConstants.PlayerAnimatorParametersName.JacketCordJitter, 0);
                        this.playerAnimator.Play(AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNamesEnum.PLAYER_JACKET_CORD_LISTENING].AnimationName);
                    }
                    else
                    {
                        this.playerAnimator.SetFloat(AnimationConstants.PlayerAnimatorParametersName.JacketCordJitter, this.PlayerJacketCordAnimationManagerComponent.DampingCurve.Evaluate(evaluationPoint));
                    }

                }
            }
        }
    }
}
