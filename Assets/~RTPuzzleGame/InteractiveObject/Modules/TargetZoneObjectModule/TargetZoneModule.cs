using UnityEngine;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public class TargetZoneModule : InteractiveObjectModule
    {
        [CustomEnum]
        public TargetZoneID TargetZoneID;

        #region Internal Dependencies
        private SphereCollider zoneDistanceDetectionCollider;
        #endregion

        #region Module Dependencies
        private LevelCompletionTriggerModule levelCompletionTriggerModule;
        #endregion

        #region Data Retrieval
        public Collider ZoneDistanceDetectionCollider { get => zoneDistanceDetectionCollider; }
        public LevelCompletionTriggerModule LevelCompletionTriggerModule { get => levelCompletionTriggerModule; }

        public static TargetZoneModule FromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<TargetZoneModule>();
        }
        #endregion

        #region State
        private bool hasInit;
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            if (!this.hasInit)
            {
                TargetZoneInherentData TargetZoneInherentData = null;
                var gameConfiguration = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
                if (interactiveObjectInitializationObject.TargetZoneInherentData == null) { TargetZoneInherentData = gameConfiguration.TargetZonesConfiguration()[this.TargetZoneID]; }
                else { TargetZoneInherentData = interactiveObjectInitializationObject.TargetZoneInherentData; }

                this.levelCompletionTriggerModule = IInteractiveObjectTypeDataRetrieval.GetLevelCompletionTriggerModule();
                this.ResolveModuleDependencies();
                this.zoneDistanceDetectionCollider.radius = TargetZoneInherentData.AIDistanceDetection;

                this.hasInit = true;
            }
        }

        private void ResolveModuleDependencies()
        {
            this.zoneDistanceDetectionCollider = GetComponent<SphereCollider>();
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

        public static class TargetZoneModuleInstancer
        {
            public static void PopulateFromDefinition(TargetZoneModule TargetZoneModule, TargetZoneModuleDefinition targetZoneModuleDefinition)
            {
                TargetZoneModule.TargetZoneID = targetZoneModuleDefinition.TargetZoneID;
            }

        }

    }


}
