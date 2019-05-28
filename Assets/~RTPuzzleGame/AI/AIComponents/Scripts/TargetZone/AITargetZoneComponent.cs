using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : AbstractAIComponent
    {
        public float TargetZoneEscapeDistance;

        protected override Type abstractManagerType => typeof(AbstractAITargetZoneManager);

#if UNITY_EDITOR
        public override void EditorGUI(Transform transform)
        {
            Handles.color = Color.green;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Handles.color;
            Handles.Label(transform.position + Vector3.up * TargetZoneEscapeDistance, nameof(this.TargetZoneEscapeDistance), labelStyle);
            Handles.DrawWireDisc(transform.position, Vector3.up, TargetZoneEscapeDistance);
        }
#endif
    }

    public abstract class AbstractAITargetZoneManager : InterfaceAIManager
    {
        #region State
        protected bool isEscapingFromTargetZone;
        #endregion

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);
        public abstract void TriggerTargetZoneEscape(TargetZone targetZone);
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public bool IsManagerEnabled()
        {
            return this.isEscapingFromTargetZone;
        }

    }

}
