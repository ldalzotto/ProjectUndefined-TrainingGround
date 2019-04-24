using System;
using UnityEngine;
using UnityEngine.AI;

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

    public abstract class AbstractAIProjectileEscapeManager : InterfaceAIManager
    {
        protected AiID aiID;
        #region External Dependencies
        protected NavMeshAgent escapingAgent;
        protected AIFOVManager AIFOVManager;
        #endregion

        #region Internal Dependencies
        protected AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region Internal Managers
        protected EscapeDestinationManager escapeDestinationManager;
        #endregion

        #region Escape Destination calculation
        protected abstract Action<NavMeshRaycastStrategy> OnTriggerEnterDestinationCalculation { get; }
        protected abstract Action<NavMeshRaycastStrategy> DestinationCalulationMethod { get; }
        #endregion

        protected AbstractAIProjectileEscapeManager(NavMeshAgent escapingAgent, AIFOVManager aIFOVManager, AiID aiID, AIProjectileEscapeComponent AIProjectileEscapeComponent)
        {
            this.escapingAgent = escapingAgent;
            this.AIFOVManager = aIFOVManager;
            this.aiID = aiID;
            this.escapeDestinationManager = new EscapeDestinationManager(this.escapingAgent);
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
        }

        #region Internal Events
        protected void OnDestinationSetFromProjectileContact()
        {
            this.escapeDestinationManager.ResetDistanceComputation(this.AIProjectileEscapeComponent.EscapeDistance);
        }
        #endregion

        #region State
        protected bool isEscapingFromProjectile;
        #endregion

        #region Logical Conditions
        public bool IsManagerEnabled()
        {
            return isEscapingFromProjectile;
        }
        #endregion

        public Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            this.escapeDestinationManager.Tick();
            return escapeDestinationManager.EscapeDestination;
        }

        protected void IntersectFOV(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
        }

        protected virtual void SetIsEscapingFromProjectile(bool value)
        {
            this.isEscapingFromProjectile = value;
        }

        protected void ResetAIProjectileEscapeManagerState()
        {
            this.OnStateReset();
        }

        public virtual void OnTriggerEnter(Vector3 impactPoint, ProjectileInherentData launchProjectileInherentData)
        {
            var localEscapeDirection = (escapingAgent.transform.position - impactPoint).normalized;
            if (launchProjectileInherentData != null)
            {
                this.OnDestinationSetFromProjectileContact();
                this.IntersectFOV(localEscapeDirection, launchProjectileInherentData.EscapeSemiAngle);
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.OnTriggerEnterDestinationCalculation, null);
            }
            this.SetIsEscapingFromProjectile(true);
        }

        public virtual void OnDestinationReached()
        {
            this.escapeDestinationManager.OnAgentDestinationReached();
            if (this.escapeDestinationManager.IsDistanceReached())
            {
                //if travelled escape distance is reached, we reset
                this.ResetAIProjectileEscapeManagerState();
            }
            else
            {
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.DestinationCalulationMethod, this.ResetAIProjectileEscapeManagerState);
            }
        }
        public virtual void OnStateReset()
        {
            this.SetIsEscapingFromProjectile(false);
        }
        public abstract void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile);
        public virtual void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            this.SetIsEscapingFromProjectile(false);
        }
        public virtual void GizmoTick()
        {
            this.escapeDestinationManager.GizmoTick();
        }

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }
        
    }

}
