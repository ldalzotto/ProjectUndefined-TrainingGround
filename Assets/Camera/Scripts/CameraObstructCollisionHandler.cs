using UnityEngine;

public class CameraObstructCollisionHandler : MonoBehaviour
{
    private CameraObstructManager CameraObstructManager;

    // Use this for initialization
    void Start()
    {
        CameraObstructManager = GameObject.FindObjectOfType<CameraObstructManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var collisionType = other.GetComponent<CollisionType>();
        if (collisionType != null)
        {
            CameraObstructManager.TriggerEnter(other, collisionType);
        }
        else
        {
            Debug.LogError("The collider : " + other.name + " has no CollisionType.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var collisionType = other.GetComponent<CollisionType>();
        if (collisionType != null)
        {
            CameraObstructManager.TriggerExit(other, collisionType);
        }
        else
        {
            Debug.LogError("The collider : " + other.name + " has no CollisionType.");
        }
    }
}
