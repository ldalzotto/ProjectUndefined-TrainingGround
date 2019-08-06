﻿using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GrabActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/GrabActionInherentData", order = 1)]
    public class GrabActionInherentData : PlayerActionInherentData
    {
        [CustomEnum()]
        public GrabObjectID GrabObjectID;

        [CustomEnum()]
        public PlayerActionId PlayerActionToIncrement;

        public GrabActionInherentData(GrabObjectID grabObjectID, PlayerActionId PlayerActionToIncrement, SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.GrabObjectID = grabObjectID;
            this.PlayerActionToIncrement = PlayerActionToIncrement;
        }
    }
}