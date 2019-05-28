using System;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPatrolComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPatrolComponent", order = 1)]
    public class AIPatrolComponent : AbstractAIComponent
    {
        protected override Type abstractManagerType => typeof(AbstractAIPatrolComponentManager);
        public float MaxDistance;

#if UNITY_EDITOR
        public override void EditorGUI(Transform transform)
        {
            Handles.color = Color.magenta;
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.magenta;
            Handles.Label(transform.position + (Vector3.up * MaxDistance), "AI Patrol distance.", labelStyle);
            Handles.DrawWireDisc(transform.position, Vector3.up, MaxDistance);
        }
#endif
    }

    public abstract class AbstractAIPatrolComponentManager : InterfaceAIManager
    {
#region External Dependencies
        protected NavMeshAgent patrollingAgent;
        protected AIFOVManager AIFOVManager;
#endregion

        protected AIPatrolComponent AIPatrolComponent;

        protected AbstractAIPatrolComponentManager(NavMeshAgent patrollingAgent, AIPatrolComponent aIPatrolComponent, AIFOVManager aIFOVManager)
        {
            this.patrollingAgent = patrollingAgent;
            AIPatrolComponent = aIPatrolComponent;
            this.AIFOVManager = aIFOVManager;
        }

        public abstract void OnDestinationReached();
        public abstract void OnStateReset();
        protected abstract bool IsPatrolling();
        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);
        public abstract void GizmoTick();

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public bool IsManagerEnabled()
        {
            return this.IsPatrolling();
        }

    }

}