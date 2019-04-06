using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIProjectileEscapeComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIProjectileEscapeComponent", order = 1)]
    public class AIProjectileEscapeComponent : AbstractAIComponent
    {
        public float EscapeDistance;
        protected override Type abstractManagerType => typeof(AbstractAIProjectileEscapeManager);
    }

    public abstract class AbstractAIProjectileEscapeManager
    {
        #region State
        protected bool isEscapingFromProjectile;
        protected bool isEscapingToFarest;
        #endregion

        #region Logical Conditions
        public bool IsEscaping()
        {
            return isEscapingFromProjectile;
        }

        public bool IsEscapingToFarest()
        {
            return isEscapingToFarest;
        }
        #endregion

        
        public abstract Nullable<Vector3> TickComponent();
        public abstract void OnTriggerEnter(Collider collider, CollisionType collisionType);
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();
        public abstract void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile);
        public abstract void OnTriggerExit(Collider collider, CollisionType collisionType);
        public abstract void GizmoTick();
    }

}
