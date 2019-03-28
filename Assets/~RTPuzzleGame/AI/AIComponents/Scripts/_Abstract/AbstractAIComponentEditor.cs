#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{

    public class AbstractAIComponentEditor<T> : Editor where T : AbstractAIComponent
    {

        private AbstractAIComponentEditorManager abstractAIComponentEditorManager;

        private void OnEnable()
        {
            T abractComponent = target as T;
            this.abstractAIComponentEditorManager = new AbstractAIComponentEditorManager();
            this.abstractAIComponentEditorManager.OnEnable(abractComponent.AbstractManagerType);
        }

        public override void OnInspectorGUI()
        {
            T abractComponent = target as T;
            base.OnInspectorGUI();
            this.abstractAIComponentEditorManager.OnInspectorGUI(abractComponent);
        }
    }

    public class AbstractAIComponentEditorManager
    {
        private Type[] elligibleTypes;
        public void OnEnable(Type abstractManagerType)
        {
            this.elligibleTypes = null;
            this.elligibleTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
               .Where(p => abstractManagerType.IsAssignableFrom(p))
               .Where(p => p.Name != abstractManagerType.Name)
               .ToArray();
        }

        public void OnInspectorGUI(AbstractAIComponent abstractAIComponent)
        {
            var selectedIndex = 0;
            if (abstractAIComponent.SelectedManagerType != null)
            {
                var eligibleT = this.elligibleTypes.ToList().Select(t => t).Where(t => t.AssemblyQualifiedName == abstractAIComponent.SelectedManagerType.AssemblyQualifiedName);
                if (eligibleT != null && eligibleT.Count() > 0)
                {
                    selectedIndex = this.elligibleTypes.ToList().IndexOf(eligibleT.First());
                }

            }


            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup(new GUIContent("AI Manager type : "), selectedIndex, this.elligibleTypes.ToList().ConvertAll(t => t.Name).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                abstractAIComponent.SelectedManagerType = this.elligibleTypes[selectedIndex];
            }

            //Initialisation
            if (abstractAIComponent.SelectedManagerType == null)
            {
                abstractAIComponent.SelectedManagerType = this.elligibleTypes[selectedIndex];
            }

            if (abstractAIComponent.SelectedManagerType != null)
            {
                var description = "AI Manager description : " + GetManagerDescription(abstractAIComponent.SelectedManagerType);
                var gContent = new GUIContent(description, description);
                GUILayout.Label(gContent, EditorStyles.miniLabel);
            }

        }

        private string GetManagerDescription(Type managerType)
        {
            string returnMessage = string.Empty;
            new AIManagerTypeSafeOperation
            {
                AIRandomPatrolComponentManangerOperation = () => { returnMessage = "Random patrolling."; return null; },
                AIProjectileEscapeManagerOperation = () => { returnMessage = "Reduce FOV when a projectile is near."; return null; },
                AIFearStunManagerOperation = () => { returnMessage = "Block any movement when FOV sum values are below a threshold."; return null; },
                AIAttractiveObjectOperation = () => { returnMessage = "Move to the nearest attractive point in range.\nOnce targeted, the movement is never cancelled by this component."; return null; },
                AITargetZoneManagerOperation = () => { returnMessage = "Detect weather the AI is in the selected target zone or not."; return null; }
            }.ForAllAIManagerTypes(managerType);
            return returnMessage;
        }
    }


}

#endif