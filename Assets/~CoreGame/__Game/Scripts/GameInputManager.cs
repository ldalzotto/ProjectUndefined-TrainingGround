using System;
using UnityEngine;

public interface IGameInputManager
{
    XInput CurrentInput { get; }
}

public class GameInputManager : MonoBehaviour, IGameInputManager
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
            return Convert.ToInt32(Input.GetButton("Camera_Rotation_Left") || Input.GetButtonDown("Camera_Rotation_Left"));
        }

        public float RightRotationCameraDH()
        {
            return Convert.ToInt32(Input.GetButton("Camera_Rotation_Right") || Input.GetButtonDown("Camera_Rotation_Right"));
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

        public float LeftRotationCameraDH()
        {
            throw new NotImplementedException();
        }

        public Vector3 LocomotionAxis()
        {
            throw new System.NotImplementedException();
        }

        public float RightRotationCameraDH()
        {
            throw new NotImplementedException();
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

        public float LeftRotationCameraDH()
        {
            throw new NotImplementedException();
        }

        public Vector3 LocomotionAxis()
        {
            throw new System.NotImplementedException();
        }

        public float RightRotationCameraDH()
        {
            throw new NotImplementedException();
        }

        public bool TimeForwardButtonDH()
        {
            throw new NotImplementedException();
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