using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private LevelZonesID levelID;

        #region Data Retrieval
        public LevelZonesID GetCurrentLevel()
        {
            return levelID;
        }
        #endregion

    }

}
