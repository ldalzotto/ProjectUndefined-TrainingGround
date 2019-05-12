using UnityEngine;
using System.Collections;
namespace CoreGame
{
    public class LevelManager : MonoBehaviour
    {
        private LevelType currentLevelType;

        [SerializeField]
        private LevelZonesID levelID;

        public LevelType CurrentLevelType { get => currentLevelType; }

        public void Init(LevelType currentLevelType)
        {
            this.currentLevelType = currentLevelType;
        }

        #region Data Retrieval
        public LevelZonesID GetCurrentLevel()
        {
            return levelID;
        }
        #endregion
    }

    public enum LevelType
    {
        ADVENTURE, PUZZLE
    }



}
