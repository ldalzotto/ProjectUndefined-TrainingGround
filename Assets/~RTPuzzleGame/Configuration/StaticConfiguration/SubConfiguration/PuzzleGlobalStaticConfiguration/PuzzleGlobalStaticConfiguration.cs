using System;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "PuzzleGlobalStaticConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzleGlobalStaticConfiguration", order = 1)]
    public class PuzzleGlobalStaticConfiguration : ScriptableObject
    {
        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData;
        public PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent;
    }


    [Serializable]
    public class PlayerInteractiveObjectInitializerData
    {
        public float MinimumDistanceToStick = 0.01f;
        public float SpeedMultiplicationFactor = 20f;
    }
}