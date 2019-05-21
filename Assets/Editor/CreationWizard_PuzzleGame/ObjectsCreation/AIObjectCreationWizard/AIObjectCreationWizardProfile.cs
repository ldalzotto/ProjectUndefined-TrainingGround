using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIObjectCreationWizardProfile", menuName = "CreationWizard/PuzzleObjectCreationWizard/AIObjectCreationWizardProfile", order = 1)]
    public class AIObjectCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        public override void OnEnable()
        {
            base.OnEnable();
            this.InitModule<EditorInformations>(false, true, false);
            this.InitModule<AIBehaviorConfigurationCreation>(false, true, false);
            this.InitModule<AIPrefabCreation>(false, true, false);
        }

        public override void OnGenerationEnd()
        {
        }

        public CreationWizardModules GetAllModules()
        {
            return new CreationWizardModules(
                this.GetModule<EditorInformations>(),
                this.GetModule<AIBehaviorConfigurationCreation>(),
                this.GetModule<AIPrefabCreation>()
            );
        }

        public static CreationWizardModules GetAllModules(Dictionary<string, CreationModuleComponent> modules)
        {
            return new CreationWizardModules(
              (EditorInformations)modules[typeof(EditorInformations).Name],
              (AIBehaviorConfigurationCreation)modules[typeof(AIBehaviorConfigurationCreation).Name],
              (AIPrefabCreation)modules[typeof(AIPrefabCreation).Name]
           );
        }
    }
    public class CreationWizardModules
    {
        public EditorInformations EditorInformations;
        public AIBehaviorConfigurationCreation AIBehaviorConfigurationCreation;
        public AIPrefabCreation AIPrefabCreation;

        public CreationWizardModules(EditorInformations editorInformations, AIBehaviorConfigurationCreation aIBehaviorConfigurationCreation, AIPrefabCreation aIPrefabCreation)
        {
            EditorInformations = editorInformations;
            AIBehaviorConfigurationCreation = aIBehaviorConfigurationCreation;
            AIPrefabCreation = aIPrefabCreation;
        }
    }

}

