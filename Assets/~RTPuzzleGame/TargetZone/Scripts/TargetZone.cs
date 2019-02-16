using UnityEngine;

namespace RTPuzzle
{
    public class TargetZone : MonoBehaviour
    {
        public TargetZoneID TargetZoneID;

        private Collider zoneCollider;

        public Collider ZoneCollider { get => zoneCollider; }

        void Start()
        {
            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            zoneCollider = GetComponent<Collider>();
            targetZoneContainer.Add(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
        }

    }

    public enum TargetZoneID { LEVEL1_TARGET_ZONE = 0 }
}
