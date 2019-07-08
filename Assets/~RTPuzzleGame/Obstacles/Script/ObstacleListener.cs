using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstacleListener : MonoBehaviour
    {
        public float Radius;

        private List<SquareObstacle> nearSquereObstacles;
        public List<SquareObstacle> NearSquereObstacles { get => nearSquereObstacles; }

        #region Internal Managers
        private ObstacleListenerChangePositionTracker ObstacleListenerChangePositionTracker;
        #endregion

        #region External Dependencies
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        #endregion

        public void Init()
        {
            this.nearSquereObstacles = new List<SquareObstacle>();
            GameObject.FindObjectOfType<ObstaclesListenerManager>().OnObstacleListenerCreation(this);
            this.ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();
            this.ObstacleFrustumCalculationManager.OnObstacleListenerCreation(this);
            this.ObstacleListenerChangePositionTracker = new ObstacleListenerChangePositionTracker(this.transform);
        }

        public void Tick(float d)
        {
            this.ObstacleListenerChangePositionTracker.Tick(d);
        }

        #region Logical Conditions
        public bool HasPositionChanged()
        {
            return this.ObstacleListenerChangePositionTracker.HasChanged;
        }
        public bool IsListenerHaveObstaclesNearby()
        {
            return this.nearSquereObstacles.Count > 0;
        }
        public bool IsPointOccludedByObstacles(Vector3 worldPositionPoint)
        {
            return this.ObstacleFrustumCalculationManager.IsPointOccludedByObstacles(this, worldPositionPoint);
        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, this.Radius);
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                var squareObstacle = SquareObstacle.FromCollisionType(collisionType);
                if (squareObstacle != null)
                {
                    Debug.Log("ObstacleListener");
                    this.AddNearSquareObstacle(squareObstacle);
                    this.ObstacleFrustumCalculationManager.OnObstacleAddedToListener(this, squareObstacle);
                }
            }
        }

        private void OnTriggerExit(Collider other)
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
            this.nearSquereObstacles.Add(squareObstacle);
            this.SortObstaclesByDistance();
        }

        private void RemoveNearSquareObstacle(SquareObstacle squareObstacle)
        {
            this.nearSquereObstacles.Remove(squareObstacle);
            this.SortObstaclesByDistance();
        }

        //Nearest obstacles are sorted by distance in order to do intersection calculation from the nearest to the farest frustum.
        //Nearest frustum has more chance to oclcude a wider space than a farest
        private void SortObstaclesByDistance()
        {
            this.nearSquereObstacles.Sort((o1, o2) => { return Vector3.Distance(o1.transform.position, this.transform.position).CompareTo(Vector3.Distance(o2.transform.position, this.transform.position)); });
        }
    }

    class ObstacleListenerChangePositionTracker
    {
        private Transform objectTransform;

        public ObstacleListenerChangePositionTracker(Transform objectTransform)
        {
            this.objectTransform = objectTransform;
            this.hasChanged = true;
        }

        private Vector3 lastFramePosition;
        private bool hasChanged;

        public bool HasChanged { get => hasChanged; }

        public void Tick(float d)
        {
            this.hasChanged = false;
            if (this.lastFramePosition != this.objectTransform.position)
            {
                this.hasChanged = true;
            }
            this.lastFramePosition = this.objectTransform.position;
        }
    }
}
