using UnityEngine;

namespace CoreGame
{
    #region Player Movement
    public class PlayerInputMoveManager
    {

        private PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;

        private Transform CameraPivotPoint;
        private GameInputManager GameInputManager;
        private Rigidbody PlayerRigidBody;

        private bool hasMoved;
        private float playerSpeedMagnitude;

        public PlayerInputMoveManager(PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent, Transform cameraPivotPoint, GameInputManager gameInputManager, Rigidbody playerRigidBody)
        {
            this.PlayerInputMoveManagerComponent = PlayerInputMoveManagerComponent;
            CameraPivotPoint = cameraPivotPoint;
            GameInputManager = gameInputManager;
            PlayerRigidBody = playerRigidBody;
        }

        private PlayerSpeedProcessingInput playerSpeedProcessingInput;

        public PlayerSpeedProcessingInput PlayerSpeedProcessingInput { get => playerSpeedProcessingInput; }
        public bool HasMoved { get => hasMoved; }
        public float PlayerSpeedMagnitude { get => playerSpeedMagnitude; }

        public void Tick(float d)
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            var playerMovementOrientation = projectedDisplacement.normalized;

            #region Calculate magnitude attenuation
            float magnitudeAttenuationDiagonal = 1f;

            playerSpeedMagnitude = inputDisplacementVector.sqrMagnitude / magnitudeAttenuationDiagonal;
            #endregion
            if (playerSpeedMagnitude > float.Epsilon)
            {
                hasMoved = true;
            }
            playerSpeedProcessingInput = new PlayerSpeedProcessingInput(playerMovementOrientation, playerSpeedMagnitude);
        }

        public void FixedTick(float d)
        {
            if (playerSpeedProcessingInput != null)
            {
                //move rigid body rotation
                if (playerSpeedProcessingInput.PlayerMovementOrientation.sqrMagnitude > .05)
                {
                    PlayerRigidBody.MoveRotation(Quaternion.LookRotation(playerSpeedProcessingInput.PlayerMovementOrientation));
                }

                //move rigid body
                PlayerRigidBody.velocity = playerSpeedProcessingInput.PlayerMovementOrientation * playerSpeedProcessingInput.PlayerSpeedMagnitude * PlayerInputMoveManagerComponent.SpeedMultiplicationFactor;
                if (playerSpeedProcessingInput.PlayerSpeedMagnitude <= float.Epsilon)
                {
                    hasMoved = false;
                }
            }

        }

        public void ResetSpeed()
        {
            playerSpeedProcessingInput.PlayerSpeedMagnitude = 0;
            playerSpeedProcessingInput.PlayerMovementOrientation = Vector3.zero;
        }
    }

    [System.Serializable]
    public class PlayerInputMoveManagerComponent
    {
        public float SpeedMultiplicationFactor;
    }

    public class PlayerSpeedProcessingInput
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

