using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
            currentInput = new JoystickInput();
            Cursor.lockState = CursorLockMode.Locked;
            //todo strange
        }

        private class JoystickInput : XInput
        {
            public bool ActionButtonD()
            {
                if (!Application.isFocused)
                {
                    return false;
                }

                return Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame;
            }

            public bool ActionButtonDH()
            {
                if (!Application.isFocused)
                {
                    return false;
                }

                return Keyboard.current.fKey.isPressed || Mouse.current.leftButton.isPressed;
            }

            public bool CancelButtonD()
            {
                if (!Application.isFocused)
                {
                    return false;
                }

                return Keyboard.current.cKey.wasPressedThisFrame;
            }

            public bool CancelButtonDH()
            {
                if (!Application.isFocused)
                {
                    return false;
                }

                return Keyboard.current.cKey.isPressed;
            }

            public bool InventoryButtonD()
            {
                if (!Application.isFocused)
                {
                    return false;
                }

                return Keyboard.current.eKey.isPressed;
            }

            public Vector3 LocomotionAxis()
            {
                if (!Application.isFocused)
                {
                    return Vector3.zero;
                }

                if (Gamepad.current != null)
                {
                    return Vector3.zero;
                }
                else
                {
                    //keyboard
                    var rawDirection = new Vector3(-Convert.ToInt32(InputHelper.IsEitherPressed(Keyboard.current.leftArrowKey, Keyboard.current.aKey)) + Convert.ToInt32(InputHelper.IsEitherPressed(Keyboard.current.rightArrowKey, Keyboard.current.dKey)), 0,
                             -Convert.ToInt32(InputHelper.IsEitherPressed(Keyboard.current.downArrowKey, Keyboard.current.sKey)) + Convert.ToInt32(InputHelper.IsEitherPressed(Keyboard.current.upArrowKey, Keyboard.current.wKey)));
                    if (Vector3.Distance(rawDirection, Vector3.zero) > 1)
                    {
                        rawDirection = rawDirection.normalized;
                    }
                    return rawDirection;
                }
            }

            public Vector3 CursorDisplacement()
            {
                if (!Application.isFocused)
                {
                    return Vector3.zero;
                }

                if (Gamepad.current != null)
                {
                    return Vector3.zero;
                }
                else
                {
                    return new Vector3(Mouse.current.delta.x.ReadValue(), 0, Mouse.current.delta.y.ReadValue());
                }
            }

            public float LeftRotationCameraDH()
            {
                if (!Application.isFocused)
                {
                    return 0f;
                }

                if (Mouse.current.rightButton.isPressed)
                {
                    return Mathf.Max(Mouse.current.delta.x.ReadValue(), 0);
                }
                else
                {
                    return 0f;
                }
            }

            public float RightRotationCameraDH()
            {
                if (!Application.isFocused)
                {
                    return 0f;
                }

                if (Mouse.current.rightButton.isPressed)
                {
                    return -Mathf.Min(Mouse.current.delta.x.ReadValue(), 0);
                }
                else
                {
                    return 0f;
                }
            }

            public bool TimeForwardButtonDH()
            {
                if (!Application.isFocused)
                {
                    return false;
                }

                return Keyboard.current.iKey.isPressed;
            }

            public bool PuzzleResetButton()
            {
                if (!Application.isFocused)
                {
                    return false;
                }

                return Keyboard.current.rKey.isPressed;
            }
        }

    }

    public interface XInput
    {
        Vector3 LocomotionAxis();
        Vector3 CursorDisplacement();
        float LeftRotationCameraDH();
        float RightRotationCameraDH();
        bool ActionButtonDH();
        bool ActionButtonD();
        bool InventoryButtonD();
        bool CancelButtonD();
        bool CancelButtonDH();
        bool TimeForwardButtonDH();
        bool PuzzleResetButton();
    }
}