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
        private TransformChangeListenerManager ObstacleListenerChangePositionTracker;
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
            this.ObstacleListenerChangePositionTracker = new TransformChangeListenerManager(this.transform, true, false, null);
        }

        public void OnObstacleListenerDestroyed()
        {
            Debug.Log(MyLog.Format("OnObstacleListenerDestroyed"));
            this.ObstaclesListenerManager.OnObstacleListenerDestroyed(this);
            this.ObstacleFrustumCalculationManager.OnObstacleListenerDestroyed(this);
        }

        public void Tick(float d)
        {
            this.ObstacleListenerChangePositionTracker.Tick();
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
        public bool IsPointOccludedByObstacles(Vector3 worldPositionPoint)
        {
            return this.ObstacleFrustumCalculationManager.IsPointOccludedByObstacles(this, worldPositionPoint);
        }
        internal bool IsBoxOccludedByObstacles(BoxCollider boxCollider)
        {
            return this.ObstacleFrustumCalculationManager.IsPointOccludedByObstacles(this, boxCollider);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, this.Radius);
        }

        public void OnRangeTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                var squareObstacle = SquareObstacle.FromCollisionType(collisionType);
                if (squareObstacle != null)
                {
                    this.AddNearSquareObstacle(squareObstacle);
                    this.ObstacleFrustumCalculationManager.OnObstacleAddedToListener(this, squareObstacle);
                }
            }
        }

        public void OnRangeTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                var squareObstacle = SquareObstacle.FromCollisionType(collisionType);
                if (squareObstacle != null)
                {
                    this.RemoveNearSquareObstacle(squareObstacle);
                    this.ObstacleFrustumCalculationManager.OnObstacleRemovedToListener(this, squareObstacle);
                }
            }
        }

        private void AddNearSquareObstacle(SquareObstacle squareObstacle)
        {
            this.nearSquareObstacles.Add(squareObstacle);
            this.SortObstaclesByDistance();
        }

        private void RemoveNearSquareObstacle(SquareObstacle squareObstacle)
        {
            this.nearSquareObstacles.Remove(squareObstacle);
            this.SortObstaclesByDistance();
        }

        //Nearest obstacles are sorted by distance in order to do intersection calculation from the nearest to the farest frustum.
        //Nearest frustum has more chance to oclcude a wider space than a farest
        private void SortObstaclesByDistance()
        {
            this.nearSquareObstacles.Sort((o1, o2) => { return Vector3.Distance(o1.transform.position, this.transform.position).CompareTo(Vector3.Distance(o2.transform.position, this.transform.position)); });
        }
    }

}
