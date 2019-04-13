using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class PlayerHoodAnimationManagerComponent
    {
        public float MaxElongationDistance;
        public Vector3 MaxLocalAngleDelta;
        public AnimationCurve ElongationPercentageByNormalizedSpeed;

        public float MinNormalizedVelocityToTriggerFlicker;
        public float MaxVerticalFlickerDistance;
        public Vector3 FlickerDeltaAngle;
        public float FlickerSpeed;
        public AnimationCurve FlickerAnimationCurve;
    }

    public class PlayerHoodAnimationManager
    {
        PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;
        private PlayerHoodAnimationManagerComponent PlayerHoodAnimationManagerComponent;
        private Transform HoodTransform;
        private Rigidbody PlayerRigidBody;

        private Vector3 initialLocalRotation;

        public PlayerHoodAnimationManager(PlayerHoodAnimationManagerComponent playerHoodAnimationManagerComponent, Transform hoodTransform, Rigidbody rigidbody, PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent)
        {
            this.PlayerInputMoveManagerComponent = PlayerInputMoveManagerComponent;
            PlayerHoodAnimationManagerComponent = playerHoodAnimationManagerComponent;
            HoodTransform = hoodTransform;
            this.PlayerRigidBody = rigidbody;
            this.initialLocalRotation = this.HoodTransform.transform.localEulerAngles;
        }

        private bool isMoving;
        private bool isFlicker;

        public void LateTick(float d)
        {
            var speedRatio = (this.PlayerRigidBody.velocity.magnitude / PlayerInputMoveManagerComponent.SpeedMultiplicationFactor);
            if (speedRatio >= PlayerHoodAnimationManagerComponent.MinNormalizedVelocityToTriggerFlicker)
            {
                this.isFlicker = true;
            }
            else
            {
                this.isFlicker = false;
            }

            if (speedRatio > 0)
            {
                this.isMoving = true;
            }
            else
            {
                this.isMoving = false;
            }

            if (this.isMoving)
            {
                var deltapos = (-this.PlayerRigidBody.transform.forward)
                   * PlayerHoodAnimationManagerComponent.MaxElongationDistance
                   * PlayerHoodAnimationManagerComponent.ElongationPercentageByNormalizedSpeed.Evaluate(this.PlayerRigidBody.velocity.magnitude);

                var deltaAngle = (this.PlayerHoodAnimationManagerComponent.MaxLocalAngleDelta
                    * PlayerHoodAnimationManagerComponent.ElongationPercentageByNormalizedSpeed.Evaluate(this.PlayerRigidBody.velocity.magnitude));

                if (this.isFlicker)
                {
                    var flickerCurveEvaluation = this.PlayerHoodAnimationManagerComponent.FlickerAnimationCurve.Evaluate(Time.time * PlayerHoodAnimationManagerComponent.FlickerSpeed);
                    deltapos.y += flickerCurveEvaluation * (PlayerHoodAnimationManagerComponent.MaxVerticalFlickerDistance);
                    deltaAngle += flickerCurveEvaluation * (PlayerHoodAnimationManagerComponent.FlickerDeltaAngle);
                }

                this.HoodTransform.position += deltapos;
                this.HoodTransform.localEulerAngles = this.initialLocalRotation + deltaAngle;

            }
            else
            {
                this.HoodTransform.localEulerAngles = this.initialLocalRotation;
            }


        }
    }

}

