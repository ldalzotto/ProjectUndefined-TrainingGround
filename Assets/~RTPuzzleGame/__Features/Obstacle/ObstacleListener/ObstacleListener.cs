using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstacleListener
    {
        public int ObstacleListenerUniqueID { get; private set; }
        public float Radius;

        private List<SquareObstacleSystem> nearSquareObstacles;
        public List<SquareObstacleSystem> NearSquareObstacles { get => nearSquareObstacles; }

        #region Internal Dependencies
        public RangeObjectV2 AssociatedRangeObject { get; private set; }
        #endregion

        public ObstacleListener(RangeObjectV2 associatedRangeObject)
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
        public List<FrustumPointsPositions> GetCalculatedFrustums()
        {
            //TODO
            return new List<FrustumPointsPositions>();
            //      return this.ObstacleFrustumCalculationManager.GetResults(this).ConvertAll(obstacleFrustumCalculation => obstacleFrustumCalculation.CalculatedFrustumPositions)
            //      .SelectMany(r => r).ToList();
        }
        #endregion

        #region Logical Conditions
        public bool HasPositionChanged()
        {
            return false;
            //return this.ObstacleListenerChangePositionTracker.TransformChangedThatFrame();
        }
        public bool IsPointOccludedByObstacles(Vector3 worldPositionPoint, bool forceObstacleOcclusionIfNecessary)
        {
            return false;
            //TODO
            //  return this.ObstacleFrustumCalculationManager.IsPointOccludedByObstacles(this, worldPositionPoint, forceObstacleOcclusionIfNecessary);
        }
        internal bool IsBoxOccludedByObstacles(BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
        {
            //TODO
            return false;
            //  return this.ObstacleFrustumCalculationManager.IsPointOccludedByObstacles(this, boxCollider, forceObstacleOcclusionIfNecessary);
        }
        #endregion

        public void AddNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Add(ObstacleInteractiveObject.SquareObstacleSystem);
            ObstaclesListenerManager.Get().OnAddedNearObstacleToObstacleListener(this, ObstacleInteractiveObject);
        }

        public void RemoveNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Remove(ObstacleInteractiveObject.SquareObstacleSystem);
            ObstaclesListenerManager.Get().OnRemovedNearObstacleToObstacleListener(this, ObstacleInteractiveObject);
        }

    }

}
