using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstacleListener
    {
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
            this.ObstacleFrustumCalculationManager = PuzzleGameSingletonInstances.ObstacleFrustumCalculationManager;
            this.ObstaclesListenerManager = PuzzleGameSingletonInstances.ObstaclesListenerManager;
            #endregion

            this.nearSquareObstacles = new List<SquareObstacleSystem>();
            this.ObstaclesListenerManager.OnObstacleListenerCreation(this);
            this.ObstacleFrustumCalculationManager.OnObstacleListenerCreation(this);
            this.ObstacleListenerChangePositionTracker = new BlittableTransformChangeListenerManager(true, false, null);
        }

        #region Internal Managers
        private BlittableTransformChangeListenerManager ObstacleListenerChangePositionTracker;
        #endregion

        #region External Dependencies
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        private ObstaclesListenerManager ObstaclesListenerManager;
        #endregion

        public void OnObstacleListenerDestroyed()
        {
            Debug.Log(MyLog.Format("OnObstacleListenerDestroyed"));
            this.ObstaclesListenerManager.OnObstacleListenerDestroyed(this);
            this.ObstacleFrustumCalculationManager.OnObstacleListenerDestroyed(this);
        }

        public void Tick(float d)
        {
            var RangeObjectV2GetWorldTransformEventReturn = this.AssociatedRangeObject.GetTransform();
            this.ObstacleListenerChangePositionTracker.Tick(RangeObjectV2GetWorldTransformEventReturn.WorldPosition, RangeObjectV2GetWorldTransformEventReturn.WorldRotation);
        }

        #region Data Retrieval
        public List<FrustumPointsPositions> GetCalculatedFrustums()
        {
            return this.ObstacleFrustumCalculationManager.GetResults(this).ConvertAll(obstacleFrustumCalculation => obstacleFrustumCalculation.CalculatedFrustumPositions)
                    .SelectMany(r => r).ToList();
        }
        #endregion

        #region Logical Conditions
        public bool HasPositionChanged()
        {
            return this.ObstacleListenerChangePositionTracker.TransformChangedThatFrame();
        }
        public bool IsListenerHaveObstaclesNearby()
        {
            return this.nearSquareObstacles.Count > 0;
        }
        public bool IsPointOccludedByObstacles(Vector3 worldPositionPoint, bool forceObstacleOcclusionIfNecessary)
        {
            return this.ObstacleFrustumCalculationManager.IsPointOccludedByObstacles(this, worldPositionPoint, forceObstacleOcclusionIfNecessary);
        }
        internal bool IsBoxOccludedByObstacles(BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
        {
            return this.ObstacleFrustumCalculationManager.IsPointOccludedByObstacles(this, boxCollider, forceObstacleOcclusionIfNecessary);
        }
        #endregion

        public void AddNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Add(ObstacleInteractiveObject.SquareObstacleSystem);
            this.SortObstaclesByDistance();
            this.ObstacleFrustumCalculationManager.OnObstacleAddedToListener(this, ObstacleInteractiveObject.SquareObstacleSystem);
        }

        public void RemoveNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            this.nearSquareObstacles.Remove(ObstacleInteractiveObject.SquareObstacleSystem);
            this.SortObstaclesByDistance();
            this.ObstacleFrustumCalculationManager.OnObstacleRemovedToListener(this, ObstacleInteractiveObject.SquareObstacleSystem);
        }

        //Nearest obstacles are sorted by distance in order to do intersection calculation from the nearest to the farest frustum.
        //Nearest frustum has more chance to oclcude a wider space than a farest
        private void SortObstaclesByDistance()
        {
            var RangeObjectV2GetWorldTransformEventReturn = this.AssociatedRangeObject.GetTransform();
            this.nearSquareObstacles.Sort((o1, o2) => { return Vector3.Distance(o1.GetPosition(), RangeObjectV2GetWorldTransformEventReturn.WorldPosition).CompareTo(Vector3.Distance(o2.GetPosition(), RangeObjectV2GetWorldTransformEventReturn.WorldPosition)); });
        }
    }

}
