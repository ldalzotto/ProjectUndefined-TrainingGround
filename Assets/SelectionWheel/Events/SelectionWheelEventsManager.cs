using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace SelectionWheel
{
    public class SelectionWheelEventsManager : GameSingleton<SelectionWheelEventsManager>
    {
        public delegate void OnWheelAwakeEventDelegate(List<SelectionWheelNodeData> availableNodes, Transform followingWorldTransform);

        public delegate void OnWheelSleepEventDelegate(bool destroyImmediate);

        private event OnWheelAwakeEventDelegate OnWheelAwakeEvent;

        public void RegisterOnWheelAwakeEventListener(OnWheelAwakeEventDelegate OnWheelAwakeEventDelegate)
        {
            OnWheelAwakeEvent += OnWheelAwakeEventDelegate;
        }

        public void OnWheelAwake(List<SelectionWheelNodeData> availableNodes, Transform followingWorldTransform)
        {
            if (OnWheelAwakeEvent != null) OnWheelAwakeEvent.Invoke(availableNodes, followingWorldTransform);
        }

        private event OnWheelSleepEventDelegate OnWheelSleepEvent;

        public void RegisterOnWheelSleepEventListener(OnWheelSleepEventDelegate OnWheelSleepEventDelegate)
        {
            OnWheelSleepEvent += OnWheelSleepEventDelegate;
        }

        public void OnWheelSleep(bool destroyImmediate)
        {
            if (OnWheelSleepEvent != null) OnWheelSleepEvent.Invoke(destroyImmediate);
        }
    }
}