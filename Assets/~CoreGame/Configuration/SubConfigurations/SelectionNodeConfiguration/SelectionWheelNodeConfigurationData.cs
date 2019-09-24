﻿using OdinSerializer;
using System;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfigurationData", menuName = "Configuration/PuzzleGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfigurationData", order = 1)]
    public class SelectionWheelNodeConfigurationData : SerializedScriptableObject
    {
        public Sprite WheelNodeIcon;
        public string DescriptionText;
    }

}

