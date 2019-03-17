﻿using ConfigurationEditor;
using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionConfiguration", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/PlayerActionConfiguration", order = 1)]
    public class PlayerActionConfiguration : ConfigurationSerialization<LevelZonesID, PlayerActionsInherentData>
    {

        public void Init()
        {
            foreach (var playerActionConfData in ConfigurationInherentData)
            {
                playerActionConfData.Value.Init();
            }
        }
    }
}