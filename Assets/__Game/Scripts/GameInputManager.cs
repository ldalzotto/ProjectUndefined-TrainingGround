﻿using System;
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
        bool ActionButtonDH();
        bool ActionButtonD();
        bool InventoryButtonD();
        bool CancelButtonD();
        bool CancelButtonDH();
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

