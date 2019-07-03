using System.Collections.Generic;
using UnityEngine;

public class Test_Sphere : MonoBehaviour
{
    public float Radius;

    private List<SquareObstacle> nearSquereObstacles;

    public List<SquareObstacle> NearSquereObstacles { get => nearSquereObstacles; }

    public void Init()
    {
        this.nearSquereObstacles = new List<SquareObstacle>();
        GameObject.FindObjectOfType<GroundObstacleRendererManager>().OntestSphereCreation(this);
    }

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
            }
        }
    }
}
