using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.Controls;

namespace CoreGame
{
    public static class InputHelper
    {
        public static bool IsEitherPressed(params KeyControl[] keys)
        {
            bool isEitherPressed = false;
            foreach (var key in keys)
            {
                if (key.isPressed) { isEitherPressed = true; }
            }
            return isEitherPressed;
        }
    }

}
