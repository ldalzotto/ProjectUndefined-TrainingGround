#if UNITY_EDITOR
using System;
using UnityEditor;

namespace RTPuzzle
{

    public class AbstractAIComponentEditor<T> : Editor where T : AbstractAIComponent
    {

        private TypeSelectionerManager abstractAIComponentEditorManager;

        private void OnEnable()
        {
            T abractComponent = target as T;
            this.abstractAIComponentEditorManager = new TypeSelectionerManager();
            this.abstractAIComponentEditorManager.OnEnable(abractComponent.AbstractManagerType, "AI Manager type : ");
        }

        public override void OnInspectorGUI()
        {
            T abractComponent = target as T;
            base.OnInspectorGUI();
            var changedType = this.abstractAIComponentEditorManager.OnInspectorGUI(abractComponent.SelectedManagerType, this.GetManagerDescription);
            if (changedType != null)
            {
                abractComponent.SelectedManagerType = changedType;
            }
        }


        private string GetManagerDescription(Type managerType)
        {
            string returnMessage = string.Empty;
            AIManagerTypeSafeOperation.ForAllAIManagerTypes(managerType,
                () => { returnMessage = "Random patrolling."; return null; },
                () => { returnMessage = "Reduce FOV when a projectile is near."; return null; },
                () => { returnMessage = "Block any movement when FOV sum values are below a threshold."; return null; },
                () => { returnMessage = "Move to the nearest attractive point in range.\nOnce targeted, the movement is never cancelled by this component."; return null; },
                () => { returnMessage = "Detect weather the AI is in the selected target zone or not."; return null; });
            return "AI Manager description : " + returnMessage;
        }
    }

    #region AI component specific editors
    [CustomEditor(typeof(AIPatrolComponent))]
    public class AIPatrolComponentEditor : AbstractAIComponentEditor<AIPatrolComponent>
    { }
    [CustomEditor(typeof(AIAttractiveObjectComponent))]
    public class AIAttractiveObjectComponentEditor : AbstractAIComponentEditor<AIAttractiveObjectComponent>
    { }
    [CustomEditor(typeof(AIFearStunComponent))]
    public class AIFearStunComponentEditor : AbstractAIComponentEditor<AIFearStunComponent>
    { }
    [CustomEditor(typeof(AIProjectileEscapeComponent))]
    public class AIProjectileEscapeComponentEditor : AbstractAIComponentEditor<AIProjectileEscapeComponent>
    { }
    [CustomEditor(typeof(AITargetZoneComponent))]
    public class AITargetZoneComponentEditor : AbstractAIComponentEditor<AITargetZoneComponent>
    { }
    #endregion
}

#endif