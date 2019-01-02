using System;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    private XInput currentInput;

    public XInput CurrentInput { get => currentInput; }

    private void Start()
    {
        currentInput = new JoystickInput();
    }

    public interface XInput
    {
        Vector3 LocomotionAxis();
        int LeftRotationCameraDH();
        int RightRotationCameraDH();
    }

    private class JoystickInput : XInput
    {
        public int LeftRotationCameraDH()
        {
            return Convert.ToInt32(Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.Joystick1Button4));
        }

        public Vector3 LocomotionAxis()
        {
            return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        public int RightRotationCameraDH()
        {
            return Convert.ToInt32(Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.Joystick1Button5));
        }
    }

    private class KeyboardInput : XInput
    {
        public int LeftRotationCameraDH()
        {
            throw new NotImplementedException();
        }

        public Vector3 LocomotionAxis()
        {
            throw new System.NotImplementedException();
        }

        public int RightRotationCameraDH()
        {
            throw new NotImplementedException();
        }
    }

    private class MobileInput : XInput
    {
        public int LeftRotationCameraDH()
        {
            throw new NotImplementedException();
        }

        public Vector3 LocomotionAxis()
        {
            throw new System.NotImplementedException();
        }

        public int RightRotationCameraDH()
        {
            throw new NotImplementedException();
        }
    }

}

