using CoreGame;
using GameConfigurationID;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIProjectileEscapeComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIProjectileEscapeComponent", order = 1)]
    public class AIProjectileEscapeComponent : AbstractAIComponent
    {
        [Inline(createSubIfAbsent: true, FileName = "EscapeDistance")]
        public ProjectileEscapeRange EscapeDistanceV2;

        [Inline(createSubIfAbsent: true, FileName = "EscapeSemiAngle")]
        public ProjectileEscapeSemiAngle EscapeSemiAngleV2;
    }

    public abstract class AbstractAIProjectileEscapeManager : AbstractAIManager, InterfaceAIManager
    {
        protected AiID aiID;
        #region External Dependencies
        protected NavMeshAgent escapingAgent;
        protected AIFOVManager AIFOVManager;
        protected PuzzleEventsManager puzzleEventsManager;
        private TransformMoveManagerComponentV3 AIDestimationMoveManagerComponent;
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

        protected void BaseInit(NavMeshAgent escapingAgent, AIFOVManager aIFOVManager, AiID aiID,
            AIProjectileEscapeComponent AIProjectileEscapeComponent, PuzzleEventsManager puzzleEventsManager, TransformMoveManagerComponentV3 AIDestimationMoveManagerComponent)
        {
            this.escapingAgent = escapingAgent;
            this.AIFOVManager = aIFOVManager;
            this.aiID = aiID;
            this.escapeDestinationManager = new EscapeDestinationManager(this.escapingAgent);
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.puzzleEventsManager = puzzleEventsManager;
            this.AIDestimationMoveManagerComponent = AIDestimationMoveManagerComponent;
        }

        #region Internal Events
        protected void OnDestinationSetFromProjectileContact(LaunchProjectileID launchProjectileId)
        {
            this.escapeDestinationManager.ResetDistanceComputation(this.AIProjectileEscapeComponent.EscapeDistanceV2.Values[launchProjectileId]);
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

        #region Data Retrieval
        public float GetMaxEscapeDistance(LaunchProjectileID launchProjectileId)
        {
            return this.AIProjectileEscapeComponent.EscapeDistanceV2.Values[launchProjectileId];
        }
        public float GetSemiAngle(LaunchProjectileID launchProjectileId)
        {
            return this.AIProjectileEscapeComponent.EscapeSemiAngleV2.Values[launchProjectileId];
        }
        #endregion

        public void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            NPCAIDestinationContext.TargetPosition = this.escapeDestinationManager.Tick();
        }

        protected virtual void SetIsEscapingFromProjectile(bool value)
        {
            this.isEscapingFromProjectile = value;
        }

        protected void ResetAIProjectileEscapeManagerState()
        {
            this.OnStateReset();
        }

        public virtual void ComponentTriggerEnter(Vector3 impactPoint, ProjectileTriggerEnterAIBehaviorEvent projectileTriggerEnterAIBehaviorEvent)
        {
            if (impactPoint != null)
            {
                this.OnDestinationSetFromProjectileContact(projectileTriggerEnterAIBehaviorEvent.LaunchProjectileId);
                this.AIFOVManager.IntersectFOV_FromEscapeDirection(impactPoint, escapingAgent.transform.position, this.AIProjectileEscapeComponent.EscapeSemiAngleV2.Values[projectileTriggerEnterAIBehaviorEvent.LaunchProjectileId]);
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.OnTriggerEnterDestinationCalculation,
                        EscapeDestinationManager.OnDestinationCalculationFailed_ForceAIFear(this.puzzleEventsManager, this.aiID, EscapeDestinationManager.ForcedFearRemainingDistanceToFearTime(this.escapeDestinationManager, this.AIDestimationMoveManagerComponent)));
            }
            this.SetIsEscapingFromProjectile(true);
        }


        public virtual void OnDestinationReached()
        {
            if (this.escapeDestinationManager.OnAgentDestinationReached())
            {
                //if travelled escape distance is reached, we reset
                this.ResetAIProjectileEscapeManagerState();
            }
            else
            {
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.DestinationCalulationMethod, EscapeDestinationManager.OnDestinationCalculationFailed_ForceAIFear(this.puzzleEventsManager, this.aiID,
                   EscapeDestinationManager.ForcedFearRemainingDistanceToFearTime(this.escapeDestinationManager, this.AIDestimationMoveManagerComponent)));
            }
        }
        public virtual void OnStateReset()
        {
            this.SetIsEscapingFromProjectile(false);
            this.escapeDestinationManager.OnStateReset();
        }
        public abstract void OnLaunchProjectileDestroyed(LaunchProjectileModule launchProjectile);
        public virtual void ComponentTriggerExit(Collider collider, CollisionType collisionType)
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
