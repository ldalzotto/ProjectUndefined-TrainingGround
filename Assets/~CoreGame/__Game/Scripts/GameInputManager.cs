using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace CoreGame
{
    public interface IGameInputManager
    {
        XInput CurrentInput { get; }
    }

    public class GameInputManager : MonoBehaviour, IGameInputManager
    {

        private XInput currentInput;

        public XInput CurrentInput { get => currentInput; }

        public void Init()
        {
            currentInput = new GameInput(new GameInputV2( CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.InputConfiguration), CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreInputConfiguration);
            Cursor.lockState = CursorLockMode.Locked;
        }

        private class GameInput : XInput
        {
            private GameInputV2 gameInputV2;
            private CoreInputConfiguration CoreInputConfiguration;

            internal GameInputV2 GameInputV2 { get => gameInputV2; }

            public GameInput(GameInputV2 gameInputV2, CoreInputConfiguration CoreInputConfiguration)
            {
                this.gameInputV2 = gameInputV2;
                this.CoreInputConfiguration = CoreInputConfiguration;
            }

            public bool ActionButtonD()
            {
                return this.gameInputV2.InputConditionsMet(InputID.ACTION_DOWN);
            }

            public bool CancelButtonD()
            {
                return this.gameInputV2.InputConditionsMet(InputID.CANCEL_DOWN);
            }

            public bool CancelButtonDH()
            {
                return this.gameInputV2.InputConditionsMet(InputID.CANCEL_DOWN_HOLD);
            }

            public bool InventoryButtonD()
            {
                return this.gameInputV2.InputConditionsMet(InputID.INVENTORY_DOWN);
            }

            public Vector3 LocomotionAxis()
            {
                var rawDirection = new Vector3(-Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.LEFT_DOWN_HOLD))
                                               + Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.RIGHT_DOWN_HOLD)),
                                               0,
                                               -Convert.ToInt32(Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.DOWN_DOWN_HOLD)))
                                               + Convert.ToInt32(Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.UP_DOWN_HOLD))));
                if (Vector3.Distance(rawDirection, Vector3.zero) > 1)
                {
                    rawDirection = rawDirection.normalized;
                }
                return rawDirection;
            }

            public Vector3 CursorDisplacement()
            {
                if (!Application.isFocused)
                {
                    return Vector3.zero;
                }
                return new Vector3(Mouse.current.delta.x.ReadValue(), 0, Mouse.current.delta.y.ReadValue()) * Screen.width * this.CoreInputConfiguration.GetCursorMovementMouseSensitivity();
            }

            public float LeftRotationCameraDH()
            {
                if (this.gameInputV2.InputConditionsMet(InputID.CAMERA_ROTATION_DOWN_HOLD))
                {
                    return Mathf.Max(Mouse.current.delta.x.ReadValue(), 0) * Screen.width * this.CoreInputConfiguration.GetCameraMovementMouseSensitivity();
                }

                return 0f;
            }

            public float RightRotationCameraDH()
            {
                if (this.gameInputV2.InputConditionsMet(InputID.CAMERA_ROTATION_DOWN_HOLD))
                {
                    return -Mathf.Min(Mouse.current.delta.x.ReadValue(), 0) * Screen.width * this.CoreInputConfiguration.GetCameraMovementMouseSensitivity();
                }

                return 0f;
            }

            public bool TimeForwardButtonDH()
            {
                return this.gameInputV2.InputConditionsMet(InputID.TIME_FORWARD_DOWN_HOLD);
            }

            public bool PuzzleResetButton()
            {
                return this.gameInputV2.InputConditionsMet(InputID.PUZZLE_RESET_DOWN_HOLD);
            }
        }

        #region Data Retrieval
        public Dictionary<Key, KeyControl> GetKeyToKeyControlLookup()
        {
            return ((GameInput)this.currentInput).GameInputV2.KeyToKeyControlLookup;
        }
        public Dictionary<MouseButton, ButtonControl> GetMouseButtonControlLookup()
        {
            return ((GameInput)this.currentInput).GameInputV2.MouseButtonControlLookup;
        }
        #endregion

    }

    class GameInputV2
    {
        private Dictionary<Key, KeyControl> keyToKeyControlLookup;
        private Dictionary<MouseButton, ButtonControl> mouseButtonControlLookup;

        private InputConfiguration InputConfiguration;

        public GameInputV2(InputConfiguration inputConfiguration)
        {
            this.keyToKeyControlLookup = new Dictionary<Key, KeyControl>();
            this.mouseButtonControlLookup = new Dictionary<MouseButton, ButtonControl>();

            foreach (var keyBoardKeyControl in Keyboard.current.allKeys)
            {
                this.keyToKeyControlLookup[keyBoardKeyControl.keyCode] = keyBoardKeyControl;
            }
            this.mouseButtonControlLookup[MouseButton.LEFT_BUTTON] = Mouse.current.leftButton;
            this.mouseButtonControlLookup[MouseButton.RIGHT_BUTTON] = Mouse.current.rightButton;

            InputConfiguration = inputConfiguration;
        }

        public Dictionary<Key, KeyControl> KeyToKeyControlLookup { get => keyToKeyControlLookup; }
        public Dictionary<MouseButton, ButtonControl> MouseButtonControlLookup { get => mouseButtonControlLookup; }

        public bool InputConditionsMet(InputID inputID)
        {
            bool inputConditionsMet = false;
            if (Application.isFocused)
            {
                var inputConfigurationInherentData = this.InputConfiguration.ConfigurationInherentData[inputID];
                foreach (var attibutedKey in inputConfigurationInherentData.AttributedKeys)
                {
                    if (inputConfigurationInherentData.Down)
                    {
                        inputConditionsMet = inputConditionsMet || this.keyToKeyControlLookup[attibutedKey].wasPressedThisFrame;
                    }
                    else if (inputConfigurationInherentData.DownHold)
                    {
                        inputConditionsMet = inputConditionsMet || this.keyToKeyControlLookup[attibutedKey].wasPressedThisFrame || this.keyToKeyControlLookup[attibutedKey].isPressed;
                    }
                }

                foreach (var attrubuteMouseButton in inputConfigurationInherentData.AttributedMouseButtons)
                {
                    if (inputConfigurationInherentData.Down)
                    {
                        inputConditionsMet = inputConditionsMet || this.mouseButtonControlLookup[attrubuteMouseButton].wasPressedThisFrame;
                    }
                    else if (inputConfigurationInherentData.DownHold)
                    {
                        inputConditionsMet = inputConditionsMet || this.mouseButtonControlLookup[attrubuteMouseButton].wasPressedThisFrame || this.mouseButtonControlLookup[attrubuteMouseButton].isPressed;
                    }
                }
            }
            return inputConditionsMet;
        }
    }

    public interface XInput
    {
        Vector3 LocomotionAxis();
        Vector3 CursorDisplacement();
        float LeftRotationCameraDH();
        float RightRotationCameraDH();
        bool ActionButtonD();
        bool InventoryButtonD();
        bool CancelButtonD();
        bool CancelButtonDH();
        bool TimeForwardButtonDH();
        bool PuzzleResetButton();
    }

    public enum MouseButton
    {
        NONE = 0,
        LEFT_BUTTON = 1,
        RIGHT_BUTTON = 2
    }
}