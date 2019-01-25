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
        float LeftRotationCameraDH();
        float RightRotationCameraDH();
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

        public float LeftRotationCameraDH()
        {
            return Convert.ToInt32(Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.Joystick1Button4));
        }

        public Vector3 LocomotionAxis()
        {
            return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        public float RightRotationCameraDH()
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
    }

    private class MobileInput : XInput
    {
        private MobileInputJoystickManager modileInputJoystickManager;

        public MobileInput(MobileInputJoystickManager modileInputJoystickManager)
        {
            this.modileInputJoystickManager = modileInputJoystickManager;
        }

        public bool ActionButtonD()
        {
            return false;
        }

        public bool ActionButtonDH()
        {
            return false;
        }

        public bool CancelButtonD()
        {
            return false;
        }

        public bool CancelButtonDH()
        {
            return false;
        }

        public bool InventoryButtonD()
        {
            return modileInputJoystickManager.InventoryHeadPressed();
        }

        public float LeftRotationCameraDH()
        {
            var currentDeltaDrag = modileInputJoystickManager.GetOuterJoystickDragDelta();
            if (currentDeltaDrag.x >= 0)
            {
                return currentDeltaDrag.x / (Screen.width / 2);
            }
            else
            {
                return 0f;
            }

        }

        public Vector3 LocomotionAxis()
        {
            var currentJoystickValues = modileInputJoystickManager.GetCurrentJoystickValue();
            return new Vector3(currentJoystickValues.x, 0f, currentJoystickValues.y);
        }

        public float RightRotationCameraDH()
        {
            var currentDeltaDrag = modileInputJoystickManager.GetOuterJoystickDragDelta();
            if (currentDeltaDrag.x <= 0)
            {
                return currentDeltaDrag.x / (Screen.width / 2);
            }
            else
            {
                return 0f;
            }
        }
    }

}

