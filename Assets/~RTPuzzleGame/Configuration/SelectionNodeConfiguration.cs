using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{

        public static class SelectionWheelNodeConfiguration
        {
            public static Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData> selectionWheelNodeConfiguration =
                new Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>()
                {
            {SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(RTPuzzle.LaunchProjectileAction)) }
                };
        }

        public enum SelectionWheelNodeConfigurationId
        {
            THROW_PLAYER_PUZZLE_WHEEL_CONFIG = 3
        }

}