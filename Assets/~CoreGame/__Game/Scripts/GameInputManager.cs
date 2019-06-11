using System;
using UnityEngine;

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
        }

        private class JoystickInput : XInput
        {
            public bool ActionButtonD()
            {
                return Input.GetButtonDown("Action") || Input.GetMouseButtonDown(0);
            }

            public bool ActionButtonDH()
            {
                return Input.GetButton("Action") || Input.GetButtonDown("Action") || Input.GetMouseButton(0) || Input.GetMouseButtonDown(0);
            }

            public bool CancelButtonD()
            {
                return Input.GetButtonDown("Cancel");
            }

            public bool CancelButtonDH()
            {
                return Input.GetButton("Cancel") || Input.GetButtonDown("Cancel");
            }

            public bool InventoryButtonD()
            {
                return Input.GetButtonDown("Inventory");
            }

            public Vector3 LocomotionAxis()
            {
                if (Mathf.Abs(Input.GetAxis("Horizontal")) >= float.Epsilon || Mathf.Abs(Input.GetAxis("Vertical")) >= float.Epsilon)
                {
                    //keyboard
                    var rawDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    if (Vector3.Distance(rawDirection, Vector3.zero) > 1)
                    {
                        rawDirection = rawDirection.normalized;
                    }
                    return rawDirection;
                }
                else
                {
                    //gamepad
                    return new Vector3(Input.GetAxis("Horizontal_PAD"), 0, Input.GetAxis("Vertical_PAD"));
                }
            }
            
            public Vector3 CursorDisplacement()
            {
                if (Mathf.Abs(Input.GetAxis("Mouse X")) >= float.Epsilon || Mathf.Abs(Input.GetAxis("Mouse Y")) >= float.Epsilon)
                {
                    //keyboard
                    return new Vector3(Input.GetAxis("Mouse X"),0, Input.GetAxis("Mouse Y"));
                }
                else
                {
                    //gamepad
                    return new Vector3(Input.GetAxis("Horizontal_PAD"), 0, Input.GetAxis("Vertical_PAD"));
                }
            }
            
            public float LeftRotationCameraDH()
            {
                var buttonPress = (Input.GetButton("Camera_Rotation_Left") || Input.GetButtonDown("Camera_Rotation_Left"));
                if (buttonPress)
                {
                    return Convert.ToInt32(buttonPress);
                }
                else if (Input.GetMouseButton(1))
                {
                    return Mathf.Max(Input.GetAxis("Mouse X"), 0);
                }
                return 0f;
            }

            public float RightRotationCameraDH()
            {
                var buttonPress = (Input.GetButton("Camera_Rotation_Right") || Input.GetButtonDown("Camera_Rotation_Right"));
                if (buttonPress)
                {
                    return Convert.ToInt32(buttonPress);
                }
                else if (Input.GetMouseButton(1))
                {
                    return Mathf.Min(Input.GetAxis("Mouse X"), 0) * -1;
                }
                return 0f;
            }

            public bool TimeForwardButtonDH()
            {
                return Input.GetButton("TimeForward") || Input.GetButtonDown("TimeForward");
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
    }
}