using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPlayerEscapeComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPlayerEscapeComponent", order = 1)]
    public class AIPlayerEscapeComponent : AbstractAIComponent
    {
        public float EscapeDistance;
        public float PlayerDetectionRadius;
        public float EscapeSemiAngle;

        protected override Type abstractManagerType => typeof(AbstractPlayerEscapeManager);

#if UNITY_EDITOR
        public override void EditorGUI(Transform transform)
        {
            Handles.color = Color.yellow;

            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Handles.color;

            Handles.Label(transform.position + Vector3.up * EscapeDistance, nameof(EscapeDistance), labelStyle);
            Handles.DrawWireDisc(transform.position, Vector3.up, EscapeDistance);

            Handles.Label(transform.position + Vector3.up * PlayerDetectionRadius, nameof(PlayerDetectionRadius), labelStyle);
            Handles.DrawWireDisc(transform.position, Vector3.up, PlayerDetectionRadius);

            Handles.Label(transform.position + transform.forward * 4, "Escape angle.", labelStyle);
            Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, EscapeSemiAngle, 5f);
            Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, -EscapeSemiAngle, 5f);
        }
#endif
    }

    public abstract class AbstractPlayerEscapeManager : InterfaceAIManager
    {
        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public abstract bool IsManagerEnabled();

        public abstract void OnPlayerEscapeStart();

        public abstract void OnDestinationReached();

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);

        public abstract void OnStateReset();
    }

}
