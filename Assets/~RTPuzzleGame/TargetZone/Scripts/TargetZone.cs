using UnityEngine;

namespace RTPuzzle
{
    public class TargetZone : MonoBehaviour
    {
        public TargetZoneID TargetZoneID;

        #region Internal Dependencies
        private SphereCollider zoneDistanceDetectionCollider;
        private TargetZoneTriggerType targetZoneTriggerType;
        #endregion

        #region Data Retrieval
        public Collider ZoneDistanceDetectionCollider { get => zoneDistanceDetectionCollider; }
        public TargetZoneTriggerType TargetZoneTriggerType { get => targetZoneTriggerType; }

        public static TargetZone FromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<TargetZone>();
        }

        #endregion

        #region State
        private bool hasInit;
        #endregion

        public void Init()
        {
            if (!this.hasInit)
            {
                var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
                this.zoneDistanceDetectionCollider = GetComponent<SphereCollider>();
                this.targetZoneTriggerType = GetComponentInChildren<TargetZoneTriggerType>();
                targetZoneTriggerType.Init();
                targetZoneContainer.Add(this);

                var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
                this.zoneDistanceDetectionCollider.radius = gameConfiguration.TargetZonesConfiguration()[this.TargetZoneID].AIDistanceDetection;

                this.hasInit = true;
            }
        }

    }

    public enum TargetZoneID
    {
        LEVEL1_TARGET_ZONE = 0,
        TEST_TARGET_ZONE = 1
    }
}
