using UnityEngine;
using UnityEngine.UI;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGlobalStaticConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzleGlobalStaticConfiguration", order = 1)]
    public class PuzzleGlobalStaticConfiguration : ScriptableObject
    {
        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData;
        public PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent;
        public TimeFlowBarUILayoutData TimeFlowBarUILayoutData;
    }


    [System.Serializable]
    public class PlayerInteractiveObjectInitializerData
    {
        public float SpeedMultiplicationFactor = 20f;
        public float MinimumDistanceToStick = 0.01f;
    }

    [System.Serializable]
    public class TimeFlowBarUILayoutData
    {
        [Header("Time flow bar UI")]
        public Vector2 TimeFlowBarSizeDeltaFactor;
        public Vector2 TimeFlowBarOffsetMinFactor;

        [Header("Time flow play/pause")]
        public Vector2 TimeFlowPlayIconSizeDeltaFactor;
        public Vector2 TimeFlowPlayIconOffsetMinFactor;
        public TimeFlowPlayPauseIconSwitchComponent TimeFlowPlayPauseIconSwitchComponent;
    }
}

