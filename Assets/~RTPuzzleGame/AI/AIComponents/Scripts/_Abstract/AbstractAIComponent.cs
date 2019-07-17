using OdinSerializer;
using System;
using UnityEngine;

namespace RTPuzzle
{
    /// <summary>
    /// <see cref="AbstractAIComponent"/> is the definition of <see cref="InterfaceAIManager"/> implementation type and configuration data associated to the <see cref="InterfaceAIManager"/>.
    /// It is used by <see cref="PuzzleAIBehavior"/> to intialize the <see cref="AIBehaviorManagerContainer"/>.
    /// </summary>
    [System.Serializable]
    public abstract class AbstractAIComponent : SerializedScriptableObject
    {
        [SerializeField]
        protected abstract Type abstractManagerType { get; }

        [SerializeField]
        public Type SelectedManagerType;
        public Type AbstractManagerType { get => abstractManagerType; }
    }

    public class AIManagerTypeSafeOperation
    {
        public static InterfaceAIManager ForAllAIManagerTypes(Type managerType,
                 Func<AIRandomPatrolComponentMananger> AIRandomPatrolComponentManangerOperation,
                 Func<AIProjectileWithCollisionEscapeManager> AIProjectileEscapeWithCollisionManagerOperation,
                 Func<AIEscapeWithoutTriggerManager> AIEscapeWithoutTriggerManagerOperation,
                 Func<AIFearStunManager> AIFearStunManagerOperation,
                 Func<AIAttractiveObjectPersistantManager> AIAttractiveObjectPersistantOperation,
                 Func<AIAttractiveObjectLooseManager> AIAttractiveObjectLooseOperation,
                 Func<AITargetZoneEscapeManager> AITargetZoneManagerOperation,
                 Func<AIPlayerEscapeManager> AIPlayerEscapeManagerOperation)
        {
            var aiRandomPatrolComponentManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIRandomPatrolComponentMananger), AIRandomPatrolComponentManangerOperation);
            if (aiRandomPatrolComponentManager != null) { return aiRandomPatrolComponentManager; }
            var AIProjectileWithCollisionEscapeManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIProjectileWithCollisionEscapeManager), AIProjectileEscapeWithCollisionManagerOperation);
            if (AIProjectileWithCollisionEscapeManager != null) { return AIProjectileWithCollisionEscapeManager; }
            var AIEscapeWithoutTriggerManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIEscapeWithoutTriggerManager), AIEscapeWithoutTriggerManagerOperation);
            if (AIEscapeWithoutTriggerManager != null) { return AIEscapeWithoutTriggerManager; }
            var AIFearStunManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIFearStunManager), AIFearStunManagerOperation);
            if (AIFearStunManager != null) { return AIFearStunManager; }
            var AIAttractiveObjectPersistantManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIAttractiveObjectPersistantManager), AIAttractiveObjectPersistantOperation);
            if (AIAttractiveObjectPersistantManager != null) { return AIAttractiveObjectPersistantManager; }
            var AIAttractiveObjectLooseOperationManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIAttractiveObjectLooseManager), AIAttractiveObjectLooseOperation);
            if (AIAttractiveObjectLooseOperationManager != null) { return AIAttractiveObjectLooseOperationManager; }
            var AITargetZoneManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AITargetZoneEscapeManager), AITargetZoneManagerOperation);
            if (AITargetZoneManager != null) { return AITargetZoneManager; }
            var AIPlayerEscapeManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIPlayerEscapeManager), AIPlayerEscapeManagerOperation);
            if (AIPlayerEscapeManager != null) { return AIPlayerEscapeManager; }

            return null;
        }

        private static InterfaceAIManager InvokeIfNotNullAndTypeCorresponds(Type managerType, Type comparedType, Func<InterfaceAIManager> action)
        {
            if (managerType == comparedType)
            {
                if (action != null)
                {
                    return action.Invoke();
                }
            }
            return null;
        }
    }

    public static class AbstractAIComponentExtensions
    {
        public static void IfSelectedTypeDefined<T>(this T AbstractAIComponent, Action<Type> actionToExecute) where T : AbstractAIComponent
        {
            if (AbstractAIComponent != null && AbstractAIComponent.SelectedManagerType != null)
            {
                actionToExecute.Invoke(AbstractAIComponent.SelectedManagerType);
            }
        }
    }

}
