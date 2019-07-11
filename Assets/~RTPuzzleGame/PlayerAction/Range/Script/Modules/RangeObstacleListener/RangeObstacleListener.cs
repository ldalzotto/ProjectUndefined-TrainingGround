using System;
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
        public bool IsPointOccludedByObstacles(Vector3 worldPositionPoint)
        {
            return this.obstacleListener.IsPointOccludedByObstacles(worldPositionPoint);
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
        public void OnRangeObstacleListenerDestroyed(RangeType rangeType)
        {
            this.obstacleListener.OnObstacleListenerDestroyed();
        }
        #endregion
    }
}
