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
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        public AIProjectileWithCollisionEscapeManager(AIProjectileEscapeComponent associatedAIComponent) : base(associatedAIComponent)
        {
          
        }

        public override void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            base.Init(AIBheaviorBuildInputData);
            this.InteractiveObjectContainer = AIBheaviorBuildInputData.InteractiveObjectContainer;
        }

        protected override void SetIsEscapingFromProjectile(bool value)
        {
            if (this.isEscapingFromProjectile && !value)
            {
                this.puzzleEventsManager.PZ_EVT_AI_Projectile_NoMoreAffected(this.AIObjectDataRetriever);
            }
            base.SetIsEscapingFromProjectile(value);
        }

        protected override Action<NavMeshRaycastStrategy> OnTriggerEnterDestinationCalculation => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            Debug.Log(MyLog.Format("EscapeToFarestWithTargetZone"));
            this.puzzleEventsManager.PZ_EVT_AI_Projectile_Hitted(this.AIObjectDataRetriever);
            this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.FovManagerCalcuation, TargetZoneHelper.GetTargetZonesTriggerColliders(this.InteractiveObjectContainer));
        };

        protected override Action<NavMeshRaycastStrategy> DestinationCalulationMethod => (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
        {
            this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.FovManagerCalcuation, TargetZoneHelper.GetTargetZonesTriggerColliders(this.InteractiveObjectContainer));
        };

        public override void OnLaunchProjectileDestroyed(LaunchProjectileModule launchProjectile)
        {
        }
    }
}
