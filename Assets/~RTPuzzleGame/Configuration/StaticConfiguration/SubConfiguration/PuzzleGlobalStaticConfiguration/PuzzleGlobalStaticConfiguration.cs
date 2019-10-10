
using InteractiveObjectTest;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGlobalStaticConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzleGlobalStaticConfiguration", order = 1)]
    public class PuzzleGlobalStaticConfiguration : ScriptableObject
    {
        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData;
    }


    [System.Serializable]
    public class PlayerInteractiveObjectInitializerData
    {
        public float SpeedMultiplicationFactor = 20f;
        public float MinimumDistanceToStick = 0.01f;
    }
}

