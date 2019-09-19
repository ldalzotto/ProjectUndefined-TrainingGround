using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface ILevelCompletionTriggerModuleDataRetriever
    {
        Collider GetTargetZoneTriggerCollider();
    }

    public class LevelCompletionTriggerModule : InteractiveObjectModule, ILevelCompletionTriggerModuleDataRetriever, RangeTypeObjectEventListener
    {
        #region External dependencies
        private ILevelCompletionManagerEvent ILevelCompletionManagerEvent;
        #endregion

        private RangeTypeObject RangeTypeObject;

        public Collider GetTargetZoneTriggerCollider()
        {
            return this.RangeTypeObject.RangeType.GetCollider();
        }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval, IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ResolveModuleDependencies();
            this.RangeTypeObject.Init(null, eventListenersFromExterior: new List<RangeTypeObjectEventListener>() { this });
        }

        private void ResolveModuleDependencies()
        {
            this.ILevelCompletionManagerEvent = PuzzleGameSingletonInstances.LevelCompletionManager;
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
                this.ILevelCompletionManagerEvent.ConditionRecalculationEvaluate();
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
