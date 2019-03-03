using UnityEngine;

namespace RTPuzzle
{
    public class TargetZone : MonoBehaviour
    {
        public TargetZoneID TargetZoneID;

        private Collider zoneCollider;

        public Collider ZoneCollider { get => zoneCollider; }

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private LevelManager LevelManager;
        #endregion

        void Start()
        {
            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            zoneCollider = GetComponent<Collider>();
            targetZoneContainer.Add(this);
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.gameObject.GetComponent<CollisionType>();
            var collidedAIManager = CollisionTypeHelper.GetAIManager(collisionType);
            if(collidedAIManager != null)
            {
                this.PuzzleEventsManager.OnLevelCompleted(this.LevelManager.GetCurrentLevel());
            }
        }

    }

    public enum TargetZoneID { LEVEL1_TARGET_ZONE = 0 }
}
