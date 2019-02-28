﻿using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class ContextActionWheelEventManager : MonoBehaviour
    {

        private PlayerManager PlayerManager;
        private ContextActionWheelManager ContextActionWheelManager;
        private InventoryManager InventoryManager;

        private void Start()
        {
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            ContextActionWheelManager = GameObject.FindObjectOfType<ContextActionWheelManager>();
            InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        }

        public void OnWheelDisabled()
        {
            PlayerManager.OnWheelDisabled();
            StartCoroutine(InventoryManager.OnContextActionWheelDisabled());
        }

        public void OnWheelEnabled(List<AContextAction> contextActions, WheelTriggerSource wheelTriggerSource)
        {
            ContextActionWheelManager.OnAwakeWheel(contextActions, wheelTriggerSource, PlayerManager.GetCurrentTargetedPOI());
        }
    }

}