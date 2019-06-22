using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class PlayerHairStrandAnimationManagerComponent
    {
        public AnimationCurve HairStrandRunningCurveDamp;
        public AnimationCurve HairStrandStaticCurveDamp;
        public float HairStrandStaticAnimationSpeed;
        public float MinDistanceMoving;
    }

    public class PlayerHairStrandAnimationManager
    {
        private PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent;
        private AnimationPositionTrackerManager playerHairPositionTracker;

        private StaticState staticState;
        private MovingState movingState;

        public PlayerHairStrandAnimationManager(AnimationPositionTrackerManager playerHairPositionTracker, GameObject hairObject, PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent)
        {
            var hairMeshRenderer = hairObject.GetComponent<SkinnedMeshRenderer>();
            this.playerHairPositionTracker = playerHairPositionTracker;
            this.PlayerHairStrandAnimationManagerComponent = PlayerHairStrandAnimationManagerComponent;

            this.staticState = new StaticState(PlayerHairStrandAnimationManagerComponent, hairMeshRenderer);
            this.staticState.onJustEnabled += () =>
            {
                this.movingState.SetIsEnabled(false);
            };

            this.movingState = new MovingState(PlayerHairStrandAnimationManagerComponent, hairMeshRenderer);
            this.movingState.onJustEnabled += () =>
            {
                this.staticState.SetIsEnabled(false);
            };
        }


        private float currentAnimationCurveRunningElapsedTime;

        private bool isRunning;
        private bool endOfAnimationCurveReached;


        public void LateTick(float d)
        {
            this.staticState.SetIsEnabled(playerHairPositionTracker.AnimationPositionTrackerInformations.CrossedDistanceSigned.HasValue
                && playerHairPositionTracker.AnimationPositionTrackerInformations.CrossedDistanceSigned.Value.magnitude <= this.PlayerHairStrandAnimationManagerComponent.MinDistanceMoving);

            this.staticState.LateTick(d);
            this.movingState.LateTick(d);
        }

        private class StaticState : AnimationProceduralState
        {
            private PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent;
            private SkinnedMeshRenderer hairMeshRenderer;

            public StaticState(PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent, SkinnedMeshRenderer hairMeshRenderer)
            {
                this.hairMeshRenderer = hairMeshRenderer;
                this.PlayerHairStrandAnimationManagerComponent = PlayerHairStrandAnimationManagerComponent;
                this.onJustEnabled += () =>
                {
                    this.endOfAnimationCurve = false;
                };
            }

            private bool endOfAnimationCurve;

            protected override void LateTickImpl(float d)
            {
                if (!this.endOfAnimationCurve)
                {
                    var evaluationTime = this.currentElapsedTimeFromActive * this.PlayerHairStrandAnimationManagerComponent.HairStrandStaticAnimationSpeed;
                    var animationBlendRunningSample =
                        this.PlayerHairStrandAnimationManagerComponent.HairStrandRunningCurveDamp.Evaluate(evaluationTime) * 100;
                    if (animationBlendRunningSample >= 0)
                    {
                        this.hairMeshRenderer.SetBlendShapeWeight(this.hairMeshRenderer.sharedMesh.GetBlendShapeIndex(PlayerAnimationConstants.PlayerHairStrandBlendShapeNames.FORWARD), animationBlendRunningSample);
                    }
                    else
                    {
                        this.hairMeshRenderer.SetBlendShapeWeight(this.hairMeshRenderer.sharedMesh.GetBlendShapeIndex(PlayerAnimationConstants.PlayerHairStrandBlendShapeNames.BACKWARD), animationBlendRunningSample * -1);
                    }
                    if (evaluationTime > 1)
                    {
                        this.endOfAnimationCurve = true;
                    }
                }
            }
        }

        private class MovingState : AnimationProceduralState
        {
            private PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent;
            private SkinnedMeshRenderer hairMeshRenderer;

            public MovingState(PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent, SkinnedMeshRenderer hairMeshRenderer)
            {
                this.hairMeshRenderer = hairMeshRenderer;
                this.PlayerHairStrandAnimationManagerComponent = PlayerHairStrandAnimationManagerComponent;

                this.onJustEnabled += () =>
                {
                    this.endOfAnimationCurve = false;
                    this.hairMeshRenderer.SetBlendShapeWeight(this.hairMeshRenderer.sharedMesh.GetBlendShapeIndex(PlayerAnimationConstants.PlayerHairStrandBlendShapeNames.FORWARD), 0);
                };
            }

            private bool endOfAnimationCurve;

            protected override void LateTickImpl(float d)
            {
                if (!this.endOfAnimationCurve)
                {
                    var evaluationTime = this.currentElapsedTimeFromActive;

                    this.currentElapsedTimeFromActive += d;
                    this.hairMeshRenderer.SetBlendShapeWeight(this.hairMeshRenderer.sharedMesh.GetBlendShapeIndex(PlayerAnimationConstants.PlayerHairStrandBlendShapeNames.BACKWARD), this.PlayerHairStrandAnimationManagerComponent.HairStrandRunningCurveDamp.Evaluate(evaluationTime) * 100);


                    if (evaluationTime > 1)
                    {
                        this.endOfAnimationCurve = true;
                    }
                }
            }
        }

    }


}
