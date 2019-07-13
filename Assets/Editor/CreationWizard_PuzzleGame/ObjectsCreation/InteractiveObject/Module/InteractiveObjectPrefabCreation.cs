using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using UnityEditor;

namespace Editor_InteractiveObjectCreationWizard
{
    [System.Serializable]
    public class InteractiveObjectPrefabCreation : CreateablePrefabComponent<InteractiveObjectType>
    {
        public override Func<AbstractCreationWizardEditorProfile, InteractiveObjectType> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                        editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseInteractiveObjectTypePrefab;
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.Create(editorInformations.CommonGameConfigurations.InstancePath.InteractiveObjectPrefabPath, editorInformations.ObjectName + NameConstants.RepelableObjectPrefab + editorInformations.ObjectDominantPrefix,
                    editorProfile);
            PrefabUtility.SavePrefabAsset(this.CreatedPrefab.gameObject);
        }
    }
}