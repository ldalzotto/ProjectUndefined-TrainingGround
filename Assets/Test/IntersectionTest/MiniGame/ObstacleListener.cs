using System.Collections.Generic;
using UnityEngine;

public class ObstacleListener : MonoBehaviour
{
    public float Radius;

    private List<SquareObstacle> nearSquereObstacles;
    public List<SquareObstacle> NearSquereObstacles { get => nearSquereObstacles; }

    #region Internal Managers
    private ObstacleListenerChangePositionTracker ObstacleListenerChangePositionTracker;
    #endregion

    private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;

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
                this.nearSquereObstacles.Add(squareObstacle);
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
                this.nearSquereObstacles.Remove(squareObstacle);
                this.ObstacleFrustumCalculationManager.OnObstacleRemovedToListener(this, squareObstacle);
            }
        }
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