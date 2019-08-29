using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    public class LevelMemoryManager : MonoBehaviour
    {
        #region Internal state
        private LevelZonesID lastAdventureLevel;
        private LevelZonesID lastPuzzleLevel;
        public LevelZonesID LastAdventureLevel { get => lastAdventureLevel; }
        #endregion

        public void Init(LevelType currentLevelType, LevelManager levelManager)
        {
            if (currentLevelType == LevelType.ADVENTURE)
            {
                this.lastAdventureLevel = levelManager.LevelID;
            }
            else
            {
                this.lastPuzzleLevel = levelManager.LevelID;
            }
        }
    }
}
