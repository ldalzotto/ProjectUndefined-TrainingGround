using UnityEngine;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.blue;
            Handles.Label(transform.position + new Vector3(0, 3f, 0), TargetZoneID.ToString(), labelStyle);
            Gizmos.DrawIcon(transform.position + new Vector3(0, 5.5f, 0), "Gizmo_TargetZone", true);
#endif
        }

    }


}
