using UnityEngine;

namespace RTPuzzle
{
    public class RangeObstacleListener : MonoBehaviour
    {
        #region Internal Managers
        private ObstacleListener obstacleListener;
        #endregion

        public ObstacleListener ObstacleListener { get => obstacleListener; }

        public void Init(RangeType associatedRangeType)
        {
            this.obstacleListener = GetComponent<ObstacleListener>();
            this.obstacleListener.Init();
            this.obstacleListener.Radius = associatedRangeType.GetRadiusRange();
        }
        
        #region Logical Conditions
        public bool IsListenerHaveObstaclesNearby()
        {
            return this.obstacleListener.IsListenerHaveObstaclesNearby();
        }
        #endregion

        #region External Events
        public void OnRangeTriggerEnter(Collider other)
        {
            this.obstacleListener.OnRangeTriggerEnter(other);
        }
        public void OnRangeTriggerExit(Collider other)
        {
            this.obstacleListener.OnRangeTriggerExit(other);
        }
        #endregion
    }
}
