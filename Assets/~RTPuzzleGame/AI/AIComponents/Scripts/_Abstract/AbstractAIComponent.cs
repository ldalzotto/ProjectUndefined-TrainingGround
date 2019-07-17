using OdinSerializer;
using System;

namespace RTPuzzle
{
    /// <summary>
    /// <see cref="AbstractAIComponent"/> is the definition of <see cref="InterfaceAIManager"/> implementation type and configuration data associated to the <see cref="InterfaceAIManager"/>.
    /// It is used by <see cref="PuzzleAIBehavior"/> to intialize the <see cref="AIBehaviorManagerContainer"/>.
    /// </summary>
    [System.Serializable]
    public abstract class AbstractAIComponent : SerializedScriptableObject
    {
    }

    public class AIManagerTypeSafeOperation
    {
        public static void ForAllAIManagerTypes(Type managerType,
                 Action AIRandomPatrolComponentManangerOperation,
                 Action AIScriptedPatrolComponentManagerOperation,
                 Action AIProjectileEscapeWithCollisionManagerOperation,
                 Action AIEscapeWithoutTriggerManagerOperation,
                 Action AIFearStunManagerOperation,
                 Action AIAttractiveObjectPersistantOperation,
                 Action AIAttractiveObjectLooseOperation,
                 Action AITargetZoneManagerOperation,
                 Action AIPlayerEscapeManagerOperation)
        {
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIRandomPatrolComponentMananger), AIRandomPatrolComponentManangerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIScriptedPatrolComponentManager), AIScriptedPatrolComponentManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIProjectileWithCollisionEscapeManager), AIProjectileEscapeWithCollisionManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIEscapeWithoutTriggerManager), AIEscapeWithoutTriggerManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIFearStunManager), AIFearStunManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIAttractiveObjectPersistantManager), AIAttractiveObjectPersistantOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIAttractiveObjectLooseManager), AIAttractiveObjectLooseOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AITargetZoneEscapeManager), AITargetZoneManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIPlayerEscapeManager), AIPlayerEscapeManagerOperation);
        }

        private static void InvokeIfNotNullAndTypeCorresponds(Type managerType, Type comparedType, Action action)
        {
            if (managerType == comparedType)
            {
                if (action != null)
                {
                    action.Invoke();
                }
            }
        }
    }

}
