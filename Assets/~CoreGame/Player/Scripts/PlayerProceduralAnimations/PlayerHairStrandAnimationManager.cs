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
        private SkinnedMeshRenderer hairMeshRenderer;

        public PlayerHairStrandAnimationManager(Rigidbody playerRigidBody, PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent)
        {
            this.hairMeshRenderer = playerRigidBody.gameObject.FindChildObjectRecursively(AnimationConstants.PlayerAnimationConstantsData.HAIR_OBJECT_NAME).GetComponent<SkinnedMeshRenderer>();
            this.PlayerHairStrandAnimationManagerComponent = PlayerHairStrandAnimationManagerComponent;
            this.lastFramePosition = hairMeshRenderer.transform.position;
        }

        private Vector3 lastFramePosition;

        private float currentAnimationCurveStaticElapsedTime;
        private float currentAnimationCurveRunningElapsedTime;

        private bool isRunning;
        private bool isStatic;
        private bool endOfAnimationCurveReached;

        public void LateTick(float d)
        {
            var magni = Vector3.Distance(this.lastFramePosition, hairMeshRenderer.transform.position);
            if (magni <= this.PlayerHairStrandAnimationManagerComponent.MinDistanceMoving)
            {
                if (!this.isStatic)
                {
                    this.currentAnimationCurveStaticElapsedTime = 0f;
                    this.endOfAnimationCurveReached = false;
                }
                this.isStatic = true;
                this.isRunning = false;

                if (!this.endOfAnimationCurveReached)
                {

                    this.currentAnimationCurveStaticElapsedTime += d;
                    var evaluationTime = this.currentAnimationCurveStaticElapsedTime * this.PlayerHairStrandAnimationManagerComponent.HairStrandStaticAnimationSpeed;
                    var animationBlendRunningSample =
                        this.PlayerHairStrandAnimationManagerComponent.HairStrandRunningCurveDamp.Evaluate(evaluationTime) * 100;
                    if (animationBlendRunningSample >= 0)
                    {
                        this.hairMeshRenderer.SetBlendShapeWeight(0, animationBlendRunningSample);
                    }
                    else
                    {
                        this.hairMeshRenderer.SetBlendShapeWeight(1, animationBlendRunningSample * -1);
                    }
                    if (evaluationTime > 1)
                    {
                        this.endOfAnimationCurveReached = true;
                    }
                }


            }
            else
            {
                if (!this.isRunning)
                {
                    this.currentAnimationCurveRunningElapsedTime = 0f;
                    this.hairMeshRenderer.SetBlendShapeWeight(0, 0);
                    this.endOfAnimationCurveReached = false;
                }
                this.isStatic = false;
                this.isRunning = true;

                if (!this.endOfAnimationCurveReached)
                {
                    var evaluationTime = this.currentAnimationCurveRunningElapsedTime;

                    this.currentAnimationCurveRunningElapsedTime += d;
                    this.hairMeshRenderer.SetBlendShapeWeight(1, this.PlayerHairStrandAnimationManagerComponent.HairStrandRunningCurveDamp.Evaluate(evaluationTime) * 100);


                    if (evaluationTime > 1)
                    {
                        this.endOfAnimationCurveReached = true;
                    }
                }

            }

            this.lastFramePosition = hairMeshRenderer.transform.position;

        }

    }

}
