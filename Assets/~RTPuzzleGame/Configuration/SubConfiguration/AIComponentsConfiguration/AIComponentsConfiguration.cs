﻿using ConfigurationEditor;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIComponentsConfiguration", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIComponentsConfiguration", order = 1)]
    public class AIComponentsConfiguration : ConfigurationSerialization<AiID, AIComponents>
    {
        public void Init()
        {
            foreach(var aiConfiguration in this.ConfigurationInherentData)
            {
                aiConfiguration.Value.Init();
            }
        }
    }

}