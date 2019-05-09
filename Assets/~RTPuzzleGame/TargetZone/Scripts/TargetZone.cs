using UnityEngine;

namespace RTPuzzle
{
    public class TargetZone : MonoBehaviour
    {
        [SearchableEnum]
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
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.Init(gameConfiguration.TargetZonesConfiguration()[this.TargetZoneID]);
        }

        public void Init(TargetZoneInherentData targetZoneInherentData)
        {
            if (!this.hasInit)
            {
                var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
                this.zoneDistanceDetectionCollider = GetComponent<SphereCollider>();
                this.targetZoneTriggerType = GetComponentInChildren<TargetZoneTriggerType>();
                targetZoneTriggerType.Init();
                targetZoneContainer.Add(this);

                this.zoneDistanceDetectionCollider.radius = targetZoneInherentData.AIDistanceDetection;

                this.hasInit = true;
            }
        }

        public static TargetZone Instanciate(TargetZoneInherentData targetZoneInherentData, Vector3 worldPosition)
        {
            var targetZone = MonoBehaviour.Instantiate(PrefabContainer.Instance.TargetZonePrefab);
            targetZone.Init(targetZoneInherentData);
            targetZone.transform.position = worldPosition;
            return targetZone;
        }

    }

    public enum TargetZoneID
    {
        LEVEL1_TARGET_ZONE = 0,
        TEST_TARGET_ZONE = 1
    }
}
