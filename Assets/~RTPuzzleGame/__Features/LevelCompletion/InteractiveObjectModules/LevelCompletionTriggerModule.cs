using UnityEngine;

namespace RTPuzzle
{
    public interface ILevelCompletionTriggerModuleDataRetriever
    {
        Collider GetTargetZoneTriggerCollider();
        RangeObjectV2 LevelCompletionRange { get; }
    }

    public class LevelCompletionTriggerModule : InteractiveObjectModule, ILevelCompletionTriggerModuleDataRetriever
    {
        #region External dependencies
        public ILevelCompletionManagerEvent ILevelCompletionManagerEvent { get; private set; }
        #endregion

        public RangeObjectV2 LevelCompletionRange { get; set; }

        public Collider GetTargetZoneTriggerCollider()
        {
            return this.LevelCompletionRange.RangeGameObjectV2.BoundingCollider;
        }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval, IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ResolveModuleDependencies();
            this.LevelCompletionRange.ReceiveEvent(new RangeExternalPhysicsOnlyAddListener { ARangeObjectV2PhysicsEventListener = new LevelCompletionTriggerModuleCompletionEventTrigger(this) });
        }

        public void ResolveModuleDependencies()
        {
            this.ILevelCompletionManagerEvent = PuzzleGameSingletonInstances.LevelCompletionManager;
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.LevelCompletionRange.OnDestroy();
        }
    }

    public class LevelCompletionTriggerModuleCompletionEventTrigger : ARangeObjectV2PhysicsEventListener
    {
        private LevelCompletionTriggerModule LevelCompletionTriggerModuleRef;

        public LevelCompletionTriggerModuleCompletionEventTrigger(LevelCompletionTriggerModule levelCompletionTriggerModuleRef)
        {
            LevelCompletionTriggerModuleRef = levelCompletionTriggerModuleRef;
        }

        public void OnRangeTriggerEnter(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            AskForLevelCompletionCalculation(RangeObjectPhysicsTriggerInfo.OtherCollisionType);
        }
        public void OnRangeTriggerExit(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            AskForLevelCompletionCalculation(RangeObjectPhysicsTriggerInfo.OtherCollisionType);
        }

        private void AskForLevelCompletionCalculation(CollisionType CollisionType)
        {
            if (CollisionType != null && (CollisionType.IsAI || CollisionType.IsPlayer))
            {
                this.LevelCompletionTriggerModuleRef.ILevelCompletionManagerEvent.ConditionRecalculationEvaluate();
            }
        }

    }
}
