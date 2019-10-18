using System;
using System.Collections.Generic;
using CoreGame;
using Obstacle;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstacleListenerSystem
    {
        private List<ObstacleInteractiveObject> nearSquareObstacles;

        #region External Dependencies

        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();

        #endregion

        #region External Dependencies

        private ObstaclesListenerManager ObstaclesListenerManager;

        #endregion

        public ObstacleListenerSystem(RangeObjectV2 associatedRangeObject)
        {
            AssociatedRangeObject = associatedRangeObject;

            #region External Dependencies

            this.ObstaclesListenerManager = ObstaclesListenerManager.Get();

            #endregion

            this.nearSquareObstacles = new List<ObstacleInteractiveObject>();
            this.ObstacleListenerUniqueID = this.ObstaclesListenerManager.OnObstacleListenerCreation(this);
        }

        public int ObstacleListenerUniqueID { get; private set; }

        public List<ObstacleInteractiveObject> NearSquareObstacles => nearSquareObstacles;

        #region Internal Dependencies

        public RangeObjectV2 AssociatedRangeObject { get; private set; }

        #endregion

        public void OnObstacleListenerDestroyed()
        {
            Debug.Log(MyLog.Format("OnObstacleListenerDestroyed"));
            this.ObstaclesListenerManager.OnObstacleListenerDestroyed(this);
            this.nearSquareObstacles.Clear();
        }

        #region Data Retrieval

        public void ForEachCalculatedFrustum(Action<FrustumPointsPositions> action)
        {
            foreach (var obstacleInteractiveObject in this.nearSquareObstacles)
            {
                foreach (var obstacleFrustumPositions in this.ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustums(this, obstacleInteractiveObject))
                {
                    action(obstacleFrustumPositions);
                }
            }
        }

        #endregion

        public void AddNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Add(ObstacleInteractiveObject);
        }

        public void RemoveNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Remove(ObstacleInteractiveObject);
        }
    }
}