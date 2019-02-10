using UnityEngine;

namespace RTPuzzle
{
    public class RTPlayerManagerDataRetriever : MonoBehaviour
    {

        private RTPlayerManager RTPlayerManager;

        public void Init()
        {
            RTPlayerManager = GameObject.FindObjectOfType<RTPlayerManager>();
        }

        #region Data Retrieval
        public float GetPlayerSpeedMagnitude()
        {
            return RTPlayerManager.GetPlayerSpeedMagnitude();
        }
        public Transform GetPlayerTransform()
        {
            return RTPlayerManager.transform;
        }
        #endregion
    }

}
