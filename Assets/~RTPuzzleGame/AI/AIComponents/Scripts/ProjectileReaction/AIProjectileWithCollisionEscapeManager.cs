using CoreGame;
using GameConfigurationID;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIProjectileWithCollisionEscapeManager : AbstractAIProjectileEscapeManager
    {

        #region External Dependencies
        private Func<Collider[]> targetZoneTriggerColliderProvider;
        #endregion

        public void Init(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AIFOVManager AIFOVManager, PuzzleEventsManager PuzzleEventsManager, AiID aiID, Func<Collider[]> targetZoneTriggerColliderProvider,
                TransformMoveManagerComponentV3 AIDestimationMoveManagerComponent)
        {
            this.BaseInit(escapingAgent, AIFOVManager, aiID, AIProjectileEscapeComponent, PuzzleEventsManager, AIDestimationMoveManagerComponent);
            this.targetZoneTriggerColliderProvider = targetZoneTriggerColliderProvider;
        }

        protected override void SetIsEscapingFromProjectile(bool value)
        {
            if (this.isEscapingFromProjectile && !value)
            {
                this.puzzleEventsManager.PZ_EVT_AI_Projectile_NoMoreAffected(this.aiID);
            }
            base.SetIsEscapingFromProjectile(value);
        }

        protected override Action<NavMeshRaycastStrategy> OnTriggerEnterDestinationCalculation => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            Debug.Log(MyLog.Format("EscapeToFarestWithTargetZone"));
            this.puzzleEventsManager.PZ_EVT_AI_Projectile_Hitted(this.aiID);
            this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.AIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
        };

        protected override Action<NavMeshRaycastStrategy> DestinationCalulationMethod => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.AIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
        };

        public override void OnLaunchProjectileDestroyed(LaunchProjectileModule launchProjectile)
        {
        }
    }
}
