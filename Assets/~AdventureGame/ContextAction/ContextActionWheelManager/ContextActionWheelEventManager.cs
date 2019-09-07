using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class ContextActionWheelEventManager : MonoBehaviour
    {
        private PlayerManager PlayerManager;
        private ContextActionWheelManager ContextActionWheelManager;
        private InventoryManager InventoryManager;
        private SelectionWheel ContextActionWheel;

        public void Init()
        {
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            ContextActionWheelManager = GameObject.FindObjectOfType<ContextActionWheelManager>();
            InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            ContextActionWheel = GameObject.FindObjectOfType<SelectionWheel>();
        }

        public void OnWheelDisabled(bool destroyImmediate = false)
        {
            ContextActionWheel.Exit(destroyImmediate);
            ContextActionWheelManager.SleepWheel();
            PlayerManager.OnWheelDisabled();
            StartCoroutine(InventoryManager.OnContextActionWheelDisabled());
        }

        public void OnWheelEnabled(List<AContextAction> contextActions, WheelTriggerSource wheelTriggerSource)
        {
            ContextActionWheelManager.OnAwakeWheel(contextActions, wheelTriggerSource, PlayerManager.GetCurrentTargetedPOI());
        }

        public void OnWheelRefresh(List<AContextAction> contextActions, WheelTriggerSource wheelTriggerSource)
        {
            this.OnWheelDisabled(destroyImmediate: true);
            this.OnWheelEnabled(contextActions, wheelTriggerSource);
        }

    }

}