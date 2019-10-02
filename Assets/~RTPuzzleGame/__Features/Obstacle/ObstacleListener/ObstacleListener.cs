using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstacleListener
    {
        public float Radius;

        private List<SquareObstacle> nearSquareObstacles;
        public List<SquareObstacle> NearSquareObstacles { get => nearSquareObstacles; }

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

            this.nearSquareObstacles = new List<SquareObstacle>();
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

        //////// TO REMOVE //////// 
        public void OnRangeTriggerEnter(CollisionType collisionType)
        {
            if (collisionType != null)
            {
                var squareObstacle = SquareObstacle.FromCollisionType(collisionType);
                if (squareObstacle != null)
                {
                    this.AddNearSquareObstacle(squareObstacle);
                }
            }
        }

        public void OnRangeTriggerExit(CollisionType collisionType)
        {
            if (collisionType != null)
            {
                var squareObstacle = SquareObstacle.FromCollisionType(collisionType);
                if (squareObstacle != null)
                {
                    this.RemoveNearSquareObstacle(squareObstacle);
                }
            }
        }
        /////////////////////////

        public void AddNearSquareObstacle(SquareObstacle squareObstacle)
        {
            this.nearSquareObstacles.Add(squareObstacle);
            this.SortObstaclesByDistance();
            this.ObstacleFrustumCalculationManager.OnObstacleAddedToListener(this, squareObstacle);
        }

        public void RemoveNearSquareObstacle(SquareObstacle squareObstacle)
        {
            this.nearSquareObstacles.Remove(squareObstacle);
            this.SortObstaclesByDistance();
            this.ObstacleFrustumCalculationManager.OnObstacleRemovedToListener(this, squareObstacle);
        }

        //Nearest obstacles are sorted by distance in order to do intersection calculation from the nearest to the farest frustum.
        //Nearest frustum has more chance to oclcude a wider space than a farest
        private void SortObstaclesByDistance()
        {
            var RangeObjectV2GetWorldTransformEventReturn = this.AssociatedRangeObject.GetTransform();
            this.nearSquareObstacles.Sort((o1, o2) => { return Vector3.Distance(o1.transform.position, RangeObjectV2GetWorldTransformEventReturn.WorldPosition).CompareTo(Vector3.Distance(o2.transform.position, RangeObjectV2GetWorldTransformEventReturn.WorldPosition)); });
        }
    }

}
