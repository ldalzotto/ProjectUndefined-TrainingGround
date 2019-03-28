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
                EditorGUILayout.LabelField("AI Manager description : " + GetManagerDescription(abstractAIComponent.SelectedManagerType), EditorStyles.miniLabel);
            }

        }

        private string GetManagerDescription(Type managerType)
        {
            string returnMessage = string.Empty;
            AIManagerTypeSafeOperation.ForAllAIManagerTypes(managerType,
                () => { returnMessage = "Random patrolling"; return null; },
                () => { returnMessage = "Escape proj"; return null; }
                );

            return returnMessage;
        }
    }


}

#endif