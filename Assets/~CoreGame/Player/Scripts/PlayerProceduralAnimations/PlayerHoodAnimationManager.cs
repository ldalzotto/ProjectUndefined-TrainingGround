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
        public float FlickerSpeed;
        public AnimationCurve FlickerAnimationCurve;
    }

    public class PlayerHoodAnimationManager : MonoBehaviour
    {
        PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;
        private PlayerHoodAnimationManagerComponent PlayerHoodAnimationManagerComponent;
        private Transform HoodTransform;
        private Rigidbody PlayerRigidBody;

        public PlayerHoodAnimationManager(PlayerHoodAnimationManagerComponent playerHoodAnimationManagerComponent, Transform hoodTransform, Rigidbody rigidbody, PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent)
        {
            this.PlayerInputMoveManagerComponent = PlayerInputMoveManagerComponent;
            PlayerHoodAnimationManagerComponent = playerHoodAnimationManagerComponent;
            HoodTransform = hoodTransform;
            this.PlayerRigidBody = rigidbody;
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

                this.HoodTransform.localEulerAngles += this.PlayerHoodAnimationManagerComponent.MaxLocalAngleDelta
                    * PlayerHoodAnimationManagerComponent.ElongationPercentageByNormalizedSpeed.Evaluate(this.PlayerRigidBody.velocity.magnitude);

                if (this.isFlicker)
                {
                    deltapos.y += this.PlayerHoodAnimationManagerComponent.FlickerAnimationCurve.Evaluate(Time.time * PlayerHoodAnimationManagerComponent.FlickerSpeed) * (PlayerHoodAnimationManagerComponent.MaxVerticalFlickerDistance);
                }


                this.HoodTransform.position += deltapos;
            }


        }
    }

}

