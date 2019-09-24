using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfigurationData", menuName = "Configuration/PuzzleGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfigurationData", order = 1)]
    public class SelectionWheelNodeConfigurationData : ScriptableObject
    {
        public Sprite WheelNodeIcon;
        public string DescriptionText;
    }

}

