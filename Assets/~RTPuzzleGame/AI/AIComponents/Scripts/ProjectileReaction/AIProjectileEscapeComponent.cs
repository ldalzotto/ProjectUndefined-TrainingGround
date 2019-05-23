using System;
using UnityEngine;
using UnityEngine.AI;

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

        public override void EditorGUI(Transform transform)
        {
            Handles.color = Color.blue;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Handles.color;
            Handles.Label(transform.position + Vector3.up * EscapeDistance, this.GetType().Name, labelStyle);
            Handles.DrawWireDisc(transform.position, Vector3.up, EscapeDistance);
        }
    }

    public abstract class AbstractAIProjectileEscapeManager : InterfaceAIManager
    {
        protected AiID aiID;
        #region External Dependencies
        protected NavMeshAgent escapingAgent;
        protected AIFOVManager AIFOVManager;
        protected PuzzleEventsManager puzzleEventsManager;
        private AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;
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

        protected AbstractAIProjectileEscapeManager(NavMeshAgent escapingAgent, AIFOVManager aIFOVManager, AiID aiID,
            AIProjectileEscapeComponent AIProjectileEscapeComponent, PuzzleEventsManager puzzleEventsManager, AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent)
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

        #region Data Retrieval
        public float GetMaxEscapeDistance()
        {
            return this.AIProjectileEscapeComponent.EscapeDistance;
        }
        #endregion

        public Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            return this.escapeDestinationManager.Tick();
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
            if (impactPoint != null)
            {
                this.OnDestinationSetFromProjectileContact();
                this.AIFOVManager.IntersectFOV_FromEscapeDirection(impactPoint, escapingAgent.transform.position, launchProjectileInherentData.EscapeSemiAngle);
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
