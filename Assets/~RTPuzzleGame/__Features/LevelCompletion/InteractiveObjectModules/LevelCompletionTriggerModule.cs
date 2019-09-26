using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface ILevelCompletionTriggerModuleDataRetriever
    {
        Collider GetTargetZoneTriggerCollider();
        RangeTypeObject RangeTypeObject { get; }
    }

    public class LevelCompletionTriggerModule : InteractiveObjectModule, ILevelCompletionTriggerModuleDataRetriever, RangeTypeObjectEventListener
    {
        #region External dependencies
        private ILevelCompletionManagerEvent ILevelCompletionManagerEvent;
        #endregion

        public RangeTypeObject RangeTypeObject { get; private set; }

        public Collider GetTargetZoneTriggerCollider()
        {
            return this.RangeTypeObject.RangeType.GetCollider();
        }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval, IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ResolveModuleDependencies();
            this.RangeTypeObject.Init(null, eventListenersFromExterior: new List<RangeTypeObjectEventListener>() { this });
        }

        public void ResolveModuleDependencies()
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

        public void OnRangeTriggerEnter(CollisionType other)
        {
            Debug.Log(MyLog.Format("LevelCompletionTriggerModule OnTriggerEnter"));
            AskForLevelCompletionCalculation(other);
        }
        public void OnRangeTriggerStay(CollisionType other) { }
        public void OnRangeTriggerExit(CollisionType other)
        {
            AskForLevelCompletionCalculation(other);
        }

        private void AskForLevelCompletionCalculation(CollisionType CollisionType)
        {
            if (CollisionType != null && (CollisionType.IsAI || CollisionType.IsPlayer))
            {
                this.ILevelCompletionManagerEvent.ConditionRecalculationEvaluate();
            }
        }
        

    }

}
