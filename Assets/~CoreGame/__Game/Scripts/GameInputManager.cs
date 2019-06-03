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
        }

        private class JoystickInput : XInput
        {
            public bool ActionButtonD()
            {
                return Input.GetButtonDown("Action");
            }

            public bool ActionButtonDH()
            {
                return Input.GetButton("Action") || Input.GetButtonDown("Action");
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
                return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
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