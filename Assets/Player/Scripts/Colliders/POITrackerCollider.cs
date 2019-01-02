using UnityEngine;

public class POITrackerCollider : MonoBehaviour
{

    private PlayerManager PlayerManager;

    private void Start()
    {
        this.PlayerManager = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.TriggerEnter(other, CollisionTag.POITracker);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerManager.TriggerExit(other, CollisionTag.POITracker);
    }

}
