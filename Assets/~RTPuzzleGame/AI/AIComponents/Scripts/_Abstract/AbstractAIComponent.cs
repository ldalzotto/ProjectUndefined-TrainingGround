﻿using OdinSerializer;
using System;


namespace RTPuzzle
{
    [System.Serializable]
    public abstract class AbstractAIComponent : SerializedScriptableObject
    {
        protected abstract Type abstractManagerType { get; }
        public Type SelectedManagerType;
        public Type AbstractManagerType { get => abstractManagerType; }
    }

    public class AIManagerTypeSafeOperation
    {
        public static void ForAllAIManagerTypes(Type managerType,
                 Func<AIRandomPatrolComponentMananger> AIRandomPatrolComponentManangerOperation,
                 Func<AIProjectileEscapeManager> AIProjectileEscapeManagerOperation,
                 Func<AIFearStunManager> AIFearStunManagerOperation,
                 Func<AIAttractiveObjectManager> AIAttractiveObjectOperation,
                 Func<AITargetZoneManager> AITargetZoneManagerOperation)
        {
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIRandomPatrolComponentMananger), AIRandomPatrolComponentManangerOperation);
            InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIProjectileEscapeManager), AIProjectileEscapeManagerOperation);
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
