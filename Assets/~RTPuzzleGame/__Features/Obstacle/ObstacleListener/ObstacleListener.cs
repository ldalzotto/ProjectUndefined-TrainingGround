using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstacleListener : MonoBehaviour
    {
        public float Radius;

        private List<SquareObstacle> nearSquareObstacles;
        public List<SquareObstacle> NearSquareObstacles { get => nearSquareObstacles; }

        #region Internal Managers
        private BlittableTransformChangeListenerManager ObstacleListenerChangePositionTracker;
        #endregion

        #region External Dependencies
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        private ObstaclesListenerManager ObstaclesListenerManager;
        #endregion

        public void Init()
        {
            #region External Dependencies
            this.ObstacleFrustumCalculationManager = PuzzleGameSingletonInstances.ObstacleFrustumCalculationManager;
            this.ObstaclesListenerManager = PuzzleGameSingletonInstances.ObstaclesListenerManager;
            #endregion

            this.nearSquareObstacles = new List<SquareObstacle>();
            this.ObstaclesListenerManager.OnObstacleListenerCreation(this);
            this.ObstacleFrustumCalculationManager.OnObstacleListenerCreation(this);
            this.ObstacleListenerChangePositionTracker = new BlittableTransformChangeListenerManager(true, false, null);
        }

        public void OnObstacleListenerDestroyed()
        {
            Debug.Log(MyLog.Format("OnObstacleListenerDestroyed"));
            this.ObstaclesListenerManager.OnObstacleListenerDestroyed(this);
            this.ObstacleFrustumCalculationManager.OnObstacleListenerDestroyed(this);
        }

        public void Tick(float d)
        {
            this.ObstacleListenerChangePositionTracker.Tick(this.transform.position, this.transform.rotation);
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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, this.Radius);
        }

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
            this.nearSquareObstacles.Sort((o1, o2) => { return Vector3.Distance(o1.transform.position, this.transform.position).CompareTo(Vector3.Distance(o2.transform.position, this.transform.position)); });
        }
    }

}
