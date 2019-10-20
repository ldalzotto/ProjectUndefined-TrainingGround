using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace SelectionWheel
{
    public class SelectionWheelEventsManager : GameSingleton<SelectionWheelEventsManager>
    {
        public delegate void OnWheelAwakeEventDelegate(List<SelectionWheelNodeData> availableNodes, Transform followingWorldTransform);

        public delegate void OnWheelRefreshEventDelegate(List<SelectionWheelNodeData> availableNodes, Transform followingWorldTransform);

        public delegate void OnWheelSleepEventDelegate(bool destroyImmediate);

        private event OnWheelAwakeEventDelegate OnWheelAwakeEvent;

        public void RegisterOnWheelAwakeEventListener(OnWheelAwakeEventDelegate OnWheelAwakeEventDelegate)
        {
            OnWheelAwakeEvent += OnWheelAwakeEventDelegate;
        }

        public void OnWheelAwake(List<SelectionWheelNodeData> availableNodes, Transform followingWorldTransform)
        {
            OnWheelAwakeEvent.Invoke(availableNodes, followingWorldTransform);
        }

        private event OnWheelSleepEventDelegate OnWheelSleepEvent;

        public void RegisterOnWheelSleepEventListener(OnWheelSleepEventDelegate OnWheelSleepEventDelegate)
        {
            OnWheelSleepEvent += OnWheelSleepEventDelegate;
        }

        public void OnWheelSleep(bool destroyImmediate)
        {
            OnWheelSleepEvent.Invoke(destroyImmediate);
        }

        private event OnWheelRefreshEventDelegate OnWheelRefreshEvent;

        public void RegisterOnWheelRefreshEvent(OnWheelRefreshEventDelegate OnWheelRefreshEventDelegate)
        {
            OnWheelRefreshEvent += OnWheelRefreshEventDelegate;
        }

        public void OnWheelRefresh(List<SelectionWheelNodeData> availableNodes, Transform followingWorldTransform)
        {
            OnWheelRefreshEvent.Invoke(availableNodes, followingWorldTransform);
        }
    }
}