using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeObstacleListenerModule : MonoBehaviour
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
        public bool IsBoxOccludedByObstacles(BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
        {
            return this.obstacleListener.IsBoxOccludedByObstacles(boxCollider, forceObstacleOcclusionIfNecessary);
        }
        public bool IsPointOccludedByObstacles(Vector3 worldPositionPoint, bool forceObstacleOcclusionIfNecessary)
        {
            return this.obstacleListener.IsPointOccludedByObstacles(worldPositionPoint, forceObstacleOcclusionIfNecessary);
        }
        #endregion

        #region External Events
        public void OnRangeTriggerEnter(CollisionType collistionType)
        {
            this.obstacleListener.OnRangeTriggerEnter(collistionType);
        }
        public void OnRangeTriggerExit(CollisionType collistionType)
        {
            this.obstacleListener.OnRangeTriggerExit(collistionType);
        }
        public void OnRangeObstacleListenerDestroyed(RangeType rangeType)
        {
            this.obstacleListener.OnObstacleListenerDestroyed();
        }
        #endregion

        #region Data Retrieval
        public List<FrustumPointsPositions> GetCalculatedFrustums()
        {
            return this.obstacleListener.GetCalculatedFrustums();
        }
        #endregion
    }
}
