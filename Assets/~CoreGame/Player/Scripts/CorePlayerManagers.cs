using UnityEngine;

namespace CoreGame
{
    #region Player Movement
    class PlayerInputMoveManager
    {

        private Transform CameraPivotPoint;
        private GameInputManager GameInputManager;
        private Rigidbody PlayerRigidBody;

        public PlayerInputMoveManager(Transform cameraPivotPoint, GameInputManager gameInputManager, Rigidbody playerRigidBody)
        {
            CameraPivotPoint = cameraPivotPoint;
            GameInputManager = gameInputManager;
            PlayerRigidBody = playerRigidBody;
        }

        private PlayerSpeedProcessingInput playerSpeedProcessingInput;

        public PlayerSpeedProcessingInput PlayerSpeedProcessingInput { get => playerSpeedProcessingInput; }

        public void Tick(float d, float SpeedMultiplicationFactor)
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            var playerMovementOrientation = projectedDisplacement.normalized;

            #region Calculate magnitude attenuation
            float magnitudeAttenuationDiagonal = 1f;

            var playerSpeedMagnitude = inputDisplacementVector.sqrMagnitude / magnitudeAttenuationDiagonal;
            #endregion
            playerSpeedProcessingInput = new PlayerSpeedProcessingInput(playerMovementOrientation, playerSpeedMagnitude);
        }

        public void FixedTick(float d, float SpeedMultiplicationFactor)
        {
            if (playerSpeedProcessingInput != null)
            {
                //move rigid body rotation
                if (playerSpeedProcessingInput.PlayerMovementOrientation.sqrMagnitude > .05)
                {
                    PlayerRigidBody.MoveRotation(Quaternion.LookRotation(playerSpeedProcessingInput.PlayerMovementOrientation));
                }

                //move rigid body
                PlayerRigidBody.velocity = playerSpeedProcessingInput.PlayerMovementOrientation * playerSpeedProcessingInput.PlayerSpeedMagnitude * SpeedMultiplicationFactor;
            }

        }

        public void ResetSpeed()
        {
            playerSpeedProcessingInput.PlayerSpeedMagnitude = 0;
            playerSpeedProcessingInput.PlayerMovementOrientation = Vector3.zero;
        }
    }

    class PlayerSpeedProcessingInput
    {
        private Vector3 playerMovementOrientation;
        private float playerSpeedMagnitude;

        public PlayerSpeedProcessingInput(Vector3 playerMovementOrientation, float playerSpeedMagnitude)
        {
            this.playerMovementOrientation = playerMovementOrientation;
            this.playerSpeedMagnitude = playerSpeedMagnitude;
        }

        public Vector3 PlayerMovementOrientation { get => playerMovementOrientation; set => playerMovementOrientation = value; }
        public float PlayerSpeedMagnitude { get => playerSpeedMagnitude; set => playerSpeedMagnitude = value; }
    }

    #endregion
}

