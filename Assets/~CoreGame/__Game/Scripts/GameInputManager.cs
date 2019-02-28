using System;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    private XInput currentInput;

    public XInput CurrentInput { get => currentInput; }

    private void Start()
    {
#if UNITY_STANDALONE
        currentInput = new JoystickInput();
#endif
#if UNITY_IOS
        var modileInputJoystickManager = GameObject.FindObjectOfType<MobileInputJoystickManager>();
        currentInput = new MobileInput(modileInputJoystickManager);
#endif
    }

    public interface XInput
    {
        Vector3 LocomotionAxis();
        Vector3 CameraRotationAxis();
        bool ActionButtonDH();
        bool ActionButtonD();
        bool InventoryButtonD();
        bool CancelButtonD();
        bool CancelButtonDH();
        bool TimeForwardButtonDH();
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

        public Vector3 CameraRotationAxis()
        {
            return new Vector3(Input.GetAxis("Camera_Vertical"), Input.GetAxis("Camera_Horizontal"), 0f);
        }

        public bool TimeForwardButtonDH()
        {
            return Input.GetButton("TimeForward") || Input.GetButtonDown("TimeForward");
        }
    }

    private class KeyboardInput : XInput
    {
        public bool ActionButtonD()
        {
            throw new NotImplementedException();
        }

        public bool ActionButtonDH()
        {
            throw new NotImplementedException();
        }

        public Vector3 CameraRotationAxis()
        {
            throw new NotImplementedException();
        }

        public bool CancelButtonD()
        {
            throw new NotImplementedException();
        }

        public bool CancelButtonDH()
        {
            throw new NotImplementedException();
        }

        public bool InventoryButtonD()
        {
            throw new NotImplementedException();
        }

        public Vector3 LocomotionAxis()
        {
            throw new System.NotImplementedException();
        }

        public bool TimeForwardButtonDH()
        {
            throw new NotImplementedException();
        }
    }

    private class MobileInput : XInput
    {
        public bool ActionButtonD()
        {
            throw new NotImplementedException();
        }

        public bool ActionButtonDH()
        {
            throw new NotImplementedException();
        }

        public Vector3 CameraRotationAxis()
        {
            throw new NotImplementedException();
        }

        public bool CancelButtonD()
        {
            throw new NotImplementedException();
        }

        public bool CancelButtonDH()
        {
            throw new NotImplementedException();
        }

        public bool InventoryButtonD()
        {
            throw new NotImplementedException();
        }

        public Vector3 LocomotionAxis()
        {
            throw new System.NotImplementedException();
        }

        public bool TimeForwardButtonDH()
        {
            throw new NotImplementedException();
        }
    }

}

