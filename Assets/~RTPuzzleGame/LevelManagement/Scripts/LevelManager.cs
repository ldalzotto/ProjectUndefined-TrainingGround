using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class LevelManager : MonoBehaviour
    {

        private LevelZonesID levelID;

        public void Init(LevelZonesID levelZonesID)
        {
            this.levelID = levelZonesID;
        }

        #region Data Retrieval
        public LevelZonesID GetCurrentLevel()
        {
            return levelID;
        }
        #endregion

    }

}
