using System;
using UnityEngine;

namespace CoreGame
{
    #region Player Movement
    public abstract class PlayerMoveManager
    {
        private PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;
        protected Rigidbody PlayerRigidBody;
        protected PlayerSpeedProcessingInput playerSpeedProcessingInput;
        private bool hasMoved;
        private float playerSpeedMagnitude;

        public PlayerMoveManager(PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent, Rigidbody playerRigidBody)
        {
            this.PlayerInputMoveManagerComponent = PlayerInputMoveManagerComponent;
            PlayerRigidBody = playerRigidBody;
        }

        public PlayerSpeedProcessingInput PlayerSpeedProcessingInput { get => playerSpeedProcessingInput; }
        public bool HasMoved { get => hasMoved; }
        public float PlayerSpeedMagnitude { get => playerSpeedMagnitude; }

        protected abstract PlayerSpeedProcessingInput ComputePlayerSpeedProcessingInput();

        public void Tick(float d)
        {
            this.playerSpeedProcessingInput = this.ComputePlayerSpeedProcessingInput();
            this.playerSpeedMagnitude = playerSpeedProcessingInput.PlayerSpeedMagnitude;
            if (playerSpeedMagnitude > float.Epsilon)
            {
                hasMoved = true;
            }
        }

        public void FixedTick(float d)
        {
            if (playerSpeedProcessingInput != null)
            {
                //move rigid body rotation
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
        
    }

    public class PlayerInputMoveManager : PlayerMoveManager
    {
        private Transform CameraPivotPoint;
        private IGameInputManager GameInputManager;

        public PlayerInputMoveManager(PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent, Transform cameraPivotPoint, IGameInputManager gameInputManager, Rigidbody playerRigidBody)
             : base(PlayerInputMoveManagerComponent, playerRigidBody)
        {
            CameraPivotPoint = cameraPivotPoint;
            GameInputManager = gameInputManager;
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

