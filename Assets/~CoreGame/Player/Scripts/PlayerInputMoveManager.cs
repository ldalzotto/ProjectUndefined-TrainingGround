using UnityEngine;

namespace CoreGame
{
    #region Player Movement
    public abstract class PlayerMoveManager
    {
        private TransformMoveManagerComponentV3 PlayerInputMoveManagerComponent;
        protected Rigidbody PlayerRigidBody;
        private bool hasMoved;
        protected float playerSpeedMagnitude;
        protected PlayerSpeedProcessingInput playerSpeedProcessingInput;

        protected abstract PlayerSpeedProcessingInput ComputePlayerSpeedProcessingInput();

        public PlayerMoveManager(TransformMoveManagerComponentV3 PlayerInputMoveManagerComponent, Rigidbody playerRigidBody)
        {
            this.PlayerInputMoveManagerComponent = PlayerInputMoveManagerComponent;
            PlayerRigidBody = playerRigidBody;
        }

        public PlayerSpeedProcessingInput PlayerSpeedProcessingInput { get => playerSpeedProcessingInput; }
        public bool HasMoved { get => hasMoved; }
        public float PlayerSpeedMagnitude { get => playerSpeedMagnitude; }

        public void Tick(float d)
        {
            this.playerSpeedMagnitude = playerSpeedProcessingInput.PlayerSpeedMagnitude;
            if (playerSpeedMagnitude > float.Epsilon)
            {
                hasMoved = true;
            }
        }

        public void FixedTick(float d)
        {
            //move rigid body rotation
            this.playerSpeedProcessingInput = ComputePlayerSpeedProcessingInput();
            if (playerSpeedProcessingInput.PlayerMovementOrientation.sqrMagnitude > .05)
            {
                PlayerRigidBody.rotation = Quaternion.LookRotation(playerSpeedProcessingInput.PlayerMovementOrientation);
                //rotation will take place at the end of physics step https://docs.unity3d.com/ScriptReference/Rigidbody-rotation.html
            }

            //move rigid body
            PlayerRigidBody.velocity = playerSpeedProcessingInput.PlayerMovementOrientation * playerSpeedProcessingInput.PlayerSpeedMagnitude * PlayerInputMoveManagerComponent.SpeedMultiplicationFactor;
            if (playerSpeedProcessingInput.PlayerSpeedMagnitude <= float.Epsilon)
            {
                hasMoved = false;
            }
        }
    }

    public class PlayerInputMoveManager : PlayerMoveManager
    {
        private Transform CameraPivotPoint;
        private IGameInputManager GameInputManager;

        public PlayerInputMoveManager(TransformMoveManagerComponentV3 PlayerInputMoveManagerComponent, Transform cameraPivotPoint, IGameInputManager gameInputManager, Rigidbody playerRigidBody)
             : base(PlayerInputMoveManagerComponent, playerRigidBody)
        {
            CameraPivotPoint = cameraPivotPoint;
            GameInputManager = gameInputManager;
            this.playerSpeedProcessingInput = ComputePlayerSpeedProcessingInput();
        }

        protected override PlayerSpeedProcessingInput ComputePlayerSpeedProcessingInput()
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            var playerMovementOrientation = projectedDisplacement.normalized;

            return new PlayerSpeedProcessingInput(playerMovementOrientation, inputDisplacementVector.sqrMagnitude);
        }

        public void ResetSpeed()
        {
            this.playerSpeedMagnitude = 0;
            base.playerSpeedProcessingInput.PlayerSpeedMagnitude = 0;
            base.playerSpeedProcessingInput.PlayerMovementOrientation = Vector3.zero;
        }
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

