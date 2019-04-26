﻿using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIProjectileWithCollisionEscapeManager : AbstractAIProjectileEscapeManager
    {

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private Func<Collider[]> targetZoneTriggerColliderProvider;
        #endregion

        public AIProjectileWithCollisionEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AIFOVManager AIFOVManager, PuzzleEventsManager PuzzleEventsManager, AiID aiID, Func<Collider[]> targetZoneTriggerColliderProvider) : base(escapingAgent, AIFOVManager, aiID, AIProjectileEscapeComponent)
        {
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.targetZoneTriggerColliderProvider = targetZoneTriggerColliderProvider;
        }

        protected override void SetIsEscapingFromProjectile(bool value)
        {
            if (this.isEscapingFromProjectile && !value)
            {
                this.PuzzleEventsManager.PZ_EVT_AI_Projectile_NoMoreAffected(this.aiID);
            }
            base.SetIsEscapingFromProjectile(value);
        }

        protected override Action<NavMeshRaycastStrategy> OnTriggerEnterDestinationCalculation => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            Debug.Log(MyLog.Format("EscapeToFarestWithTargetZone"));
            this.PuzzleEventsManager.PZ_EVT_AI_Projectile_Hitted(this.aiID, 1);
            this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.AIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
        };

        protected override Action<NavMeshRaycastStrategy> DestinationCalulationMethod => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.AIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
        };

        public override void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {
        }
    }

    public class AIProjectileIgnorePhysicsEscapeManager : AbstractAIProjectileEscapeManager
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public AIProjectileIgnorePhysicsEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AIFOVManager AIFOVManager, PuzzleEventsManager PuzzleEventsManager, AiID aiID) : base(escapingAgent, AIFOVManager, aiID, AIProjectileEscapeComponent)
        {
            this.PuzzleEventsManager = PuzzleEventsManager;
        }

        protected override void SetIsEscapingFromProjectile(bool value)
        {
            if (this.isEscapingFromProjectile && !value)
            {
                this.PuzzleEventsManager.PZ_EVT_AI_Projectile_NoMoreAffected(this.aiID);
            }
            base.SetIsEscapingFromProjectile(value);
        }

        protected override Action<NavMeshRaycastStrategy> OnTriggerEnterDestinationCalculation => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            Debug.Log("EscapeToFarest");
            this.PuzzleEventsManager.PZ_EVT_AI_Projectile_Hitted(this.aiID, 2);
            this.escapeDestinationManager.EscapeToFarest(5, navMeshRaycastStrategy, this.AIFOVManager);
        };

        protected override Action<NavMeshRaycastStrategy> DestinationCalulationMethod => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            this.escapeDestinationManager.EscapeToFarest(5, navMeshRaycastStrategy, this.AIFOVManager);
        };

        public override void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {

        }
    }
}
