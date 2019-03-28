using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
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
        protected bool isEscapingFromTargetZone;
        protected bool isEscapingFromProjectile;
        protected bool isInTargetZone;
        protected Nullable<Vector3> escapeDestination;
        #endregion

        #region Logical Conditions
        public bool IsEscaping()
        {
            return isEscapingFromTargetZone || isEscapingFromProjectile;
        }
        #endregion


        public abstract void ClearEscapeDestination();
        public abstract Nullable<Vector3> ForceComputeEscapePoint();
        public abstract Nullable<Vector3> GetCurrentEscapeDirection();
        public abstract void OnTriggerEnter(Collider collider, CollisionType collisionType);
        public abstract void OnDestinationReached();
        public abstract void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile);
        public abstract void OnTriggerExit(Collider collider, CollisionType collisionType);
        public abstract void GizmoTick();
    }

}
