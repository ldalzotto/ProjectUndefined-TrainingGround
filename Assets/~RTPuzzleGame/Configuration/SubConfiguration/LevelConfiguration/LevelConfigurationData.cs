using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelConfigurationData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfigurationDataData", order = 1)]
    public class LevelConfigurationData : ScriptableObject
    {
        [SerializeField]
        private float availableTimeAmount;

        public LevelConfigurationData(float availableTimeAmount)
        {
            this.availableTimeAmount = availableTimeAmount;
        }

        public float AvailableTimeAmount { get => availableTimeAmount; }
    }

}
