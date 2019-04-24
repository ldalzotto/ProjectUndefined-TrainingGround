﻿using OdinSerializer;
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
        public static InterfaceAIManager ForAllAIManagerTypes(Type managerType,
                 Func<AIRandomPatrolComponentMananger> AIRandomPatrolComponentManangerOperation,
                 Func<AIProjectileWithCollisionEscapeManager> AIProjectileEscapeWithCollisionManagerOperation,
                 Func<AIProjectileIgnorePhysicsEscapeManager> AIProjectileEscapeWithoutCollisionManagerOperation,
                 Func<AIFearStunManager> AIFearStunManagerOperation,
                 Func<AIAttractiveObjectManager> AIAttractiveObjectOperation,
                 Func<AITargetZoneManager> AITargetZoneManagerOperation)
        {
            var aiRandomPatrolComponentManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIRandomPatrolComponentMananger), AIRandomPatrolComponentManangerOperation);
            if (aiRandomPatrolComponentManager != null) { return aiRandomPatrolComponentManager; }
            var AIProjectileWithCollisionEscapeManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIProjectileWithCollisionEscapeManager), AIProjectileEscapeWithCollisionManagerOperation);
            if (AIProjectileWithCollisionEscapeManager != null) { return AIProjectileWithCollisionEscapeManager; }
            var AIProjectileIgnorePhysicsEscapeManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIProjectileIgnorePhysicsEscapeManager), AIProjectileEscapeWithoutCollisionManagerOperation);
            if (AIProjectileIgnorePhysicsEscapeManager != null) { return AIProjectileIgnorePhysicsEscapeManager; }
            var AIFearStunManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIFearStunManager), AIFearStunManagerOperation);
            if (AIFearStunManager != null) { return AIFearStunManager; }
            var AIAttractiveObjectManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AIAttractiveObjectManager), AIAttractiveObjectOperation);
            if (AIAttractiveObjectManager != null) { return AIAttractiveObjectManager; }
            var AITargetZoneManager = InvokeIfNotNullAndTypeCorresponds(managerType, typeof(AITargetZoneManager), AITargetZoneManagerOperation);
            if (AITargetZoneManager != null) { return AITargetZoneManager; }

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

}
