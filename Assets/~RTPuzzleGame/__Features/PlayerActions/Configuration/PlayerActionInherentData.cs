﻿using System;
using CoreGame;
using GameConfigurationID;
using InteractiveObjects;
using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    public abstract class PlayerActionInherentData : SerializedScriptableObject
    {
        public CorePlayerActionDefinition CorePlayerActionDefinition;

        public abstract RTPPlayerAction BuildPlayerAction(PlayerInteractiveObject PlayerInteractiveObject);
    }

    [Serializable]
    public struct CorePlayerActionDefinition
    {
        [CustomEnum()] public PlayerActionType PlayerActionType;

        [CustomEnum(ConfigurationType = typeof(SelectionWheelNodeConfiguration))]
        public SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId;

        [Tooltip("Number of times the action can be executed. -1 is infinite. -2 is not displayed")]
        public int ExecutionAmount;

        public float CoolDownTime;
    }
}