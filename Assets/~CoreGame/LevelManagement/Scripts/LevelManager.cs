using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CoreGame
{
    public class LevelManager : MonoBehaviour
    {
        private LevelType currentLevelType;

        [SerializeField]
        private LevelZonesID levelID;

        public LevelType CurrentLevelType { get => currentLevelType; }
        public LevelZonesID LevelID { get => levelID; }

        public void Init(LevelType currentLevelType)
        {
            this.currentLevelType = currentLevelType;
            if (SceneConstants.LevelSceneChunkRetriever.ContainsKey(this.levelID))
            {
                if (!SceneManager.GetSceneByName(SceneConstants.LevelSceneChunkRetriever[this.levelID]).isLoaded)
                {
                    SceneManager.LoadScene(SceneConstants.LevelSceneChunkRetriever[this.levelID], LoadSceneMode.Additive);
                }
            }
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
