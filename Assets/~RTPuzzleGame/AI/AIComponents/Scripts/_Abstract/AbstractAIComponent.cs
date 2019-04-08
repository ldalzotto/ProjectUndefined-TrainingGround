using OdinSerializer;
using System;
using UnityEngine;

namespace RTPuzzle
{
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
        public static void ForAllAIManagerTypes(Type managerType,
                 Func<AIRandomPatrolComponentMananger> AIRandomPatrolComponentManangerOperation,
                 Func<AIProjectileWithCollisionEscapeManager> AIProjectileEscapeWithCollisionManagerOperation,
                 Func<AIProjectileIgnorePhysicsEscapeManager> AIProjectileEscapeWithoutCollisionManagerOperation,
                 Func<AIFearStunManager> AIFearStunManagerOperation,
                 Func<AIAttractiveObjectManager> AIAttractiveObjectOperation,
                 Func<AITargetZoneManager> AITargetZoneManagerOperation)
        {
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIRandomPatrolComponentMananger), AIRandomPatrolComponentManangerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIProjectileWithCollisionEscapeManager), AIProjectileEscapeWithCollisionManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIProjectileIgnorePhysicsEscapeManager), AIProjectileEscapeWithoutCollisionManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIFearStunManager), AIFearStunManagerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIAttractiveObjectManager), AIAttractiveObjectOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AITargetZoneManager), AITargetZoneManagerOperation);
        }

        private static bool InvokeIfNotNullAndTypeCorresponds(Type managerType, Type comparedType, Func<object> action)
        {
            if (managerType == comparedType)
            {
                if (action != null)
                {
                    action.Invoke();
                }
                return true;
            }
            return false;
        }
    }

}
