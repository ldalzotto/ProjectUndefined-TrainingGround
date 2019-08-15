using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public interface ITutorialEventListener
    {
        void OnPlayerActionWheelAwake();
        void OnPlayerActionWheelSleep();
        void OnPlayerActionWheelNodeSelected();
    }

    public enum TutorialGraphEventType
    {
        PUZZLE_ACTION_WHEEL_AWAKE
    }
}
