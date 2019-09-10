using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class LevelCompletionTriggerModule : InteractiveObjectModule, RangeTypeObjectEventListener
    {
        #region External dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private RangeTypeObject RangeTypeObject;

        public Collider GetTargetZoneTriggerCollider()
        {
            return this.RangeTypeObject.RangeType.GetCollider();
        }

        public void Init()
        {
            this.ResolveModuleDependencies();
            this.RangeTypeObject.Init(null, eventListenersFromExterior: new List<RangeTypeObjectEventListener>() { this });
        }

        private void ResolveModuleDependencies()
        {
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            if (this.RangeTypeObject == null)
            {
                this.RangeTypeObject = GetComponentInChildren<RangeTypeObject>();
            }
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.RangeTypeObject.OnRangeDestroyed();
        }

        public void OnRangeTriggerEnter(Collider other)
        {
            Debug.Log(MyLog.Format("LevelCompletionTriggerModule OnTriggerEnter"));
            AskForLevelCompletionCalculation(other);
        }

        public void OnRangeTriggerExit(Collider other)
        {
            AskForLevelCompletionCalculation(other);
        }

        private void AskForLevelCompletionCalculation(Collider other)
        {
            var CollisionType = other.GetComponent<CollisionType>();
            if (CollisionType != null && (CollisionType.IsAI || CollisionType.IsPlayer))
            {
                this.PuzzleEventsManager.PZ_EVT_LevelCompletion_ConditionRecalculationEvaluate();
            }
        }

        public static class LevelCompletionTriggerModuleInstancer
        {
            public static void PopuplateFromDefinition(
                                LevelCompletionTriggerModule LevelCompletionTriggerModule,
                                LevelCompletionTriggerModuleDefinition LevelCompletionTriggerModuleDefinition,
                                PuzzlePrefabConfiguration puzzlePrefabConfiguration)
            {
                LevelCompletionTriggerModule.ResolveModuleDependencies();
                if (LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionIDPicker)
                {
                    LevelCompletionTriggerModule.RangeTypeObject.RangeTypeObjectDefinitionID = LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionID;
                }
                else
                {
                    LevelCompletionTriggerModule.RangeTypeObject.RangeTypeObjectDefinitionID = RangeTypeObjectDefinitionID.NONE;
                    LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionInherentData.DefineRangeTypeObject(LevelCompletionTriggerModule.RangeTypeObject, puzzlePrefabConfiguration);
                }

            }
        }

    }

}
