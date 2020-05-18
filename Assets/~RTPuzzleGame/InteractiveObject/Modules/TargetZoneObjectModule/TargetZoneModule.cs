﻿using UnityEngine;
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

        public void Init(LevelCompletionTriggerModule levelCompletionTriggerModule)
        {
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.Init(levelCompletionTriggerModule, gameConfiguration.TargetZonesConfiguration()[this.TargetZoneID]);
        }

        public void Init(LevelCompletionTriggerModule levelCompletionTriggerModule, TargetZoneInherentData targetZoneInherentData)
        {
            if (!this.hasInit)
            {
                this.levelCompletionTriggerModule = levelCompletionTriggerModule;
                this.ResolveModuleDependencies();
                this.zoneDistanceDetectionCollider.radius = targetZoneInherentData.AIDistanceDetection;

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
