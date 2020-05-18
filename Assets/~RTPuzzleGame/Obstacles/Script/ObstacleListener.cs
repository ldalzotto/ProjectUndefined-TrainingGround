﻿using System;
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
        private ObstaclesListenerManager ObstaclesListenerManager;
        #endregion

        public void Init()
        {
            #region External Dependencies
            this.ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();
            this.ObstaclesListenerManager = GameObject.FindObjectOfType<ObstaclesListenerManager>();
            #endregion

            this.nearSquereObstacles = new List<SquareObstacle>();
            this.ObstaclesListenerManager.OnObstacleListenerCreation(this);
            this.ObstacleFrustumCalculationManager.OnObstacleListenerCreation(this);
            this.ObstacleListenerChangePositionTracker = new ObstacleListenerChangePositionTracker(this);
        }

        public void OnObstacleListenerDestroyed()
        {
            this.ObstaclesListenerManager.OnObstacleListenerDestroyed(this);
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

        public void OnRangeTriggerEnter(Collider other)
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
        private ObstacleListener trackedObject;

        public ObstacleListenerChangePositionTracker(ObstacleListener trackedObject)
        {
            this.trackedObject = trackedObject;
            this.hasChanged = true;
        }

        private Vector3 lastFramePosition;
        private bool hasChanged;

        public bool HasChanged { get => hasChanged; }

        public void Tick(float d)
        {
            this.hasChanged = false;
            if (this.lastFramePosition != this.trackedObject.transform.position)
            {
                this.hasChanged = true;
            }
            this.lastFramePosition = this.trackedObject.transform.position;
        }
    }
}
