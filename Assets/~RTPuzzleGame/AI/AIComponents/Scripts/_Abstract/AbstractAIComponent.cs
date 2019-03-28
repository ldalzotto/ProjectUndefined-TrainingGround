using OdinSerializer;
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
    
    public struct AIManagerTypeSafeOperation
    {
        public Func<AIRandomPatrolComponentMananger> AIRandomPatrolComponentManangerOperation;
        public Func<AIProjectileEscapeManager> AIProjectileEscapeManagerOperation;
        public Func<AIFearStunManager> AIFearStunManagerOperation;
        public Func<AIAttractiveObjectManager> AIAttractiveObjectOperation;
        public Func<AITargetZoneManager> AITargetZoneManagerOperation;

        public void ForAllAIManagerTypes(Type managerType)
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
