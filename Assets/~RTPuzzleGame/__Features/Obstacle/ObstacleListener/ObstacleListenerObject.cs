using CoreGame;
using InteractiveObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstacleListenerObject
    {
        public int ObstacleListenerUniqueID { get; private set; }
        public float Radius;

        private List<SquareObstacleSystem> nearSquareObstacles;
        public List<SquareObstacleSystem> NearSquareObstacles { get => nearSquareObstacles; }

        #region Internal Dependencies
        public RangeObjectV2 AssociatedRangeObject { get; private set; }
        #endregion

        #region External Dependencies
        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
        #endregion

        public ObstacleListenerObject(RangeObjectV2 associatedRangeObject)
        {
            AssociatedRangeObject = associatedRangeObject;

            #region External Dependencies
            this.ObstaclesListenerManager = ObstaclesListenerManager.Get();
            #endregion

            this.nearSquareObstacles = new List<SquareObstacleSystem>();
            this.ObstacleListenerUniqueID = this.ObstaclesListenerManager.OnObstacleListenerCreation(this);
        }

        #region External Dependencies
        private ObstaclesListenerManager ObstaclesListenerManager;
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
            foreach (var nearSquareObstacle in this.nearSquareObstacles)
            {
                foreach(var obstacleFrustumPositions in this.ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustums(this, nearSquareObstacle))
                {
                    action(obstacleFrustumPositions);
                }
            }
        }
        #endregion

        public void AddNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Add(ObstacleInteractiveObject.SquareObstacleSystem);
        }

        public void RemoveNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Remove(ObstacleInteractiveObject.SquareObstacleSystem);
        }

    }

}
